# ============================================
# Lifes - Quick Test Runner
# ============================================
# Run tests quickly without coverage report
# ============================================

param(
    [string]$Filter = "",
    [switch]$Watch
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "⚡ Lifes - Quick Test Runner" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get script directory and project root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptDir "..\..")

# Change to project root
Push-Location $projectRoot

try {
    if ($Watch) {
        Write-Host "👀 Running tests in watch mode..." -ForegroundColor Yellow
        Write-Host "   Press Ctrl+C to stop" -ForegroundColor Gray
        Write-Host ""
        
        if ($Filter -ne "") {
            dotnet watch test --filter "$Filter"
        } else {
            dotnet watch test
        }
    } else {
        Write-Host "🧪 Running tests..." -ForegroundColor Yellow
        Write-Host ""
        
        if ($Filter -ne "") {
            Write-Host "   Filter: $Filter" -ForegroundColor Gray
            dotnet test --filter "$Filter" --verbosity normal
        } else {
            dotnet test --verbosity normal
        }
        
        Write-Host ""
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ All tests passed!" -ForegroundColor Green
        } else {
            Write-Host "❌ Some tests failed!" -ForegroundColor Red
        }
        
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Cyan
        Write-Host ""
    }
}
finally {
    # Return to original directory
    Pop-Location
}
