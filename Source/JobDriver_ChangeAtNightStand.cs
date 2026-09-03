using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace NightChange
{
    /// <summary>
    /// Le passage au portant, dans un sens ou dans l'autre.
    /// </summary>
    /// <remarks>
    /// Le sens n'est pas dans le job : il se lit sur le grand livre a l'arrivee. Si le portant tient
    /// deja les habits de ce pion, c'est un retour ; sinon c'est un depart. Un job scribe qui
    /// reprend apres un chargement retrouve donc toujours le bon sens, sans champ supplementaire.
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
        /// L'ordre n'est pas negociable.
        /// </summary>
        /// <remarks>
        /// <b>Chaque retrait detruit le marqueur de port force.</b>
        /// <c>Pawn_ApparelTracker.Notify_ApparelRemoved</c> appelle <c>SetForced(ap, false)</c> sans
        /// condition : a l'instant ou un duster porte de force entre dans le portant, le fait qu'il
        /// etait force cesse d'exister ou que ce soit dans le jeu. On le note donc <b>avant</b> le
        /// retrait, et on le repose <b>apres</b> l'habillage. Le pilote vanilla, lui, force tout ce
        /// qu'il distribue, y compris les habits de ville au retour : l'erreur inverse.
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
                // Ce qui etait force revient force ; ce qui ne l'etait pas repasse sous la politique
                // vestimentaire. Et le marqueur pose sur la tenue de nuit doit partir, sinon elle
                // reste epinglee au pion pour toujours.
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
                // Sans marqueur, JobGiver_OptimizeApparel deshabille le pyjama a son prochain tour.
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
