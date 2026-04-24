# Automation Tasks - Structure Overview

## 📋 Document Information

| Field | Value |
|-------|-------|
| **Document Name** | Automation Tasks Structure Overview |
| **Version** | 1.1.0 |
| **Last Updated** | 2026-02-06 |
| **Purpose** | Overview of automation structure and implemented tasks |
| **Target Audience** | Developers, AI Agents |

---

## 🎯 Overview

Tài liệu này cung cấp overview về toàn bộ automation infrastructure của Lifes, bao gồm cấu trúc file, các tasks đã implement, và hướng dẫn sử dụng.

**Để biết chi tiết về quy tắc và guidelines**, xem: [`tasks_rule.md`](tasks_rule.md)

---

## 📁 Automation Structure

### Complete File Structure

```
Lifes/
├── run.ps1                                # Main menu - entry point
├── test.ps1                               # Quick alias - test with coverage
├── test-quick.ps1                         # Quick alias - fast test
├── coverlet.runsettings                   # Coverage configuration
│
├── tasks/                                 # All automation tasks
│   ├── run-tests/                         # Testing automation (US-2.1)
│   │   ├── run-tests-with-coverage.ps1    # Full test + HTML coverage report
│   │   ├── run-tests-quick.ps1            # Quick test (no coverage)
│   │   ├── run-tests-specific.ps1         # Test specific project
│   │   ├── README-Testing.md              # Full testing documentation
│   │   ├── QUICK-START.md                 # Quick reference guide
│   │   └── CHANGES.md                     # Change history
│   │
│   └── build-deploy/                      # Build & Deploy automation (US-3.1)
│       ├── build.ps1                      # Build application only
│       ├── build-deploy.ps1               # Build + Deploy (with backup v1.1.0)
│       ├── deploy-config.json             # Deployment configuration
│       ├── README.md                      # Full build/deploy documentation
│       ├── QUICK-START.md                 # Quick reference guide
│       └── CHANGES.md                     # Change history (v1.1.0)
│
├── docs/
│   ├── user-stories/
│   │   ├── us-2.1/                        # US-2.1: Testing Layer
│   │   │   └── user-story-2.1.md          # Testing layer requirements
│   │   └── us-3.1/                        # US-3.1: Build & Deploy
│   │       └── user-story-3.1.md          # Build & deploy requirements
│   │
│   └── structures/
│       ├── task_structured.md             # THIS FILE - Overview
│       ├── tasks_rule.md                  # Automation rules & guidelines
│       ├── testing-structure.md           # Testing architecture
│       └── be-all-structure.md            # Overall backend structure
│
├── work_rule.md                           # Development workflow (with automation refs)
├── AUTOMATION-SUMMARY.md                  # Complete automation summary
└── README.md                              # Project README

Output directories (generated):
├── test-results/                          # Test results & coverage
│   ├── coverage-report/                   # HTML coverage report
│   │   └── index.html                     # Open this for coverage
│   ├── coverage.json                      # JSON coverage data
│   └── *.trx                             # Test result files
│
└── [backup-path]/                         # Deployment backups (configurable)
    └── Lifes_yyyyMMdd_HHmmss.zip      # Timestamped backups
```

---

## 🎯 Implemented Tasks

### US-2.1: Testing Layer Infrastructure ✅

**Status**: ✅ Implemented  
**Location**: `tasks/run-tests/`  
**User Story**: `docs/user-stories/us-2.1/user-story-2.1.md`

#### Scripts

1. **`run-tests-with-coverage.ps1`**
   - Full test suite with code coverage
   - Generates HTML report
   - Auto-opens report in browser
   - Supports filters

2. **`run-tests-quick.ps1`**
   - Quick test execution (no coverage)
   - Watch mode support
   - Ideal for development

3. **`run-tests-specific.ps1`**
   - Test specific test project
   - With or without coverage
   - Targeted testing

#### Root Aliases

- `test.ps1` → calls `run-tests-with-coverage.ps1`
- `test-quick.ps1` → calls `run-tests-quick.ps1`

#### Features

- ✅ xUnit, Moq, FluentAssertions, Coverlet
- ✅ HTML coverage reports with ReportGenerator
- ✅ Watch mode for TDD
- ✅ Filter support by test name
- ✅ Multiple test projects support
- ✅ Clear colored output
- ✅ Self-contained navigation

#### Quick Usage

```powershell
# Full test with coverage (via alias)
.\test.ps1

# Quick test during development
.\test-quick.ps1 -Watch

# View coverage report
.\test-results\coverage-report\index.html
```

---

### US-3.1: Build & Deploy Automation ✅

**Status**: ✅ Implemented (v1.1.0 with backup)  
**Location**: `tasks/build-deploy/`  
**User Story**: `docs/user-stories/us-3.1/user-story-3.1.md`

#### Scripts

1. **`build.ps1`**
   - Build WPF application
   - Debug/Release configuration
   - Clean build option
   - Skip restore option
   - Output location display

2. **`build-deploy.ps1`** (v1.1.0)
   - Build + Deploy in one command
   - **Automatic backup before deploy** (NEW v1.1.0)
     - Creates timestamped zip: `Lifes_yyyyMMdd_HHmmss.zip`
     - Configurable backup location
     - Keep only zip files
   - Exclude files (*.pdb, *.xml, user settings)
   - Preserve user settings during update
   - Deployment summary

#### Configuration

**`deploy-config.json`**:
```json
{
  "deployPath": "C:\\Tools\\Lifes",
  "backupPath": "C:\\Tools\\Lifes_Backups",
  "configuration": "Release",
  "projectPath": "src/Lifes.Presentation.WPF/Lifes.Presentation.WPF.csproj",
  "excludeFiles": ["appsettings.user.json", "*.pdb", "*.xml"],
  "preserveFiles": ["appsettings.user.json"],
  "backupSettings": {
    "enabled": true,
    "keepZipOnly": true,
    "zipNameFormat": "Lifes_yyyyMMdd_HHmmss"
  }
}
```

#### Features

- ✅ Debug/Release builds
- ✅ **Automatic backup** (v1.1.0)
- ✅ Configurable deploy path
- ✅ File exclusion patterns
- ✅ User settings preservation
- ✅ Deployment summary
- ✅ Error handling
- ✅ Self-contained navigation

#### Quick Usage

```powershell
# Build only
.\tasks\build-deploy\build.ps1

# Build & Deploy (with backup)
.\tasks\build-deploy\build-deploy.ps1

# Deploy without backup (faster)
.\tasks\build-deploy\build-deploy.ps1 -NoBackup

# Rollback from backup
Expand-Archive "C:\Tools\Lifes_Backups\Lifes_20260206_143025.zip" `
  -DestinationPath "C:\Tools\Lifes" -Force
```

---

## 🎯 Main Menu System

**Script**: `run.ps1` (root)  
**Purpose**: Central navigation for all automation tasks

### Menu Structure

```
========================================
    Lifes - Automation Menu
========================================

Select an option:

  1. Run Tests (Quick)             → run-tests-quick.ps1
  2. Run Tests (With Coverage)     → run-tests-with-coverage.ps1
  3. Build Application             → build.ps1
  4. Build & Deploy Application    → build-deploy.ps1
  5. Exit
```

### Usage

```powershell
# Launch main menu
.\run.ps1

# Menu stays open after each task
# Returns to menu automatically
# Press 5 to exit
```

---

## 📊 Task Summary

| Task | Status | Scripts | Documentation | Features |
|------|--------|---------|---------------|----------|
| **Testing** | ✅ Complete | 3 scripts | README, QUICK-START, CHANGES | Coverage, Watch, Filter |
| **Build & Deploy** | ✅ Complete (v1.1.0) | 2 scripts | README, QUICK-START, CHANGES | Backup, Exclude, Preserve |
| **Build Electron** | ✅ Complete | 2 scripts | US-11.3 | 1-Click Build & Auto-Run Backend |
| **Main Menu** | ✅ Complete | 1 script | Inline help | Navigation hub |

---

## 🚀 Quick Start Guide

### First Time Setup

1. **Configure Deployment**:
   ```powershell
   # Edit deploy-config.json
   notepad tasks\build-deploy\deploy-config.json
   
   # Set your paths:
   # - deployPath: Where to deploy
   # - backupPath: Where to backup
   ```

2. **Test the Setup**:
   ```powershell
   # Test build
   .\tasks\build-deploy\build.ps1
   
   # Test full test suite
   .\test.ps1
   ```

### Daily Workflow

#### Development

```powershell
# Start watch mode for TDD
.\test-quick.ps1 -Watch

# Make changes...
# Tests auto-run on save
```

#### Before Commit

```powershell
# Full test with coverage
.\test.ps1

# Review coverage
.\test-results\coverage-report\index.html
```

#### Release/Deploy

```powershell
# Option 1: Use main menu
.\run.ps1
# Select: 4. Build & Deploy Application

# Option 2: Direct command
.\tasks\build-deploy\build-deploy.ps1 -Clean
```

---

## 📚 Documentation Guide

### For New Developers

**Start here**:
1. Read this file (`task_structured.md`) - Overview
2. Check `AUTOMATION-SUMMARY.md` - Complete guide
3. Review `work_rule.md` → Automation section

**When working on tasks**:
1. `tasks_rule.md` - Rules and guidelines
2. Specific task README in `tasks/[task-name]/`
3. QUICK-START.md for quick reference

### For Creating New Tasks

**Must read**:
1. [`tasks_rule.md`](tasks_rule.md) - Complete guidelines
2. Follow checklist in `tasks_rule.md`
3. Update this file with new task info

**Template structure**:
```
tasks/
└── [new-task]/
    ├── [main-script].ps1
    ├── README.md
    ├── QUICK-START.md (optional)
    └── CHANGES.md (optional)
```

---

## 🔧 Configuration Files

### Global Configuration

| File | Purpose | Location |
|------|---------|----------|
| `coverlet.runsettings` | Coverage configuration | Root |
| `deploy-config.json` | Deployment settings | `tasks/build-deploy/` |

### Coverage Configuration

**File**: `coverlet.runsettings`

**Key Settings**:
- Output formats: Json, Cobertura, OpenCover
- Exclude: Test projects, migrations, generated code
- Include: All source projects

**Edit when**:
- Need to exclude new patterns
- Change coverage output format
- Adjust coverage thresholds

### Deployment Configuration

**File**: `tasks/build-deploy/deploy-config.json`

**Key Settings**:
- `deployPath`: Target deployment directory
- `backupPath`: Backup zip file directory (v1.1.0)
- `excludeFiles`: Files to skip (*.pdb, etc.)
- `preserveFiles`: Files to preserve (user settings)
- `backupSettings`: Backup configuration (v1.1.0)

**Edit when**:
- Change deploy location
- Add files to exclude
- Add settings to preserve
- Enable/disable backup

---

## 🆘 Troubleshooting

### Common Issues

#### Tests Not Running

**Symptom**: `dotnet test` fails or no tests found

**Solution**:
```powershell
# Restore packages
dotnet restore

# Clean and rebuild
.\tasks\build-deploy\build.ps1 -Clean

# Try again
.\test-quick.ps1
```

#### Coverage Report Not Generated

**Symptom**: Test runs but no HTML report

**Solution**:
1. Check ReportGenerator is installed:
   ```powershell
   dotnet tool list -g
   ```
2. Install if missing:
   ```powershell
   dotnet tool install -g dotnet-reportgenerator-globaltool
   ```

#### Build Fails

**Symptom**: Build returns errors

**Solution**:
```powershell
# Check .NET SDK
dotnet --version  # Should be 6.0+

# Restore packages
dotnet restore

# Clean build
.\tasks\build-deploy\build.ps1 -Clean
```

#### Deploy Path Error

**Symptom**: Cannot create deploy directory

**Solution**:
1. Check path in `deploy-config.json`
2. Ensure parent directory exists
3. Check write permissions
4. Try with explicit path:
   ```powershell
   .\tasks\build-deploy\build-deploy.ps1 -DeployPath "D:\MyPath"
   ```

#### Backup Fails (v1.1.0)

**Symptom**: Backup error during deploy

**Solution**:
1. Check backup path permissions
2. Ensure disk space available
3. Check no processes locking files
4. Or skip backup:
   ```powershell
   .\tasks\build-deploy\build-deploy.ps1 -NoBackup
   ```

---

## 📈 Future Enhancements

### Planned

- [ ] Multiple deploy targets (Dev, Staging, Prod)
- [ ] Automated backup retention policy
- [ ] Version tagging automation
- [ ] Deploy history log
- [ ] Post-deploy health checks
- [ ] Database migration automation
- [ ] Code generation tasks

### Under Consideration

- [ ] Docker container builds
- [ ] CI/CD pipeline integration
- [ ] Performance benchmarking
- [ ] Security scanning
- [ ] Automated changelog generation

---

## 🔗 Related Documents

### Core Documents
- [`tasks_rule.md`](tasks_rule.md) - Automation rules & guidelines
- [`work_rule.md`](../../work_rule.md) - Development workflow
- [`AUTOMATION-SUMMARY.md`](../../AUTOMATION-SUMMARY.md) - Complete automation guide

### User Stories
- [`us-2.1/user-story-2.1.md`](../user-stories/us-2.1/user-story-2.1.md) - Testing Layer
- [`us-3.1/user-story-3.1.md`](../user-stories/us-3.1/user-story-3.1.md) - Build & Deploy

### Task Documentation
- [`tasks/run-tests/README-Testing.md`](../../tasks/run-tests/README-Testing.md) - Testing guide
- [`tasks/build-deploy/README.md`](../../tasks/build-deploy/README.md) - Build/deploy guide

### Structure Documents
- [`testing-structure.md`](testing-structure.md) - Testing architecture
- [`be-all-structure.md`](be-all-structure.md) - Overall backend structure

---

## 📊 Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.2.0 | 2026-04-24 | Added Electron Build & Deploy automation (US-11.3) |
| 1.1.0 | 2026-02-06 | File reorganization, added backup feature overview |
| 1.0.0 | 2026-02-06 | Initial creation with US-2.1 and US-3.1 |

---

**Document Version**: 1.2.0  
**Last Updated**: 2026-04-24  
**Status**: ✅ Active  
**Related**: US-2.1, US-3.1, US-11.3
