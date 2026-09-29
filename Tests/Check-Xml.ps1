$ErrorActionPreference = 'Stop'
Set-Location (Split-Path $PSScriptRoot -Parent)
$files = @(Get-ChildItem Mod -Recurse -Filter *.xml)
foreach ($file in $files) { $null = [xml](Get-Content $file.FullName -Raw) }
"XML syntax: $($files.Count) files passed"
$tables = @{}
foreach ($lang in @('English', 'French')) {
    $table = @{}
    [xml]$doc = Get-Content "Mod/Languages/$lang/Keyed/NightChange.xml" -Raw
    foreach ($node in $doc.LanguageData.ChildNodes | Where-Object NodeType -eq Element) {
        if ($table.ContainsKey($node.Name) -or [string]::IsNullOrWhiteSpace($node.InnerText)) { throw "Duplicate/empty key: $lang $($node.Name)" }
        $table[$node.Name] = $node.InnerText
    }
    $tables[$lang] = $table
}
$source = (Get-Content Source/*.cs -Raw) -join "`n"
$keys = @([regex]::Matches($source, '"(NightChange_[^"]+)"\.Translate\(') | ForEach-Object { $_.Groups[1].Value } | Sort-Object -Unique)
foreach ($key in $keys) {
    foreach ($lang in @('English', 'French')) { if (!$tables[$lang].ContainsKey($key)) { throw "Missing $lang $key" } }
    $en = @([regex]::Matches($tables.English[$key], '\{\d+\}') | ForEach-Object Value) -join ','
    $fr = @([regex]::Matches($tables.French[$key], '\{\d+\}') | ForEach-Object Value) -join ','
    if ($en -ne $fr) { throw "Parameter mismatch: $key" }
}
"Keyed: $($keys.Count) used keys covered in EN/FR, no empty or duplicate entries; parameter parity passed"
foreach ($name in @('LICENSE', 'ATTRIBUTION.md')) {
    if ((Get-FileHash $name).Hash -ne (Get-FileHash "Mod/$name").Hash) { throw "Copy mismatch: $name" }
    "$name distribution copy matches"
}
Add-Type -AssemblyName System.Drawing
foreach ($name in @('Preview', 'ModIcon')) {
    $file = Get-Item "Mod/About/$name.png"
    $img = [System.Drawing.Image]::FromFile($file.FullName)
    "$name : $($img.Width)x$($img.Height), $($file.Length) bytes, format $($img.RawFormat)"
    $img.Dispose()
}
[xml]$about = Get-Content Mod/About/About.xml -Raw
if (!$about.ModMetaData.description.Trim().EndsWith('[url=https://github.com/vbardales/Rimworld-Night-Change]Source code on GitHub[/url]')) { throw 'Missing final repository link' }
[xml]$buttons = Get-Content Mod/Defs/NightChange_MainButtons.xml -Raw
$button = $buttons.Defs.MainButtonDef
if ($button.buttonVisible -ne 'false' -or $button.workerClass -ne 'NightChange.MainButtonWorker_NightChangeSettings') { throw 'Shortcut contract mismatch' }
[xml]$french = Get-Content Mod/Languages/French/DefInjected/MainButtonDef/NightChange.xml -Raw
foreach ($field in @('label', 'description')) {
    if (!$french.LanguageData.SelectSingleNode("NightChange_Settings.$field").InnerText) { throw "Missing shortcut translation: $field" }
}
"Description and hidden shortcut XML contract: passed"
