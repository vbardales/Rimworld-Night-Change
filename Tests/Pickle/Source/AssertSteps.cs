using System.Collections.Generic;
using System.Linq;
using RimWorks.Pickle;
using RimWorld;
using Verse;

namespace NightChange.PickleSteps
{
    /// <summary>
    /// What Pickle's own steps cannot read: the mod's ledger, what the stand holds and whether a
    /// garment is forced. Each failure prints the actual values, since a report keeps no stack trace.
    /// </summary>
    [PickleSteps]
    public class AssertSteps
    {
        private static string Describe(IEnumerable<Thing> things) =>
            string.Join(", ", things.Select(t => t.def.defName).ToArray());

        [Then("Night Change: {string} is wearing {string}")]
        public void IsWearing(PickleContext ctx, string name, string defName) =>
            Driver.Worn(ctx, Driver.Pawn(ctx, name), defName);

        [Then("Night Change: {string} is not wearing {string}")]
        public void IsNotWearing(PickleContext ctx, string name, string defName)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            ctx.Assert(!Driver.WornOf(pawn, defName).Any(),
                $"{name} wears {defName}; they wear: "
                + string.Join(", ", pawn.apparel.WornApparel.Select(a => a.def.defName).ToArray()));
        }

        [Then("Night Change: the stand holds {string}")]
        public void StandHolds(PickleContext ctx, string defName)
        {
            var stand = Driver.Stand(ctx);
            ctx.Assert(stand.HeldItems.Any(t => t.def.defName == defName),
                $"the stand does not hold {defName}; it holds: {Describe(stand.HeldItems)}");
        }

        [Then("Night Change: the stand does not hold {string}")]
        public void StandDoesNotHold(PickleContext ctx, string defName)
        {
            var stand = Driver.Stand(ctx);
            ctx.Assert(!stand.HeldItems.Any(t => t.def.defName == defName),
                $"the stand holds {defName}; it holds: {Describe(stand.HeldItems)}");
        }

        [Then("Night Change: the stand's borrower is {string}")]
        public void BorrowerIs(PickleContext ctx, string name)
        {
            Pawn borrower = Driver.Comp(ctx).Borrower;
            ctx.Assert(borrower != null && borrower.LabelShort == name,
                $"the stand's borrower is {(borrower == null ? "nobody" : borrower.LabelShort)}, expected {name}");
        }

        [Then("Night Change: the stand has no borrower")]
        public void NoBorrower(PickleContext ctx)
        {
            Pawn borrower = Driver.Comp(ctx).Borrower;
            ctx.Assert(borrower == null, $"the stand still names {borrower?.LabelShort} as its borrower");
        }

        [Then("Night Change: the worn {string} of {string} is still forced")]
        public void StillForced(PickleContext ctx, string defName, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            Apparel apparel = Driver.Worn(ctx, pawn, defName);
            ctx.Assert(pawn.outfits.forcedHandler.IsForced(apparel),
                $"{name}'s {defName} is not forced: the flag was lost on the way to the stand and back");
        }

        [Then("Night Change: the worn {string} of {string} is not forced")]
        public void NotForced(PickleContext ctx, string defName, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            Apparel apparel = Driver.Worn(ctx, pawn, defName);
            ctx.Assert(!pawn.outfits.forcedHandler.IsForced(apparel),
                $"{name}'s {defName} is forced: it would stay pinned to them");
        }

        /// <summary>
        /// Worn, in the stand, or lying on the floor of the bedroom: every garment of the fixture, so a
        /// change that loses or duplicates one is seen whatever became of it.
        /// </summary>
        private static int Garments(PickleContext ctx)
        {
            Map map = Driver.Map(ctx);
            Driver.Origin o = Driver.Room(ctx);
            int total = 0;
            foreach (Pawn pawn in map.mapPawns.FreeColonists) total += pawn.apparel.WornApparelCount;
            foreach (Building_OutfitStand stand in map.listerBuildings.AllBuildingsColonistOfClass<Building_OutfitStand>())
                total += stand.HeldItems.Count(t => t is Apparel);
            for (int dx = 1; dx <= 5; dx++)
                for (int dz = 1; dz <= 5; dz++)
                    total += Driver.Cell(o, dx, dz).GetThingList(map).Count(t => t is Apparel);
            return total;
        }

        [When("Night Change: I count the garments of the bedroom")]
        public void CountGarments(PickleContext ctx) => ctx.Set(new GarmentCount { Value = Garments(ctx) });

        [Then("Night Change: the bedroom holds as many garments as counted")]
        public void SameGarments(PickleContext ctx)
        {
            int before = ctx.Get<GarmentCount>().Value;
            int now = Garments(ctx);
            ctx.Assert(before == now, $"the bedroom held {before} garments before and holds {now} now: one was lost or duplicated");
        }

        private sealed class GarmentCount
        {
            public int Value;
        }

        [Then("Night Change: the mod has not disabled itself")]
        public void NotDisabled(PickleContext ctx) =>
            ctx.Assert(!FailOpen.Disabled, "FailOpen.Disabled is true: a hook threw and the mod switched itself off, see the log");

        [Then("Night Change: the stand carries {int} assignable comps")]
        public void AssignableComps(PickleContext ctx, int expected)
        {
            int count = Driver.Stand(ctx).AllComps.OfType<CompAssignableToPawn>().Count();
            ctx.Assert(count == expected, $"the stand carries {count} CompAssignableToPawn, expected {expected}");
        }
    }
}
