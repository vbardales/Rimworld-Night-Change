using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace NightChange
{
    /// <summary>
    /// What the vanilla stand does not know: <b>whose</b> the clothes inside are.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Odyssey's <see cref="Building_OutfitStand"/> is an undifferentiated bag: it scribes its
    /// container, its storage settings and its toggle, nothing else. Its own
    /// <c>JobDriver_UseOutfitStand</c> hands every wearable item to whoever arrives and pushes
    /// their displaced clothes back into the same bag, so two pawns sharing one stand walk off in
    /// each other's clothes. Unusable as a return trip.
    /// </para>
    /// <para>
    /// This comp adds the two missing things: <b>ownership</b> (the machinery beds and thrones
    /// use, free from XML) and a <b>ledger</b> - who borrowed, what they parked, what they took,
    /// and which of the parked garments were force-worn at check-in.
    /// </para>
    /// <para>
    /// <b>The borrower, not the owner, is the ledger's truth.</b> Rebuilding the return trip from
    /// the assigned owner would be wrong the moment a stand is reassigned overnight.
    /// </para>
    /// <para>
    /// <b>Mod-prefixed scribe keys.</b> Comps scribe flat into the thing's save node. Two
    /// subclasses of <see cref="CompAssignableToPawn"/> on the same def - exactly the case when
    /// Shift Change is installed - would both write <c>assignedPawns</c> and cross-read each other
    /// on load. Hence <see cref="PostExposeData"/>, which <b>does not</b> call the base and
    /// prefixes everything.
    /// </para>
    /// </remarks>
    public class CompNightStand : CompAssignableToPawn
    {
        private Pawn borrower;
        private List<Apparel> parked = new List<Apparel>();
        private List<Apparel> taken = new List<Apparel>();
        private List<Apparel> parkedForced = new List<Apparel>();

        /// <summary>True while the stand holds somebody's day clothes.</summary>
        public bool InUse => borrower != null;

        public Pawn Borrower => borrower;

        public List<Apparel> Parked => parked;

        public List<Apparel> Taken => taken;

        public bool WasForced(Apparel apparel) => parkedForced.Contains(apparel);

        public Building_OutfitStand Stand => parent as Building_OutfitStand;

        public override IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                if (!parent.Spawned)
                {
                    return Enumerable.Empty<Pawn>();
                }

                return parent.Map.mapPawns.FreeColonists.Where(p => p.apparel != null);
            }
        }

        protected override string GetAssignmentGizmoLabel() => "NightChange_AssignGizmo".Translate();

        protected override string GetAssignmentGizmoDesc() => "NightChange_AssignGizmoDesc".Translate();

        /// <summary>
        /// The base comp hardcodes its gizmo to <c>KeyBindingDefOf.Misc4</c>, which is <b>N</b>.
        /// Harmless on beds. The outfit stand, however, is a <b>storage</b> building, and the
        /// storage settings clipboard binds copy to that same Misc4. Reusing a vanilla comp on a
        /// building category it never shipped on can import a hotkey collision. We strip the
        /// binding and touch nothing else.
        /// </summary>
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                if (gizmo is Command command)
                {
                    command.hotKey = null;
                }

                yield return gizmo;
            }

            if (borrower == null)
            {
                yield break;
            }

            Pawn sleeper = borrower;
            yield return new Command_Action
            {
                defaultLabel = "NightChange_ChangeBackGizmo".Translate(),
                defaultDesc = "NightChange_ChangeBackGizmoDesc".Translate(sleeper.LabelShort),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner"),
                action = delegate
                {
                    if (sleeper.Spawned && !sleeper.Dead)
                    {
                        sleeper.jobs?.EndCurrentJob(JobCondition.InterruptForced);
                    }
                }
            };
        }

        // ------------------------------------------------------------------ the ledger

        public void NotifyDressed(Pawn pawn, List<Apparel> parkedApparel, List<Apparel> takenApparel,
            List<Apparel> forcedAtCheckIn)
        {
            borrower = pawn;
            parked = parkedApparel ?? new List<Apparel>();
            taken = takenApparel ?? new List<Apparel>();
            parkedForced = forcedAtCheckIn ?? new List<Apparel>();
            NightChangeTracker.Current?.Register(pawn, this);
        }

        public void NotifyUndressed()
        {
            NightChangeTracker.Current?.Unregister(borrower);
            borrower = null;
            parked.Clear();
            taken.Clear();
            parkedForced.Clear();
        }

        /// <summary>
        /// A stand that is uninstalled or destroyed returns its clothes the vanilla way (the
        /// contents drop on the floor), but the ledger would go on naming a borrower. We empty it,
        /// or the pawn stays flagged as being in night clothes and the prefix keeps sending them
        /// back to a stand that no longer exists.
        /// </summary>
        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            base.PostDeSpawn(map, mode);
            if (mode != DestroyMode.WillReplace)
            {
                NotifyUndressed();
            }
        }

        /// <summary>
        /// Frees the stand when vanilla considers ownership to end: death, capture, trade, map
        /// exit, banishment. Deliberately <b>eager</b>: a stand that merely disbelieves a ledger
        /// naming a departed pawn believes it again the moment that pawn is recruited back,
        /// because the ledger was never emptied.
        /// </summary>
        public void Reap(Pawn pawn)
        {
            if (borrower == pawn)
            {
                NotifyUndressed();
            }

            TryUnassignPawn(pawn);
        }

        public override string CompInspectStringExtra()
        {
            return borrower == null ? null : "NightChange_InspectInUse".Translate(borrower.LabelShort);
        }

        public override void PostExposeData()
        {
            // Deliberately without base.PostExposeData(): see the class note on scribe keys.
            Scribe_Collections.Look(ref assignedPawns, "NightChange_assignedPawns", LookMode.Reference);
            Scribe_Collections.Look(ref uninstalledAssignedPawns, "NightChange_uninstalledAssignedPawns",
                LookMode.Reference);
            Scribe_References.Look(ref borrower, "NightChange_borrower");
            Scribe_Collections.Look(ref parked, "NightChange_parked", LookMode.Reference);
            Scribe_Collections.Look(ref taken, "NightChange_taken", LookMode.Reference);
            Scribe_Collections.Look(ref parkedForced, "NightChange_parkedForced", LookMode.Reference);

            if (Scribe.mode != LoadSaveMode.PostLoadInit)
            {
                return;
            }

            assignedPawns ??= new List<Pawn>();
            uninstalledAssignedPawns ??= new List<Pawn>();
            parked ??= new List<Apparel>();
            taken ??= new List<Apparel>();
            parkedForced ??= new List<Apparel>();

            assignedPawns.RemoveAll(x => x == null);
            uninstalledAssignedPawns.RemoveAll(x => x == null);
            parked.RemoveAll(x => x == null);
            taken.RemoveAll(x => x == null);
            parkedForced.RemoveAll(x => x == null);

            if (borrower == null || borrower.Dead)
            {
                NotifyUndressed();
            }
        }
    }
}
