# Testing

Functional scenarios: `TEST_SCENARIOS.md` (10 manual checks, none run). Offline results: `Tests/RESULTS.md`, one line per run in `docs/runs/history.md`.

## Offline (green 2026-09-29)

- `dotnet build Tests/NightChange.Tests.csproj -c Release --nologo`
- `.build/tests/NightChange.Tests.exe "<RimWorld Managed dir>" <tmp dir>` (13 assertions)
- `powershell -File Tests/Check-Xml.ps1`
- `scripts/Check-DefInjected.ps1 -TransMod <folder>/Mod` (monorepo script)

## Pickle passes (not written yet)

1. Without optional mods (Core, Odyssey, Harmony, Pickle, the mod), English.
2. Same, French (`-Language French`).
3. With optional mods: one pass per exclusive combination (Shift Change, Outfit Stands Plus, Biotech). No declared incompatibility, so no incompat pass.

## Evidence to keep

Reports live on disk in `Tests/Pickle/Evidence/` (gitignored, never committed). Per scenario keep the latest report for the revision now in the repository (plus an older one only if it is the sole proof of a check the latest run did not repeat). Minify: JPEG for captures, keep `summary.json` and `junit.xml`, drop the rest. Delete a report once a newer one for the same scenario replaces it, after repointing any `STATUS.md` field that names it. History = one line per run in `docs/runs/`.