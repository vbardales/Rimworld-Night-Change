using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace NightChange
{
    /// <summary>
    /// Ce que le portant vanilla ne sait pas : <b>a qui</b> sont les vetements qu'il contient.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Le portant d'Odyssey (<see cref="Building_OutfitStand"/>) est un sac indifferencie : il
    /// scribe son contenu, ses reglages de stockage et son interrupteur, rien de plus. Son propre
    /// <c>JobDriver_UseOutfitStand</c> distribue tout ce qui est portable au premier venu et
    /// repousse les habits deplaces dans le meme sac -- deux pions qui partagent un portant
    /// repartent dans les habits l'un de l'autre. Inutilisable comme trajet retour.
    /// </para>
    /// <para>
    /// Ce comp ajoute les deux choses manquantes : une <b>assignation</b> (la machinerie des lits
    /// et des trones, gratuite en XML) et un <b>grand livre</b> -- qui a emprunte, ce qu'il a
    /// depose, ce qu'il a pris, et lesquels de ses habits deposes etaient portes de force.
    /// </para>
    /// <para>
    /// <b>Le grand livre dit la verite, pas l'assignation.</b> Reconstruire le trajet retour depuis
    /// le proprietaire assigne serait faux des qu'un portant est reassigne pendant la nuit.
    /// </para>
    /// <para>
    /// <b>Cles de scribe prefixees.</b> Les comps s'ecrivent a plat dans le noeud de sauvegarde de
    /// l'objet. Deux sous-classes de <see cref="CompAssignableToPawn"/> posees sur le meme def --
    /// c'est exactement le cas quand Shift Change est installe -- ecriraient toutes deux
    /// <c>assignedPawns</c> et se reliraient l'une l'autre au chargement. D'ou
    /// <see cref="PostExposeData"/> qui <b>n'appelle pas</b> la base et prefixe tout.
    /// </para>
    /// </remarks>
    public class CompNightStand : CompAssignableToPawn
    {
        private Pawn borrower;
        private List<Apparel> parked = new List<Apparel>();
        private List<Apparel> taken = new List<Apparel>();
        private List<Apparel> parkedForced = new List<Apparel>();

        /// <summary>Vrai quand le portant tient les habits de jour de quelqu'un.</summary>
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
        /// La base cable son gizmo sur <c>KeyBindingDefOf.Misc4</c>, c'est-a-dire <b>N</b>. Anodin
        /// sur un lit ; le portant, lui, est un batiment de <b>stockage</b>, et le presse-papier des
        /// reglages de stockage lie la copie a ce meme Misc4. Reutiliser un comp vanilla sur une
        /// categorie de batiment ou il n'a jamais servi peut importer une collision de raccourci.
        /// On retire la liaison, on ne touche pas au reste.
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

        // ------------------------------------------------------------------ grand livre

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
        /// Un portant demonte ou detruit rend ses habits par la voie vanilla (le contenu tombe au
        /// sol), mais le grand livre, lui, continuerait a nommer un emprunteur. On le vide, sinon le
        /// pion reste marque « en pyjama » et le prefixe le renvoie indefiniment vers un portant qui
        /// n'existe plus.
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
        /// Libere le portant quand le vanilla considere que la propriete cesse : mort, capture,
        /// vente, sortie de carte, bannissement. Volontairement <b>eager</b> : un portant qui se
        /// contente de ne plus croire un grand livre nommant un pion parti y recroit des qu'il est
        /// recrute a nouveau, puisque le livre n'a jamais ete vide.
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
            // Volontairement sans base.PostExposeData() : voir la note de classe sur les cles.
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
