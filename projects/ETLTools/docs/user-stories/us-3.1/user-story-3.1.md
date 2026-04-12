# User Story: US-3.1

## Story Information

| Field | Value |
|-------|-------|
| **ID** | US-3.1 |
| **Title** | Build & Deploy Automation with Main Menu System |
| **Priority** | High |
| **Estimate** | 4 hours |
| **Sprint** | Sprint 3 |
| **Created Date** | 2026-02-06 |
| **Status** | 📋 Planned |
| **Type** | Automation Task |

---

## User Story

- **As a** Developer
- **I want to** have automated build and deploy scripts with a main menu navigation system
- **So that** I can quickly build and deploy the WPF application to target directory without manual steps

---

## Business Context

### Problem Statement

Hiện tại việc build và deploy WPF application phải thực hiện thủ công:
- ⚠️ Phải mở Visual Studio để build
- ⚠️ Phải manually copy files đến deploy location
- ⚠️ Dễ quên exclude các files như `appsettings.user.json`
- ⚠️ Không có menu để access các automation tasks
- ⚠️ Mỗi lần build tốn thời gian và dễ sai sót

### Solution

Tạo automation system bao gồm:
- ✅ Main menu script (`run.ps1`) để navigate các automation tasks
- ✅ Build script để compile WPF project
- ✅ Deploy script để copy files đến target directory
- ✅ Exclude files configuration (appsettings.user.json)
- ✅ Clean build option
- ✅ Configuration management (Debug/Release)

---

## Acceptance Criteria

### AC-1: Main Menu Navigation System
**Given** developer ở project root  
**When** chạy `.\run.ps1`  
**Then** hiển thị menu với options:
```
1. Run Tests (Quick)
2. Run Tests (With Coverage)
3. Build Application
4. Build & Deploy Application
5. Exit
```

### AC-2: Build Script
**Given** WPF project tại `src/ETLTools.Presentation.WPF/`  
**When** developer chọn "Build Application"  
**Then**:
- Build project với configuration (Debug/Release)
- Display build progress
- Show build success/failure
- Display output location
- Exit code 0 nếu success

### AC-3: Build & Deploy Script
**Given** WPF project đã được build thành công  
**When** developer chọn "Build & Deploy"  
**Then**:
- Build project nếu chưa build
- Copy output files đến deploy directory
- Exclude `appsettings.user.json` file
- Preserve existing `appsettings.user.json` tại deploy location
- Show deployed files list
- Display deploy location

### AC-4: Configuration Management
**Given** build-deploy script  
**When** developer chạy script  
**Then**:
- Default configuration: Release
- Option để chọn Debug/Release
- Configuration được display trong output
- Build artifacts đúng theo configuration

### AC-5: Exclude Files Feature
**Given** deploy script  
**When** copy files đến deploy location  
**Then**:
- `appsettings.user.json` không được copy
- Existing `appsettings.user.json` tại deploy location không bị overwrite
- Other config files được copy normally
- Display excluded files trong output

### AC-6: Error Handling
**Given** build hoặc deploy script  
**When** có errors xảy ra  
**Then**:
- Display clear error message
- Show error location
- Exit with non-zero exit code
- Không deploy nếu build failed

---

## Technical Design

### Automation Structure

```
ETLTools/
├── run.ps1                         # Main menu navigation (NEW)
├── test.ps1                        # Quick test alias (existing)
├── test-quick.ps1                  # Quick test alias (existing)
│
└── tasks/
    ├── run-tests/                  # Testing tasks (existing)
    │   ├── run-tests-with-coverage.ps1
    │   ├── run-tests-quick.ps1
    │   └── README-Testing.md
    │
    └── build-deploy/               # Build & Deploy tasks (NEW)
        ├── build.ps1               # Build script
        ├── build-deploy.ps1        # Build & deploy script
        ├── deploy-config.json      # Deploy configuration
        ├── README.md               # Documentation
        └── QUICK-START.md          # Quick reference
```

### Script Specifications

#### 1. run.ps1 (Main Menu)

**Purpose**: Central navigation hub cho tất cả automation tasks

**Features**:
- Interactive menu with numbered options
- Clear screen và display banner
- Call appropriate scripts based on selection
- Loop until user exits
- Color-coded output

**Menu Options**:
1. Run Tests (Quick) → `.\test-quick.ps1`
2. Run Tests (With Coverage) → `.\test.ps1`
3. Build Application → `.\tasks\build-deploy\build.ps1`
4. Build & Deploy Application → `.\tasks\build-deploy\build-deploy.ps1`
5. Exit

#### 2. build.ps1

**Purpose**: Build WPF application

**Parameters**:
- `-Configuration` (Debug/Release, default: Release)
- `-Clean` (switch: Clean before build)
- `-NoRestore` (switch: Skip restore)

**Workflow**:
1. Navigate to project root
2. Display build configuration
3. Clean if requested
4. Restore NuGet packages (unless -NoRestore)
5. Build WPF project
6. Display build summary
7. Show output location

**Output Location**:
- Debug: `src/ETLTools.Presentation.WPF/bin/Debug/net6.0-windows/`
- Release: `src/ETLTools.Presentation.WPF/bin/Release/net6.0-windows/`

#### 3. build-deploy.ps1

**Purpose**: Build và deploy WPF application

**Parameters**:
- `-Configuration` (Debug/Release, default: Release)
- `-DeployPath` (default: from deploy-config.json)
- `-Clean` (switch: Clean before build)
- `-Force` (switch: Overwrite existing files)

**Workflow**:
1. Read deploy-config.json
2. Build application (call build.ps1)
3. Check build success
4. Create deploy directory if not exists
5. Copy files excluding appsettings.user.json
6. Preserve existing appsettings.user.json
7. Display deployment summary

**Exclude Files**:
- `appsettings.user.json`
- `*.pdb` (debug symbols - optional)
- `*.xml` (documentation - optional)

#### 4. deploy-config.json

**Purpose**: Configuration cho deployment

```json
{
  "deployPath": "C:\\Deploy\\ETLTools",
  "configuration": "Release",
  "excludeFiles": [
    "appsettings.user.json",
    "*.pdb"
  ],
  "includePatterns": [
    "*.dll",
    "*.exe",
    "*.config",
    "*.json"
  ]
}
```

---

## Tasks Breakdown

### Phase 1: Main Menu System (1 hour)

- [ ] **Task 1.1**: Create run.ps1 main menu script
  - [ ] Display banner
  - [ ] Show numbered menu options
  - [ ] Read user input
  - [ ] Call appropriate scripts
  - [ ] Loop until exit
  - [ ] Error handling

- [ ] **Task 1.2**: Test menu navigation
  - [ ] Test all menu options
  - [ ] Test invalid inputs
  - [ ] Test exit option

### Phase 2: Build Script (1 hour)

- [ ] **Task 2.1**: Create tasks/build-deploy/ folder
  - [ ] Create folder structure
  - [ ] Initialize README.md

- [ ] **Task 2.2**: Implement build.ps1
  - [ ] Parameter validation
  - [ ] Navigate to project root
  - [ ] Clean build logic
  - [ ] NuGet restore
  - [ ] MSBuild execution
  - [ ] Output display
  - [ ] Error handling

- [ ] **Task 2.3**: Test build script
  - [ ] Test Debug build
  - [ ] Test Release build
  - [ ] Test clean build
  - [ ] Test error scenarios

### Phase 3: Deploy Script (1.5 hours)

- [ ] **Task 3.1**: Create deploy-config.json
  - [ ] Define deploy path
  - [ ] Define exclude files
  - [ ] Define include patterns

- [ ] **Task 3.2**: Implement build-deploy.ps1
  - [ ] Read configuration
  - [ ] Call build script
  - [ ] Check build success
  - [ ] Create deploy directory
  - [ ] Copy files with exclusions
  - [ ] Preserve user settings
  - [ ] Display summary

- [ ] **Task 3.3**: Test deploy script
  - [ ] Test first-time deploy
  - [ ] Test update deploy (preserve settings)
  - [ ] Test exclude files
  - [ ] Test error scenarios

### Phase 4: Documentation (0.5 hour)

- [ ] **Task 4.1**: Create README.md
  - [ ] Overview
  - [ ] Usage examples
  - [ ] Configuration guide
  - [ ] Troubleshooting

- [ ] **Task 4.2**: Create QUICK-START.md
  - [ ] Quick commands
  - [ ] Common scenarios

- [ ] **Task 4.3**: Update work_rule.md
  - [ ] Add build-deploy to Quick Reference
  - [ ] Add to Common Commands

---

## Files to Create/Modify

### New Files

**Main Menu**:
- [ ] `run.ps1` - Main navigation menu

**Build & Deploy**:
- [ ] `tasks/build-deploy/build.ps1`
- [ ] `tasks/build-deploy/build-deploy.ps1`
- [ ] `tasks/build-deploy/deploy-config.json`
- [ ] `tasks/build-deploy/README.md`
- [ ] `tasks/build-deploy/QUICK-START.md`

### Modified Files

- [ ] `work_rule.md` - Update Quick Reference
- [ ] `README.md` - Add build-deploy documentation (optional)

---

## Dependencies

- **Depends on**: 
  - US-2.1 (Testing Layer) - Completed (test scripts exist)
  - WPF project structure - Exists
  
- **Blocked by**: None

- **Requires**:
  - .NET 6.0 SDK installed
  - MSBuild available
  - PowerShell 5.1+

---

## Definition of Done

- [ ] Main menu script (run.ps1) implemented và tested
- [ ] Build script (build.ps1) implemented và tested
- [ ] Build-deploy script (build-deploy.ps1) implemented và tested
- [ ] Deploy configuration (deploy-config.json) created
- [ ] Exclude files feature working correctly
- [ ] appsettings.user.json preserved during deploy
- [ ] All scripts follow tasks_rule.md guidelines:
  - [ ] Self-contained navigation
  - [ ] Proper error handling
  - [ ] Clear colored output
  - [ ] Try-finally blocks
- [ ] Documentation created:
  - [ ] README.md in tasks/build-deploy/
  - [ ] QUICK-START.md in tasks/build-deploy/
  - [ ] work_rule.md updated
- [ ] Testing completed:
  - [ ] Menu navigation works
  - [ ] Build Debug works
  - [ ] Build Release works
  - [ ] Deploy works (first time)
  - [ ] Deploy works (update - preserves settings)
  - [ ] Exclude files work
  - [ ] Error scenarios handled
- [ ] User Story approved

---

## Configuration Example

### deploy-config.json

```json
{
  "deployPath": "F:\\Deploy\\ETLTools",
  "configuration": "Release",
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
  ]
}
```

---

## Usage Examples

### Main Menu

```powershell
# Launch main menu
.\run.ps1

# Output:
# ========================================
# ETLTools - Automation Menu
# ========================================
# 
# 1. Run Tests (Quick)
# 2. Run Tests (With Coverage)
# 3. Build Application
# 4. Build & Deploy Application
# 5. Exit
#
# Select option (1-5):
```

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
# Build and deploy (default config)
.\tasks\build-deploy\build-deploy.ps1

# Custom deploy path
.\tasks\build-deploy\build-deploy.ps1 -DeployPath "D:\MyDeploy"

# Force overwrite
.\tasks\build-deploy\build-deploy.ps1 -Force
```

---

## Notes

### Deployment Behavior

1. **First Time Deploy**:
   - Deploy directory created
   - All files copied (except excluded)
   - No appsettings.user.json in deploy

2. **Update Deploy**:
   - Existing files overwritten (unless -Force:$false)
   - appsettings.user.json preserved (not overwritten)
   - New files added

3. **Excluded Files**:
   - Never copied from build output
   - Existing files in deploy preserved
   - Logged in deployment summary

### Future Enhancements

- [ ] Support multiple deploy targets (Dev, Staging, Prod)
- [ ] Backup before deploy
- [ ] Version tagging
- [ ] Deploy history log
- [ ] Rollback capability

---

## Implementation Progress

### Files Created

- [ ] Scripts not yet created

### Current Status

- **Status**: 📋 Planned
- **Completed**: 0%
- **Blockers**: None
- **Notes**: User Story created, ready for implementation

---

## Final Status

- **Status**: 📋 Planned (Awaiting Implementation)
- **Created Date**: 2026-02-06
- **Approved By**: Pending
- **Implementation Start Date**: TBD
- **Completed Date**: TBD

---

**Document Version**: 1.0.0  
**Last Updated**: 2026-02-06  
**Author**: Development Team
