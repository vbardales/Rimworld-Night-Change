---
localization: complete
translation_en: complete
translation_fr: complete
settings_audit: complete
mod:          Night Change
packageId:    nelim.nightchange
repo:         Rimworld-Night-Change
visibility:   public
detached:     yes
stage:        done
workflow_stage: done
licence:      open
licence_at:   LICENSE and Mod/LICENSE, MIT, copyright Nelim 2026; design debt documented in ATTRIBUTION.md
upstream_mod_remotes:
  - https://github.com/beverage/shift-change
dependencies: declared
showcase:     complete
tested_on:
workshop:     3806765493 (prepublished 0.1.0, item private; ID file dated 2026-09-23)
remaining:
  - blocking (done -> tested): NOT MET - no scenario in @wip (0 of 40 scenarios are @wip)
  - blocking (done -> tested): NOT MET - conditional scenarios not yet played: 08 (RIMMSQOL), 11 (Shift Change), 12 (Biotech present), 13 (without Biotech); all six passes of Tests/Pickle/README.md are unrun
  - blocking (done -> tested): NOT MET - manual tests to validate: fire, downed, killed or sold borrower, garment removed from the stand, blocked path, and every @review capture still to open
  - unverified: no Pickle run yet, so the fixture (bedroom at x=30 z=30 on test-colony), timing waits and every scenario are unproven; the raid scenarios are the least stable
  - unverified: Outfit Stands Plus (loadAfter) cannot be staged: Workshop item 3545172389 was taken down, the fork 3724311713 has no established packageId
  - unverified: logs, EN/FR UI, MainButtons shortcut by RIMMSQOL, new game and existing save, in a real game
  - feature: PUBLICATION.md (gallery order, thank-you drafts, adult-content answers) for prepublishedsession:      maj: 2026-09-29, audit AUDIT.md
updated:      2026-09-29, audit; stage preTest
---

# Night Change - status

## Audit - 2026-09-29 (replaces the 2026-09-13 decision below)

Previous stage: `dansMonoRepo`. Retained: **done** (first pass gave preTest; the Pickle suite was written the same day, see below). Audited revision:
`origin/main` 51261b2 plus the uncommitted 2026-09-13 correction set, committed in the same session
(see git log). RimWorld was not launched.

| Transition | Result |
| --- | --- |
| dansMonoRepo -> horsMonoRepo | Met 2026-09-29: standalone repo in the folder, origin `vbardales/Rimworld-Night-Change` (public, pushed commit 51261b2); monorepo commit "Night Change leaves the monorepo". |
| -> ModIcon / Preview | Met: build passes, rebuilt DLL SHA256 equals shipped `DA2890A6...25F92B88`; icon 128x128 25,567 B; Preview 896x504 647,695 B. Icon 32 px legibility not re-judged (owner-only). |
| -> preOptions | Met: English description ends with `[url=...]Source code on GitHub[/url]`; cyan accent added 2026-09-13. |
| -> options | Met: 4 settings, Mod options entry, MainButton shortcut hidden by default (`buttonVisible=false`), 13 offline assertions pass. |
| -> l10n | Met: 13 Keyed keys EN/FR, 3 French DefInjected, `Check-DefInjected` 0 errors. Plurals (rule of 2026-09-25): the only counted noun is "cells" (slider 3..40, never 1), so no `.One`/`.Many` form is reachable; justified exclusion. |
| -> preTest | Met: Harmony + Odyssey required, Biotech conditional, Shift Change / Outfit Stands Plus in `loadAfter` only; no LoadFolders. |
| preTest -> done | Met 2026-09-29 (after a first pass that found it not met): Pickle suite written, 14 features / 40 scenarios, step assembly builds, scope justified in `Tests/Pickle/README.md`; automated (13) and XML checks re-run green. Execution is a `done -> tested` criterion. |

Checks re-run 2026-09-29: `dotnet build Tests/NightChange.Tests.csproj -c Release` 0 warnings/errors;
`NightChange.Tests.exe` 13 assertions passed; `Tests/Check-Xml.ps1` passed (8 XML, 13 keys);
`scripts/Check-DefInjected.ps1` 3 keys, 0 errors. One line in `docs/runs/history.md`.

Upstream: Night Change is original work. The only source repository is Shift Change
(`beverage/shift-change`, MIT), listed in `upstream_mod_remotes`. Nothing of it is reused, so there is no
port to base on and no pull request owed (BACKLOG.md).

Evidence: no `.dds` and no Pickle evidence was ever tracked in git. `.dds`, `Tests/Pickle/Evidence/`,
`Tests/Pickle/runs/` and `evidence/` are now in `.gitignore`. The old `.audit/` logs were superseded by `docs/runs/history.md` and the folder was deleted 2026-09-29.

Next transition (`done -> tested`): play the six passes through the dispatcher, open every `@review` capture, replay the conditional scenarios, and clear the three blocking items in `remaining`.

## Corrections — 2026-09-13 (superseded 2026-09-29, kept as history)

The user requested fixes after the audit. The original audit below is historical and is
not the current defect list. The cumulative stage remains **dansMonoRepo** because the
installed Mod junction still targets the monorepo folder. The standalone copy is ready at
`C:\Users\nelim\Documents\RimWorldMods\NightChange`, branch `codex/audit-fixes`, cloned from
the existing public repository at `51261b291dceb5be8251c24ea6699b16bc479a1b` with its own `.git`
and origin remote. Source/Mod/Art/tests/docs were copied and checked byte-for-byte, explicitly
excluding Git metadata. No push or publication was performed.

Automatic approval review rejected moving the working directory and replacing it with a
junction, citing repository metadata risk and insufficiently explicit authorization for
the path change. That operation was not executed. The safer standalone-copy preparation
was approved and succeeded. Activating the standalone path remains pending user approval.
The monorepo index, history and other mods are untouched. No source folder was deleted.

### Corrected defects and independent gate results

- Added `NightChange_Settings` MainButtonDef, hidden with native `buttonVisible=false`.
  Its custom worker opens `Dialog_ModSettings(NightChangeMod.Instance)`, the same native
  settings interface and state as Mod options. Visibility is inherited and never forcibly
  reset, allowing customization tools to reveal it. No RIMMSQOL dependency was added.
- Added translated scope/application/save timing guidance. Loaded numeric settings are
  normalized to margin 0..10 and distance 3..40; non-finite margin falls back to 2.
- About description now ends with the exact required source link, matching its URL/origin.
- Delivered Preview now has a clearly distinct cyan divider. Direct visual QA performed
  after downsampling: 896 x 504 PNG, 647,695 bytes; exact title/subtitle retained. Old image
  and generated source preserved in Art/. Built-in imagegen prompt and method recorded in
  `Art/Preview-edit.md`; minor generated rendering changes outside the divider are acknowledged.
- Settings technical gate passes under the user's override permitting offline tests;
  no interactive integration is certified. Localization resources complete: 13 owned Keyed
  keys in EN/FR and 3 French DefInjected fields, with English Def source fallbacks.
- Functional scenarios are now written in TEST_SCENARIOS.md. Automated settings tests and
  XML checks are written and executed, so independent criteria through `done` pass.
  After approved migration activation and path verification, `done` is the next justified
  cumulative status. `tested` still requires final game execution.

### Executed verification

- `dotnet build Tests/NightChange.Tests.csproj -c Release --nologo`: passed, zero warnings
  and errors; compiled mod installed in Mod/Assemblies. SDK 8.0.424, game refs 1.6.4871,
  Harmony 2.4.2. Actual shipped SHA256:
  `CE8EFA93A61D74EBC6D95C1A0D45012E901FFD152488D549AADC2651F800C98C`.
- `.build/tests/NightChange.Tests.exe <installed Managed> <temporary output>`: **13 assertions
  passed** against actual mod/game assemblies: defaults, numeric bounds and non-finite
  values, temperature decision thresholds, actual Scribe persistence, missing fields and
  loaded out-of-range values. No Unity UI or RimWorld process was launched.
- `Tests/Check-Xml.ps1`: 8 XML files parsed; 13 Keyed keys covered, no duplicate/empty
  entries, parameters match; attribution/licence copies match; image metadata verified;
  description link and shortcut definition/FR fields passed.
- Parent `scripts/Check-DefInjected.ps1 -TransMod <Mod>`: 33 patch operations applied,
  11,589 Defs indexed, 3 keys checked, 0 errors/unresolved targets reported.
- Logs: `.audit/fix-build.log`, `.audit/settings-tests.log`, `.audit/fix-resources.log`,
  `.audit/fix-definjected.log`. Reproducible commands and limitations: Tests/RESULTS.md.

The uncommitted corrections are based on the monorepo revision recorded in the initial
audit. Only this mod's files and the prepared standalone clone were modified. Initial
local STATUS edits and all old evidence remain preserved. Runtime EN/FR layout, primary
entry, actual RIMMSQOL/other integrations, restart persistence and new/existing game
scenarios remain explicitly **unverified**. These are pending validation, not known failures.

## Audit — 2026-09-13

The `.audit/` logs cited in the 2026-09-13 sections were deleted 2026-09-29; their results are summarised in `docs/runs/history.md`.

Previous stage was empty (not a certified stage). Retained stage: **dansMonoRepo**.
The stage field uses the literal names of the requested workflow, not legacy codes:
`dansMonoRepo -> horsMonoRepo -> ModIcon générée -> Preview générée -> preOptions -> options -> l10n -> preTest -> done -> tested`.
Independent later checks below do not advance the cumulative stage past a failed gate.
The user's audit instructions take precedence over the protocols, notably: settings runtime
checks belong to `tested`, not the `options` gate. RimWorld was not launched.

### Scope and revision

- Audited folder: `C:\Users\nelim\Documents\rimworld\NightChange`.
- Actual Git root: `C:\Users\nelim\Documents\rimworld`, HEAD
  `75c3000e1833d325cc7626a402981e4aa881d47a`.
- Distributed content: this folder's `Mod/`. The installed
  `C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\NightChange`
  junction points exactly to it. No standalone Night Change checkout was identified
  in `Documents/RimWorldMods` or the monorepo worktree list.
- Before this audit, the only local change within NightChange was STATUS.md's three
  unchecked localization fields. Existing content and historical notes are preserved.
  This audit changes STATUS.md and adds `.audit/` evidence; build intermediates are in
  `.build/`. No source, distributed DLL, image or other shipped file was changed.
- Read parent AGENTS.md, PUBLISHING.md, STYLE_RIMWORLD.md, MOD_SETTINGS.md and
  TRANSLATIONS.md. No development, image generation, publication or game execution performed.

### Ordered transitions

| Transition | Result | Evidence / remaining criterion |
| --- | --- | --- |
| dansMonoRepo -> horsMonoRepo | Defect observed | Actual distributed project has no autonomous Git root outside the monorepo. GitHub repository exists and is PUBLIC, but the configured `night-change` remote belongs to the monorepo. |
| horsMonoRepo -> ModIcon générée | Partial independent validation | Release build passes and exactly matches the shipped DLL; PNG icon is 128 x 128. Development completion cannot be certified while the required settings shortcut is absent. |
| ModIcon générée -> Preview générée | Validated independently | Directly inspected PNG, 896 x 504, 482,931 bytes, below both 900 KB and 1 MB. |
| Preview générée -> preOptions | Defects observed | English description lacks the final `[url=https://github.com/vbardales/Rimworld-Night-Change]Source code on GitHub[/url]`. The amber rule is close to the dominant amber/brown scene rather than a clearly separate accent family. |
| preOptions -> options | Partial | Useful settings and primary entry exist; required hidden, revealable MainButtons shortcut does not. Applicable automated settings tests are missing/unexecuted. |
| options -> l10n | Pending settings gate; independent resource checks pass | Twelve owned Keyed keys and one JobDef report are covered in EN/FR. Final localization gate remains partial because settings audit is partial. |
| l10n -> preTest | Dependency declarations validated independently | Harmony and Odyssey required; Biotech conditional; Shift Change and Outfit Stands Plus optional sorting entries. No LoadFolders or additional version folders. Runtime coexistence not tested. |
| preTest -> done | Non-verified requirements | No written functional scenario suite or automated behavior tests found. XML audit checks were written and executed successfully, but do not replace behavior tests or exercise every DLC combination. |
| done -> tested | Non-verified | No attributable current game test results/log review found. EN/FR UI, settings persistence, shortcut, new game and existing-save scenarios remain unexecuted. |

### Repository, rights and documentation

Read-only live checks succeeded after sandbox access restrictions were lifted:
`gh repo view vbardales/Rimworld-Night-Change --json name,visibility,url,defaultBranchRef`
reports PUBLIC, default branch `main`, exact expected URL.
`git ls-remote https://github.com/vbardales/Rimworld-Night-Change.git HEAD` reports
`51261b291dceb5be8251c24ea6699b16bc479a1b`: a pushed commit exists.
This does not establish an autonomous local checkout. No missing monorepo remote is alleged.

`Night Change`, `nelim.nightchange`, `Rimworld-Night-Change`, and `NightChange` are coherent
display/package/repository/folder names. No separate packageName field, linking words,
or required licence suffix applies. The explicit MIT licence belongs to **Nelim**, not
MrBeverage; the previous licence_at conflated the credited design source with this mod.
`open` is supported by LICENSE, with design influence and no copied third-party material
documented in ATTRIBUTION.md. No new third-party permission is inferred by this audit.
README, ATTRIBUTION, CHANGELOG and licence are present in English. Distributed LICENSE
and ATTRIBUTION copies match byte-for-byte. README's "no art" wording describes gameplay
assets; About artwork is present. Upstream historical design claims are not fresh provenance tests.

### Build and artifact verification

Command: `dotnet build Source/NightChange.csproj -c Release --nologo -p:OutputPath=../.audit/build/`.
SDK 8.0.424; resolved references Krafs.Rimworld.Ref 1.6.4871 and Lib.Harmony 2.4.2.
First sandbox attempt failed with MSB4184 (SDK directory access denied), an environment
restriction, not a code defect. Authorized retry passed: zero warnings, zero errors.
Logs: `.audit/build.log`, `.audit/build-retry.log`.

Rebuilt and distributed DLL SHA256 both:
`5E623A3F755DF4A471BBC9469FD489724C8ED072FAE8CD71EF80C3CE59AA6DCD`.
The build output was isolated; the shipped assembly was not replaced.
CI only builds and checks DLL existence; it is not a functional test suite.

Both delivered images were opened and visually inspected directly. Icon: 128 x 128 PNG,
25,567 bytes, orange winking mascot, nightcap and moon. Preview: readable English title
and summary, bedroom/stand subject, high oblique floor view, cool shadows and warm light.
No concrete camera defect was identified; no historical generation report or comparison
capture is required. The amber rule has limited chromatic separation from the dominant
warm floor/light. No secondary tag or title suffix exists, so comparing a secondary-text
colour is not applicable. Original PNGs remain under Art/. No image was regenerated.

### Settings audit

Source review: all eight C# files, all delivered Defs/patches and NightChange.csproj.
Four useful global settings in NightChangeSettings: inherit bed owner (true), cold guard
(true), cold margin (2 degrees C), bed-to-stand distance (12 cells). They affect ownership
selection, temperature rejection and stand search, respectively, in NightSwap.cs. Values
are read at subsequent decisions; they do not explicitly restart a job already underway.
The primary entry uses SettingsCategory and DoSettingsWindowContents, with translated labels.
Sliders expose 0..10 degrees and 3..40 cells; margin appears only while cold guard is on.
ExposeData declares matching serialization defaults. No manual XML editing is needed.
There is no MainButtonDef, MainTabWindow or equivalent shortcut implementation anywhere
in the delivered sources/Defs: a confirmed missing required feature.

No automated test suite exercises these settings. Defaults and data flow were reviewed,
not executed as functional tests. Deserialized numeric values are not explicitly clamped
in ExposeData; malformed/older values remain a robustness verification to perform, not a
demonstrated runtime failure. No RIMMSQOL/customization integration was tested. UI access,
actual effects, persistence and translated layout in game remain for final validation.
`settings_audit: partial` is blocked by missing shortcut and applicable technical tests,
not by the lack of an in-game run.

### Translation and XML audit

Executed `.audit/Check-Resources.ps1`; results in `.audit/resources.log`:
six XML files parse; twelve literal Translate keys have nonempty EN/FR entries without
duplicates and with matching numbered parameters. Full source review found no additional
owned player-facing hardcoded strings or dynamic translation keys. Technical log strings,
scribe keys, toil names and identifiers are excluded. Existing vanilla assignment UI is
inherited; no third-party text key is explicitly reused.

Owned Def text is `NightChange_ChangeAtStand.reportString`: English source
`changing at TargetA.` and French `se change à TargetA.` preserve the native target token.
No redundant English DefInjected file is needed. Meaning and parameter use were reviewed.
Executed `../scripts/Check-DefInjected.ps1 -TransMod <this folder>/Mod` against installed
game data: 33 patch operations applied, 11,588 Defs indexed, 1 key checked, 0 errors;
no unresolved target reported. Output: `.audit/definjected.log`.
These results cover resources as they exist today; new shortcut text must be audited
when implemented. Runtime text rendering is not certified by the complete language fields.

### Dependencies and outstanding tests

About targets RimWorld 1.6. Source references Harmony and vanilla/DLC APIs, with no
Shift Change or Outfit Stands Plus assembly reference. Odyssey's installed
`Defs/ThingDefs_Buildings/Buildings_Furniture.xml` contains both target stands; the kid
stand has `MayRequire=Ludeon.RimWorld.Biotech`, matching this mod's conditional patches.
Required dependency declarations and load order are coherent. No extra mandatory DLC
or customization mod is imposed. Optional mod versions/coexistence and Biotech absent
execution are not established by this source/XML inspection.

To pass the **next** transition, establish the actual standalone checkout outside the
monorepo, with its GitHub remote and pushed commit, preserving the audited content and
documentation. GitHub existence/public visibility and initial push already pass.
Later gates separately require the description link, distinct Preview accent, settings
shortcut and applicable tests. Final game scenarios must cover normal evening/morning
changes, medical/forced sleep, cold guard and distance boundaries, assignment, emergency
behavior, interrupted changes, forced apparel flags, stand removal, persistence, and
optional integrations, in EN/FR and new/existing saves. None is claimed executed here.

## Historical sweep notes (preserved)

The following describes the 2026-09-12 sweep and its old field vocabulary, not current
audit evidence. Its empty tested_on field and absence of game verification remain unchanged.

Read by a sweep across every mod, rather than by asking each thread in turn. It lives at the
root, never inside `Mod/`, so Steam never receives it.

The fields above were read off the disk on 2026-09-12. Four cannot be, and wait for whoever
holds this mod:

- **`stage`** — one of `port`, `showcase`, `preTest`, `done`, `tested`, `published`. Filled in
  from the session group where one exists; confirm it.
- **`tested_on`** — the date of the last run in game. Empty means never.
- **`dependencies`** — `declared` when every mod this one needs is named in the About's
  `modDependencies`, `to check` when a non-vanilla `loadAfter` suggests a dependency that is not
  declared, `none` when the mod needs nothing. An undeclared dependency is not cosmetic: on
  2026-09-11 Reequilibrage animaux took 47 vanilla animals down with it, Muffalo included, because
  the class it injects belongs to a mod that was not declared and not loaded.
- **`remaining`** — what is left, in three kinds: `feature` for something missing from a first
  release, `defect` for a known fault left unfixed, `unverified` for what could not be checked.
  The line already there is true of nearly the whole repository; replace it once it stops being.

`licence` vocabulary: `open` an explicit licence, `silent` no licence and a dead source,
`alive` no licence but a living source, `forbidden` a written refusal, `original` owing nothing
to anyone — not a name, not an idea traceable to one mod, not a value derived from its assets.
