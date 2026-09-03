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
        /// A stand with no owner set serves whoever owns a bed in the same room. Unticked, it
        /// serves only pawns assigned to it explicitly.
        /// </summary>
        public bool inheritOwnerFromBed = true;

        /// <summary>
        /// Refuse the change when the night clothes insulate less than the day clothes and the
        /// bedroom is too cold for the difference. See <see cref="TemperatureGuard"/>.
        /// </summary>
        public bool coldGuard = true;

        /// <summary>Safety margin of the cold guard, in degrees.</summary>
        public float coldGuardMargin = 2f;

        /// <summary>Maximum distance, in cells, between the bed and the stand.</summary>
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
    /// The safety switch. This mod hooks <c>Pawn_JobTracker.StartJob</c>, which is the start of
    /// <b>every</b> job of <b>every</b> pawn: an unhandled exception in there is a bricked colony.
    /// Every hook catches, logs once, disables the mod for the session, and lets vanilla proceed.
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
            Log.Error($"[Night Change] Error in {where}. The mod disables itself for this session "
                      + $"and lets the game carry on as normal.\n{ex}");
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
