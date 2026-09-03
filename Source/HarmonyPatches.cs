using System;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace NightChange
{
    /// <summary>
    /// L'aller : le pion part se coucher, on l'envoie d'abord au portant.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Prefixe sur <c>Pawn_JobTracker.StartJob</c>. On se greffe la et pas sur un JobGiver parce que
    /// la decision « ce pion va dormir maintenant » est deja prise par le vanilla :
    /// <see cref="JobGiver_GetRest"/> a lu l'emploi du temps, le niveau de repos,
    /// <c>canSleepTick</c>, l'etat du seigneur, et a trouve un lit. La relire nous-memes, ce serait
    /// dupliquer une demi-douzaine de regles qui bougent d'une version a l'autre.
    /// </para>
    /// <para>
    /// Le motif d'insertion est celui du vanilla lui-meme, qui glisse un halage opportuniste devant
    /// le job en cours (<c>Pawn_JobTracker.cs:331-347</c>) : <b>demarrer le detour d'abord, remettre
    /// le job d'origine en tete de file ensuite</b>. Si le detour leve et que le job d'origine etait
    /// deja en file, le filet de securite le laisserait demarrer aussi -- un meme objet Job a deux
    /// endroits. <c>StartJob</c> ne lit jamais la file, donc l'ordre inverse est equivalent en cas
    /// de succes et plus sur en cas d'echec.
    /// </para>
    /// <para>
    /// Le job differe n'est <b>pas</b> re-reserve avant le detour, contrairement a ce que fait Shift
    /// Change. La cible ici est le lit du pion, que personne d'autre ne va lui prendre pendant qu'il
    /// enfile son pyjama ; et la file du vanilla re-reserve d'elle-meme au demarrage. Payer la
    /// gymnastique de reservation de Shift Change (qui existe parce qu'une cible de <i>travail</i>
    /// est disputee) serait du bruit.
    /// </para>
    /// </remarks>
    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.StartJob))]
    public static class Patch_StartJob
    {
        /// <summary>
        /// Demarrer un job depuis un prefixe de <c>StartJob</c> re-entre dans ce prefixe. Ce garde
        /// fait passer l'appel interieur.
        /// </summary>
        private static bool reentrant;

        [HarmonyPrefix]
        public static bool Prefix(Pawn_JobTracker __instance, Job newJob, Pawn ___pawn)
        {
            if (FailOpen.Disabled || reentrant || newJob == null || ___pawn == null)
            {
                return true;
            }

            try
            {
                return TryDivert(__instance, newJob, ___pawn);
            }
            catch (Exception ex)
            {
                FailOpen.Fail("Patch_StartJob", ex);
                return true;
            }
        }

        private static bool TryDivert(Pawn_JobTracker tracker, Job newJob, Pawn pawn)
        {
            if (newJob.def != JobDefOf.LayDown || newJob.playerForced)
            {
                return true;
            }

            if (!(newJob.targetA.Thing is Building_Bed bed) || !Eligible(pawn))
            {
                return true;
            }

            // Un blesse qui va au lit medical n'a rien a faire au portant : le vanilla l'y envoie
            // parce qu'il a besoin de soins, pas parce que c'est l'heure.
            if (HealthAIUtility.ShouldSeekMedicalRest(pawn))
            {
                return true;
            }

            // Rien ne se change pendant un raid ou un incendie. La porte ne couvre que l'aller :
            // le retour, lui, est un pion qui marche vers son propre equipement.
            if (pawn.Map.dangerWatcher.DangerRating != StoryDanger.None)
            {
                return true;
            }

            NightChangeTracker index = NightChangeTracker.Current;
            if (index == null || index.LedgerFor(pawn) != null || index.ChangedRecently(pawn))
            {
                return true;
            }

            Building_OutfitStand stand = StandFinder.ForBed(pawn, bed, out SwapPlan plan);
            if (stand == null || plan == null || !plan.MovesAnything)
            {
                return true;
            }

            Job swap = JobMaker.MakeJob(NightChangeDefOf.NightChange_ChangeAtStand, stand);

            reentrant = true;
            try
            {
                tracker.StartJob(swap);
            }
            finally
            {
                reentrant = false;
            }

            tracker.jobQueue.EnqueueFirst(newJob);
            return false;
        }

        private static bool Eligible(Pawn pawn)
        {
            return pawn.Spawned
                   && pawn.Map != null
                   && pawn.apparel != null
                   && pawn.outfits != null
                   && pawn.IsColonistPlayerControlled
                   && !pawn.Drafted
                   && !pawn.Downed
                   && !pawn.InMentalState
                   && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
        }
    }

    /// <summary>
    /// Tant que le pion est en tenue de nuit, l'optimiseur vestimentaire est coupe.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Le marqueur de port force protege le pyjama : l'optimiseur ne le retirera pas. Mais il voit
    /// un pion sous-vetu -- son duster et son gilet sont dans le portant, hors de portee puisque
    /// <c>allowRemovingItems</c> est faux -- et part lui chercher <b>autre chose</b> dans les
    /// stocks. Un colon en pyjama et parka.
    /// </para>
    /// <para>
    /// Coupe aussi la branche de recoloration d'Ideology, qui roule sur ce meme JobGiver et
    /// repeindrait la tenue de nuit a la couleur preferee du pion. L'ordre direct par la station de
    /// style passe a cote du JobGiver et reste donc disponible.
    /// </para>
    /// </remarks>
    [HarmonyPatch(typeof(JobGiver_OptimizeApparel), "TryGiveJob")]
    public static class Patch_OptimizeApparel
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn pawn, ref Job __result)
        {
            if (FailOpen.Disabled)
            {
                return true;
            }

            try
            {
                if (NightChangeTracker.Current?.LedgerFor(pawn) == null)
                {
                    return true;
                }

                __result = null;
                return false;
            }
            catch (Exception ex)
            {
                FailOpen.Fail("Patch_OptimizeApparel", ex);
                return true;
            }
        }
    }

    /// <summary>
    /// La propriete cesse quand le vanilla dit qu'elle cesse.
    /// </summary>
    /// <remarks>
    /// <c>Pawn_Ownership.UnclaimAll()</c> est appele a la mort, a la vente, a l'enlevement et a la
    /// sortie de carte, mais il ne libere qu'une liste cablee -- lit, tombe, trone, caisson de
    /// mort-sommeil. Il ne parcourt pas les batiments a <c>CompAssignableToPawn</c>. Ce postfix
    /// etend le meme instant aux portants, et moissonne aussi les portants simplement
    /// <b>empruntes</b>, que la desassignation seule manquerait entierement : un emprunteur non
    /// assigne ne l'etait de toute facon nulle part.
    /// </remarks>
    [HarmonyPatch(typeof(Pawn_Ownership), nameof(Pawn_Ownership.UnclaimAll))]
    public static class Patch_UnclaimAll
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn ___pawn)
        {
            Reaper.Reap(___pawn, "Patch_UnclaimAll");
        }
    }

    /// <summary>
    /// Le bannissement n'est pas sur cette liste, et n'y arrive pas non plus par la sortie de carte.
    /// </summary>
    /// <remarks>
    /// Sur un colon pose sur la carte, <c>PawnBanishUtility.Banish</c> efface le statut d'invite
    /// puis appelle <c>SetFaction(null)</c> : il n'atteint aucun <c>UnclaimAll</c>. Et la route de
    /// sortie de carte ne rattrape rien apres coup, <c>Pawn.ExitMap</c> conditionnant son
    /// <c>UnclaimAll</c> a un drapeau que l'effacement du statut d'invite a deja rendu faux. Le pion
    /// s'en va vivant, en pyjama, avec le grand livre intact derriere lui.
    /// </remarks>
    [HarmonyPatch(typeof(PawnBanishUtility), nameof(PawnBanishUtility.Banish), typeof(Pawn), typeof(PlanetTile), typeof(bool))]
    public static class Patch_Banish
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn)
        {
            Reaper.Reap(pawn, "Patch_Banish");
        }
    }

    internal static class Reaper
    {
        internal static void Reap(Pawn pawn, string where)
        {
            if (FailOpen.Disabled || pawn == null)
            {
                return;
            }

            try
            {
                // Un balayage complet plutot qu'une consultation de l'index : celui-ci ne connait
                // que les emprunteurs, alors qu'un portant peut etre simplement assigne a un pion
                // qui ne s'y est jamais change. C'est rare (une mort, un bannissement), donc le
                // cout est sans objet.
                foreach (Map map in Find.Maps)
                {
                    foreach (Building_OutfitStand stand in
                             map.listerBuildings.AllBuildingsColonistOfClass<Building_OutfitStand>())
                    {
                        stand.GetComp<CompNightStand>()?.Reap(pawn);
                    }
                }
            }
            catch (Exception ex)
            {
                FailOpen.Fail(where, ex);
            }
        }
    }
}
