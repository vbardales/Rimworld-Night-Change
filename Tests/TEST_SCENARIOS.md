# Final game validation

All scenarios are **not executed**. Use RimWorld 1.6, Harmony and Odyssey. Capture game
version, mod commit, DLL hash, language, active mods, expected/observed results and Player.log.
Run each applicable scenario in English and French. Start with a new colony, then repeat
the ordinary clothing cycle and persistence on a copy of an existing save. Never use the
only copy of a save. No automated test below certifies interactive game behavior.

| Scenario / preconditions | Actions | Expected result |
| --- | --- | --- |
| Clean settings, no customization mods | Open Mod options -> Night Change; close and reopen | Defaults true/true/2 C/12 cells; translated labels; no visible or grey shortcut |
| Global settings | Toggle both checkboxes; set margin 0 and 10, distance 3 and 40; close, restart and load another save | Values persist globally; margin hides while guard off; no clipping or raw keys |
| RIMMSQOL installed (record version) | Reveal NightChange_Settings, open it, edit settings, reopen through Mod options, hide shortcut and restart | Same native dialog and shared values; visibility choice retained; no required dependency introduced |
| Other claimed customization integration | Repeat previous scenario separately, recording name/version | Claim compatibility only after successful execution |
| Bedroom, assigned bed, reachable stand with wearable night clothes | Let an undrafted healthy colonist choose sleep automatically; wait until sleep schedule ends | Night clothes replace unlocked day clothes; day clothes restored in morning, original forced flags retained |
| Unassigned stand and bed owner | Toggle owner inheritance off/on, allow next sleep decision | Unassigned stand unused while off, serves bed owner while on |
| Two colonists in shared room | Assign stand to one sleeper; let both go to bed | Only assigned sleeper uses it; ledger follows borrower even if reassigned overnight |
| Stand outside range, then inside range | Set distance just below/at actual distance, allow a fresh sleep decision | Stand selected only within range and same room |
| Lower-insulation night clothes | Set bedroom below/at predicted minimum plus margin; toggle guard and vary margin | Guard refuses below threshold, accepts at threshold; disabled guard permits change |
| Sleep hour, pawn gets midnight snack | Let pawn leave bed then return during Sleep schedule | No premature morning change; ordinary morning restoration still works |
| Forced sleep or medical rest | Order bed use directly; repeat with wounded pawn needing rest | No clothing detour or forced removal from bed |
| Raid/fire; pawn drafted/downed | Trigger applicable emergency while changing or wearing night clothes | Outbound change not selected in danger; priorities and restrictions respected; no loop |
| Change in progress | Save/reload during walk and clothing delay | Job resumes safely; no lost or duplicated apparel; ledger remains consistent |
| Borrowed outfit | Save/restart/reload overnight and finish morning | Clothes and ownership restored; forced flags preserved |
| Stand occupied overnight | Destroy/uninstall stand; separately remove garments or make path inaccessible | No repeated errors, stale borrower loop or item duplication |
| Pawn departure | Banish, kill or send borrowing pawn off-map on a disposable save | Ownership/ledger cleaned; no stale claim after return |
| Biotech absent, then present | Load without Biotech; separately use kid outfit stand with Biotech | No missing-target errors without DLC; conditional stand works with it |
| Optional outfit mods | Repeat cycle with Shift Change, Outfit Stands Plus, then both; record versions | No duplicate comp/save keys, hotkey collision, exceptions or lost clothes |

After each run inspect Player.log for exceptions, repeated errors, XML errors, unresolved
keys and patch failures. Check settings, stand gizmos, tooltip, inspect text and job report
for EN/FR fallback and clipping. Record failures and rerun affected scenarios after fixes.
