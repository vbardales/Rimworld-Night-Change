# Runs history

One line per run. Newest last. Evidence files stay on disk (gitignored); only these lines are versioned.

- 2026-09-13 audit build: `dotnet build Source/NightChange.csproj -c Release` 0 warnings/0 errors, DLL rebuilt equal to shipped (superseded by the 2026-09-13 correction build).
- 2026-09-13 correction set: build 0/0; `NightChange.Tests.exe` 13 assertions passed; `Tests/Check-Xml.ps1` passed (8 XML, 13 keys); `Check-DefInjected` 3 keys, 0 errors. Shipped DLL SHA256 CE8EFA93A61D74EBC6D95C1A0D45012E901FFD152488D549AADC2651F800C98C. No game run.
- 2026-09-29 audit (revision origin/main 51261b2 + correction set): build 0/0, DLL SHA256 unchanged, 13 assertions passed, Check-Xml passed, Check-DefInjected 0 errors. No game run, no Pickle run (no suite yet). Old `.audit/` logs superseded by this line (folder deleted 2026-09-29).- 2026-09-29 Pickle suite written (never run): 14 features, 40 scenarios, step assembly builds with 0 warnings, all step lines matched to a step by pattern. No game run.
- 2026-09-29 review fix: settings shortcut guards a null mod instance; build 0/0, 13 assertions passed; DLL SHA256 DA2890A6CE7E5D02938ACE7000B884477FA1881AC327E96F423CEC3825F92B88.
