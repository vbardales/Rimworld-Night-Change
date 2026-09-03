using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NightChange
{
    /// <summary>
    /// The "who is in night clothes, and which stand holds their day clothes" index.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Nothing here is scribed: the truth lives in each <see cref="CompNightStand"/>'s ledger,
    /// which travels with the stand and survives saving. This index is only a cache, rebuilt on
    /// load.
    /// </para>
    /// <para>
    /// It exists for one reason: the <c>StartJob</c> prefix has to answer "is this pawn in night
    /// clothes?" for <b>every</b> job of <b>every</b> pawn. Sweeping every stand on the map each
    /// time would be paying for a building scan to get an answer that is no more than 99% of the
    /// time.
    /// </para>
    /// </remarks>
    public class NightChangeTracker : GameComponent
    {
        public static NightChangeTracker Current;

        private const int ChangeCooldownTicks = 1250;

        private readonly Dictionary<Pawn, CompNightStand> borrowers = new Dictionary<Pawn, CompNightStand>();

        /// <summary>
        /// Last tick this pawn changed, to stop a change/change-back loop. Not scribed: at worst a
        /// load opens a twenty-second window where the net does not catch, which is harmless.
        /// </summary>
        private readonly Dictionary<Pawn, int> lastChangeTick = new Dictionary<Pawn, int>();

        public NightChangeTracker(Game game)
        {
            Current = this;
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Rebuild();
        }

        public void Rebuild()
        {
            borrowers.Clear();
            lastChangeTick.Clear();

            List<Map> maps = Find.Maps;
            for (int i = 0; i < maps.Count; i++)
            {
                foreach (Building_OutfitStand stand in
                         maps[i].listerBuildings.AllBuildingsColonistOfClass<Building_OutfitStand>())
                {
                    CompNightStand comp = stand.GetComp<CompNightStand>();
                    if (comp?.Borrower != null)
                    {
                        borrowers[comp.Borrower] = comp;
                    }
                }
            }
        }

        public void Register(Pawn pawn, CompNightStand comp)
        {
            if (pawn != null)
            {
                borrowers[pawn] = comp;
            }
        }

        public void Unregister(Pawn pawn)
        {
            if (pawn != null)
            {
                borrowers.Remove(pawn);
            }
        }

        public void NotifyChanged(Pawn pawn)
        {
            if (pawn == null)
            {
                return;
            }

            int now = Find.TickManager.TicksGame;

            // Lazy purge: without it the dictionary would keep one entry per dead colonist for the
            // life of the game.
            if (lastChangeTick.Count > 128)
            {
                lastChangeTick.RemoveAll(kv => now - kv.Value >= ChangeCooldownTicks);
            }

            lastChangeTick[pawn] = now;
        }

        public bool ChangedRecently(Pawn pawn)
        {
            return pawn != null
                   && lastChangeTick.TryGetValue(pawn, out int tick)
                   && Find.TickManager.TicksGame - tick < ChangeCooldownTicks;
        }

        /// <summary>The stand holding this pawn's day clothes, or null.</summary>
        public CompNightStand LedgerFor(Pawn pawn)
        {
            if (pawn == null || !borrowers.TryGetValue(pawn, out CompNightStand comp))
            {
                return null;
            }

            // The stand may have been destroyed or uninstalled without going through
            // NotifyUndressed.
            if (comp == null || comp.parent.DestroyedOrNull() || comp.Borrower != pawn)
            {
                borrowers.Remove(pawn);
                return null;
            }

            return comp;
        }
    }
}
