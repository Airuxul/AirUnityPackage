#Requires -Version 5.1
$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
Push-Location $repoRoot
try {
    git config core.hooksPath .githooks
    Write-Host "Installed hooks path: .githooks" -ForegroundColor Green
    Write-Host "Pre-commit runs: tools/validate-docs.ps1"
}
finally {
    Pop-Location
}
