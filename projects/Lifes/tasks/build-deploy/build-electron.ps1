# ============================================
# Lifes - Build & Deploy Electron
# ============================================
# This script builds both the .NET WebApi and the Angular frontend,
# and packages them into a single Electron application.
# ============================================

param(
    [switch]$Clean
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Building Electron Application (Frontend + Backend)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get script directory and project root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptDir "..\..")

# Change to project root
Push-Location $projectRoot

try {
    # 1. Paths
    $webApiPath = "src\Lifes.Presentation.WebApi"
    $electronPath = "src\Lifes.Presentation.Electron"
    $backendDistPath = Join-Path $electronPath "backend-dist"

    if ($Clean) {
        Write-Host "Cleaning backend-dist..." -ForegroundColor Yellow
        if (Test-Path $backendDistPath) {
            Remove-Item -Recurse -Force $backendDistPath
        }
        $distElectronPath = Join-Path $electronPath "dist-electron"
        if (Test-Path $distElectronPath) {
            Write-Host "Cleaning dist-electron..." -ForegroundColor Yellow
            Remove-Item -Recurse -Force $distElectronPath
        }
    }

    # 2. Build .NET Backend
    Write-Host "`n[1/3] Building .NET WebApi Backend..." -ForegroundColor Yellow
    
    # We build the backend into a folder inside the Electron project so electron-builder can grab it
    $dotnetArgs = @("publish", $webApiPath, "-c", "Release", "-o", $backendDistPath)
    
    $dotnetProcess = Start-Process dotnet -ArgumentList $dotnetArgs -NoNewWindow -Wait -PassThru
    if ($dotnetProcess.ExitCode -ne 0) {
        throw "Failed to build .NET WebApi Backend (Exit Code: $($dotnetProcess.ExitCode))"
    }

    # 3. Build Angular Frontend
    Write-Host "`n[2/3] Building Angular Frontend..." -ForegroundColor Yellow
    Push-Location $electronPath
    
    try {
        # Using cmd /c npm because npm.ps1 might be blocked by execution policy
        $npmBuildProcess = Start-Process cmd -ArgumentList "/c npm run build" -NoNewWindow -Wait -PassThru
        if ($npmBuildProcess.ExitCode -ne 0) {
            throw "Failed to build Angular Frontend (Exit Code: $($npmBuildProcess.ExitCode))"
        }

        # 4. Package Electron App
        Write-Host "`n[3/3] Packaging Electron Application..." -ForegroundColor Yellow
        $npmPackageProcess = Start-Process cmd -ArgumentList "/c npm run package" -NoNewWindow -Wait -PassThru
        
        # Check if output directory exists even if exit code is non-zero
        $outputDir = Join-Path "dist-electron" "win-unpacked"
        
        if ($npmPackageProcess.ExitCode -ne 0) {
            if (Test-Path $outputDir) {
                Write-Host "Warning: Electron packaging reported an error, but the unpacked directory was found. Proceeding..." -ForegroundColor Yellow
            } else {
                throw "Failed to package Electron Application (Exit Code: $($npmPackageProcess.ExitCode))"
            }
        }
    }
    finally {
        Pop-Location # Return to projectRoot
    }

    Write-Host "`n========================================" -ForegroundColor Green
    Write-Host "Build & Package Successful!" -ForegroundColor Green
    Write-Host "Output is located in: $electronPath\dist-electron" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    exit 0
}
catch {
    Write-Host "Exception: $_" -ForegroundColor Red
    exit 1
}
finally {
    # Cleanup and return to original directory
    Pop-Location
}
