# ============================================
# Lifes - Build & Deploy Electron
# ============================================
# Build and deploy the Electron application to target directory
# ============================================

param(
    [Parameter(Mandatory=$false)]
    [string]$DeployPath = "",
    
    [switch]$Clean,
    [switch]$NoBackup
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Lifes - Build & Deploy Electron" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Read configuration
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$configPath = Join-Path $scriptDir "deploy-config-electron.json"
if (-not (Test-Path $configPath)) {
    Write-Host "Error: deploy-config-electron.json not found!" -ForegroundColor Red
    exit 1
}

$config = Get-Content $configPath | ConvertFrom-Json

if ($DeployPath -eq "") {
    $DeployPath = $config.deployPath
}

$backupPath = $config.backupPath
$backupEnabled = $config.backupSettings.enabled -and (-not $NoBackup)

# Get script directory and project root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptDir "..\..")

# Set paths
$electronPath = Join-Path $projectRoot $config.projectPath
$distPath = Join-Path $electronPath "dist-electron"

Write-Host "Deploy Path: $DeployPath" -ForegroundColor Yellow
if ($backupEnabled) {
    Write-Host "Backup Path: $backupPath" -ForegroundColor Yellow
}
Write-Host ""

# Change to project root
Push-Location $projectRoot

try {
    # Step 1: Build application
    Write-Host "Step 1: Building Electron application..." -ForegroundColor Cyan
    Write-Host ""
    
    $buildScript = Join-Path $scriptDir "build-electron.ps1"
    
    if ($Clean) {
        & $buildScript -Clean
    } else {
        & $buildScript
    }
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed! Deployment aborted." -ForegroundColor Red
        exit 1
    }
    
    Write-Host ""
    
    # Step 2: Backup existing deployment (if enabled)
    if ($backupEnabled -and (Test-Path $DeployPath)) {
        Write-Host "Step 2: Backing up existing deployment..." -ForegroundColor Cyan
        Write-Host ""
        
        # Create backup directory if not exists
        if (-not (Test-Path $backupPath)) {
            Write-Host "Creating backup directory..." -ForegroundColor Yellow
            New-Item -ItemType Directory -Path $backupPath -Force | Out-Null
            Write-Host "Backup directory created: $backupPath" -ForegroundColor Green
            Write-Host ""
        }
        
        # Generate timestamp for backup
        $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
        $zipFileName = "Lifes_Electron_$timestamp.zip"
        $zipFilePath = Join-Path $backupPath $zipFileName
        $tempBackupFolder = Join-Path $backupPath "temp_$timestamp"
        
        try {
            Write-Host "Copying current deployment to temp folder..." -ForegroundColor Yellow
            New-Item -ItemType Directory -Path $tempBackupFolder -Force | Out-Null
            
            $deployFiles = Get-ChildItem $DeployPath -Recurse -File
            Write-Host "Copying $($deployFiles.Count) files..." -ForegroundColor Gray
            Copy-Item -Path "$DeployPath\*" -Destination $tempBackupFolder -Recurse -Force
            
            Write-Host "Creating zip archive..." -ForegroundColor Yellow
            Compress-Archive -Path "$tempBackupFolder\*" -DestinationPath $zipFilePath -Force
            
            Write-Host "Cleaning up temp folder..." -ForegroundColor Gray
            Remove-Item -Path $tempBackupFolder -Recurse -Force
            
            Write-Host "Backup complete: $zipFileName" -ForegroundColor Green
            Write-Host ""
        } catch {
            Write-Host "Warning: Backup failed: $_" -ForegroundColor Yellow
            Write-Host "Continuing with deployment..." -ForegroundColor Yellow
            Write-Host ""
            if (Test-Path $tempBackupFolder) { Remove-Item -Path $tempBackupFolder -Recurse -Force -ErrorAction SilentlyContinue }
        }
    } elseif ($backupEnabled -and (-not (Test-Path $DeployPath))) {
        Write-Host "Step 2: Backup skipped (first-time deployment)" -ForegroundColor Gray
        Write-Host ""
    }

    Write-Host "Step 3: Deploying application..." -ForegroundColor Cyan
    Write-Host ""
    
    if (-not (Test-Path $distPath)) {
        Write-Host "Error: Build output not found: $distPath" -ForegroundColor Red
        exit 1
    }
    
    # Create deploy directory if not exists
    if (-not (Test-Path $DeployPath)) {
        Write-Host "Creating deploy directory..." -ForegroundColor Yellow
        New-Item -ItemType Directory -Path $DeployPath -Force | Out-Null
        Write-Host "Deploy directory created: $DeployPath" -ForegroundColor Green
        Write-Host ""
    }
    
    # Copy all files from win-unpacked
    Write-Host "Copying unpacked application files..." -ForegroundColor Yellow
    
    $unpackedPath = Join-Path $distPath "win-unpacked"
    if (-not (Test-Path $unpackedPath)) {
        Write-Host "Error: win-unpacked directory not found in $distPath" -ForegroundColor Red
        exit 1
    }

    # Copy contents of win-unpacked to DeployPath
    Copy-Item -Path "$unpackedPath\*" -Destination $DeployPath -Recurse -Force
    Write-Host "Application files copied successfully." -ForegroundColor Green
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Deployment Successful!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Deploy Location:" -ForegroundColor Yellow
    Write-Host "  $DeployPath" -ForegroundColor Gray
    
    if ($backupEnabled) {
        Write-Host ""
        Write-Host "Backup Location:" -ForegroundColor Yellow
        Write-Host "  $backupPath" -ForegroundColor Gray
    }
    
    # Show main executable
    $exeFile = Join-Path $DeployPath "Lifes.exe"
    if (Test-Path $exeFile) {
        Write-Host ""
        Write-Host "Run Application:" -ForegroundColor Yellow
        Write-Host "  $exeFile" -ForegroundColor Cyan
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
