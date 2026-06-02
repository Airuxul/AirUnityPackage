#Requires -Version 5.1
param()

$ErrorActionPreference = "Stop"

function Fail($message) {
    Write-Host "docs-check failed: $message" -ForegroundColor Red
    exit 1
}

$staged = @(git diff --cached --name-only)
if ($staged.Count -eq 0) {
    exit 0
}

$docChanged = $false
foreach ($file in $staged) {
    if ($file -match '\.md$' -or $file -match '^docs/' -or $file -match '^README(\..+)?\.md$') {
        $docChanged = $true
        break
    }
}

if (-not $docChanged) {
    exit 0
}

$requiredFiles = @(
    "docs/AGENTS.md",
    "docs/DOC_GOVERNANCE.md",
    "README.md",
    "README.zh-CN.md"
)

foreach ($file in $requiredFiles) {
    if (-not (Test-Path -LiteralPath $file)) {
        Fail "missing required file: $file"
    }
}

$userFiles = @("README.md", "README.zh-CN.md")
$userChanged = @($staged | Where-Object { $_ -in $userFiles }).Count -gt 0
$agentChanged = @($staged | Where-Object { $_ -like "docs/*.md" }).Count -gt 0

if ($userChanged -and -not $agentChanged) {
    Fail "user docs changed but no docs/*.md file changed (update agent markdown or document exemption in commit message)"
}

if ($agentChanged -and -not $userChanged) {
    Fail "docs/ changed but README.md and README.zh-CN.md were not updated"
}

$readmeChanged = $staged -contains "README.md"
$zhChanged = $staged -contains "README.zh-CN.md"
if ($readmeChanged -xor $zhChanged) {
    Fail "README.md and README.zh-CN.md must be updated together"
}

Write-Host "docs-check passed" -ForegroundColor Green
exit 0
