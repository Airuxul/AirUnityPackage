# Compile + console check via unity-cmd (fixed profile: editor).
param(
    [int]$CompileTimeoutMs = 20000,
    [int]$ConsoleLines = 200,
    [switch]$IncludeWarnings
)

$ErrorActionPreference = "Stop"
$cmdRoot = Join-Path $PSScriptRoot "..\packages\unity-cli\unity-cmd"

if (-not (Test-Path $cmdRoot)) {
    Write-Host "unity-cmd folder not found: $cmdRoot" -ForegroundColor Red
    exit 2
}

Push-Location $cmdRoot
try {
    function Invoke-CmdJson {
        param([string[]]$CmdArgs)
        $raw = & node bin/unity-cmd.js @CmdArgs 2>&1 | Out-String
        try {
            return ($raw | ConvertFrom-Json)
        } catch {
            Write-Host "Failed to parse unity-cmd JSON output." -ForegroundColor Red
            Write-Host $raw -ForegroundColor DarkYellow
            throw
        }
    }

    function Print-EditorLogHints {
        $log = Join-Path $env:LOCALAPPDATA "Unity\Editor\Editor.log"
        if (-not (Test-Path $log)) {
            return
        }
        $errs = Select-String -Path $log -Pattern "error CS" | Select-Object -Last 8
        if ($errs) {
            Write-Host "Recent Editor.log CS errors:" -ForegroundColor Red
            $errs | ForEach-Object { Write-Host $_.Line }
        }
    }

    $consoleTypes = if ($IncludeWarnings) { "error,warning" } else { "error" }

    $ping = Invoke-CmdJson -CmdArgs @("--profile", "editor", "ping")
    if (-not $ping.ok) {
        Write-Host "Unity unreachable (profile=editor)." -ForegroundColor Yellow
        Print-EditorLogHints
        exit 2
    }

    $compile = Invoke-CmdJson -CmdArgs @("--profile", "editor", "compile", "--timeout", "$CompileTimeoutMs")
    if (-not $compile.ok) {
        Write-Host "Compile request failed: $($compile.error)" -ForegroundColor Red
        exit 1
    }
    Write-Host "Compile request ok. command_id=$($compile.command_id)" -ForegroundColor Green

    $console = Invoke-CmdJson -CmdArgs @("--profile", "editor", "console", "--type", $consoleTypes, "--lines", "$ConsoleLines")
    $entries = @($console.data.entries)
    $count = [int]$console.data.count
    if ($count -eq 0) {
        Write-Host "No $consoleTypes in console." -ForegroundColor Green
        exit 0
    }

    Write-Host "$count issue(s) found in console:" -ForegroundColor Red
    foreach ($entry in $entries) {
        $kind = $entry.type
        $msg = $entry.message
        Write-Host "[$kind] $msg"
    }
    Write-Host "Fix issues and run this script again." -ForegroundColor Yellow
    exit 1
}
finally {
    Pop-Location
}
