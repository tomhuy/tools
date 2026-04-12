# Changes Log - Build & Deploy Automation

## Version 1.1.0 (2026-02-06) - Backup Feature Added

### NEW: Automatic Backup Before Deploy

**Feature**: Backup existing deployment before overwriting

**How it works**:
1. Build application (Step 1)
2. **Backup existing deployment** (Step 2) - NEW!
   - Copy all files from deploy location to temp folder
   - Create zip: `Lifes_yyyyMMdd_HHmmss.zip`
   - Delete temp folder (keep only zip)
3. Deploy new version (Step 3)
4. Preserve user settings

**Configuration**:
```json
{
  "backupPath": "C:\\Tools\\Lifes_Backups",
  "backupSettings": {
    "enabled": true,
    "keepZipOnly": true,
    "zipNameFormat": "Lifes_yyyyMMdd_HHmmss"
  }
}
```

**New Parameter**:
- `-NoBackup`: Skip backup for faster deployment

**Benefits**:
- Safety: Always have recent backup
- Rollback: Easy restore from zip
- History: Timestamped backups
- Space efficient: Only zip files kept

**Backup Example**:
```
C:\Tools\Lifes_Backups\
  - Lifes_20260206_143025.zip
  - Lifes_20260206_095412.zip
  - Lifes_20260205_172033.zip
```

---

## Version 1.0.0 (2026-02-06) - Initial Release

### What's New?

Created build and deploy automation system for Lifes WPF application (US-3.1).

---

## ✅ Created Files

### Scripts
1. **build.ps1**
   - Build WPF project with Debug/Release
   - Clean build option
   - NuGet restore
   - Output location display

2. **build-deploy.ps1**
   - Build + Deploy in one command
   - **Auto backup before deploy** (v1.1.0)
   - Exclude files support
   - Preserve user settings
   - Deployment summary

### Configuration
3. **deploy-config.json**
   - Deploy path configuration
   - Exclude files list
   - Preserve files list
   - Include patterns

### Documentation
4. **README.md**
   - Full documentation
   - Usage examples
   - Troubleshooting guide

5. **QUICK-START.md**
   - Quick reference
   - Most common commands

6. **CHANGES.md** (this file)
   - Change log

---

## ✅ Updated Files

### Main Menu
- **run.ps1** (root)
  - Converted from simple alias to interactive menu
  - Added options for build & deploy
  - Central navigation hub

### Documentation
- **work_rule.md**
  - Added build-deploy to Quick Reference
  - Updated Automation Tasks section
  - Added main menu commands

---

## 🎯 Key Features

### 1. Main Menu Navigation
```
Lifes - Automation Menu
1. Run Tests (Quick)
2. Run Tests (With Coverage)
3. Build Application
4. Build & Deploy Application
5. Exit
```

### 2. Build Options
- Debug/Release configuration
- Clean build
- Skip restore for faster builds

### 3. Deploy Features
- **Automatic backup before deploy** (v1.1.0)
- Configurable deploy path
- Exclude files (appsettings.user.json, *.pdb, *.xml)
- Preserve existing user settings
- Copy summary with counts

### 4. Error Handling
- Build validation
- Path verification
- Graceful error messages
- Proper exit codes

---

## 📁 File Structure

```
Lifes/
├── run.ps1                         # NEW: Main menu
├── test.ps1                        # Updated: Kept as alias
├── test-quick.ps1                  # Kept as alias
│
└── tasks/
    ├── run-tests/                  # Existing
    │   └── ...
    │
    └── build-deploy/               # NEW
        ├── build.ps1
        ├── build-deploy.ps1
        ├── deploy-config.json
        ├── README.md
        ├── QUICK-START.md
        └── CHANGES.md (this file)
```

---

## 🚀 How to Use

### Launch Main Menu (Easiest)
```powershell
.\run.ps1
```

### Direct Commands
```powershell
# Build
.\tasks\build-deploy\build.ps1

# Build & Deploy
.\tasks\build-deploy\build-deploy.ps1

# Custom deploy location
.\tasks\build-deploy\build-deploy.ps1 -DeployPath "D:\MyDeploy"
```

---

## ⚙️ Configuration

Edit `deploy-config.json` to customize:

```json
{
  "deployPath": "F:\\Deploy\\Lifes",     // Change deploy location
  "configuration": "Release",                // Default build config
  "excludeFiles": [                          // Files to skip
    "appsettings.user.json",
    "*.pdb"
  ],
  "preserveFiles": [                         // Files to preserve
    "appsettings.user.json"
  ]
}
```

---

## 📝 Related User Story

**User Story**: US-3.1 - Build & Deploy Automation with Main Menu System

**Documentation**:
- `docs/user-stories/us-3.1/user-story-3.1.md`
- `docs/structures/task_structured.md` (overview)
- `docs/structures/tasks_rule.md` (rules & guidelines)

---

## 🔮 Future Enhancements

- [ ] Multiple deploy targets (Dev, Staging, Prod)
- [x] Pre-deploy backup (✅ v1.1.0)
- [ ] Version tagging
- [ ] Deploy history log
- [ ] Rollback capability (manual via zip restore available)
- [ ] Post-deploy health check
- [ ] Backup retention policy (auto-cleanup old backups)

---

**Created**: 2026-02-06  
**Version**: 1.1.0  
**Last Updated**: 2026-02-06  
**Status**: ✅ Implemented
