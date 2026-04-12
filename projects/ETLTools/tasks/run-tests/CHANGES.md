# 📝 Changes Log - Test Scripts Migration

## What Changed?

Scripts đã được move từ root vào `tasks/run-tests/` folder.

---

## ✅ All Scripts Updated

### 1. **run-tests-with-coverage.ps1**
- ✅ Auto-detect script location
- ✅ Navigate to project root
- ✅ Find test projects: `../../tests/`
- ✅ Find runsettings: `../../coverlet.runsettings`
- ✅ Output to: `[root]/TestResults/` và `[root]/CoverageReport/`
- ✅ Auto-cleanup on exit (Pop-Location)

### 2. **run-tests-quick.ps1**
- ✅ Auto-detect script location
- ✅ Navigate to project root
- ✅ All tests run from correct location

### 3. **run-tests-specific.ps1**
- ✅ Auto-detect script location
- ✅ Test project paths: `../../tests/[ProjectName]`
- ✅ Cross-references other scripts correctly

### 4. **README-Testing.md**
- ✅ Updated all command examples
- ✅ Added "Location" note
- ✅ Updated output paths
- ✅ Added file structure diagram

### 5. **NEW: QUICK-START.md**
- ✅ Quick reference guide
- ✅ Common commands
- ✅ Workflow examples

### 6. **NEW: Root Aliases**
- ✅ `test.ps1` - Run with coverage from root
- ✅ `test-quick.ps1` - Quick test from root

---

## 🎯 How to Use

### Option 1: From Project Root (Simplest)

```powershell
# Full test với coverage
.\test.ps1

# Quick test
.\test-quick.ps1

# With filter
.\test.ps1 -Filter "VersionInfo"
.\test-quick.ps1 -Watch
```

### Option 2: From tasks/run-tests/ (Direct)

```powershell
cd tasks\run-tests

# Full test
.\run-tests-with-coverage.ps1

# Quick test
.\run-tests-quick.ps1 -Watch

# Specific project
.\run-tests-specific.ps1 -Project Domain
```

### Option 3: Full Path (Anywhere)

```powershell
.\tasks\run-tests\run-tests-with-coverage.ps1
.\tasks\run-tests\run-tests-quick.ps1 -Watch
```

---

## 📁 File Structure After Changes

```
Lifes/                             # Project root
├── test.ps1                          # ← NEW: Root alias
├── test-quick.ps1                    # ← NEW: Root alias  
├── coverlet.runsettings             # Coverage config (unchanged)
│
├── tasks/
│   └── run-tests/                    # ← Scripts moved here
│       ├── run-tests-with-coverage.ps1   # ✅ Updated
│       ├── run-tests-quick.ps1           # ✅ Updated
│       ├── run-tests-specific.ps1        # ✅ Updated
│       ├── README-Testing.md             # ✅ Updated
│       ├── QUICK-START.md                # ← NEW
│       └── CHANGES.md                    # ← This file
│
├── tests/                            # Test projects (unchanged)
│   ├── Lifes.Domain.Tests/
│   ├── Lifes.Application.Tests/
│   └── ...
│
├── TestResults/                      # Generated: test output
└── CoverageReport/                   # Generated: HTML report
```

---

## ✅ What Still Works

- ✅ All tests run correctly
- ✅ Coverage collection works
- ✅ HTML report generation works
- ✅ Auto-open browser works
- ✅ Filters work
- ✅ Watch mode works
- ✅ Visual Studio Test Explorer works
- ✅ dotnet test commands work

---

## 🎉 New Features

1. **Scripts work from anywhere**
   - Auto-detect location
   - Navigate to correct paths

2. **Root aliases for convenience**
   - `.\test.ps1` instead of long path
   - `.\test-quick.ps1` for quick tests

3. **Better documentation**
   - QUICK-START.md for quick reference
   - Updated README-Testing.md
   - This CHANGES.md file

---

## 🚀 Quick Test

Try running from root:
```powershell
.\test-quick.ps1
```

Should see:
- ✅ Scripts navigate to root
- ✅ Find test projects
- ✅ Run tests
- ✅ Display results

---

**Migration Date**: 2026-02-06  
**Status**: ✅ Complete & Tested
