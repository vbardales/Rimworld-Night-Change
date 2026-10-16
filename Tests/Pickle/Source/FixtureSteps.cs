using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RimWorks.Pickle;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace NightChange.PickleSteps
{
    /// <summary>
    /// Builds the situation and makes the game do the thing. Every step text starts with
    /// "Night Change:", because Pickle loads the steps of every active suite into one namespace and
    /// two suites declaring the same text make healthy scenarios fail with "Ambiguous step". No text
    /// uses parentheses or slashes, which Cucumber expressions read as optional text and alternatives.
    ///
    /// The morning and the evening are not waited for. A pawn is sent to bed through
    /// Pawn_JobTracker.StartJob with an ordinary LayDown job, which is exactly the call vanilla's
    /// JobGiver_GetRest ends in and the one the mod's prefix hooks. The hour of the day is set with
    /// Pickle's own "I set the hour to {int}".
    /// </summary>
    [PickleSteps]
    public class FixtureSteps
    {
        // ------------------------------------------------------------------ the bedroom

        [Given("Night Change: a bedroom is built at x={int} z={int}")]
        public async Task BuildBedroom(PickleContext ctx, int x, int z)
        {
            Map map = Driver.Map(ctx);
            var origin = new Driver.Origin { X = x, Z = z };
            ctx.Set(origin);

            // test-colony carries hostile insects and hives (PickleTools/docs/FIXTURES.md): they would keep the
            // danger watcher off StoryDanger.None and spawn more. Take them, and their buildings, off the map.
            foreach (Thing hostile in map.listerThings.AllThings
                         .Where(t => t.Faction != null && t.Faction != Faction.OfPlayer && t.Faction.HostileTo(Faction.OfPlayer))
                         .ToList())
            {
                if (!hostile.Destroyed) hostile.Destroy(DestroyMode.Vanish);
            }

            ThingDef wall = ThingDefOf.Wall;
            ThingDef door = ThingDefOf.Door;
            for (int dx = 0; dx <= 6; dx++)
            {
                for (int dz = 0; dz <= 6; dz++)
                {
                    IntVec3 cell = Driver.Cell(origin, dx, dz);
                    ctx.Require(cell.InBounds(map), $"x={cell.x} z={cell.z} is off the map: build the bedroom further from the edge");
                    Driver.Clear(map, cell);
                    bool ring = dx == 0 || dx == 6 || dz == 0 || dz == 6;
                    if (ring)
                    {
                        Driver.SpawnAt(ctx, dx == 3 && dz == 0 ? door : wall, cell, Rot4.North);
                    }
                    else
                    {
                        map.terrainGrid.SetTerrain(cell, TerrainDefOf.WoodPlankFloor);
                    }

                    map.roofGrid.SetRoof(cell, RoofDefOf.RoofConstructed);
                }
            }

            map.regionAndRoomUpdater.TryRebuildDirtyRegionsAndRooms();
            await ctx.WaitFrames(2);

            Room room = Driver.Cell(origin, 3, 3).GetRoom(map);
            ctx.Assert(room != null && !room.PsychologicallyOutdoors,
                "the walled and roofed cells did not make an indoor room: the wall ring, the door or the roof failed");
        }

        [Given("Night Change: {string} is placed in the bedroom")]
        public void PlacePawn(PickleContext ctx, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            pawn.Position = Driver.Cell(Driver.Room(ctx), 3, 1);
            pawn.Notify_Teleported();
            pawn.jobs?.EndCurrentJob(JobCondition.InterruptForced);
        }

        [Given("Night Change: {string} owns the first bed of the bedroom")]
        public void OwnFirstBed(PickleContext ctx, string name) => Own(ctx, name, 0, 2, 2);

        [Given("Night Change: {string} owns a second bed of the bedroom")]
        public void OwnSecondBed(PickleContext ctx, string name) => Own(ctx, name, 1, 4, 2);

        private static void Own(PickleContext ctx, string name, int index, int dx, int dz)
        {
            Driver.Origin origin = Driver.Room(ctx);
            Pawn pawn = Driver.Pawn(ctx, name);
            Building_Bed bed = Driver.SpawnAt(ctx, ThingDefOf.Bed, Driver.Cell(origin, dx, dz), Rot4.North) as Building_Bed;
            ctx.Assert(bed != null, "the Bed def did not produce a Building_Bed");
            bed.CompAssignableToPawn.TryAssignPawn(pawn);
            ctx.Assert(bed.OwnersForReading.Contains(pawn), $"{name} could not be assigned to the bed at index {index}");
        }

        [Given("Night Change: an outfit stand is placed in the bedroom")]
        public void PlaceStand(PickleContext ctx) => PlaceStandAt(ctx, Driver.StandDef, 3, 4);

        [Given("Night Change: an outfit stand is placed in the far corner of the bedroom")]
        public void PlaceFarStand(PickleContext ctx) => PlaceStandAt(ctx, Driver.StandDef, 5, 5);

        [Given("Night Change: a kid outfit stand is placed in the bedroom")]
        public void PlaceKidStand(PickleContext ctx) => PlaceStandAt(ctx, Driver.KidStandDef, 3, 4);

        private static void PlaceStandAt(PickleContext ctx, string defName, int dx, int dz)
        {
            Driver.Origin origin = Driver.Room(ctx);
            Driver.SpawnAt(ctx, Driver.Def(ctx, defName), Driver.Cell(origin, dx, dz), Rot4.South);
        }

        [Given("Night Change: I hang {string} on the stand")]
        public void HangOnStand(PickleContext ctx, string defName)
        {
            Building_OutfitStand stand = Driver.Stand(ctx);
            Apparel apparel = Driver.MakeApparel(ctx, defName);
            stand.AddApparel(apparel);
            ctx.Assert(stand.HeldItems.Contains(apparel), $"the stand refused {defName}: it holds "
                + string.Join(", ", stand.HeldItems.Select(t => t.def.defName).ToArray()));
        }

        [Given("Night Change: the stand is assigned to {string}")]
        public void AssignStand(PickleContext ctx, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            CompNightStand comp = Driver.Comp(ctx);
            comp.TryAssignPawn(pawn);
            ctx.Assert(comp.AssignedPawnsForReading.Contains(pawn), $"{name} could not be assigned to the stand");
        }

        [Given("Night Change: {string} is dressed in {string}")]
        public void Dress(PickleContext ctx, string name, string defName)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            pawn.apparel.Wear(Driver.MakeApparel(ctx, defName), dropReplacedApparel: false);
            ctx.Assert(Driver.WornOf(pawn, defName).Any(), $"{name} did not end up wearing {defName}");
        }

        [Given("Night Change: the worn {string} of {string} is forced")]
        public void Force(PickleContext ctx, string defName, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            Apparel apparel = Driver.Worn(ctx, pawn, defName);
            pawn.outfits.forcedHandler.SetForced(apparel, true);
            ctx.Assert(pawn.outfits.forcedHandler.IsForced(apparel), $"{defName} is not forced after SetForced");
        }

        [Given("Night Change: the bedroom is at {int} degrees")]
        public void RoomTemperature(PickleContext ctx, int degrees)
        {
            Room room = Driver.Cell(Driver.Room(ctx), 3, 3).GetRoom(Driver.Map(ctx));
            ctx.Assert(room != null, "the bedroom has no room");
            room.Temperature = degrees;
            ctx.Assert(Math.Abs(room.Temperature - degrees) < 1f, $"the room reads {room.Temperature} after being set to {degrees}");
        }

        [Given("Night Change: {string} has a wound that calls for medical rest")]
        public void MedicalRest(PickleContext ctx, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            BodyPartRecord torso = pawn.RaceProps.body.corePart;
            Hediff wound = HediffMaker.MakeHediff(HediffDefOf.Cut, pawn, torso);
            wound.Severity = 6f;
            pawn.health.AddHediff(wound);
            ctx.Assert(HealthAIUtility.ShouldSeekMedicalRest(pawn),
                $"{name} does not need medical rest with this wound: the fixture is broken, not the mod");
        }

        // ------------------------------------------------------------------ the settings

        private sealed class SettingsBackup
        {
            public bool Inherit;
            public bool Cold;
            public float Margin;
            public int Distance;
        }

        /// <summary>
        /// Setting names are the field names of NightChangeSettings. Whatever a scenario changes is
        /// put back by the hook below, pass or fail, so the next scenario starts from the defaults.
        /// </summary>
        [Given("Night Change: the setting {word} is {word}")]
        public void SetSetting(PickleContext ctx, string name, string value)
        {
            NightChangeSettings settings = NightChangeMod.Settings;
            ctx.Require(settings != null, "NightChangeMod.Settings is null: the mod is not loaded");
            Remember(ctx, settings);
            FieldInfo field = typeof(NightChangeSettings).GetField(name);
            ctx.Require(field != null, $"no setting named {name}; NightChangeSettings has: "
                + string.Join(", ", typeof(NightChangeSettings).GetFields().Select(f => f.Name).ToArray()));
            object parsed = field.FieldType == typeof(bool) ? (object)bool.Parse(value)
                : field.FieldType == typeof(int) ? (object)int.Parse(value)
                : (object)float.Parse(value, System.Globalization.CultureInfo.InvariantCulture);
            field.SetValue(settings, parsed);
        }

        private static void Remember(PickleContext ctx, NightChangeSettings s)
        {
            try { ctx.Get<SettingsBackup>(); return; }
            catch (InvalidOperationException) { }
            ctx.Set(new SettingsBackup
            {
                Inherit = s.inheritOwnerFromBed,
                Cold = s.coldGuard,
                Margin = s.coldGuardMargin,
                Distance = s.maxStandDistance,
            });
        }

        [AfterScenario]
        public void PutSettingsBack(PickleContext ctx)
        {
            SettingsBackup backup;
            try { backup = ctx.Get<SettingsBackup>(); }
            catch (Exception) { return; }
            NightChangeSettings s = NightChangeMod.Settings;
            if (s == null || backup == null) return;
            s.inheritOwnerFromBed = backup.Inherit;
            s.coldGuard = backup.Cold;
            s.coldGuardMargin = backup.Margin;
            s.maxStandDistance = backup.Distance;
        }

        // ------------------------------------------------------------------ the actions

        /// <summary>
        /// The call vanilla's JobGiver_GetRest ends in: an ordinary, unforced LayDown on the pawn's bed.
        /// </summary>
        [When("Night Change: {string} goes to bed of their own accord")]
        public void GoToBed(PickleContext ctx, string name) => LayDown(ctx, name, forced: false);

        [When("Night Change: {string} is ordered to bed by the player")]
        public void OrderToBed(PickleContext ctx, string name) => LayDown(ctx, name, forced: true);

        private static void LayDown(PickleContext ctx, string name, bool forced)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            Building_Bed bed = pawn.ownership?.OwnedBed;
            ctx.Assert(bed != null, $"{name} owns no bed");
            Job job = JobMaker.MakeJob(JobDefOf.LayDown, bed);
            job.playerForced = forced;
            pawn.jobs.StartJob(job, JobCondition.InterruptForced, null, false, true);
            ctx.Assert(pawn.CurJob != null, $"{name} has no job after being sent to bed");
        }

        [When("Night Change: {string} gets up")]
        public void GetUp(PickleContext ctx, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            pawn.jobs.ClearQueuedJobs();
            pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
        }

        [When("Night Change: the stand is destroyed")]
        public void DestroyStand(PickleContext ctx) => Driver.Stand(ctx).Destroy(DestroyMode.Deconstruct); // Vanish deletes the held clothes; deconstruction drops them, as the mod relies on

        [When("Night Change: {string} is banished")]
        public void Banish(PickleContext ctx, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            PawnBanishUtility.Banish(pawn, pawn.Map.Tile);
        }

        [When("Night Change: {string} is killed")]
        public void Kill(PickleContext ctx, string name) => Driver.Pawn(ctx, name).Kill(null);

        /// <summary>
        /// What a sale does to ownership: Pawn.PreTraded runs Pawn_Ownership.UnclaimAll (decompiled,
        /// first thing after dropping the pawn's belongings), then the pawn leaves the map. PreTraded
        /// itself is not called: with a fixture trader it raises a tale error (SoldPrisoner, no trader
        /// kind) that Pickle counts as a failure, and the trade window is not driven either.
        /// </summary>
        [When("Night Change: {string} is sold")]
        public void Sell(PickleContext ctx, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            pawn.ownership.UnclaimAll();
            if (pawn.Spawned) pawn.DeSpawn();
        }
        [When("Night Change: {string} is downed")]
        public void Down(PickleContext ctx, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            HealthUtility.DamageUntilDowned(pawn, false);
            ctx.Assert(pawn.Downed, $"{name} is not downed after the damage: the fixture is broken, not the mod");
        }

        [When("Night Change: {string} is healed")]
        public void Heal(PickleContext ctx, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs.ToList()) pawn.health.RemoveHediff(hediff);
            ctx.Assert(!pawn.Downed, $"{name} is still downed after every hediff was removed");
        }

        /// <summary>
        /// A fire on the stand's own cell: the job's FailOnBurningImmobile is what the scenario exercises.
        /// </summary>
        [When("Night Change: a fire starts on the stand")]
        public void FireOnStand(PickleContext ctx)
        {
            Building_OutfitStand stand = Driver.Stand(ctx);
            ctx.Assert(FireUtility.TryStartFireIn(stand.Position, Driver.Map(ctx), 1f, null), "the fire did not start on the stand");
        }

        /// <summary>
        /// The garment leaves the stand and lands on the floor of the bedroom, as when a player takes it
        /// out by hand: it still exists, only the ledger no longer finds it where it parked it.
        /// </summary>
        [When("Night Change: the parked {string} is taken off the stand")]
        public void TakeOffStand(PickleContext ctx, string defName)
        {
            Building_OutfitStand stand = Driver.Stand(ctx);
            Apparel apparel = stand.HeldItems.OfType<Apparel>().FirstOrDefault(a => a.def.defName == defName);
            ctx.Assert(apparel != null, $"the stand does not hold {defName}; it holds: "
                + string.Join(", ", stand.HeldItems.Select(t => t.def.defName).ToArray()));
            stand.RemoveApparel(apparel);
            GenPlace.TryPlaceThing(apparel, stand.Position, stand.Map, ThingPlaceMode.Near);
            ctx.Assert(apparel.Spawned, $"the {defName} taken off the stand did not land on the floor");
        }

        /// <summary>
        /// The outfit stand is passable and has no interaction cell of its own, so one wall is not
        /// enough (ticket E1, 2026-10-16: Alice changed anyway). Every neighbour of the stand that is
        /// free of buildings gets a wall. A bed cell next to the stand stays: the diagonal step from it
        /// is closed by the walls beside it. The step proves its fixture: if a colonist in the room can
        /// still reach the stand it fails on that line.
        /// </summary>
        [When("Night Change: the stand's way is walled up")]
        public void WallUp(PickleContext ctx)
        {
            Building_OutfitStand stand = Driver.Stand(ctx);
            Map map = stand.Map;
            foreach (IntVec3 cell in GenAdj.CellsAdjacent8Way(stand).ToList())
            {
                if (cell.GetThingList(map).Any(t => t is Building_Bed)) continue;
                Driver.Clear(map, cell);
                Driver.SpawnAt(ctx, ThingDefOf.Wall, cell, Rot4.North);
            }

            map.regionAndRoomUpdater.TryRebuildDirtyRegionsAndRooms();
            Driver.Origin o = Driver.Room(ctx);
            Pawn probe = map.mapPawns.FreeColonistsSpawned.FirstOrDefault(p =>
                p.Position.x > o.X && p.Position.x < o.X + 6 && p.Position.z > o.Z && p.Position.z < o.Z + 6);
            ctx.Require(probe != null, "no colonist in the bedroom to probe the path with");
            ctx.Assert(!probe.CanReach(stand, PathEndMode.InteractionCell, Danger.Deadly),
                $"the stand is still reachable from {probe.Position} with its neighbours walled: the fixture is broken, not the mod");
        }

        [When("Night Change: the walls around the stand are removed")]
        public void WallDown(PickleContext ctx)
        {
            Building_OutfitStand stand = Driver.Stand(ctx);
            foreach (IntVec3 cell in GenAdj.CellsAdjacent8Way(stand).ToList())
            {
                foreach (Thing wall in cell.GetThingList(stand.Map).Where(t => t.def == ThingDefOf.Wall).ToList())
                    wall.Destroy(DestroyMode.Vanish);
            }

            stand.Map.regionAndRoomUpdater.TryRebuildDirtyRegionsAndRooms();
        }
        /// <summary>
        /// A raider on the map makes the danger watcher leave StoryDanger.None. The watcher updates
        /// on its own cadence, so the scenario waits ticks and then asserts the danger.
        /// </summary>
        [When("Night Change: a raider stands at the edge of the colony")]
        public void SpawnRaider(PickleContext ctx)
        {
            Map map = Driver.Map(ctx);
            Faction enemy = Find.FactionManager.RandomEnemyFaction(allowHidden: false, allowDefeated: false,
                allowNonHumanlike: false);
            ctx.Require(enemy != null, "the world has no hostile humanlike faction to send a raider from");
            // One raider is not enough (assumption): the danger watcher may need several hostiles to leave
            // StoryDanger.None. Ticket A, 2026-10-14: a lone raider never moved it in 1800 ticks.
            List<Pawn> raiders = new List<Pawn>();
            for (int i = 0; i < 4; i++)
            {
                Pawn raider = PawnGenerator.GeneratePawn(PawnKindDefOf.Pirate, enemy);
                GenSpawn.Spawn(raider, CellFinder.RandomEdgeCell(map), map);
                ctx.Assert(raider.Spawned, "a raider did not spawn");
                raiders.Add(raider);
            }

            // The danger watcher counts only hostiles that belong to an active lord, not a lone pawn.
            LordMaker.MakeNewLord(enemy, new LordJob_AssaultColony(enemy), map, raiders);
        }

        /// <summary>
        /// Diagnostic for ticket B (2026-10-14): who dresses the sleeper after "Change back now", and
        /// what both ledgers say. Written to the game log as a warning, read from Player.log.
        /// </summary>
        [Then("Night Change: report the state of {string}")]
        public void ReportState(PickleContext ctx, string name)
        {
            Pawn pawn = Driver.Pawn(ctx, name);
            Building_OutfitStand stand = Driver.Stand(ctx);
            CompNightStand ours = stand.GetComp<CompNightStand>();
            ThingComp theirs = stand.AllComps.FirstOrDefault(c => c.GetType().FullName != null
                && c.GetType().FullName.StartsWith("ShiftChange.CompShiftStand", StringComparison.Ordinal));
            object theirBorrower = theirs?.GetType().GetProperty("Borrower")?.GetValue(theirs, null);
            string worn = string.Join(", ", pawn.apparel.WornApparel.Select(a => a.def.defName).ToArray());
            string held = string.Join(", ", stand.HeldItems.Select(t => t.def.defName).ToArray());
            string queue = string.Join(", ", pawn.jobs.jobQueue.Select(q => q.job.def.defName).ToArray());
            Log.Warning("[NightChange diag] tick " + Find.TickManager.TicksGame + ", " + name
                + ": job=" + (pawn.CurJob?.def.defName ?? "none")
                + ", queue=[" + queue + "], worn=[" + worn + "], stand holds=[" + held + "]"
                + ", NightChange borrower=" + (ours?.Borrower?.LabelShort ?? "none")
                + ", ShiftChange comp=" + (theirs == null ? "absent" : "present")
                + ", ShiftChange borrower=" + ((theirBorrower as Pawn)?.LabelShort ?? "none")
                + ", timetable=" + pawn.timetable?.CurrentAssignment?.defName);
        }

        // ------------------------------------------------------------------ waits

        /// <summary>
        /// Ticks are waited in slices of sixty so that a paused game is unpaused by WaitTicks, which is
        /// the only wait that does it; a real-time WaitUntil on a state only ticks bring about would
        /// wait for ever on a paused game.
        /// </summary>
        [Then("Night Change: {string} ends up wearing {string}", TimeoutSeconds = 90f)]
        public async Task EndsUpWearing(PickleContext ctx, string name, string defName)
        {
            for (int i = 0; i < 60; i++)
            {
                if (Driver.WornOf(Driver.Pawn(ctx, name), defName).Any()) return;
                await ctx.WaitTicks(60);
            }

            Pawn pawn = Driver.Pawn(ctx, name);
            ctx.Assert(false, $"{name} still does not wear {defName} after 3600 ticks; they wear "
                + string.Join(", ", pawn.apparel.WornApparel.Select(a => a.def.defName).ToArray())
                + $" and their job is {pawn.CurJob?.def.defName ?? "none"}");
        }

        [Then("Night Change: the map reports danger", TimeoutSeconds = 60f)]
        public async Task DangerReported(PickleContext ctx)
        {
            for (int i = 0; i < 30; i++)
            {
                if (Driver.Map(ctx).dangerWatcher.DangerRating != StoryDanger.None) return;
                await ctx.WaitTicks(60);
            }

            ctx.Assert(false, "the danger watcher still reports StoryDanger.None 1800 ticks after the raider spawned");
        }

        [Then("Night Change: the map reports no danger")]
        public void NoDanger(PickleContext ctx) =>
            ctx.Assert(Driver.Map(ctx).dangerWatcher.DangerRating == StoryDanger.None, "the danger watcher reports danger");
    }
}
