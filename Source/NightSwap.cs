using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace NightChange
{
    /// <summary>
    /// Ce qu'un passage au portant deplacerait, pour ce pion, dans ce sens.
    /// </summary>
    /// <remarks>
    /// La question « ce trajet vaut-il la peine ? » et la question « qu'est-ce que je deplace en
    /// arrivant ? » sont posees a deux endroits differents -- par le selecteur de portant et par le
    /// pilote de job. Deux implementations divergent : le selecteur finit par retenir un portant
    /// dont le pion ne peut rien porter, le pion y va, et ne change rien. Une seule autorite, donc,
    /// et <see cref="Wearability"/> n'est appelee que d'ici.
    /// </remarks>
    public class SwapPlan
    {
        /// <summary>Ce que le pion va enfiler (actuellement dans le portant, ou porte sur lui).</summary>
        public List<Apparel> ToWear = new List<Apparel>();

        /// <summary>Ce qu'il va deposer dans le portant.</summary>
        public List<Apparel> ToPark = new List<Apparel>();

        public bool MovesAnything => ToWear.Count > 0 || ToPark.Count > 0;

        /// <summary>
        /// Aller : le pion prend la tenue de nuit et depose ses habits de jour.
        /// </summary>
        /// <remarks>
        /// Contrairement a une blouse de laboratoire, un pyjama <b>remplace</b> les habits, il ne se
        /// porte pas par-dessus. On depose donc tout ce qui n'est pas verrouille, pas seulement ce
        /// qui entre en conflit. C'est aussi ce qui rend <see cref="TemperatureGuard"/> necessaire :
        /// se coucher deshabille dans une chambre a -5 degres tue.
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

            // Le portant ne tient structurellement qu'une tenue (HasRoomForApparelOfDef refuse tout
            // ce qui ne se porte pas avec ce qu'il contient deja), mais un autre mod peut avoir
            // elargi le filtre : on ne retient que ce qui se porte ensemble.
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
        /// Retour : le pion reprend ce qu'il avait depose et rend ce qu'il avait pris.
        /// </summary>
        /// <remarks>
        /// Construit depuis le grand livre, jamais depuis le proprietaire assigne : le portant a pu
        /// etre reassigne pendant la nuit. On ne garde que les pieces encore la ou le livre les dit
        /// -- le joueur a pu ejecter un vetement, ou le pion se faire desaper par un raid.
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
    /// Le portant qui sert de penderie de chambre, s'il y en a un.
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
        /// Un portant assigne ne sert que ses proprietaires. Un portant libre sert le proprietaire
        /// du lit de la piece -- c'est le cas courant, une chambre, et il ne demande aucun reglage.
        /// En dortoir, plusieurs pions peuvent pretendre au meme portant : le premier arrive le
        /// prend, les autres vont se coucher habilles. C'est ce que l'assignation explicite regle.
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
    /// Refuse le deshabillage quand la chambre est trop froide pour la tenue de nuit.
    /// </summary>
    /// <remarks>
    /// <c>ComfyTemperatureMin</c> lu sur le pion tient deja compte de ce qu'il porte. On lui
    /// applique donc la <b>difference</b> d'isolation entre ce qu'il va enfiler et ce qu'il depose,
    /// et on compare a la temperature de la case du lit. Le vanilla ne protege pas de ca : rien
    /// n'empeche un colon de dormir nu par -20, c'est simplement que rien ne l'y envoie.
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
