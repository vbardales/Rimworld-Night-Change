# Testing

Functional scenarios: `TEST_SCENARIOS.md` (18 rows, the manual originals, none run by hand; the Pickle suite below replaces them where the table says so). Offline results: `Tests/RESULTS.md`, one line per run in `docs/runs/history.md`.

## Offline (green 2026-09-29)

- `dotnet build Tests/NightChange.Tests.csproj -c Release --nologo`
- `.build/tests/NightChange.Tests.exe "<RimWorld Managed dir>" <tmp dir>` (13 assertions)
- `powershell -File Tests/Check-Xml.ps1`
- `scripts/Check-DefInjected.ps1 -TransMod <folder>/Mod` (monorepo script)

## Pickle suite (written 2026-09-29, never run)

Scope, features, passes and the fixture: [Tests/Pickle/README.md](Tests/Pickle/README.md). Six passes:
without optional mods (English, French), with RIMMSQOL, with Shift Change, restart chain, without Biotech.
No pass for Outfit Stands Plus (Workshop item taken down, not stageable) and none for a declared
incompatibility (there is none).

### Where each manual scenario of TEST_SCENARIOS.md went

| TEST_SCENARIOS row | Now |
| --- | --- |
| Clean settings, defaults, no visible or grey shortcut | `07` (+ offline defaults) |
| Global settings persist | `09`/`10` restart chain; layout and clipping by the `07` captures |
| RIMMSQOL | `08` |
| Other claimed customization mod | Not applicable: no other integration is claimed |
| Bedroom cycle, forced flags | `02` |
| Unassigned stand, owner inheritance | `04` |
| Two colonists in a shared room, reassigned overnight | `04` |
| Distance | `04` |
| Cold guard | `03` (boundary values offline) |
| Midnight snack | `02` |
| Forced sleep, medical rest | `03` |
| Raid; drafted | `03`. **Fire and downed are not exercised** (unverified) |
| Change in progress, save and reload | `05` (a save and reload in one process, not a process restart) |
| Borrowed outfit across reload | `05` |
| Stand destroyed | `06` (garment removed from the stand, or a blocked path: not exercised) |
| Pawn departure | `06` banish (death, sale, off-map: not exercised) |
| Biotech absent / present | `12`, `13` |
| Optional outfit mods | `11` Shift Change; Outfit Stands Plus not stageable |
## Evidence to keep

Reports live on disk in `Tests/Pickle/Evidence/` (gitignored, never committed). Per scenario keep the latest report for the revision now in the repository (plus an older one only if it is the sole proof of a check the latest run did not repeat). Minify: JPEG for captures, keep `summary.json` and `junit.xml`, drop the rest. Delete a report once a newer one for the same scenario replaces it, after repointing any `STATUS.md` field that names it. History = one line per run in `docs/runs/`.