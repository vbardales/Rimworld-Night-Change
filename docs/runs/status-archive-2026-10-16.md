# Night Change - status journal, folded out of STATUS.md 2026-10-16

Dated sections moved verbatim (compressed form) from STATUS.md; current state stays in STATUS.md.

## Audit - 2026-09-29 (replaces the 2026-09-13 decision below)

Previous stage: `dansMonoRepo`. Retained: **done** (first pass gave preTest; Pickle suite written same day, see below). Audited revision:
`origin/main` 51261b2 plus uncommitted 2026-09-13 correction set, committed same session
(see git log). RimWorld not launched.

| Transition | Result |
| --- | --- |
| dansMonoRepo -> horsMonoRepo | Met 2026-09-29: standalone repo in folder, origin `vbardales/Rimworld-Night-Change` (public, pushed commit 51261b2); monorepo commit "Night Change leaves the monorepo". |
| -> ModIcon / Preview | Met: build passes, rebuilt DLL SHA256 equals shipped `DA2890A6...25F92B88`; icon 128x128 25,567 B; Preview 896x504 647,695 B. Icon 32 px legibility not re-judged (owner-only). |
| -> preOptions | Met: English description ends with `[url=...]Source code on GitHub[/url]`; cyan accent added 2026-09-13. |
| -> options | Met: 4 settings, Mod options entry, MainButton shortcut hidden by default (`buttonVisible=false`), 13 offline assertions pass. |
| -> l10n | Met: 13 Keyed keys EN/FR, 3 French DefInjected, `Check-DefInjected` 0 errors. Plurals (rule of 2026-09-25): only counted noun is "cells" (slider 3..40, never 1), so no `.One`/`.Many` form reachable; justified exclusion. |
| -> preTest | Met: Harmony + Odyssey required, Biotech conditional, Shift Change / Outfit Stands Plus in `loadAfter` only; no LoadFolders. |
| preTest -> done | Met 2026-09-29 (after first pass found it not met): Pickle suite written, 14 features / 40 scenarios, step assembly builds, scope justified in `Tests/Pickle/README.md`; automated (13) and XML checks re-run green. Execution is `done -> tested` criterion. |

Checks re-run 2026-09-29: `dotnet build Tests/NightChange.Tests.csproj -c Release` 0 warnings/errors;
`NightChange.Tests.exe` 13 assertions passed; `Tests/Check-Xml.ps1` passed (8 XML, 13 keys);
`scripts/Check-DefInjected.ps1` 3 keys, 0 errors. One line in `docs/runs/history.md`.

Upstream: Night Change original work. Only source repository is Shift Change
(`beverage/shift-change`, MIT), listed in `upstream_mod_remotes`. Nothing reused, so no
port to base on and no pull request owed (BACKLOG.md).

Evidence: no `.dds` and no Pickle evidence ever tracked in git. `.dds`, `Tests/Pickle/Evidence/`,
`Tests/Pickle/runs/` and `evidence/` now in `.gitignore`. Old `.audit/` logs superseded by `docs/runs/history.md`, folder deleted 2026-09-29.

Next transition (`done -> tested`): play six passes through dispatcher, open every `@review` capture, replay conditional scenarios, clear three blocking items in `remaining`.

## Corrections — 2026-09-13 (superseded 2026-09-29, kept as history)

User requested fixes after audit. Original audit below historical, not current defect list. Cumulative stage remains **dansMonoRepo** because installed Mod junction still targets monorepo folder. Standalone copy ready at
`C:\Users\nelim\Documents\RimWorldMods\NightChange`, branch `codex/audit-fixes`, cloned from
existing public repository at `51261b291dceb5be8251c24ea6699b16bc479a1b` with own `.git`
and origin remote. Source/Mod/Art/tests/docs copied and checked byte-for-byte, explicitly
excluding Git metadata. No push or publication performed.

Automatic approval review rejected moving working directory and replacing it with junction, citing repository metadata risk and insufficiently explicit authorization for path change. Operation not executed. Safer standalone-copy preparation approved and succeeded. Activating standalone path remains pending user approval.
Monorepo index, history and other mods untouched. No source folder deleted.

### Corrected defects and independent gate results

- Added `NightChange_Settings` MainButtonDef, hidden with native `buttonVisible=false`.
  Custom worker opens `Dialog_ModSettings(NightChangeMod.Instance)`, same native
  settings interface and state as Mod options. Visibility inherited, never forcibly
  reset, allowing customization tools to reveal it. No RIMMSQOL dependency added.
- Added translated scope/application/save timing guidance. Loaded numeric settings
  normalized to margin 0..10 and distance 3..40; non-finite margin falls back to 2.
- About description now ends with exact required source link, matching URL/origin.
- Delivered Preview now has clearly distinct cyan divider. Direct visual QA after downsampling: 896 x 504 PNG, 647,695 bytes; exact title/subtitle retained. Old image
  and generated source preserved in Art/. Built-in imagegen prompt and method recorded in
  `Art/Preview-edit.md`; minor generated rendering changes outside divider acknowledged.
- Settings technical gate passes under user's override permitting offline tests;
  no interactive integration certified. Localization resources complete: 13 owned Keyed
  keys in EN/FR and 3 French DefInjected fields, with English Def source fallbacks.
- Functional scenarios now written in TEST_SCENARIOS.md. Automated settings tests and
  XML checks written and executed, so independent criteria through `done` pass.
  After approved migration activation and path verification, `done` is next justified
  cumulative status. `tested` still requires final game execution.

### Executed verification

- `dotnet build Tests/NightChange.Tests.csproj -c Release --nologo`: passed, zero warnings
  and errors; compiled mod installed in Mod/Assemblies. SDK 8.0.424, game refs 1.6.4871,
  Harmony 2.4.2. Actual shipped SHA256:
  `CE8EFA93A61D74EBC6D95C1A0D45012E901FFD152488D549AADC2651F800C98C`.
- `.build/tests/NightChange.Tests.exe <installed Managed> <temporary output>`: **13 assertions
  passed** against actual mod/game assemblies: defaults, numeric bounds and non-finite
  values, temperature decision thresholds, actual Scribe persistence, missing fields and
  loaded out-of-range values. No Unity UI or RimWorld process launched.
- `Tests/Check-Xml.ps1`: 8 XML files parsed; 13 Keyed keys covered, no duplicate/empty
  entries, parameters match; attribution/licence copies match; image metadata verified;
  description link and shortcut definition/FR fields passed.
- Parent `scripts/Check-DefInjected.ps1 -TransMod <Mod>`: 33 patch operations applied,
  11,589 Defs indexed, 3 keys checked, 0 errors/unresolved targets reported.
- Logs: `.audit/fix-build.log`, `.audit/settings-tests.log`, `.audit/fix-resources.log`,
  `.audit/fix-definjected.log`. Reproducible commands and limitations: Tests/RESULTS.md.

Uncommitted corrections based on monorepo revision recorded in initial
audit. Only this mod's files and prepared standalone clone modified. Initial
local STATUS edits and all old evidence preserved. Runtime EN/FR layout, primary
entry, actual RIMMSQOL/other integrations, restart persistence and new/existing game
scenarios remain explicitly **unverified**. Pending validation, not known failures.

## Audit — 2026-09-13

`.audit/` logs cited in 2026-09-13 sections deleted 2026-09-29; results summarised in `docs/runs/history.md`.

Previous stage empty (not certified stage). Retained stage: **dansMonoRepo**.
Stage field uses literal names of requested workflow, not legacy codes:
`dansMonoRepo -> horsMonoRepo -> ModIcon générée -> Preview générée -> preOptions -> options -> l10n -> preTest -> done -> tested`.
Independent later checks below do not advance cumulative stage past failed gate.
User's audit instructions take precedence over protocols, notably: settings runtime
checks belong to `tested`, not `options` gate. RimWorld not launched.

### Scope and revision

- Audited folder: `C:\Users\nelim\Documents\rimworld\NightChange`.
- Actual Git root: `C:\Users\nelim\Documents\rimworld`, HEAD
  `75c3000e1833d325cc7626a402981e4aa881d47a`.
- Distributed content: this folder's `Mod/`. Installed
  `C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\NightChange`
  junction points exactly to it. No standalone Night Change checkout identified
  in `Documents/RimWorldMods` or monorepo worktree list.
- Before audit, only local change within NightChange was STATUS.md's three
  unchecked localization fields. Existing content and historical notes preserved.
  Audit changes STATUS.md and adds `.audit/` evidence; build intermediates in
  `.build/`. No source, distributed DLL, image or other shipped file changed.
- Read parent AGENTS.md, PUBLISHING.md, STYLE_RIMWORLD.md, MOD_SETTINGS.md and
  TRANSLATIONS.md. No development, image generation, publication or game execution performed.

### Ordered transitions

| Transition | Result | Evidence / remaining criterion |
| --- | --- | --- |
| dansMonoRepo -> horsMonoRepo | Defect observed | Actual distributed project has no autonomous Git root outside monorepo. GitHub repository exists and is PUBLIC, but configured `night-change` remote belongs to monorepo. |
| horsMonoRepo -> ModIcon générée | Partial independent validation | Release build passes and exactly matches shipped DLL; PNG icon 128 x 128. Development completion cannot be certified while required settings shortcut absent. |
| ModIcon générée -> Preview générée | Validated independently | Directly inspected PNG, 896 x 504, 482,931 bytes, below both 900 KB and 1 MB. |
| Preview générée -> preOptions | Defects observed | English description lacks final `[url=https://github.com/vbardales/Rimworld-Night-Change]Source code on GitHub[/url]`. Amber rule close to dominant amber/brown scene rather than clearly separate accent family. |
| preOptions -> options | Partial | Useful settings and primary entry exist; required hidden, revealable MainButtons shortcut does not. Applicable automated settings tests missing/unexecuted. |
| options -> l10n | Pending settings gate; independent resource checks pass | Twelve owned Keyed keys and one JobDef report covered in EN/FR. Final localization gate remains partial because settings audit partial. |
| l10n -> preTest | Dependency declarations validated independently | Harmony and Odyssey required; Biotech conditional; Shift Change and Outfit Stands Plus optional sorting entries. No LoadFolders or additional version folders. Runtime coexistence not tested. |
| preTest -> done | Non-verified requirements | No written functional scenario suite or automated behavior tests found. XML audit checks written and executed successfully, but do not replace behavior tests or exercise every DLC combination. |
| done -> tested | Non-verified | No attributable current game test results/log review found. EN/FR UI, settings persistence, shortcut, new game and existing-save scenarios remain unexecuted. |

### Repository, rights and documentation

Read-only live checks succeeded after sandbox access restrictions lifted:
`gh repo view vbardales/Rimworld-Night-Change --json name,visibility,url,defaultBranchRef`
reports PUBLIC, default branch `main`, exact expected URL.
`git ls-remote https://github.com/vbardales/Rimworld-Night-Change.git HEAD` reports
`51261b291dceb5be8251c24ea6699b16bc479a1b`: pushed commit exists.
Does not establish autonomous local checkout. No missing monorepo remote alleged.

`Night Change`, `nelim.nightchange`, `Rimworld-Night-Change`, and `NightChange` coherent
display/package/repository/folder names. No separate packageName field, linking words,
or required licence suffix applies. Explicit MIT licence belongs to **Nelim**, not
MrBeverage; previous licence_at conflated credited design source with this mod.
`open` supported by LICENSE, with design influence and no copied third-party material
documented in ATTRIBUTION.md. No new third-party permission inferred by audit.
README, ATTRIBUTION, CHANGELOG and licence present in English. Distributed LICENSE
and ATTRIBUTION copies match byte-for-byte. README's "no art" wording describes gameplay
assets; About artwork present. Upstream historical design claims not fresh provenance tests.

### Build and artifact verification

Command: `dotnet build Source/NightChange.csproj -c Release --nologo -p:OutputPath=../.audit/build/`.
SDK 8.0.424; resolved references Krafs.Rimworld.Ref 1.6.4871 and Lib.Harmony 2.4.2.
First sandbox attempt failed with MSB4184 (SDK directory access denied), environment
restriction, not code defect. Authorized retry passed: zero warnings, zero errors.
Logs: `.audit/build.log`, `.audit/build-retry.log`.

Rebuilt and distributed DLL SHA256 both:
`5E623A3F755DF4A471BBC9469FD489724C8ED072FAE8CD71EF80C3CE59AA6DCD`.
Build output isolated; shipped assembly not replaced.
CI only builds and checks DLL existence; not functional test suite.

Both delivered images opened and visually inspected directly. Icon: 128 x 128 PNG,
25,567 bytes, orange winking mascot, nightcap and moon. Preview: readable English title
and summary, bedroom/stand subject, high oblique floor view, cool shadows and warm light.
No concrete camera defect identified; no historical generation report or comparison
capture required. Amber rule has limited chromatic separation from dominant
warm floor/light. No secondary tag or title suffix exists, so comparing secondary-text
colour not applicable. Original PNGs remain under Art/. No image regenerated.

### Settings audit

Source review: all eight C# files, all delivered Defs/patches and NightChange.csproj.
Four useful global settings in NightChangeSettings: inherit bed owner (true), cold guard
(true), cold margin (2 degrees C), bed-to-stand distance (12 cells). Affect ownership
selection, temperature rejection and stand search, respectively, in NightSwap.cs. Values
read at subsequent decisions; do not explicitly restart job already underway.
Primary entry uses SettingsCategory and DoSettingsWindowContents, with translated labels.
Sliders expose 0..10 degrees and 3..40 cells; margin appears only while cold guard on.
ExposeData declares matching serialization defaults. No manual XML editing needed.
No MainButtonDef, MainTabWindow or equivalent shortcut implementation anywhere
in delivered sources/Defs: confirmed missing required feature.

No automated test suite exercises these settings. Defaults and data flow reviewed,
not executed as functional tests. Deserialized numeric values not explicitly clamped
in ExposeData; malformed/older values remain robustness verification to perform, not
demonstrated runtime failure. No RIMMSQOL/customization integration tested. UI access,
actual effects, persistence and translated layout in game remain for final validation.
`settings_audit: partial` blocked by missing shortcut and applicable technical tests,
not lack of in-game run.

### Translation and XML audit

Executed `.audit/Check-Resources.ps1`; results in `.audit/resources.log`:
six XML files parse; twelve literal Translate keys have nonempty EN/FR entries without
duplicates and with matching numbered parameters. Full source review found no additional
owned player-facing hardcoded strings or dynamic translation keys. Technical log strings,
scribe keys, toil names and identifiers excluded. Existing vanilla assignment UI
inherited; no third-party text key explicitly reused.

Owned Def text is `NightChange_ChangeAtStand.reportString`: English source
`changing at TargetA.` and French `se change à TargetA.` preserve native target token.
No redundant English DefInjected file needed. Meaning and parameter use reviewed.
Executed `../scripts/Check-DefInjected.ps1 -TransMod <this folder>/Mod` against installed
game data: 33 patch operations applied, 11,588 Defs indexed, 1 key checked, 0 errors;
no unresolved target reported. Output: `.audit/definjected.log`.
Results cover resources as they exist today; new shortcut text must be audited
when implemented. Runtime text rendering not certified by complete language fields.

### Dependencies and outstanding tests

About targets RimWorld 1.6. Source references Harmony and vanilla/DLC APIs, with no
Shift Change or Outfit Stands Plus assembly reference. Odyssey's installed
`Defs/ThingDefs_Buildings/Buildings_Furniture.xml` contains both target stands; kid
stand has `MayRequire=Ludeon.RimWorld.Biotech`, matching this mod's conditional patches.
Required dependency declarations and load order coherent. No extra mandatory DLC
or customization mod imposed. Optional mod versions/coexistence and Biotech absent
execution not established by source/XML inspection.

To pass **next** transition, establish actual standalone checkout outside
monorepo, with GitHub remote and pushed commit, preserving audited content and
documentation. GitHub existence/public visibility and initial push already pass.
Later gates separately require description link, distinct Preview accent, settings
shortcut and applicable tests. Final game scenarios must cover normal evening/morning
changes, medical/forced sleep, cold guard and distance boundaries, assignment, emergency
behavior, interrupted changes, forced apparel flags, stand removal, persistence, and
optional integrations, in EN/FR and new/existing saves. None claimed executed here.

## Historical sweep notes (preserved)

Describes 2026-09-12 sweep and old field vocabulary, not current
audit evidence. Empty tested_on field and absence of game verification remain unchanged.

Read by sweep across every mod, rather than asking each thread in turn. Lives at
root, never inside `Mod/`, so Steam never receives it.

Fields above read off disk 2026-09-12. Four cannot be, wait for whoever
holds this mod:

- **`stage`** — one of `port`, `showcase`, `preTest`, `done`, `tested`, `published`. Filled in
  from session group where one exists; confirm it.
- **`tested_on`** — date of last run in game. Empty means never.
- **`dependencies`** — `declared` when every mod this one needs named in About's
  `modDependencies`, `to check` when non-vanilla `loadAfter` suggests undeclared dependency,
  `none` when mod needs nothing. Undeclared dependency not cosmetic: on
  2026-09-11 Reequilibrage animaux took 47 vanilla animals down with it, Muffalo included, because
  class it injects belongs to mod not declared and not loaded.
- **`remaining`** — what is left, three kinds: `feature` for something missing from first
  release, `defect` for known fault left unfixed, `unverified` for what could not be checked.
  Line already there true of nearly whole repository; replace once it stops being.

`licence` vocabulary: `open` explicit licence, `silent` no licence and dead source,
`alive` no licence but living source, `forbidden` written refusal, `original` owing nothing
to anyone — not a name, not an idea traceable to one mod, not a value derived from its assets.