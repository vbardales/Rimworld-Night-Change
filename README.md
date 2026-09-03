# Night Change

A RimWorld 1.6 mod. Colonists change into night clothes on their way to bed, and back out of them
when they get up.

The mod ships **no clothing, no buildings and no art**: it puts behaviour on Odyssey's outfit
stand. Put a stand in a bedroom, hang a set of night clothes on it, and the colonist who sleeps
there stops at it on the way to bed.

## What it does, on one page

| | |
|---|---|
| Trigger | The **automatic** `LayDown` job targeting a bed, that is, `JobGiver_GetRest`'s decision, made by the game rather than by us |
| The stand | A vanilla `Building_OutfitStand` in the same room as the bed, within a configurable range |
| Whose it is | Assigned, or by default the owner of the bed in the room, so a bedroom needs no setup |
| Return trip | A `JobGiver` on `Humanlike_PreMain`: after fleeing a fire, safe temperature, emergency work and the apparel optimizer; before ordinary work |
| Save data | The ledger lives on the stand's comp, under mod-prefixed keys |

## Decisions that are not obvious

**The outbound trip is a `StartJob` prefix, the return trip is a `JobGiver`.** Not out of a taste
for broken symmetry: from `StartJob` it is impossible to tell reliably whether the pawn is staying
in bed. `pawn.jobs.posture` is reset by no code in `Pawn_JobTracker` — it stays at `LayingInBed`
until a toil of the *new* job decides otherwise. A colonist watching television in bed does start
new jobs, and a prefix would have pulled them out of bed to get dressed. In the think tree the
problem disappears: `ThinkNode_ConditionalMustKeepLyingDown` sits at the very top and
short-circuits everything else.

**The timetable decides when the night ends.** While the current hour is marked "sleep", the
colonist stays in night clothes. Without that gate, getting up for a snack at two in the morning
costs three trips to the stand. The game's default timetable sleeps from 22h to 5h, so the rule
bites in the very first colony, with nothing to configure.

**The cold guard.** Unlike a lab coat, night clothes *replace* what is worn rather than layering
over it. A full change can therefore strip a colonist of all their insulation, and vanilla does not
protect against that — simply because nothing normally sends anyone to bed undressed. The mod
compares the two insulation totals, applies the difference to the colonist's own comfortable
minimum, and gives up if the bedroom is colder than that.

**No re-reservation of the deferred job**, unlike Shift Change. Its target is a *work* target,
contested between colonists; ours is the pawn's own bed, which nobody is going to take while they
put on their night clothes. And vanilla's queue re-reserves by itself on start.

**Fail open.** The prefix runs at the start of **every** job of **every** pawn. Every hook catches,
logs once, disables the mod for the session, and lets vanilla proceed.

## Living alongside Shift Change

[Shift Change](https://steamcommunity.com/sharedfiles/filedetails/?id=3783456242) (MrBeverage, MIT)
dresses colonists by the **room's role**, for work and for recreation, and says itself that it
leaves the bed alone. Both mods put comps on the same stand def without treading on each other:

- a bedroom is not a work room, so neither claims the other's stand;
- our scribe keys are prefixed `NightChange_`, because comps scribe flat into the thing's save node
  and two `CompAssignableToPawn` subclasses on the same def would cross-read each other;
- our gizmo drops its hotkey binding, the base comp hardcoding it to `Misc4` (**N**), which already
  serves the storage settings clipboard on a storage building.

`loadAfter` names Shift Change and Outfit Stands Plus so every comp lands in one deterministic
order.

## Building

```
dotnet build Source/NightChange.csproj -c Release
```

Reference assemblies come from NuGet (`Krafs.Rimworld.Ref`), so no RimWorld installation is needed
to compile.

## Credits and licence

MIT (`LICENSE`). Written with the help of an AI assistant. See `ATTRIBUTION.md` for what is owed to
whom — in particular to Shift Change's design notes, from which nothing was copied but a good deal
was learned.
