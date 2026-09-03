using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace NightChange
{
    /// <summary>
    /// The return trip: the pawn is up, in night clothes, and their day is starting.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike the outbound trip, the return is a <b>JobGiver</b> placed in the think tree rather
    /// than a prefix on <c>StartJob</c>. The reason fits in one sentence: from <c>StartJob</c> it
    /// is impossible to tell reliably whether the pawn is staying in bed.
    /// <c>pawn.jobs.posture</c> is reset by no code in <c>Pawn_JobTracker</c> - it stays at
    /// <c>LayingInBed</c> until a toil of the <i>new</i> job decides otherwise. A colonist watching
    /// television in bed or meditating lying down does start new jobs, and the prefix would have
    /// pulled them out of bed to get dressed.
    /// </para>
    /// <para>
    /// In the think tree the problem disappears: the "must keep lying down" branch
    /// (<c>ThinkNode_ConditionalMustKeepLyingDown</c>) sits at the very top and short-circuits
    /// everything else. A JobGiver is therefore only consulted by a pawn who is getting up for
    /// good.
    /// </para>
    /// <para>
    /// Inserted on <c>Humanlike_PreMain</c>, just before the colonist behaviour core: so
    /// <b>after</b> the allowed area, safe temperature, emergency work and the apparel optimizer,
    /// and <b>before</b> ordinary work and recreation. A fire comes before the trousers; the
    /// trousers come before the carpentry bench.
    /// </para>
    /// </remarks>
    public class JobGiver_ChangeBack : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (FailOpen.Disabled)
            {
                return null;
            }

            try
            {
                CompNightStand comp = NightChangeTracker.Current?.LedgerFor(pawn);
                if (comp == null)
                {
                    return null;
                }

                if (!pawn.Spawned || pawn.Downed || pawn.InMentalState || pawn.Drafted
                    || comp.parent.Map != pawn.Map)
                {
                    return null;
                }

                // An injured pawn stays in bed: medical rest sits lower in the tree than we do, so
                // it would not defend itself.
                if (HealthAIUtility.ShouldSeekMedicalRest(pawn))
                {
                    return null;
                }

                // While the timetable says "sleep", the night is not over. Without this gate, a
                // colonist getting up for a snack at two in the morning dresses head to toe, eats,
                // and goes back to the stand to turn in - three trips for a midnight snack. The
                // game's default timetable sleeps from 22h to 5h, so the rule bites in the very
                // first colony, with nothing to configure.
                if (pawn.timetable?.CurrentAssignment == TimeAssignmentDefOf.Sleep)
                {
                    return null;
                }

                // Anti-loop net. If the deferred bed job fails repeatedly (bed taken, path cut),
                // the pawn could change, fail, change back, forever.
                if (NightChangeTracker.Current.ChangedRecently(pawn))
                {
                    return null;
                }

                // Danger is deliberately ignored here. Changing <i>into</i> night clothes is a
                // detour nobody should take mid-raid; changing <i>out</i> of them is a pawn moving
                // toward their own gear. Shift Change learned that difference in play: four
                // colonists spent a raid in evening dress with their flak vests parked in a stand,
                // and the gate meant to protect them was the reason they could not go and get them.
                if (!pawn.CanReserveAndReach(comp.parent, PathEndMode.InteractionCell, Danger.Deadly))
                {
                    return null;
                }

                SwapPlan plan = SwapPlan.ForUndressing(pawn, comp);
                if (plan == null)
                {
                    // Nothing left to give back or take back: the player emptied the stand, or a
                    // raid stripped the pawn. The ledger is lying, so we close it.
                    comp.NotifyUndressed();
                    return null;
                }

                return JobMaker.MakeJob(NightChangeDefOf.NightChange_ChangeAtStand, comp.parent);
            }
            catch (Exception ex)
            {
                FailOpen.Fail("JobGiver_ChangeBack", ex);
                return null;
            }
        }
    }
}
