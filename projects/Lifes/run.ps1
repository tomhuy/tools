# ============================================
# Lifes - Main Automation Menu
# ============================================
# Central navigation hub for all automation tasks
# ============================================

function Show-Banner {
    Clear-Host
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "    Lifes - Automation Menu" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
}

function Show-Menu {
    Write-Host "Select an option:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  1. " -NoNewline -ForegroundColor White
    Write-Host "Run Tests (Quick)" -ForegroundColor Gray
    Write-Host "  2. " -NoNewline -ForegroundColor White
    Write-Host "Run Tests (With Coverage)" -ForegroundColor Gray
    Write-Host "  3. " -NoNewline -ForegroundColor White
    Write-Host "Build Application" -ForegroundColor Gray
    Write-Host "  4. " -NoNewline -ForegroundColor White
    Write-Host "Build & Deploy Application" -ForegroundColor Gray
    Write-Host "  5. " -NoNewline -ForegroundColor White
    Write-Host "Build Electron Application" -ForegroundColor Gray
    Write-Host "  6. " -NoNewline -ForegroundColor White
    Write-Host "Build & Deploy Electron Application" -ForegroundColor Gray
    Write-Host "  7. " -NoNewline -ForegroundColor White
    Write-Host "Exit" -ForegroundColor Gray
    Write-Host ""
}

function Invoke-TestsQuick {
    Write-Host "Launching Quick Tests..." -ForegroundColor Green
    Write-Host ""
    $scriptPath = Join-Path $PSScriptRoot "test-quick.ps1"
    & $scriptPath
    Write-Host ""
    Write-Host "Press any key to return to menu..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

function Invoke-TestsCoverage {
    Write-Host "Launching Tests with Coverage..." -ForegroundColor Green
    Write-Host ""
    $scriptPath = Join-Path $PSScriptRoot "test.ps1"
    & $scriptPath
    Write-Host ""
    Write-Host "Press any key to return to menu..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

function Invoke-Build {
    Write-Host "Launching Build..." -ForegroundColor Green
    Write-Host ""
    $scriptPath = Join-Path $PSScriptRoot "tasks\build-deploy\build.ps1"
    & $scriptPath
    Write-Host ""
    Write-Host "Press any key to return to menu..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

function Invoke-BuildDeploy {
    Write-Host "Launching Build & Deploy..." -ForegroundColor Green
    Write-Host ""
    $scriptPath = Join-Path $PSScriptRoot "tasks\build-deploy\build-deploy.ps1"
    & $scriptPath
    Write-Host ""
    Write-Host "Press any key to return to menu..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

function Invoke-BuildElectron {
    Write-Host "Launching Build Electron Application..." -ForegroundColor Green
    Write-Host ""
    $scriptPath = Join-Path $PSScriptRoot "tasks\build-deploy\build-electron.ps1"
    & $scriptPath
    Write-Host ""
    Write-Host "Press any key to return to menu..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

function Invoke-BuildDeployElectron {
    Write-Host "Launching Build & Deploy Electron Application..." -ForegroundColor Green
    Write-Host ""
    $scriptPath = Join-Path $PSScriptRoot "tasks\build-deploy\build-deploy-electron.ps1"
    & $scriptPath
    Write-Host ""
    Write-Host "Press any key to return to menu..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}

# Main loop
$continue = $true
while ($continue) {
    Show-Banner
    Show-Menu
    
    Write-Host "Enter your choice (1-7): " -NoNewline -ForegroundColor Yellow
    $choice = Read-Host
    
    Write-Host ""
    
    switch ($choice) {
        "1" {
            Invoke-TestsQuick
        }
        "2" {
            Invoke-TestsCoverage
        }
        "3" {
            Invoke-Build
        }
        "4" {
            Invoke-BuildDeploy
        }
        "5" {
            Invoke-BuildElectron
        }
        "6" {
            Invoke-BuildDeployElectron
        }
        "7" {
            Write-Host "Exiting..." -ForegroundColor Green
            $continue = $false
        }
        default {
            Write-Host "Invalid option. Please select 1-7." -ForegroundColor Red
            Start-Sleep -Seconds 2
        }
    }
}

Write-Host ""
Write-Host "Goodbye!" -ForegroundColor Cyan
Write-Host ""
