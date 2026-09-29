using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorks.Pickle;
using RimWorld;
using UnityEngine;
using Verse;

namespace NightChange.PickleSteps
{
    /// <summary>
    /// Shared lookups for every step class here. Nothing is cached across steps: a save reload
    /// replaces every object in the game, so pawns, the bed and the stand are found again by name or
    /// by kind each time. Every miss names itself: a report keeps no stack trace.
    ///
    /// The bedroom the fixture builds has a fixed layout, taken from an origin (ox, oz):
    ///   walls        the ring from (ox, oz) to (ox+6, oz+6), a door at (ox+3, oz)
    ///   interior     (ox+1 .. ox+5, oz+1 .. oz+5), floored and roofed
    ///   first bed    (ox+2, oz+2), two cells tall
    ///   second bed   (ox+4, oz+2), two cells tall
    ///   near stand   (ox+3, oz+4)     2.2 cells from the first bed
    ///   far stand    (ox+5, oz+5)     4.2 cells from the first bed
    ///   pawns stand  (ox+3, oz+1)
    /// </summary>
    public static class Driver
    {
        public const string StandDef = "Building_OutfitStand";
        public const string KidStandDef = "Building_KidOutfitStand";
        public const string ChangeJob = "NightChange_ChangeAtStand";

        public sealed class Origin
        {
            public int X;
            public int Z;
        }

        public static Map Map(PickleContext ctx)
        {
            ctx.Require(Current.Game != null && Find.CurrentMap != null,
                "no current map: load the fixture ('the save \"test-colony\" is loaded') before this step");
            return Find.CurrentMap;
        }

        public static Origin Room(PickleContext ctx)
        {
            Origin origin;
            try { origin = ctx.Get<Origin>(); }
            catch (InvalidOperationException) { origin = null; }
            ctx.Require(origin != null, "no bedroom: 'Night Change: a bedroom is built at x=.. z=..' comes first");
            return origin;
        }

        public static IntVec3 Cell(Origin o, int dx, int dz) => new IntVec3(o.X + dx, 0, o.Z + dz);

        public static Pawn Pawn(PickleContext ctx, string name)
        {
            IReadOnlyList<Pawn> spawned = Map(ctx).mapPawns.AllPawnsSpawned;
            Pawn found = spawned.FirstOrDefault(p =>
                (p.Name is NameTriple triple && triple.Nick == name)
                || (p.Name is NameSingle single && single.Name == name)
                || p.LabelShort == name);
            ctx.Assert(found != null,
                $"no spawned pawn named \"{name}\"; the map holds: " + string.Join(", ", spawned.Select(p => p.LabelShort).ToArray()));
            return found;
        }

        public static Building_OutfitStand Stand(PickleContext ctx)
        {
            List<Building_OutfitStand> stands = Map(ctx).listerBuildings
                .AllBuildingsColonistOfClass<Building_OutfitStand>().ToList();
            ctx.Assert(stands.Count == 1,
                $"the bedroom should hold exactly one outfit stand, the map holds {stands.Count}");
            return stands[0];
        }

        public static CompNightStand Comp(PickleContext ctx)
        {
            Building_OutfitStand stand = Stand(ctx);
            CompNightStand comp = stand.GetComp<CompNightStand>();
            ctx.Assert(comp != null,
                $"{stand.def.defName} carries no NightChange.CompNightStand: the patch on it did not apply");
            return comp;
        }

        public static Building_Bed Bed(PickleContext ctx, int index)
        {
            List<Building_Bed> beds = Map(ctx).listerBuildings.AllBuildingsColonistOfClass<Building_Bed>()
                .OrderBy(b => b.Position.x).ToList();
            ctx.Assert(index < beds.Count, $"the bedroom holds {beds.Count} bed(s), the step asked for number {index + 1}");
            return beds[index];
        }

        public static ThingDef Def(PickleContext ctx, string defName)
        {
            ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
            ctx.Assert(def != null, $"no ThingDef named \"{defName}\"");
            return def;
        }

        public static Apparel MakeApparel(PickleContext ctx, string defName)
        {
            ThingDef def = Def(ctx, defName);
            ctx.Assert(def.IsApparel, $"{defName} is not apparel");
            return (Apparel)ThingMaker.MakeThing(def, GenStuff.DefaultStuffFor(def));
        }

        public static IEnumerable<Apparel> WornOf(Pawn pawn, string defName) =>
            pawn.apparel.WornApparel.Where(a => a.def.defName == defName);

        public static Apparel Worn(PickleContext ctx, Pawn pawn, string defName)
        {
            Apparel found = WornOf(pawn, defName).FirstOrDefault();
            ctx.Assert(found != null,
                $"{pawn.LabelShort} does not wear {defName}; they wear: "
                + string.Join(", ", pawn.apparel.WornApparel.Select(a => a.def.defName).ToArray()));
            return found;
        }

        public static Thing SpawnAt(PickleContext ctx, ThingDef def, IntVec3 cell, Rot4 rot)
        {
            Map map = Map(ctx);
            ctx.Require(cell.InBounds(map), $"x={cell.x} z={cell.z} is off the map: build the bedroom further from the edge");
            Thing thing = ThingMaker.MakeThing(def, def.MadeFromStuff ? GenStuff.DefaultStuffFor(def) : null);
            if (thing.def.CanHaveFaction) thing.SetFactionDirect(Faction.OfPlayer);
            GenSpawn.Spawn(thing, cell, map, rot);
            ctx.Assert(thing.Spawned, $"{def.defName} did not spawn at x={cell.x} z={cell.z}");
            return thing;
        }

        public static void Clear(Map map, IntVec3 cell)
        {
            foreach (Thing thing in cell.GetThingList(map).ToList())
            {
                if (thing is Pawn) continue;
                if (thing.def.destroyable) thing.Destroy(DestroyMode.Vanish);
            }
        }

        public static string Language(PickleContext ctx)
        {
            string folder = LanguageDatabase.activeLanguage?.folderName;
            ctx.Require(!string.IsNullOrEmpty(folder), "no active language: the game has not finished loading one");
            return folder;
        }

        internal const BindingFlags InstanceAny = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    }
}
