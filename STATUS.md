---
localization: complete
translation_en: complete
translation_fr: complete
settings_audit: partial
mod:          Night Change
packageId:    nelim.nightchange
repo:         Rimworld-Night-Change
visibility:   public
detached:     yes
workflow_stage: playTests[1.0.0]
licence:      open
licence_at:   LICENSE and Mod/LICENSE, MIT, copyright Nelim 2026; design debt documented in ATTRIBUTION.md
upstream_mod_remotes:
  - https://github.com/beverage/shift-change
dependencies: declared
showcase:     complete
tested_on:
workshop:     3806765493 (prepublished 0.1.0, item private; ID file dated 2026-09-23)
remaining:
  - blocking (playTests -> shootGallery): NOT MET - conditional scenarios not yet played: 08 (RIMMSQOL), 11 (Shift Change), 12 (Biotech present), 13 (without Biotech); all six passes of Tests/Pickle/README.md are unrun
  - blocking (playTests -> shootGallery): NOT MET - the former manual tests are now Pickle scenarios written 2026-10-16 (03: burning stand, downed before bed, downed overnight, blocked path outbound; 06: borrower killed, sold, parked garment taken off the stand, blocked path in the morning), E1 `e1-emergencies` 6/9 green (fire, downed x2, killed, parked garment, gizmo capture); the 3 red were fixture faults. E2 `e2-emergencies` 2/3 green (blocked path outbound, sold); morning blocked path red on its fixture (colonist standing on the stand cell), fixed 2026-10-17, replay E3 `e3-emergencies` (id 20261017-161442-172-25ff) 1/1 green 2026-10-17, so all 09 emergency scenarios now green (evidence Tests/Pickle/Evidence/e3-emergencies). English gizmo bar capture accepted 2026-10-16 (labels readable, unclipped); French 07/14 captures accepted (F1 4/5; the red was the key-count step reading English from memory, fixed), key resolution replay F2 `f2-french` (id 20261017-162157-772-55f4) 1/1 green 2026-10-17 (evidence Tests/Pickle/Evidence/f2-french). Fire note: DangerWatcher counts hostiles only, so a fire elsewhere does not gate the change; only a burning stand fails the job (FailOnBurningImmobile)
  - @review captures opened 2026-10-16 by the session (07 EN/FR, 08, 14 EN/FR, runs of 2026-09-29/30): settings page accepted (no raw key, no clipping, margin slider shown with the guard on, same page by Mod options, shortcut and RIMMSQOL); inspect line accepted ("Holding Alice's clothes" / "Contient les habits d'Alice"). Gizmo bar was NOT visible in the 14 capture (screenshot mode hides it) and all captures predate the 2026-10-16 string rewording: 14 now takes a plain capture of the gizmo bar, replayed in E1 (English); French 07/14 to replay in a small French ticket
  - defect B, closed by ticket B3 (2026-10-16, `b3-button-shiftchange` 1/1): with Shift Change the swap job ShiftChange_SwapAtStand ends on its own (GotoWander at tick 136683, about 700 ticks after the button) and Alice wears the parka; the B2 failure was a state report taken too early
  - blocking (playTests -> shootGallery): the without-Biotech pass (13) cannot run: the test-colony save was made with Biotech and vanilla throws a NullReferenceException (ListerHaulables, Building_Grave.Accepts, ThingFilter.Allows) on load without it; needs another fixture. 12 (with Biotech) is green
  - unverified: 12 replayed alone after the 2026-10-15 patch change: `d-biotech` 1/1 green (2026-10-16); raid scenarios green (A2, four raiders), settings 07 and restart chain 09/10 green on earlier runs, RIMMSQOL 08 green (p3 3/3)
  - unverified: Outfit Stands Plus (loadAfter) cannot be staged: Workshop item 3545172389 was taken down, the fork 3724311713 has no established packageId
  - unverified: logs, EN/FR UI, MainButtons shortcut by RIMMSQOL, new game and existing save, in a real game
  - feature: gallery captures (shootGallery); PUBLICATION.md written 2026-10-13, its gallery table is a plan
updated:      2026-10-17, playTests[1.0.0]; audit replayed 1 to 8
protocols_read_sha: 43dd90d4b7873c500347fba1be01275a48d5795b
---
# Night Change - status

## Audit replay - 2026-10-15

Replayed against protocols as of 86cf2bf7 plus 2026-10-15 changes (markers moved to Workflow Board, gallery subtitles, tag check: none touches mod). Revision `b8794fc` plus changes below. RimWorld not launched by session; Pickle ran through tickets.

| Step | Result |
| --- | --- |
| 3.b build | Met. Source build 0 warnings, 0 errors. Rebuilt DLL differs from shipped one byte for byte (1250360F vs DA2890A6, not pdb path: same with old build folder), but decompiled C# of both identical (ilspycmd diff, only csproj name differs), so shipped DLL is these sources. |
| 4 dependencies | **Defect found and fixed.** `Mod/Patches/NightChange_OutfitStand.xml` put `MayRequire` on two `<Operation>` elements, which AUDIT.md 4.b forbids. Kid-stand patch now guarded by def existence (PatchOperationConditional, then sequence): same effect with Biotech, nothing and no log without it. About: Harmony and Odyssey required, Biotech conditional, Shift Change and Outfit Stands Plus in loadAfter only, no LoadFolders. |
| 5 settings | `settings_audit: partial`: analysis, 13 offline assertions green, in-game 07 and 09/10 green 2026-09-30; RIMMSQOL route 08 green (p3). Complete needs settings captures read and shortcut checked in revision tested. |
| 6 localization | Met. `translation_en` and `translation_fr` complete, French reviewed by Virginie 2026-10-13. |
| 7 writeTests | Met. Offline checks replayed today: build 0/0, 13 assertions, Check-Xml (last-line check now expects plain `Source code on GitHub (url)` description generator writes), Check-DefInjected 0 errors, Check-Status 0 errors. |
| 8 playTests | Open: see remaining. No `code_review_sha` yet (8.m, due before shootGallery, patch change above reopens any earlier review). No `tested_on` yet. |

Stage kept at playTests: defect found fixed in same change, transition out of playTests blocked by open items.

## Translation audit

French lives in `Mod/Languages/French/Keyed/NightChange.xml`, `DefInjected/JobDef/NightChange_Jobs.xml` and `DefInjected/MainButtonDef/NightChange.xml`. No grammar files.

- 2026-09-30, gender-agreement read (session): every French text read. Six Keyed strings agreed with pawn in masculine only (`celui`, `il`, `le colon`, `le dormeur`, `propriétaire explicite`); all reworded with no agreement (`la personne`, `l'envoie`, `Choisir qui dort ici`), so no `{PAWN_gender}` switch needed. Unsure, for Virginie: `portant` for outfit stand, `Choisir qui dort ici` (replaced 2026-10-16 by `Choisir la personne qui se change ici`) as button label, `habits` vs `tenue`.
- French review by Virginie: done 2026-10-13, reviewer Virginie, FRENCH_REVIEW.md generated from revision 10ee883 (French texts unchanged since 76f89a3); corrections requested: none recorded, she validated translation in chat. English sentence changed in Steam description at 10ee883 read with PUBLICATION.md at writeDocs (no separate About review, AUDIT.md 11.j).
- French review of four strings changed 2026-10-16 (`NightChange_AssignGizmo`, `AssignGizmoDesc`, `SettingsScope`, `ChangeBackGizmoDesc`): validated by Virginie in chat 2026-10-16, no correction requested; FRENCH_REVIEW.md regenerated at commit b8d3572.
- 2026-10-17 ChatGPT review (review-translation): `NightChange_SettingInheritOwnerDesc` ("Décochez" to "Désactiver cette option") and `NightChange_AssignGizmoDesc` ("Désignez qui" to "Choisir la personne qui") applied, Virginie said "applique ce qui est pertinent"; `ChangeBackGizmoDesc` ("remettre ses habits") not applied, taste only. FRENCH_REVIEW.md regenerated at fb98575. 2026-10-17 second pass: `SettingInheritOwnerDesc` "propriétaire assigné" to "personne assignée" applied; `ChangeBackGizmoDesc` habits to vêtements not applied (inspect line says habits, would disagree).


## Older audits

The audits of 2026-09-13 and 2026-09-29, the corrections of 2026-09-13 and the historical sweep notes are in `docs/runs/status-archive-2026-10-16.md`.

## Preview

2026-10-27: Preview regenerated from the new `Art/Preview.config.json` (commit 4899983), validated by Virginie in chat.
