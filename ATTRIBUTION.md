# Attribution

## Ce mod

Écrit pour l'occasion. Aucun def, aucune texture, aucune ligne de code repris de quiconque.
Licence MIT (`LICENSE`), **publiable tel quel**.

## RimWorld

Les classes du jeu sur lesquelles le mod se greffe — `Building_OutfitStand`,
`CompAssignableToPawn`, `Pawn_JobTracker`, `Pawn_ApparelTracker`, `JobGiver_OptimizeApparel`,
`Pawn_Ownership`, `PawnBanishUtility` — appartiennent à **Ludeon Studios**. Elles sont appelées ou
dérivées, jamais copiées.

Le portant à vêtements (`Building_OutfitStand`) est un contenu d'**Odyssey**, déclaré en
`modDependencies`. Le mod ne le redistribue pas : il ajoute deux comps à son def par
`PatchOperation`, et le def reste celui de Ludeon.

## Shift Change

- **Auteur :** MrBeverage
- **Source :** Steam Workshop
  [3783456242](https://steamcommunity.com/sharedfiles/filedetails/?id=3783456242),
  `MrBeverage.ShiftChange`, RimWorld 1.6. Dépôt : <https://github.com/beverage/shift-change>.
- **Licence :** MIT, Copyright (c) 2026 MrBeverage. Sources et `docs/DESIGN.md` livrés dans le mod.

**Rien n'en est repris.** Ni def, ni texture, ni code. Shift Change n'est ni une dépendance ni un
incompatible : les deux mods peuvent tourner ensemble, séparément, ou pas du tout.

Ce qui lui est dû, en revanche, est le **savoir**. Son `docs/DESIGN.md` documente, avec les
références de ligne dans l'assembly décompilé, une série de pièges du portant à vêtements que ce
mod aurait sinon découverts en jeu :

| Piège | Où il joue ici |
|---|---|
| Chaque retrait de vêtement détruit le marqueur de port forcé (`Notify_ApparelRemoved` appelle `SetForced(false)` sans condition) | `JobDriver_ChangeAtNightStand.DoTransfer` capture les marqueurs avant le retrait et les repose après l'habillage |
| Deux `CompAssignableToPawn` sur le même def se relisent l'une l'autre, les comps s'écrivant à plat | `CompNightStand.PostExposeData` n'appelle pas la base et préfixe toutes ses clés |
| Le gizmo de la base est câblé sur `Misc4` (**N**), qui sert au presse-papier de stockage | `CompNightStand.CompGetGizmosExtra` retire la liaison |
| Un def laissé avec deux nœuds `<comps>` se résout au dernier arrivé, avec une erreur rouge | Le patch assure le nœud avant de le remplir |
| La porte du danger doit être au-dessus de l'habillage et en-dessous du déshabillage | Le préfixe la porte, le `JobGiver` de retour ne la porte pas |
| `PawnBanishUtility.Banish` n'atteint aucun `UnclaimAll`, et `Pawn.ExitMap` ne rattrape rien après coup | `Patch_Banish` moissonne au moment du bannissement |
| Le pilote vanilla du portant force tout ce qu'il rend, y compris les habits de ville | Le nôtre ne repose que les marqueurs notés au dépôt |

Une dette intellectuelle n'est pas une dette de licence, mais elle se cite.

## Vérifications faites sur le jeu

Les affirmations de code ont été vérifiées sur l'assembly décompilée de RimWorld **1.6**
(`ilspycmd` sur `Assembly-CSharp.dll`). Les noms de méthode dérivent lentement d'une version à
l'autre ; les numéros de ligne, vite.
