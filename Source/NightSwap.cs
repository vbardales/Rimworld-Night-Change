using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace NightChange
{
    /// <summary>
    /// What a trip to the stand would move, for this pawn, in this direction.
    /// </summary>
    /// <remarks>
    /// "Is this trip worth taking?" and "what do I move on arrival?" are asked in two different
    /// places - by the stand selector and by the job driver. Two implementations diverge: the
    /// selector ends up picking a stand holding nothing the pawn can wear, the pawn walks there,
    /// and moves nothing. So there is one authority, and <see cref="Wearability"/> is called from
    /// here and nowhere else.
    /// </remarks>
    public class SwapPlan
    {
        /// <summary>What the pawn will put on (currently in the stand, or worn).</summary>
        public List<Apparel> ToWear = new List<Apparel>();

        /// <summary>What they will park in the stand.</summary>
        public List<Apparel> ToPark = new List<Apparel>();

        public bool MovesAnything => ToWear.Count > 0 || ToPark.Count > 0;

        /// <summary>
        /// Outbound: the pawn takes the night clothes and parks their day clothes.
        /// </summary>
        /// <remarks>
        /// Unlike a lab coat, night clothes <b>replace</b> what is worn rather than layering over
        /// it. So everything unlocked is parked, not only what conflicts. That is also what makes
        /// <see cref="TemperatureGuard"/> necessary: going to bed undressed in a room at -5 kills.
        /// </remarks>
        public static SwapPlan ForDressing(Pawn pawn, Building_OutfitStand stand)
        {
            var plan = new SwapPlan();

            foreach (Thing thing in stand.HeldItems)
            {
                if (thing is Apparel apparel
                    && Wearability.CanWear(pawn, apparel)
                    && !pawn.apparel.WornApparel.Contains(apparel))
                {
                    plan.ToWear.Add(apparel);
                }
            }

            if (plan.ToWear.Count == 0)
            {
                return null;
            }

            // A stand is one outfit's worth, structurally (HasRoomForApparelOfDef refuses anything
            // that cannot be worn together with what is already inside), but another mod may have
            // widened the filter: keep only what can be worn together.
            for (int i = plan.ToWear.Count - 1; i >= 1; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (!ApparelUtility.CanWearTogether(plan.ToWear[i].def, plan.ToWear[j].def, pawn.RaceProps.body))
                    {
                        plan.ToWear.RemoveAt(i);
                        break;
                    }
                }
            }

            plan.ToPark.AddRange(pawn.apparel.WornApparel.Where(a => !pawn.apparel.IsLocked(a)));
            return plan;
        }

        /// <summary>
        /// Return trip: the pawn takes back what they parked and gives back what they took.
        /// </summary>
        /// <remarks>
        /// Built from the ledger, never from the assigned owner: the stand may have been
        /// reassigned overnight. Only the pieces still where the ledger says they are count - the
        /// player may have ejected a garment, or a raid stripped the pawn.
        /// </remarks>
        public static SwapPlan ForUndressing(Pawn pawn, CompNightStand comp)
        {
            if (comp.Borrower != pawn)
            {
                return null;
            }

            var plan = new SwapPlan();
            var held = comp.Stand.HeldItems;

            foreach (Apparel apparel in comp.Parked)
            {
                if (held.Contains(apparel) && Wearability.CanWear(pawn, apparel))
                {
                    plan.ToWear.Add(apparel);
                }
            }

            foreach (Apparel apparel in comp.Taken)
            {
                if (pawn.apparel.WornApparel.Contains(apparel) && !pawn.apparel.IsLocked(apparel))
                {
                    plan.ToPark.Add(apparel);
                }
            }

            return plan.MovesAnything ? plan : null;
        }
    }

    public static class Wearability
    {
        public static bool CanWear(Pawn pawn, Apparel apparel)
        {
            if (apparel == null || apparel.Destroyed || pawn.apparel == null)
            {
                return false;
            }

            if (!apparel.PawnCanWear(pawn) || !ApparelUtility.HasPartsToWear(pawn, apparel.def))
            {
                return false;
            }

            CompBiocodable biocode = apparel.TryGetComp<CompBiocodable>();
            return biocode == null || !biocode.Biocoded || biocode.CodedPawn == pawn;
        }
    }

    /// <summary>
    /// The stand that serves as the bedroom wardrobe, if there is one.
    /// </summary>
    public static class StandFinder
    {
        public static Building_OutfitStand ForBed(Pawn pawn, Building_Bed bed, out SwapPlan plan)
        {
            plan = null;
            if (bed?.Map == null || pawn.apparel == null)
            {
                return null;
            }

            Room room = bed.GetRoom();
            if (room == null || room.PsychologicallyOutdoors)
            {
                return null;
            }

            int maxDist = NightChangeMod.Settings.maxStandDistance;

            foreach (Building_OutfitStand stand in
                     bed.Map.listerBuildings.AllBuildingsColonistOfClass<Building_OutfitStand>())
            {
                if (stand.GetRoom() != room || !stand.Position.InHorDistOf(bed.Position, maxDist))
                {
                    continue;
                }

                CompNightStand comp = stand.GetComp<CompNightStand>();
                if (comp == null || !Serves(comp, pawn, bed))
                {
                    continue;
                }

                if (!pawn.CanReserveAndReach(stand, PathEndMode.InteractionCell, Danger.Some))
                {
                    continue;
                }

                SwapPlan candidate = SwapPlan.ForDressing(pawn, stand);
                if (candidate == null || !TemperatureGuard.Allows(pawn, bed, candidate))
                {
                    continue;
                }

                plan = candidate;
                return stand;
            }

            return null;
        }

        /// <summary>
        /// An assigned stand serves only its owners. A free stand serves whoever owns a bed in the
        /// room - the common case, a bedroom, and it needs no setup at all. In a barracks several
        /// pawns may lay claim to the same stand: first come, first served, and the rest go to bed
        /// dressed. That is what explicit assignment fixes.
        /// </summary>
        private static bool Serves(CompNightStand comp, Pawn pawn, Building_Bed bed)
        {
            if (comp.InUse && comp.Borrower != pawn)
            {
                return false;
            }

            List<Pawn> assigned = comp.AssignedPawnsForReading;
            if (assigned.Count > 0)
            {
                return assigned.Contains(pawn);
            }

            if (!NightChangeMod.Settings.inheritOwnerFromBed)
            {
                return false;
            }

            List<Pawn> owners = bed.OwnersForReading;
            return owners.Count == 0 || owners.Contains(pawn);
        }
    }

    /// <summary>
    /// Refuses the change when the bedroom is too cold for the night clothes.
    /// </summary>
    /// <remarks>
    /// <c>ComfyTemperatureMin</c> read on the pawn already accounts for what they are wearing. So
    /// the <b>difference</b> in insulation between what they will put on and what they park is
    /// applied to it, and compared against the temperature of the bed's cell. Vanilla does not
    /// protect against this: nothing stops a colonist sleeping naked at -20, it is simply that
    /// nothing normally sends them there.
    /// </remarks>
    public static class TemperatureGuard
    {
        public static bool Allows(Pawn pawn, Building_Bed bed, SwapPlan plan)
        {
            if (!NightChangeMod.Settings.coldGuard)
            {
                return true;
            }

            float delta = Insulation(plan.ToWear) - Insulation(plan.ToPark);
            if (delta >= 0f)
            {
                return true;
            }

            float minNow = pawn.GetStatValue(StatDefOf.ComfyTemperatureMin);
            float minAfter = minNow - delta;
            float roomTemp = bed.Position.GetTemperature(bed.Map);

            return roomTemp >= minAfter + NightChangeMod.Settings.coldGuardMargin;
        }

        private static float Insulation(List<Apparel> set)
        {
            float total = 0f;
            foreach (Apparel apparel in set)
            {
                total += apparel.GetStatValue(StatDefOf.Insulation_Cold);
            }

            return total;
        }
    }
}
