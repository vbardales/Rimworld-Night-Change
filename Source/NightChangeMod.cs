using System;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace NightChange
{
    public class NightChangeSettings : ModSettings
    {
        /// <summary>
        /// Un portant sans proprietaire assigne sert le proprietaire du lit de la piece. Decoche,
        /// il ne sert que les pions explicitement assignes.
        /// </summary>
        public bool inheritOwnerFromBed = true;

        /// <summary>
        /// Refuse le changement quand la tenue de nuit isole moins que les habits de jour et que la
        /// chambre est trop froide pour la difference. Voir <see cref="TemperatureGuard"/>.
        /// </summary>
        public bool coldGuard = true;

        /// <summary>Marge de securite du garde-froid, en degres.</summary>
        public float coldGuardMargin = 2f;

        /// <summary>Distance maximale, en cases, entre le lit et le portant.</summary>
        public int maxStandDistance = 12;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref inheritOwnerFromBed, "inheritOwnerFromBed", true);
            Scribe_Values.Look(ref coldGuard, "coldGuard", true);
            Scribe_Values.Look(ref coldGuardMargin, "coldGuardMargin", 2f);
            Scribe_Values.Look(ref maxStandDistance, "maxStandDistance", 12);
        }
    }

    public class NightChangeMod : Mod
    {
        public static NightChangeSettings Settings;

        public NightChangeMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<NightChangeSettings>();
            new Harmony("nelim.nightchange").PatchAll();
        }

        public override string SettingsCategory() => "NightChange_ModTitle".Translate();

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var list = new Listing_Standard();
            list.Begin(inRect);

            list.CheckboxLabeled("NightChange_SettingInheritOwner".Translate(),
                ref Settings.inheritOwnerFromBed, "NightChange_SettingInheritOwnerDesc".Translate());
            list.Gap();

            list.CheckboxLabeled("NightChange_SettingColdGuard".Translate(),
                ref Settings.coldGuard, "NightChange_SettingColdGuardDesc".Translate());
            if (Settings.coldGuard)
            {
                list.Label("NightChange_SettingColdMargin".Translate(Settings.coldGuardMargin.ToString("0")));
                Settings.coldGuardMargin = Mathf.Round(list.Slider(Settings.coldGuardMargin, 0f, 10f));
            }
            list.Gap();

            list.Label("NightChange_SettingDistance".Translate(Settings.maxStandDistance));
            Settings.maxStandDistance = Mathf.RoundToInt(list.Slider(Settings.maxStandDistance, 3f, 40f));

            list.End();
        }
    }

    /// <summary>
    /// Interrupteur de securite. Ce mod se greffe sur <c>Pawn_JobTracker.StartJob</c>, c'est-a-dire
    /// sur le demarrage de <b>tous</b> les jobs de <b>tous</b> les pions : une exception non
    /// rattrapee la-dedans, et la colonie est bloquee. Chaque greffe rattrape, journalise une seule
    /// fois, se desactive pour la session, et laisse le vanilla continuer.
    /// </summary>
    public static class FailOpen
    {
        public static bool Disabled { get; private set; }

        public static void Fail(string where, Exception ex)
        {
            if (Disabled)
            {
                return;
            }

            Disabled = true;
            Log.Error($"[Night Change] Erreur dans {where}. Le mod se desactive pour cette session "
                      + $"et laisse le jeu poursuivre normalement.\n{ex}");
        }
    }

    [DefOf]
    public static class NightChangeDefOf
    {
        public static JobDef NightChange_ChangeAtStand;

        static NightChangeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(NightChangeDefOf));
        }
    }
}
