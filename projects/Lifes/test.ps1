# Quick alias to run tests with coverage
# Usage: .\test.ps1 [-Filter "TestName"] [-SkipBuild] [-SkipReport]

param(
    [switch]$SkipBuild,
    [switch]$SkipReport,
    [string]$Filter = ""
)

$scriptPath = Join-Path $PSScriptRoot "tasks\run-tests\run-tests-with-coverage.ps1"

if ($Filter -ne "") {
    & $scriptPath -Filter $Filter -SkipBuild:$SkipBuild -SkipReport:$SkipReport
} else {
    & $scriptPath -SkipBuild:$SkipBuild -SkipReport:$SkipReport
}
