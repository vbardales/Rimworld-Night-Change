# Runs history

One line per run. Newest last. Evidence files stay on disk (gitignored); only these lines are versioned.

- 2026-09-13 audit build: `dotnet build Source/NightChange.csproj -c Release` 0 warnings/0 errors, DLL rebuilt equal to shipped (superseded by the 2026-09-13 correction build).
- 2026-09-13 correction set: build 0/0; `NightChange.Tests.exe` 13 assertions passed; `Tests/Check-Xml.ps1` passed (8 XML, 13 keys); `Check-DefInjected` 3 keys, 0 errors. Shipped DLL SHA256 CE8EFA93A61D74EBC6D95C1A0D45012E901FFD152488D549AADC2651F800C98C. No game run.
- 2026-09-29 audit (revision origin/main 51261b2 + correction set): build 0/0, DLL SHA256 unchanged, 13 assertions passed, Check-Xml passed, Check-DefInjected 0 errors. No game run, no Pickle run (no suite yet). Old `.audit/` logs superseded by this line (folder left on disk, gitignored).