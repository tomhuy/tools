# ============================================
# ETLTools - Build & Deploy Script
# ============================================
# Build and deploy WPF application to target directory
# ============================================

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [string]$DeployPath = "",
    
    [switch]$Clean,
    [switch]$Force,
    [switch]$NoBackup
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "ETLTools - Build & Deploy" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get script directory and project root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptDir "..\..")

# Read configuration
$configPath = Join-Path $scriptDir "deploy-config.json"
if (-not (Test-Path $configPath)) {
    Write-Host "Error: deploy-config.json not found!" -ForegroundColor Red
    exit 1
}

$config = Get-Content $configPath | ConvertFrom-Json

# Use provided deploy path or from config
if ($DeployPath -eq "") {
    $DeployPath = $config.deployPath
}

$backupPath = $config.backupPath
$projectPath = Join-Path $projectRoot $config.projectPath
$excludeFiles = $config.excludeFiles
$preserveFiles = $config.preserveFiles
$backupEnabled = $config.backupSettings.enabled -and (-not $NoBackup)

Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Deploy Path: $DeployPath" -ForegroundColor Yellow
if ($backupEnabled) {
    Write-Host "Backup Path: $backupPath" -ForegroundColor Yellow
}
Write-Host ""

# Change to project root
Push-Location $projectRoot

try {
    # Step 1: Build application
    Write-Host "Step 1: Building application..." -ForegroundColor Cyan
    Write-Host ""
    
    $buildScript = Join-Path $scriptDir "build.ps1"
    
    if ($Clean) {
        & $buildScript -Configuration $Configuration -Clean
    } else {
        & $buildScript -Configuration $Configuration
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
        $zipFileName = "ETLTools_$timestamp.zip"
        $zipFilePath = Join-Path $backupPath $zipFileName
        
        # Create temp folder for backup
        $tempBackupFolder = Join-Path $backupPath "temp_$timestamp"
        
        try {
            Write-Host "Copying current deployment to temp folder..." -ForegroundColor Yellow
            New-Item -ItemType Directory -Path $tempBackupFolder -Force | Out-Null
            
            # Copy all files from deploy location
            $deployFiles = Get-ChildItem $DeployPath -Recurse -File
            $fileCount = $deployFiles.Count
            Write-Host "Copying $fileCount files..." -ForegroundColor Gray
            
            Copy-Item -Path "$DeployPath\*" -Destination $tempBackupFolder -Recurse -Force
            
            Write-Host "Creating zip archive..." -ForegroundColor Yellow
            
            # Create zip file
            Compress-Archive -Path "$tempBackupFolder\*" -DestinationPath $zipFilePath -Force
            
            Write-Host "Backup created: $zipFileName" -ForegroundColor Green
            
            # Remove temp folder (keep only zip)
            Write-Host "Cleaning up temp folder..." -ForegroundColor Gray
            Remove-Item -Path $tempBackupFolder -Recurse -Force
            
            Write-Host "Backup complete!" -ForegroundColor Green
            Write-Host ""
        }
        catch {
            Write-Host "Warning: Backup failed: $_" -ForegroundColor Yellow
            Write-Host "Continuing with deployment..." -ForegroundColor Yellow
            Write-Host ""
            
            # Cleanup temp folder if exists
            if (Test-Path $tempBackupFolder) {
                Remove-Item -Path $tempBackupFolder -Recurse -Force -ErrorAction SilentlyContinue
            }
        }
    } elseif ($backupEnabled -and (-not (Test-Path $DeployPath))) {
        Write-Host "Step 2: Backup skipped (first-time deployment)" -ForegroundColor Gray
        Write-Host ""
    }
    
    Write-Host "Step 3: Deploying application..." -ForegroundColor Cyan
    Write-Host ""
    
    # Determine build output path
    $outputPath = Join-Path (Split-Path $projectPath) "bin\$Configuration\net6.0-windows"
    
    if (-not (Test-Path $outputPath)) {
        Write-Host "Error: Build output not found: $outputPath" -ForegroundColor Red
        exit 1
    }
    
    # Create deploy directory if not exists
    if (-not (Test-Path $DeployPath)) {
        Write-Host "Creating deploy directory..." -ForegroundColor Yellow
        New-Item -ItemType Directory -Path $DeployPath -Force | Out-Null
        Write-Host "Deploy directory created: $DeployPath" -ForegroundColor Green
        Write-Host ""
    }
    
    # Backup preserve files if they exist in deploy location
    $backupFiles = @{}
    foreach ($file in $preserveFiles) {
        $targetFile = Join-Path $DeployPath $file
        if (Test-Path $targetFile) {
            $tempPath = "$targetFile.backup"
            Copy-Item $targetFile $tempPath -Force
            $backupFiles[$file] = $tempPath
            Write-Host "Backed up: $file" -ForegroundColor Gray
        }
    }
    
    # Copy files
    Write-Host "Copying files..." -ForegroundColor Yellow
    
    $copiedFiles = 0
    $excludedFiles = 0
    $excludePatterns = $excludeFiles
    
    Get-ChildItem $outputPath -Recurse -File | ForEach-Object {
        $file = $_
        $relativePath = $file.FullName.Substring($outputPath.Length + 1)
        $targetPath = Join-Path $DeployPath $relativePath
        
        # Check if file should be excluded
        $shouldExclude = $false
        foreach ($pattern in $excludePatterns) {
            if ($file.Name -like $pattern) {
                $shouldExclude = $true
                Write-Host "  Excluded: $($file.Name)" -ForegroundColor DarkGray
                $excludedFiles++
                break
            }
        }
        
        if (-not $shouldExclude) {
            $targetDir = Split-Path $targetPath -Parent
            if (-not (Test-Path $targetDir)) {
                New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
            }
            
            Copy-Item $file.FullName $targetPath -Force
            Write-Host "  Copied: $($file.Name)" -ForegroundColor Green
            $copiedFiles++
        }
    }
    
    # Restore preserved files
    if ($backupFiles.Count -gt 0) {
        Write-Host ""
        Write-Host "Restoring preserved files..." -ForegroundColor Yellow
        foreach ($file in $backupFiles.Keys) {
            $targetFile = Join-Path $DeployPath $file
            $backupFile = $backupFiles[$file]
            
            Copy-Item $backupFile $targetFile -Force
            Remove-Item $backupFile -Force
            Write-Host "  Restored: $file" -ForegroundColor Green
        }
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Deployment Successful!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Summary:" -ForegroundColor Yellow
    Write-Host "  Files Copied: $copiedFiles" -ForegroundColor Green
    Write-Host "  Files Excluded: $excludedFiles" -ForegroundColor Gray
    Write-Host "  Files Preserved: $($backupFiles.Count)" -ForegroundColor Cyan
    if ($backupEnabled -and (Test-Path $DeployPath)) {
        $latestBackup = Get-ChildItem $backupPath -Filter "ETLTools_*.zip" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
        if ($latestBackup) {
            Write-Host "  Backup Created: $($latestBackup.Name)" -ForegroundColor Cyan
        }
    }
    Write-Host ""
    Write-Host "Deploy Location:" -ForegroundColor Yellow
    Write-Host "  $DeployPath" -ForegroundColor Gray
    if ($backupEnabled) {
        Write-Host ""
        Write-Host "Backup Location:" -ForegroundColor Yellow
        Write-Host "  $backupPath" -ForegroundColor Gray
    }
    Write-Host ""
    
    # Show main executable
    $exeFile = Get-ChildItem $DeployPath -Filter "*.exe" | Select-Object -First 1
    if ($exeFile) {
        Write-Host "Run Application:" -ForegroundColor Yellow
        Write-Host "  $($exeFile.FullName)" -ForegroundColor Cyan
        Write-Host ""
    }
    
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
