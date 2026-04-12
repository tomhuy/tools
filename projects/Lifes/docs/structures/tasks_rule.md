# Task Structure - Automation & Scripting

## 📋 Document Information

| Field | Value |
|-------|-------|
| **Document Name** | Task Structure & Automation Guidelines |
| **Version** | 1.0.0 |
| **Last Updated** | 2026-02-06 |
| **Purpose** | Define structure and guidelines for automation tasks and scripts |
| **Target Audience** | Developers, AI Agents |

---

## 🎯 Overview

Tài liệu này định nghĩa cấu trúc và quy tắc cho các automation tasks và scripts trong dự án Lifes. Mỗi automation task được tổ chức theo cấu trúc rõ ràng để dễ maintain và reuse.

---

## 📁 Task Organization Structure

### Folder Structure

```
tasks/
├── [task-name]/                    # Task folder
│   ├── [main-script].ps1          # Main executable script
│   ├── [helper-script].ps1        # Helper scripts (optional)
│   ├── README.md                  # Documentation
│   ├── QUICK-START.md             # Quick reference (optional)
│   └── CHANGES.md                 # Change log (optional)
│
└── [another-task]/
    └── ...
```

### Example: Test Automation Task

```
tasks/
└── run-tests/
    ├── run-tests-with-coverage.ps1    # Main script
    ├── run-tests-quick.ps1            # Quick test script
    ├── run-tests-specific.ps1         # Specific project script
    ├── README-Testing.md              # Full documentation
    ├── QUICK-START.md                 # Quick reference
    └── CHANGES.md                     # Migration log
```

---

## 📝 Script Structure Guidelines

### PowerShell Script Template

```powershell
# ============================================
# [Project Name] - [Script Purpose]
# ============================================
# [Brief description of what this script does]
# ============================================

param(
    [switch]$OptionFlag,
    [string]$Parameter = "default",
    [Parameter(Mandatory=$false)]
    [string]$OptionalParam
)

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "[Script Title]" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Get script directory and project root
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptDir "..\..")

# Change to project root
Push-Location $projectRoot

try {
    # Main script logic here
    Write-Host "Doing something..." -ForegroundColor Yellow
    
    # Error handling
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error occurred!" -ForegroundColor Red
        exit 1
    }
    
    Write-Host "Done!" -ForegroundColor Green
}
catch {
    Write-Host "Exception: $_" -ForegroundColor Red
    exit 1
}
finally {
    # Cleanup and return to original directory
    Pop-Location
}
```

### Key Principles

1. **Self-Contained Navigation**
   - Script tự động detect location
   - Navigate về project root
   - Cleanup với `finally` block

2. **Clear Output**
   - Use colors: Yellow (info), Green (success), Red (error)
   - Progress indicators
   - Clear section separators

3. **Error Handling**
   - Try-catch-finally blocks
   - Check `$LASTEXITCODE`
   - Meaningful error messages
   - Exit with proper codes

4. **Parameters**
   - Use typed parameters
   - Provide defaults when appropriate
   - Use `[switch]` for boolean flags
   - Document parameters in header

---

## 📚 Documentation Requirements

### README.md Structure

```markdown
# [Task Name] - Documentation

## Overview
[Brief description of what this task does]

## Quick Reference
[Most common commands]

## Usage
### Basic Usage
[Simple examples]

### Advanced Usage
[Complex examples with options]

## Options/Parameters
[Detailed parameter documentation]

## Examples
[Real-world usage examples]

## Troubleshooting
[Common issues and solutions]

## See Also
[Links to related documentation]
```

### QUICK-START.md (Optional)

For complex tasks with multiple scripts:
- One-page quick reference
- Most common commands
- Copy-paste examples
- Minimal explanations

### CHANGES.md (Optional)

When task structure changes:
- What changed
- Why it changed
- Migration guide
- Breaking changes

---

## 🎨 Best Practices

### Script Design

1. **Single Responsibility**
   - One main purpose per script
   - Extract helpers for complex logic
   - Keep scripts focused

2. **Reusability**
   - Parameterize instead of hardcode
   - Make paths relative to project root
   - Support running from anywhere

3. **Idempotent Operations**
   - Safe to run multiple times
   - Clean up previous results
   - Handle existing files/folders

4. **User Experience**
   - Clear progress indicators
   - Helpful error messages
   - Summary at the end
   - Option to skip steps

### File Organization

1. **Logical Grouping**
   - Related scripts in same folder
   - Clear naming convention
   - README in each task folder

2. **Output Management**
   - Output to project root (not script folder)
   - Clear output location messages
   - Clean up old outputs

3. **Cross-Platform Considerations**
   - Use `Join-Path` for paths
   - Test on Windows (primary)
   - Document platform requirements

---

## 🔧 Common Task Types

### 1. Build & Compilation Tasks

**Purpose**: Compile code, generate artifacts

**Example Structure**:
```
tasks/
└── build/
    ├── build.ps1              # Full build
    ├── build-debug.ps1        # Debug build
    ├── build-release.ps1      # Release build
    └── README.md
```

**Key Features**:
- Configuration support (Debug/Release)
- Parallel builds
- Output directory management
- Build logs

### 2. Testing Tasks

**Purpose**: Run tests, generate coverage reports

**Example Structure**:
```
tasks/
└── run-tests/
    ├── run-tests-with-coverage.ps1
    ├── run-tests-quick.ps1
    ├── run-tests-specific.ps1
    └── README-Testing.md
```

**Key Features**:
- Multiple test modes
- Coverage reporting
- Filter support
- Watch mode

### 3. Deployment Tasks

**Purpose**: Deploy to environments

**Example Structure**:
```
tasks/
└── deploy/
    ├── deploy-dev.ps1
    ├── deploy-staging.ps1
    ├── deploy-prod.ps1
    └── README.md
```

**Key Features**:
- Environment configuration
- Pre-deployment checks
- Rollback capability
- Deployment logs

### 4. Maintenance Tasks

**Purpose**: Cleanup, migrations, utilities

**Example Structure**:
```
tasks/
└── maintenance/
    ├── cleanup-artifacts.ps1
    ├── migrate-data.ps1
    ├── backup.ps1
    └── README.md
```

**Key Features**:
- Safe defaults
- Confirmation prompts
- Backup before operations
- Detailed logs

### 5. Code Generation Tasks

**Purpose**: Generate boilerplate code

**Example Structure**:
```
tasks/
└── generate/
    ├── new-feature.ps1
    ├── new-test.ps1
    ├── templates/
    └── README.md
```

**Key Features**:
- Templates
- Naming conventions
- File structure creation
- Post-generation instructions

---

## 📋 Task Checklist

### Creating New Task

- [ ] **Planning**
  - [ ] Define task purpose clearly
  - [ ] Identify inputs and outputs
  - [ ] Plan script structure
  - [ ] Consider error scenarios

- [ ] **Implementation**
  - [ ] Create task folder: `tasks/[task-name]/`
  - [ ] Implement main script
  - [ ] Add parameter validation
  - [ ] Implement error handling
  - [ ] Add progress indicators
  - [ ] Test from different locations

- [ ] **Documentation**
  - [ ] Create README.md
  - [ ] Document parameters
  - [ ] Add usage examples
  - [ ] Add troubleshooting section
  - [ ] Create QUICK-START.md (if needed)

- [ ] **Testing**
  - [ ] Test happy path
  - [ ] Test error scenarios
  - [ ] Test from project root
  - [ ] Test from script folder
  - [ ] Test with various parameters

- [ ] **Integration**
  - [ ] Add to root README.md
  - [ ] Create root alias (if appropriate)
  - [ ] Update work_rule.md Quick Reference
  - [ ] Commit with clear message

---

## 🎯 Example: Test Automation Task

### Structure

```
tasks/run-tests/
├── run-tests-with-coverage.ps1    # Full test + coverage
├── run-tests-quick.ps1            # Quick test only
├── run-tests-specific.ps1         # Test specific project
├── README-Testing.md              # Full documentation
├── QUICK-START.md                 # Quick reference
└── CHANGES.md                     # Migration notes
```

### Features Implemented

✅ **Multiple Modes**:
- Full test with coverage report
- Quick test (no coverage)
- Specific project testing
- Watch mode support

✅ **Smart Navigation**:
- Auto-detect script location
- Navigate to project root
- Find test projects automatically
- Return to original location

✅ **User Experience**:
- Colored output
- Progress indicators
- Summary reports
- Auto-open HTML reports

✅ **Documentation**:
- Comprehensive README
- Quick-start guide
- Usage examples
- Troubleshooting guide

---

## 🔗 Related Documents

- **Coding Rules**: `rule.md`
- **Development Workflow**: `work_rule.md`
- **Testing Guidelines**: `docs/guidelines/testing-guidelines.md`
- **Testing Structure**: `docs/structures/testing-structure.md`

---

## 📝 Naming Conventions

### Task Folders
- Format: `kebab-case`
- Examples: `run-tests`, `build-release`, `deploy-prod`

### Script Files
- Format: `kebab-case.ps1`
- Examples: `run-tests-with-coverage.ps1`, `build-debug.ps1`
- Main script: Should clearly indicate primary purpose
- Helper scripts: Should indicate specific function

### Documentation Files
- `README.md` - Main documentation
- `QUICK-START.md` - Quick reference
- `CHANGES.md` - Change log
- `TROUBLESHOOTING.md` - Troubleshooting guide (if complex)

---

## ✅ Success Criteria

A well-structured automation task should:

1. ✅ **Be Self-Documenting**
   - Clear script names
   - Inline comments for complex logic
   - Comprehensive README

2. ✅ **Be User-Friendly**
   - Run from anywhere
   - Clear output
   - Helpful error messages
   - Examples in documentation

3. ✅ **Be Maintainable**
   - Clean code structure
   - Proper error handling
   - Version controlled
   - Change log maintained

4. ✅ **Be Reliable**
   - Consistent behavior
   - Proper cleanup
   - Safe to run multiple times
   - Good error recovery

5. ✅ **Be Discoverable**
   - Listed in root README
   - Referenced in work_rule.md
   - Clear documentation
   - Examples provided

---

**Document Version**: 1.0.0  
**Last Updated**: 2026-02-06  
**Status**: ✅ Active
