# Quick Start - Build & Deploy

## Most Common Commands

### From Project Root

```powershell
# Build Release
.\tasks\build-deploy\build.ps1

# Build & Deploy
.\tasks\build-deploy\build-deploy.ps1

# Use Main Menu (Easiest)
.\run.ps1
# Then select option 3 or 4
```

### From tasks/build-deploy/ Folder

```powershell
cd tasks\build-deploy

# Build
.\build.ps1

# Deploy
.\build-deploy.ps1
```

---

## Quick Options

```powershell
# Build Debug
.\build.ps1 -Configuration Debug

# Clean build
.\build.ps1 -Clean

# Deploy to custom location
.\build-deploy.ps1 -DeployPath "D:\MyDeploy"

# Clean build and deploy
.\build-deploy.ps1 -Clean

# Deploy without backup (faster)
.\build-deploy.ps1 -NoBackup
```

---

## Configuration

Edit `deploy-config.json`:

```json
{
  "deployPath": "C:\\Tools\\ETLTools",         // ← Change this
  "backupPath": "C:\\Tools\\ETLTools_Backups", // ← Backup location
  "excludeFiles": [
    "appsettings.user.json"                    // ← Add files to exclude
  ],
  "backupSettings": {
    "enabled": true                            // ← Enable/disable backup
  }
}
```

---

## Workflow

### Development
```powershell
.\build.ps1 -Configuration Debug
```

### Production Deploy
```powershell
.\build-deploy.ps1 -Clean
```

### Update Existing Deploy
```powershell
.\build-deploy.ps1
# Auto backup → Deploy → Preserve user settings
# Backup: ETLTools_yyyyMMdd_HHmmss.zip
```

---

## Troubleshooting

### Build fails?
```powershell
.\build.ps1 -Clean
```

### Wrong deploy location?
Edit `deploy-config.json` → `deployPath`

### Settings overwritten?
Add to `preserveFiles` in `deploy-config.json`

### Backup fails?
Check permissions on `backupPath`, or use `-NoBackup`

### Restore backup?
```powershell
Expand-Archive "C:\Tools\ETLTools_Backups\ETLTools_20260206_143025.zip" -DestinationPath "C:\Tools\ETLTools" -Force
```

---

**See README.md for full documentation**
