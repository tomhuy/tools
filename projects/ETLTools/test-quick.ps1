# Quick alias to run tests without coverage
# Usage: .\test-quick.ps1 [-Filter "TestName"] [-Watch]

param(
    [string]$Filter = "",
    [switch]$Watch
)

$scriptPath = Join-Path $PSScriptRoot "tasks\run-tests\run-tests-quick.ps1"

if ($Filter -ne "") {
    & $scriptPath -Filter $Filter -Watch:$Watch
} else {
    & $scriptPath -Watch:$Watch
}
