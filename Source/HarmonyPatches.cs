using System;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;
using Verse.AI;

namespace NightChange
{
    /// <summary>
    /// Outbound: the pawn is off to bed, so we send them to the stand first.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A prefix on <c>Pawn_JobTracker.StartJob</c>. We hook there rather than on a JobGiver because
    /// the decision "this pawn is going to sleep now" has already been made by vanilla:
    /// <see cref="JobGiver_GetRest"/> has read the timetable, the rest need, <c>canSleepTick</c>,
    /// the lord's state, and found a bed. Reading all that ourselves would duplicate half a dozen
    /// rules that drift from version to version.
    /// </para>
    /// <para>
    /// The insertion pattern is vanilla's own, which slips an opportunistic haul ahead of the
    /// incoming job (<c>Pawn_JobTracker.cs:331-347</c>): <b>start the detour first, enqueue the
    /// original second</b>. If starting the detour threw and the original were already queued, the
    /// fail-open catch would let the original also start, putting one Job object in two places.
    /// <c>StartJob</c> never reads the queue, so enqueueing after is equivalent on success and
    /// safer on failure.
    /// </para>
    /// <para>
    /// The deferred job is <b>not</b> re-reserved before the detour, unlike Shift Change. Its
    /// target is a <i>work</i> target, contested between colonists; ours is the pawn's own bed,
    /// which nobody is going to take while they put on their night clothes, and vanilla's queue
    /// re-reserves by itself on start. Paying for Shift Change's reservation dance would be noise.
    /// </para>
    /// </remarks>
    [HarmonyPatch(typeof(Pawn_JobTracker), nameof(Pawn_JobTracker.StartJob))]
    public static class Patch_StartJob
    {
        /// <summary>
        /// Starting a job from inside a <c>StartJob</c> prefix re-enters that prefix. This guard
        /// makes the inner call pass through.
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

            // A wounded pawn heading for a medical bed has no business at the stand: vanilla sends
            // them there because they need treatment, not because it is bedtime.
            if (HealthAIUtility.ShouldSeekMedicalRest(pawn))
            {
                return true;
            }

            // Nobody changes during a raid or a fire. This gate covers the outbound trip only: the
            // return trip is a pawn walking toward their own gear.
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
    /// While the pawn is in night clothes, the wardrobe optimizer is switched off.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The forced flag protects the night clothes: the optimizer will not remove them. But it sees
    /// an underdressed pawn - their duster and vest are in the stand, out of reach since
    /// <c>allowRemovingItems</c> is false - and goes to fetch them <b>something else</b> from the
    /// stockpiles. A colonist in pyjamas and a parka.
    /// </para>
    /// <para>
    /// This also switches off Ideology's apparel-recolor branch, which rides that same JobGiver and
    /// would permanently dye the night clothes the pawn's favourite colour. The styling station's
    /// float-menu order bypasses the giver and stays available.
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
    /// Ownership must end when vanilla thinks it ends.
    /// </summary>
    /// <remarks>
    /// <c>Pawn_Ownership.UnclaimAll()</c> is called on death, trade, kidnap and map exit, but it
    /// unclaims a hardcoded list only - bed, grave, throne, deathrest casket. It does not walk
    /// <c>CompAssignableToPawn</c> buildings. This postfix extends the same moment to stands, and
    /// it also reaps <b>borrowed</b> stands, which unassignment alone would miss entirely: a pool
    /// borrower was never assigned to anything.
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
    /// Banishment is not on that list, and does not join it later either.
    /// </summary>
    /// <remarks>
    /// On a spawned colonist, <c>PawnBanishUtility.Banish</c> clears guest status and runs
    /// <c>SetFaction(null)</c>: it reaches no <c>UnclaimAll</c>. Nor does the map-exit route rescue
    /// it afterwards, <c>Pawn.ExitMap</c> gating its <c>UnclaimAll</c> on a flag that the
    /// guest-status clear has already made false. So the pawn walks away alive, in night clothes,
    /// with the ledger intact behind them.
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
                // A full sweep rather than an index lookup: the index knows borrowers only, whereas
                // a stand may simply be assigned to a pawn who never changed at it. This is rare (a
                // death, a banishment), so the cost is beside the point.
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
