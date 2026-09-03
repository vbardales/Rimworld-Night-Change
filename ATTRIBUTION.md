# Attribution

## This mod

Written for the purpose. Not a def, not a texture, not a line of code taken from anyone.
MIT licensed (`LICENSE`), **publishable as it stands**.

Written with the help of an AI assistant.

## RimWorld

The game classes this mod hooks into — `Building_OutfitStand`, `CompAssignableToPawn`,
`Pawn_JobTracker`, `Pawn_ApparelTracker`, `JobGiver_OptimizeApparel`, `Pawn_Ownership`,
`PawnBanishUtility` — belong to **Ludeon Studios**. They are called or derived from, never copied.

The outfit stand (`Building_OutfitStand`) is **Odyssey** content, declared in `modDependencies`.
The mod does not redistribute it: it adds two comps to its def through a `PatchOperation`, and the
def stays Ludeon's.

## Shift Change

- **Author:** MrBeverage
- **Source:** Steam Workshop
  [3783456242](https://steamcommunity.com/sharedfiles/filedetails/?id=3783456242),
  `MrBeverage.ShiftChange`, RimWorld 1.6. Repository: <https://github.com/beverage/shift-change>.
- **Licence:** MIT, Copyright (c) 2026 MrBeverage. Sources and `docs/DESIGN.md` ship with the mod.

**Nothing is taken from it.** Not a def, not a texture, not code. Shift Change is neither a
dependency nor an incompatibility: the two mods can run together, separately, or not at all.

What is owed to it is the **knowledge**. Its `docs/DESIGN.md` documents, with line references into
the decompiled assembly, a series of outfit stand pitfalls this mod would otherwise have found the
hard way, in play:

| Pitfall | Where it plays out here |
|---|---|
| Every apparel removal destroys the forced flag (`Notify_ApparelRemoved` calls `SetForced(false)` unconditionally) | `JobDriver_ChangeAtNightStand.DoTransfer` captures the flags before removal and restores them after the wear |
| Two `CompAssignableToPawn` on the same def cross-read each other, comps scribing flat | `CompNightStand.PostExposeData` does not call the base and prefixes all its keys |
| The base comp's gizmo is hardcoded to `Misc4` (**N**), which serves the storage clipboard | `CompNightStand.CompGetGizmosExtra` strips the binding |
| A def left with two `<comps>` nodes resolves last-wins, with a red error | The patch ensures the node before filling it |
| The danger gate belongs above the dressing path and below the undressing one | The prefix carries it; the return-trip `JobGiver` does not |
| `PawnBanishUtility.Banish` reaches no `UnclaimAll`, and `Pawn.ExitMap` does not rescue it afterwards | `Patch_Banish` reaps at the moment of banishment |
| Vanilla's own stand driver force-wears everything it hands back, street clothes included | Ours only restores the flags recorded at check-in |

An intellectual debt is not a licence debt, but it is worth naming.

## Checks made against the game

Code claims were verified against the decompiled RimWorld **1.6** assembly (`ilspycmd` on
`Assembly-CSharp.dll`). Method names drift slowly from one version to the next; line numbers drift
fast.
