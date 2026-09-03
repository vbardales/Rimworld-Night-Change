# Night Change

Mod RimWorld 1.6. Les colons enfilent une tenue de nuit en allant se coucher, et la quittent en se
levant.

Le mod n'ajoute **ni vêtement, ni bâtiment, ni texture** : il pose un comportement sur le portant à
vêtements d'Odyssey. On met un portant dans une chambre, on y accroche une tenue de nuit, et le
colon qui dort là s'y arrête en allant au lit.

## Ce qu'il fait, en une page

| | |
|---|---|
| Déclencheur | Le job `LayDown` **automatique** visant un lit, c'est-à-dire la décision de `JobGiver_GetRest`, prise par le jeu et non par nous |
| Le portant | Un `Building_OutfitStand` vanilla dans la même pièce que le lit, à portée réglable |
| À qui il est | Assigné, ou par défaut au propriétaire du lit de la pièce — une chambre ne demande donc aucun réglage |
| Retour | Un `JobGiver` posé sur `Humanlike_PreMain` : après la fuite d'un incendie, la température vitale, le travail d'urgence et l'optimiseur vestimentaire ; avant le travail ordinaire |
| Sauvegarde | Le grand livre vit sur le comp du portant, avec des clés préfixées |

## Décisions qui ne se devinent pas

**L'aller est un préfixe de `StartJob`, le retour est un `JobGiver`.** Pas par goût de la symétrie
brisée : depuis `StartJob`, il est impossible de savoir de façon fiable si le pion reste couché.
`pawn.jobs.posture` n'est remis à zéro par aucun code du `Pawn_JobTracker` — il reste à
`LayingInBed` jusqu'à ce qu'un toil du *nouveau* job en décide autrement. Un colon qui regarde la
télévision au lit démarre bel et bien un nouveau job, et un préfixe l'aurait tiré hors du lit pour
se rhabiller. Dans l'arbre de décision, le problème disparaît : la branche
`ThinkNode_ConditionalMustKeepLyingDown` est tout en haut et court-circuite tout le reste.

**L'emploi du temps décide de la fin de la nuit.** Tant que l'heure courante est marquée
« sommeil », le colon reste en pyjama. Sans cette porte, se lever grignoter à deux heures du matin
coûte trois trajets au portant. L'emploi du temps par défaut du jeu dort de 22 h à 5 h : la règle
mord dès la première partie, sans rien régler.

**Le garde-froid.** Contrairement à une blouse de laboratoire, un pyjama *remplace* les habits au
lieu de se superposer. Un changement complet peut donc retirer au colon toute son isolation, et le
vanilla ne protège pas de ça — simplement parce que rien ne l'envoie normalement dormir déshabillé.
Le mod compare l'isolation des deux tenues, applique la différence au minimum confortable du colon,
et renonce si la chambre est plus froide.

**Aucune re-réservation du job différé**, contrairement à Shift Change. Sa cible à lui est une cible
de *travail*, disputée entre colons ; la nôtre est le lit du pion, que personne ne va lui prendre
pendant qu'il enfile son pyjama. Et la file du vanilla re-réserve d'elle-même au démarrage.

**Fail open.** Le préfixe s'exécute au démarrage de **tous** les jobs de **tous** les pions. Chaque
greffe rattrape, journalise une seule fois, désactive le mod pour la session, et laisse le vanilla
continuer.

## Cohabitation avec Shift Change

[Shift Change](https://steamcommunity.com/sharedfiles/filedetails/?id=3783456242) (MrBeverage, MIT)
habille les colons selon le **rôle de la pièce**, pour le travail et le loisir, et dit lui-même
qu'il laisse le lit tranquille. Les deux mods se posent sur le même def de portant sans se marcher
dessus :

- une chambre n'est pas une pièce de travail, donc aucun des deux ne réclame le portant de l'autre ;
- nos clés de scribe sont préfixées `NightChange_`, parce que les comps s'écrivent à plat dans le
  nœud de sauvegarde de l'objet et que deux sous-classes de `CompAssignableToPawn` sur le même def
  se reliraient l'une l'autre ;
- notre gizmo perd sa liaison de raccourci, la base la câblant sur `Misc4` (**N**) qui sert déjà au
  presse-papier des réglages de stockage sur un bâtiment de stockage.

`loadAfter` cite Shift Change et Outfit Stands Plus pour que tous les comps atterrissent dans un
ordre déterministe.

## Compiler

```
dotnet build Source/NightChange.csproj -c Release
```

Les assemblies de référence viennent de NuGet (`Krafs.Rimworld.Ref`) : aucune installation de
RimWorld n'est nécessaire pour compiler.
