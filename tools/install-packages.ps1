#Requires -Version 5.1
<#
.SYNOPSIS
  Add missing local Air UPM packages to Unity Packages/manifest.json (file: dependencies).

.PARAMETER UnityProject
  Unity project root (folder containing Assets/ and Packages/).

.PARAMETER MetaRoot
  AirUnityPackage meta repo root (contains config/registry.json).

.PARAMETER RegistryPath
  Override path to registry JSON.
#>
param(
    [Parameter(Mandatory = $false)]
    [string] $UnityProject,

    [Parameter(Mandatory = $false)]
    [string] $MetaRoot,

    [Parameter(Mandatory = $false)]
    [string] $RegistryPath
)

$ErrorActionPreference = "Stop"

function Get-RelativePathCompat {
    param([string] $From, [string] $To)
    $fromFull = [System.IO.Path]::GetFullPath($From)
    $toFull = [System.IO.Path]::GetFullPath($To)
    if ($fromFull.EndsWith([System.IO.Path]::DirectorySeparatorChar) -eq $false) {
        $fromFull += [System.IO.Path]::DirectorySeparatorChar
    }
    $fromUri = New-Object System.Uri($fromFull)
    $toUri = New-Object System.Uri($toFull)
    return [System.Uri]::UnescapeDataString($fromUri.MakeRelativeUri($toUri).ToString())
}

if (-not $MetaRoot) {
    $scriptDir = if ($PSScriptRoot) {
        $PSScriptRoot
    } else {
        Split-Path -Parent $MyInvocation.MyCommand.Path
    }
    $MetaRoot = Split-Path -Parent $scriptDir
}

if (-not $RegistryPath) {
    $RegistryPath = Join-Path $MetaRoot "config\registry.json"
}

if (-not (Test-Path -LiteralPath $RegistryPath)) {
    throw "Registry not found: $RegistryPath"
}

$registry = Get-Content -LiteralPath $RegistryPath -Raw -Encoding UTF8 | ConvertFrom-Json

if (-not $UnityProject) {
    $UnityProject = $registry.defaultUnityProject
    if (-not [System.IO.Path]::IsPathRooted($UnityProject)) {
        $UnityProject = Join-Path $MetaRoot $UnityProject
    }
}

$UnityProject = (Resolve-Path -LiteralPath $UnityProject).Path
$manifestPath = Join-Path $UnityProject "Packages\manifest.json"

if (-not (Test-Path -LiteralPath $manifestPath)) {
    throw "Unity manifest not found: $manifestPath`nPass -UnityProject <path> (project root with Packages/manifest.json)."
}

$packageIds = @(
    $registry.packages | Where-Object { $_.installDefault } | ForEach-Object { $_.id }
)
if ($packageIds.Count -eq 0) {
    throw "No packages with installDefault in registry: $RegistryPath"
}

$manifestJson = Get-Content -LiteralPath $manifestPath -Raw -Encoding UTF8
$manifest = $manifestJson | ConvertFrom-Json

if (-not $manifest.dependencies) {
    $manifest | Add-Member -NotePropertyName "dependencies" -NotePropertyValue ([PSCustomObject]@{})
}

$packagesDir = Join-Path $UnityProject "Packages"
$packagesRoot = if ($registry.packagesRoot) { $registry.packagesRoot } else { "" }
$byId = @{}
foreach ($entry in $registry.packages) {
    $byId[$entry.id] = $entry
}

$added = @()
$skipped = @()
$changed = $false

foreach ($packageId in $packageIds) {
    if ($manifest.dependencies.PSObject.Properties[$packageId]) {
        $skipped += $packageId
        continue
    }

    $entry = $byId[$packageId]
    if (-not $entry) {
        throw "Package '$packageId' (installDefault) is missing from registry.packages."
    }

    $packageDir = if ($packagesRoot) {
        Join-Path $MetaRoot (Join-Path $packagesRoot $entry.folder)
    } else {
        Join-Path $MetaRoot $entry.folder
    }
    if (-not (Test-Path -LiteralPath $packageDir)) {
        throw "Package folder missing: $packageDir`nRun: git submodule update --init --recursive"
    }

    $packageDir = (Resolve-Path -LiteralPath $packageDir).Path
    $pkgJsonPath = Join-Path $packageDir "package.json"
    if (-not (Test-Path -LiteralPath $pkgJsonPath)) {
        throw "Invalid UPM package (no package.json): $packageDir"
    }

    $relative = Get-RelativePathCompat -From $packagesDir -To $packageDir
    $relative = $relative.Replace('\', '/')
    $fileRef = "file:$relative"

    $manifest.dependencies | Add-Member -NotePropertyName $packageId -NotePropertyValue $fileRef
    $changed = $true

    $pkgMeta = Get-Content -LiteralPath $pkgJsonPath -Raw -Encoding UTF8 | ConvertFrom-Json
    $added += [PSCustomObject]@{
        Id      = $packageId
        Version = $pkgMeta.version
        File    = $fileRef
    }
}

if ($changed) {
    $out = $manifest | ConvertTo-Json -Depth 20
    $out = ($out | ConvertFrom-Json | ConvertTo-Json -Depth 20)
    $utf8NoBom = New-Object System.Text.UTF8Encoding $false
    [System.IO.File]::WriteAllText($manifestPath, $out + "`n", $utf8NoBom)
}

Write-Host ""
if ($changed) {
    Write-Host "Updated:" -ForegroundColor Green
    Write-Host "  $manifestPath"
} else {
    Write-Host "No changes (all packages already in manifest):" -ForegroundColor Green
    Write-Host "  $manifestPath"
}
Write-Host ""

if ($added.Count -gt 0) {
    Write-Host "Added:" -ForegroundColor Cyan
    foreach ($row in $added) {
        Write-Host ("  {0}@{1}" -f $row.Id, $row.Version)
        Write-Host ("    -> {0}" -f $row.File) -ForegroundColor DarkGray
    }
    Write-Host ""
}

if ($skipped.Count -gt 0) {
    Write-Host "Skipped (already configured):" -ForegroundColor DarkGray
    foreach ($id in $skipped) {
        Write-Host ("  {0}" -f $id)
    }
    Write-Host ""
}

if ($changed) {
    Write-Host "Reopen Unity or wait for Package Manager to resolve." -ForegroundColor Yellow
}
