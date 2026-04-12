# ============================================
# Lifes - Build Script
# ============================================
# Build WPF application with specified configuration
# ============================================

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [switch]$Clean,
    [switch]$NoRestore
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Lifes - Build Application" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get script directory and project root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptDir "..\..")

# Read configuration
$configPath = Join-Path $scriptDir "deploy-config.json"
if (Test-Path $configPath) {
    $config = Get-Content $configPath | ConvertFrom-Json
    $projectPath = Join-Path $projectRoot $config.projectPath
} else {
    Write-Host "Warning: deploy-config.json not found, using default path" -ForegroundColor Yellow
    $projectPath = Join-Path $projectRoot "src\Lifes.Presentation.WPF\Lifes.Presentation.WPF.csproj"
}

# Change to project root
Push-Location $projectRoot

try {
    # Verify project exists
    if (-not (Test-Path $projectPath)) {
        Write-Host "Error: Project file not found: $projectPath" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
    Write-Host "Project: $projectPath" -ForegroundColor Gray
    Write-Host ""
    
    # Clean if requested
    if ($Clean) {
        Write-Host "Cleaning previous build..." -ForegroundColor Yellow
        dotnet clean "$projectPath" --configuration $Configuration
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Clean failed!" -ForegroundColor Red
            exit 1
        }
        Write-Host "Clean complete!" -ForegroundColor Green
        Write-Host ""
    }
    
    # Restore NuGet packages
    if (-not $NoRestore) {
        Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
        dotnet restore "$projectPath"
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Restore failed!" -ForegroundColor Red
            exit 1
        }
        Write-Host "Restore complete!" -ForegroundColor Green
        Write-Host ""
    }
    
    # Build project
    Write-Host "Building application..." -ForegroundColor Yellow
    Write-Host ""
    
    dotnet build "$projectPath" --configuration $Configuration --no-restore
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "Build failed!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Build Successful!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    
    # Display output location
    $outputPath = Join-Path (Split-Path $projectPath) "bin\$Configuration\net6.0-windows"
    Write-Host ""
    Write-Host "Output Location:" -ForegroundColor Yellow
    Write-Host "  $outputPath" -ForegroundColor Gray
    Write-Host ""
    
    # Check if output exists
    if (Test-Path $outputPath) {
        $exeFile = Get-ChildItem $outputPath -Filter "*.exe" | Select-Object -First 1
        if ($exeFile) {
            Write-Host "Executable: $($exeFile.Name)" -ForegroundColor Green
        }
    }
    
    Write-Host ""
    Write-Host "Done!" -ForegroundColor Green
    Write-Host ""
}
catch {
    Write-Host ""
    Write-Host "Error: $_" -ForegroundColor Red
    exit 1
}
finally {
    Pop-Location
}
