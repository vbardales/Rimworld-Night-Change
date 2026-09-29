# In-game scenarios, run by Pickle

The scenarios of [TESTING.md](../../TESTING.md) that only a running game can show, and only those.
`Mod/` is a companion mod, **Night Change - Pickle tests** (`nelim.nightchange.pickletests`), never
published. It holds the feature files and a step assembly (`Source/`, built into `Mod/Pickle/Assemblies/`),
so nothing test-related ships in the Workshop folder.

## Status

**Written 2026-09-29, never run.** The step assembly builds (0 warnings) against the shipped
`NightChange.dll`, and every step line of every feature matches one local step, one of Pickle's own or one
of PickleTools' (checked by pattern, not by Pickle's expression engine, so a collision is still possible).
A `@review` capture asserts nothing about an image: a green scenario says the trajectory ran, not that the
picture shows anything. The fixture (a walled, roofed 5 by 5 bedroom built at x=30 z=30, layout in
`Source/Driver.cs`) assumes the `test-colony` map has room there (thing-free by a static read of the save, terrain not checked; see `PickleTools/docs/FIXTURES.md`). The bedroom builder first removes the hostile insects and hives the fixture carries; a first run may still need another origin.

## What stays offline, and why

The settings values, the clamps, the cold guard's threshold arithmetic and the Scribe round trip are proved
outside the game (`Tests/Program.cs`, 13 assertions), and the XML and translation resources by
`Tests/Check-Xml.ps1`. Nothing here repeats them. What only a game shows: the real StartJob hook, the real
room temperature and insulation stats, the ledger across a save, the real `Dialog_ModSettings`, the hidden
shortcut in the real bar, and the text in the active language.

## The features

| Feature | What it stages | Passes |
| --- | --- | --- |
| `01-loads` | the def patch, jobs, think tree and shortcut def are registered; FailOpen not tripped | all |
| `02-cycle` | evening change, morning return with forced flags, non-forced return, midnight snack, "Change back now" | all |
| `03-refusals` | forced sleep, medical rest, drafted, raid (outbound refused, return allowed), cold guard refuse / accept / off | all |
| `04-assignment` | owner inheritance on/off, stand assigned to another, shared room, reassigned overnight, distance | all |
| `05-persistence` | save overnight then morning return; save in the middle of a change | all |
| `06-departure` | stand destroyed overnight; borrower banished | all |
| `07-settings-and-shortcut` | both routes open Night Change's dialog, shortcut hidden / revealed / hidden, captures | all, both languages |
| `08-rimmsqol-shortcut` | RIMMSQOL lists, reveals, hides and forgets the shortcut | avec-rimmsqol |
| `09-restart-write`, `10-restart-read` | the four settings survive a process restart | restart chain |
| `11-shift-change` | both comps on the stand, cycle and save beside Shift Change | avec-shiftchange |
| `12-biotech` | the kid stand carries the comp when Biotech is loaded | all (needs Biotech) |
| `13-without-biotech` | no kid stand, no error, cycle works | sans-biotech |
| `14-labels` | all 13 keys resolve in the pass language; gizmos and inspect line; a capture | all, both languages |

## Passes

Run from the collection root through the dispatcher's `Submit-PickleRun.ps1`, never by hand
(`AUDIT.md`, "Déposer un run au lieu de le lancer"). Filters name the mod's display name first.

| # | Pass | Map | Language | Filter |
| --- | --- | --- | --- | --- |
| 1 | without optional mods | `wsl-deps.sans-facultatifs.map` | English | `Night Change - Pickle tests,!@restart,!@sans-biotech` |
| 2 | the same | `wsl-deps.sans-facultatifs.map` | French | the same |
| 3 | with RIMMSQOL | `wsl-deps.avec-rimmsqol.map` | English | `08-rimmsqol-shortcut` |
| 4 | with Shift Change | `wsl-deps.avec-shiftchange.map` | English | `11-shift-change,02-cycle` |
| 5 | restart chain | `wsl-deps.sans-facultatifs.map` | English | `09-restart-write` then `-Then 10-restart-read` |
| 6 | without Biotech | `wsl-deps.sans-biotech.map` | English | `13-without-biotech,02-cycle` |

**No pass for Outfit Stands Plus** (`khamenman.OutfitStandsPLus`, in `loadAfter`): the Workshop item
(3545172389) was taken down by Steam and cannot be staged; a fork exists (3724311713) whose packageId is
not established. Recorded as unverified in `STATUS.md`. **No declared incompatibility**, so no
incompatibility pass.

## What a person looks at

| Capture | The one question |
| --- | --- |
| `07` settings, three captures | Is it Night Change's page by both routes? Any raw key, clipping, or a margin slider that shows while the guard is off? |
| `14` stand with its borrower | Does the inspect line read "Holding Alice's clothes" (or its French), and are both gizmo labels readable and unclipped? |
| `08` shortcut opened by RIMMSQOL | Same page as `07`? |

## Evidence

Reports go to `Tests/Pickle/Evidence/<run>` (gitignored) through `-EvidenceDir`. What to keep and delete:
[TESTING.md](../../TESTING.md#evidence-to-keep). One line per run in `docs/runs/history.md`.