using System.Collections.Generic;
using RimWorld;
using Verse;

namespace NightChange
{
    /// <summary>
    /// Index « qui est en tenue de nuit, et dans quel portant sont ses habits ».
    /// </summary>
    /// <remarks>
    /// <para>
    /// Rien n'est scribe ici : la verite vit dans le grand livre de chaque
    /// <see cref="CompNightStand"/>, qui part avec le portant et survit a la sauvegarde. Cet index
    /// n'est qu'un cache, reconstruit au chargement.
    /// </para>
    /// <para>
    /// Il existe pour une seule raison : le prefixe de <c>StartJob</c> doit repondre « ce pion
    /// est-il en pyjama ? » a <b>chaque</b> job de <b>chaque</b> pion. Balayer tous les portants de
    /// la carte a chaque fois serait payer un parcours de batiments pour une reponse qui est non
    /// dans plus de 99 % des cas.
    /// </para>
    /// </remarks>
    public class NightChangeTracker : GameComponent
    {
        public static NightChangeTracker Current;

        private const int ChangeCooldownTicks = 1250;

        private readonly Dictionary<Pawn, CompNightStand> borrowers = new Dictionary<Pawn, CompNightStand>();

        /// <summary>
        /// Dernier tick ou ce pion s'est change, pour empecher un aller-retour en boucle. Non
        /// scribe : au pire, un chargement ouvre une fenetre de vingt secondes ou le filet ne joue
        /// pas, ce qui est sans consequence.
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

            // Purge paresseuse : sans elle, le dictionnaire garderait une entree par colon mort
            // pour la duree de la partie.
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

        /// <summary>Le portant qui tient les habits de jour de ce pion, ou null.</summary>
        public CompNightStand LedgerFor(Pawn pawn)
        {
            if (pawn == null || !borrowers.TryGetValue(pawn, out CompNightStand comp))
            {
                return null;
            }

            // Le portant a pu etre detruit ou deminiaturise sans passer par NotifyUndressed.
            if (comp == null || comp.parent.DestroyedOrNull() || comp.Borrower != pawn)
            {
                borrowers.Remove(pawn);
                return null;
            }

            return comp;
        }
    }
}
