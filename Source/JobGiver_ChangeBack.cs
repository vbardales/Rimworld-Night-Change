using System;
using RimWorld;
using Verse;
using Verse.AI;

namespace NightChange
{
    /// <summary>
    /// Le retour : le pion est debout, en pyjama, et sa journee commence.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Contrairement a l'aller, le retour est un <b>JobGiver</b> pose dans l'arbre de decision, et
    /// non un prefixe de <c>StartJob</c>. La raison tient en une phrase : depuis <c>StartJob</c>, il
    /// est impossible de savoir de facon fiable si le pion reste couche. <c>pawn.jobs.posture</c>
    /// n'est remis a zero par aucun code du <c>Pawn_JobTracker</c> -- il reste a
    /// <c>LayingInBed</c> jusqu'a ce qu'un toil du <i>nouveau</i> job en decide autrement. Un pion
    /// qui regarde la television au lit ou qui medite couche demarre bel et bien de nouveaux jobs,
    /// et le prefixe l'aurait tire hors du lit pour se rhabiller.
    /// </para>
    /// <para>
    /// Dans l'arbre, le probleme disparait : la branche « il faut rester couche »
    /// (<c>ThinkNode_ConditionalMustKeepLyingDown</c>) est tout en haut et court-circuite tout le
    /// reste. Un JobGiver n'est donc consulte que par un pion qui se leve pour de bon.
    /// </para>
    /// <para>
    /// Insere sur <c>Humanlike_PreMain</c>, juste avant le coeur du comportement de colon : donc
    /// <b>apres</b> la zone autorisee, la temperature vitale, le travail d'urgence et l'optimiseur
    /// vestimentaire, et <b>avant</b> le travail ordinaire et les loisirs. Un incendie passe avant
    /// le pantalon ; le pantalon passe avant la table de menuiserie.
    /// </para>
    /// </remarks>
    public class JobGiver_ChangeBack : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (FailOpen.Disabled)
            {
                return null;
            }

            try
            {
                CompNightStand comp = NightChangeTracker.Current?.LedgerFor(pawn);
                if (comp == null)
                {
                    return null;
                }

                if (!pawn.Spawned || pawn.Downed || pawn.InMentalState || pawn.Drafted
                    || comp.parent.Map != pawn.Map)
                {
                    return null;
                }

                // Un blesse reste au lit : le repos medical est plus bas dans l'arbre que nous, il
                // ne se defendrait pas tout seul.
                if (HealthAIUtility.ShouldSeekMedicalRest(pawn))
                {
                    return null;
                }

                // Tant que l'emploi du temps dit « sommeil », la nuit n'est pas finie. Sans cette
                // porte, un colon qui se leve grignoter a deux heures du matin se rhabille de pied
                // en cap, mange, et repasse au portant pour se recoucher -- trois trajets pour un
                // casse-croute. L'emploi du temps par defaut du jeu dort de 22 h a 5 h, donc la
                // regle mord des la premiere partie, sans rien regler.
                if (pawn.timetable?.CurrentAssignment == TimeAssignmentDefOf.Sleep)
                {
                    return null;
                }

                // Filet anti-boucle. Si le job de coucher differe echoue en boucle (lit pris,
                // chemin coupe), le pion pourrait se changer, echouer, se rechanger, indefiniment.
                if (NightChangeTracker.Current.ChangedRecently(pawn))
                {
                    return null;
                }

                // Danger volontairement ignore ici. Se changer <i>en</i> tenue est un detour que
                // personne ne devrait prendre en plein raid ; se changer <i>hors</i> de la tenue est
                // un pion qui marche vers son propre equipement. Shift Change a appris cette
                // difference en jeu : quatre colons ont passe un raid en tenue de soiree, leur
                // gilet pare-balles gare dans le portant, et la porte censee les proteger etait la
                // raison pour laquelle ils ne pouvaient pas aller le rechercher.
                if (!pawn.CanReserveAndReach(comp.parent, PathEndMode.InteractionCell, Danger.Deadly))
                {
                    return null;
                }

                SwapPlan plan = SwapPlan.ForUndressing(pawn, comp);
                if (plan == null)
                {
                    // Plus rien a rendre ni a reprendre : le joueur a vide le portant, ou un raid a
                    // desape le pion. Le grand livre ment, on le ferme.
                    comp.NotifyUndressed();
                    return null;
                }

                return JobMaker.MakeJob(NightChangeDefOf.NightChange_ChangeAtStand, comp.parent);
            }
            catch (Exception ex)
            {
                FailOpen.Fail("JobGiver_ChangeBack", ex);
                return null;
            }
        }
    }
}
