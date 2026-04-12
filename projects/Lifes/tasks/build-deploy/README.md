# Build & Deploy - Documentation

## Overview

Automation scripts để build và deploy Lifes WPF application. Scripts hỗ trợ:
- Build với Debug/Release configuration
- Clean build option
- Deploy đến target directory
- Exclude files (appsettings.user.json)
- Preserve user settings khi deploy

---

## Quick Reference

### Build Only

```powershell
# Build Release (default)
.\tasks\build-deploy\build.ps1

# Build Debug
.\tasks\build-deploy\build.ps1 -Configuration Debug

# Clean build
.\tasks\build-deploy\build.ps1 -Clean
```

### Build & Deploy

```powershell
# Build and deploy (default)
.\tasks\build-deploy\build-deploy.ps1

# Custom deploy path
.\tasks\build-deploy\build-deploy.ps1 -DeployPath "D:\MyDeploy"

# Clean build before deploy
.\tasks\build-deploy\build-deploy.ps1 -Clean
```

---

## Scripts

### 1. build.ps1

**Purpose**: Build WPF application

**Parameters**:
- `-Configuration` (Debug/Release) - Build configuration (default: Release)
- `-Clean` (switch) - Clean before build
- `-NoRestore` (switch) - Skip NuGet restore

**Example**:
```powershell
# Basic build
.\build.ps1

# Build Debug with clean
.\build.ps1 -Configuration Debug -Clean

# Build without restore (faster if packages already restored)
.\build.ps1 -NoRestore
```

### 2. build-deploy.ps1

**Purpose**: Build và deploy application đến target directory

**Parameters**:
- `-Configuration` (Debug/Release) - Build configuration (default: Release)
- `-DeployPath` (string) - Target deploy directory (default: from deploy-config.json)
- `-Clean` (switch) - Clean before build
- `-Force` (switch) - Force overwrite files
- `-NoBackup` (switch) - Skip backup step

**Example**:
```powershell
# Basic deploy (with backup)
.\build-deploy.ps1

# Deploy to custom location
.\build-deploy.ps1 -DeployPath "C:\Deploy\MyApp"

# Clean build and deploy
.\build-deploy.ps1 -Clean

# Deploy without backup
.\build-deploy.ps1 -NoBackup
```

---

## Configuration

### deploy-config.json

Configuration file cho deployment settings:

```json
{
  "deployPath": "C:\\Tools\\Lifes",
  "backupPath": "C:\\Tools\\Lifes_Backups",
  "configuration": "Release",
  "projectPath": "src/Lifes.Presentation.WPF/Lifes.Presentation.WPF.csproj",
  "excludeFiles": [
    "appsettings.user.json",
    "*.pdb",
    "*.xml"
  ],
  "preserveFiles": [
    "appsettings.user.json"
  ],
  "includePatterns": [
    "*.dll",
    "*.exe",
    "*.exe.config",
    "*.json",
    "*.xaml"
  ],
  "backupSettings": {
    "enabled": true,
    "keepZipOnly": true,
    "zipNameFormat": "Lifes_yyyyMMdd_HHmmss"
  }
}
```

**Fields**:
- `deployPath`: Default target directory for deployment
- `backupPath`: Directory to store backup zip files
- `configuration`: Default build configuration
- `projectPath`: Path to WPF project file (relative to root)
- `excludeFiles`: Files to exclude from deployment (wildcards supported)
- `preserveFiles`: Files to preserve if they exist at deploy location
- `includePatterns`: File patterns to include (optional)
- `backupSettings`: Backup configuration
  - `enabled`: Enable/disable backup (true/false)
  - `keepZipOnly`: Keep only zip file, delete temp folder (true/false)
  - `zipNameFormat`: Backup zip file naming format

---

## Features

### Exclude Files

Files matching `excludeFiles` patterns sẽ không được copy khi deploy:
- `appsettings.user.json` - User-specific settings
- `*.pdb` - Debug symbols
- `*.xml` - Documentation files

### Backup Before Deploy

Trước khi deploy, script tự động backup existing deployment:

**Workflow**:
1. Create backup directory (nếu chưa có)
2. Generate timestamp: `yyyyMMdd_HHmmss`
3. Copy all files từ deploy location vào temp folder
4. Compress temp folder thành zip: `Lifes_20260206_143025.zip`
5. Delete temp folder (chỉ giữ zip)
6. Continue deployment

**Backup Location**: Configured trong `backupPath` (default: `C:\Tools\Lifes_Backups`)

**Zip Format**: `Lifes_yyyyMMdd_HHmmss.zip`
- Example: `Lifes_20260206_143025.zip` (2026-02-06 14:30:25)

**Options**:
- Enable/disable: Edit `backupSettings.enabled` trong config
- Skip backup: Use `-NoBackup` parameter

### Preserve Files

Files trong `preserveFiles` list sẽ được preserve nếu đã tồn tại tại deploy location:
1. Backup file trước khi deploy
2. Copy new files
3. Restore backed up files
4. Delete backup

**Use case**: Developer có custom settings trong `appsettings.user.json` tại deploy location. Khi update deploy, settings này được giữ nguyên.

### Clean Build

Option `-Clean` sẽ:
1. Clean previous build artifacts
2. Remove obj/ và bin/ folders
3. Fresh build from scratch

Useful khi:
- Build có issues
- Switch giữa Debug/Release
- Ensure clean state

---

## Workflows

### Development Workflow

```powershell
# Quick build to test
.\build.ps1 -Configuration Debug

# Build and deploy to test location
.\build-deploy.ps1 -Configuration Debug -DeployPath "C:\TestDeploy"
```

### Release Workflow

```powershell
# Clean Release build
.\build.ps1 -Configuration Release -Clean

# Build and deploy to production location
.\build-deploy.ps1 -Configuration Release
```

### Update Deployment

```powershell
# Update existing deployment (with backup + preserve settings)
.\build-deploy.ps1

# Output:
# Step 1: Building...
# Step 2: Backing up existing deployment...
#   Creating zip archive...
#   Backup created: Lifes_20260206_143025.zip
# Step 3: Deploying application...
#   New binaries copied
#   appsettings.user.json preserved
# Summary displayed
```

### Deploy Without Backup

```powershell
# Skip backup step (faster)
.\build-deploy.ps1 -NoBackup
```

---

## Output Locations

### Build Output

**Debug**:
```
src/Lifes.Presentation.WPF/bin/Debug/net6.0-windows/
```

**Release**:
```
src/Lifes.Presentation.WPF/bin/Release/net6.0-windows/
```

### Deploy Output

Default: `F:\Deploy\Lifes` (configurable in deploy-config.json)

Structure:
```
F:\Deploy\Lifes/
├── Lifes.Presentation.WPF.exe
├── Lifes.Application.dll
├── Lifes.Domain.dll
├── Lifes.Infrastructure.dll
├── Lifes.Core.dll
├── appsettings.json
├── appsettings.user.json    (preserved if exists)
└── (other dependencies)
```

---

## Troubleshooting

### Build Fails

**Issue**: `dotnet build` fails

**Solution**:
1. Ensure .NET 6.0 SDK installed: `dotnet --version`
2. Try clean build: `.\build.ps1 -Clean`
3. Restore manually: `dotnet restore`
4. Check project file path in deploy-config.json

### Deploy Path Not Found

**Issue**: Deploy directory doesn't exist

**Solution**:
- Script automatically creates directory
- Ensure parent directory exists and has write permissions
- Check deploy path in deploy-config.json

### Files Not Excluded

**Issue**: appsettings.user.json được copy khi deploy

**Solution**:
- Check `excludeFiles` in deploy-config.json
- Ensure file name matches pattern exactly
- Wildcards supported: `*.pdb`, `*.xml`

### User Settings Overwritten

**Issue**: Custom settings bị mất sau deploy

**Solution**:
- Add file vào `preserveFiles` trong deploy-config.json
- Script sẽ backup → deploy → restore
- Check script output for "Restored: [filename]"

### Backup Fails

**Issue**: Cannot create backup zip

**Solution**:
- Check backup path exists and has write permissions
- Ensure no processes locking files in deploy directory
- Check disk space
- Deployment continues even if backup fails (with warning)

### Restore From Backup

**Issue**: Need to rollback to previous version

**Solution**:
```powershell
# Find backup in backup directory
cd C:\Tools\Lifes_Backups

# Extract zip to deploy location
Expand-Archive -Path "Lifes_20260206_143025.zip" -DestinationPath "C:\Tools\Lifes" -Force
```

---

## Examples

### Example 1: First Time Deploy

```powershell
PS> .\build-deploy.ps1

# Output:
# Building application...
# Build successful!
# Creating deploy directory...
# Copying files...
#   Copied: Lifes.Presentation.WPF.exe
#   Copied: Lifes.Application.dll
#   ...
#   Excluded: appsettings.user.json
# Deployment Successful!
# Files Copied: 45
# Files Excluded: 3
```

### Example 2: Update Existing Deploy (With Backup)

```powershell
PS> .\build-deploy.ps1

# Output:
# Step 1: Building application...
# Build successful!
#
# Step 2: Backing up existing deployment...
# Copying 52 files...
# Creating zip archive...
# Backup created: Lifes_20260206_143025.zip
# Backup complete!
#
# Step 3: Deploying application...
# Backed up: appsettings.user.json
# Copying files...
#   Copied: Lifes.Presentation.WPF.exe
#   ...
#   Excluded: appsettings.user.json
# Restoring preserved files...
#   Restored: appsettings.user.json
# Deployment Successful!
# Files Copied: 45
# Files Excluded: 3
# Files Preserved: 1
# Backup Created: Lifes_20260206_143025.zip
```

### Example 3: Custom Deploy Location

```powershell
PS> .\build-deploy.ps1 -DeployPath "D:\CustomLocation"

# Deploys to D:\CustomLocation instead of default
```

---

## Integration with Main Menu

Scripts được integrate vào main menu (`run.ps1`):

```
Lifes - Automation Menu
========================================

1. Run Tests (Quick)
2. Run Tests (With Coverage)
3. Build Application              ← calls build.ps1
4. Build & Deploy Application     ← calls build-deploy.ps1
5. Exit
```

---

## Best Practices

1. **Use Configuration**: Edit `deploy-config.json` cho settings thay vì parameters

2. **Preserve User Settings**: Add user-specific files vào `preserveFiles`

3. **Clean Build for Release**: Always clean build cho production:
   ```powershell
   .\build-deploy.ps1 -Clean
   ```

4. **Test Deploy Location**: Test với custom path trước khi deploy production:
   ```powershell
   .\build-deploy.ps1 -DeployPath "C:\Test"
   ```

5. **Version Control**: Commit `deploy-config.json` nhưng không commit `appsettings.user.json`

---

## Related Documentation

- **Main Menu**: `../../run.ps1`
- **Task Overview**: `../../docs/structures/task_structured.md`
- **Task Rules**: `../../docs/structures/tasks_rule.md`
- **User Story**: `../../docs/user-stories/us-3.1/user-story-3.1.md`
- **Work Rules**: `../../work_rule.md`

---

## Future Enhancements

- [ ] Multiple deploy targets (Dev, Staging, Prod)
- [ ] Backup before deploy option
- [ ] Version tagging
- [ ] Deploy history log
- [ ] Rollback capability
- [ ] Automated testing after deploy

---

**Last Updated**: 2026-02-06  
**Version**: 1.0.0
