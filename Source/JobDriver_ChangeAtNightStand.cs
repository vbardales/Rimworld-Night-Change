using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace NightChange
{
    /// <summary>
    /// The trip to the stand, in either direction.
    /// </summary>
    /// <remarks>
    /// The direction is not carried in the job: it is read off the ledger on arrival. If the stand
    /// already holds this pawn's clothes it is a return trip, otherwise it is an outbound one. A
    /// scribed job resuming after a load therefore always finds the right direction, with no extra
    /// field.
    /// </remarks>
    public class JobDriver_ChangeAtNightStand : JobDriver
    {
        private int duration;
        private List<Apparel> toWear = new List<Apparel>();
        private List<Apparel> toPark = new List<Apparel>();

        private Building_OutfitStand Stand => job.targetA.Thing as Building_OutfitStand;

        private CompNightStand Comp => Stand?.GetComp<CompNightStand>();

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref duration, "NightChange_duration", 0);
            Scribe_Collections.Look(ref toWear, "NightChange_toWear", LookMode.Reference);
            Scribe_Collections.Look(ref toPark, "NightChange_toPark", LookMode.Reference);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                toWear ??= new List<Apparel>();
                toPark ??= new List<Apparel>();
            }
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        public override void Notify_Starting()
        {
            base.Notify_Starting();

            CompNightStand comp = Comp;
            if (comp == null)
            {
                return;
            }

            SwapPlan plan = comp.Borrower == pawn
                ? SwapPlan.ForUndressing(pawn, comp)
                : SwapPlan.ForDressing(pawn, Stand);

            toWear = plan?.ToWear ?? new List<Apparel>();
            toPark = plan?.ToPark ?? new List<Apparel>();

            duration = 0;
            foreach (Apparel apparel in toWear.Concat(toPark))
            {
                duration += (int)(apparel.GetStatValue(StatDefOf.EquipDelay) * 60f);
            }
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnBurningImmobile(TargetIndex.A);

            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell)
                .FailOnDespawnedNullOrForbidden(TargetIndex.A);

            Toil change = ToilMaker.MakeToil("NightChange_Change");
            change.WithProgressBarToilDelay(TargetIndex.A);
            change.defaultCompleteMode = ToilCompleteMode.Delay;
            change.defaultDuration = duration;
            yield return change;

            Toil transfer = ToilMaker.MakeToil("NightChange_Transfer");
            transfer.AddFinishAction(DoTransfer);
            transfer.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return transfer;
        }

        /// <summary>
        /// The ordering is not optional.
        /// </summary>
        /// <remarks>
        /// <b>Every apparel removal destroys the forced flag.</b>
        /// <c>Pawn_ApparelTracker.Notify_ApparelRemoved</c> calls <c>SetForced(ap, false)</c>
        /// unconditionally: the moment a force-worn duster is parked in the stand, the fact that it
        /// was force-worn ceases to exist anywhere in the game. So it is captured <b>before</b> the
        /// removal and restored <b>after</b> the wear. Vanilla's own driver instead force-wears
        /// everything it hands back, street clothes included, which is the opposite error.
        /// </remarks>
        private void DoTransfer()
        {
            Building_OutfitStand stand = Stand;
            CompNightStand comp = Comp;
            if (stand == null || comp == null)
            {
                return;
            }

            bool returning = comp.Borrower == pawn;

            List<Apparel> park = toPark
                .Where(a => a != null && !a.Destroyed && pawn.apparel.WornApparel.Contains(a)
                            && !pawn.apparel.IsLocked(a))
                .ToList();

            List<Apparel> wear = toWear
                .Where(a => a != null && !a.Destroyed && stand.HeldItems.Contains(a)
                            && Wearability.CanWear(pawn, a))
                .ToList();

            List<Apparel> forcedAtCheckIn = park
                .Where(a => pawn.outfits.forcedHandler.IsForced(a))
                .ToList();

            foreach (Apparel apparel in park)
            {
                pawn.apparel.Remove(apparel);
            }

            foreach (Apparel apparel in wear)
            {
                stand.RemoveApparel(apparel);
                pawn.apparel.Wear(apparel, dropReplacedApparel: false);
            }

            foreach (Apparel apparel in park)
            {
                stand.AddApparel(apparel);
            }

            if (returning)
            {
                // Garments that were forced come back forced; garments that were not stay
                // policy-managed. And the flag on the night clothes must go, or they are pinned to
                // the pawn forever.
                foreach (Apparel apparel in wear)
                {
                    if (comp.WasForced(apparel))
                    {
                        pawn.outfits.forcedHandler.SetForced(apparel, forced: true);
                    }
                }

                comp.NotifyUndressed();
            }
            else
            {
                // Without the flag, JobGiver_OptimizeApparel un-swaps the night clothes at its next
                // tick.
                foreach (Apparel apparel in wear)
                {
                    pawn.outfits.forcedHandler.SetForced(apparel, forced: true);
                }

                comp.NotifyDressed(pawn, park, wear, forcedAtCheckIn);
            }

            NightChangeTracker.Current?.NotifyChanged(pawn);
        }
    }
}
