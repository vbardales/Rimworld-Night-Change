# Validation results — 2026-09-13

The audit correction build succeeds with .NET SDK 8.0.424, Krafs.Rimworld.Ref 1.6.4871,
Lib.Harmony 2.4.2: zero warnings and errors. `Tests/NightChange.Tests.csproj` references
the actual mod project, and its console executable uses the installed game assemblies.
No RimWorld executable is started.

13 assertions passed: clean defaults; NaN and both infinite margin fallbacks; lower/upper
numeric bounds; exact cold threshold, below threshold, changed margin and no insulation
loss; actual Scribe save/load round trip; missing-value defaults; out-of-range loaded values.
This verifies serialization and decision logic, not Unity rendering or a game restart.

`Tests/Check-Xml.ps1` passed: 8 XML files, 13 EN/FR Keyed keys with matching parameters,
3 validated DefInjected fields (parent checker: zero errors, 11,589 indexed Defs).
Shipped DLL SHA256: `DA2890A6CE7E5D02938ACE7000B884477FA1881AC327E96F423CEC3825F92B88`.
These results correspond to the source correction set described in STATUS.md, not the
older audit DLL. The script validates XML syntax, used Keyed resources and parameters, matching
distributed attribution/licence copies, image metadata, final description link and hidden
shortcut definition/French fields. The parent's Check-DefInjected.ps1 validates actual
injection paths against installed RimWorld data. Detailed outputs are retained locally
under `.audit/`; this directory is intentionally not shipped or committed.

The shortcut inherits native MainButtonWorker visibility, which reads `def.buttonVisible`.
Its Activate method opens Dialog_ModSettings with the same NightChangeMod instance.
The native dialog calls WriteSettings on close. These mechanisms were checked against
the installed decompiled assembly and compiled, not interactively tested with RIMMSQOL.
No version of a customization mod is claimed tested. See TEST_SCENARIOS.md for all pending
runtime checks; no `tested` status is claimed.
