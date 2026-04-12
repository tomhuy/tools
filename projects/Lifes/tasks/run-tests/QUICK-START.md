# ⚡ Quick Start - Test Scripts

## 🎯 Chạy Từ Đâu?

**Option 1**: Từ project root
```powershell
.\tasks\run-tests\run-tests-with-coverage.ps1
```

**Option 2**: cd vào folder scripts (gõ ngắn hơn)
```powershell
cd tasks\run-tests
.\run-tests-with-coverage.ps1
```

---

## 📝 Commands Chính

### 1. Full Test + Coverage Report
```powershell
.\run-tests-with-coverage.ps1
```
- ✅ Build solution
- ✅ Run tất cả tests
- ✅ Generate HTML coverage report
- ✅ Auto-open trong browser

### 2. Quick Test (Không Coverage)
```powershell
.\run-tests-quick.ps1
```
- ⚡ Nhanh - không generate report
- ✅ Xem test pass/fail
- Dùng khi: developing, quick check

### 3. Watch Mode (Auto-run on Change)
```powershell
.\run-tests-quick.ps1 -Watch
```
- 👀 Auto-run khi code thay đổi
- ⚡ Fast feedback loop
- Dùng khi: đang code feature

### 4. Test Specific Project
```powershell
.\run-tests-specific.ps1 -Project Application
```
- Projects: Domain, Application, Infrastructure, Presentation, Integration, All

---

## 🎨 Common Options

### With Filter
```powershell
# Test specific class/method
.\run-tests-quick.ps1 -Filter "VersionInfo"

# Test với coverage
.\run-tests-with-coverage.ps1 -Filter "ScanProjects"
```

### Skip Build
```powershell
# Nếu đã build rồi
.\run-tests-with-coverage.ps1 -SkipBuild
```

### Specific Project + Coverage
```powershell
.\run-tests-specific.ps1 -Project Domain -WithCoverage
```

---

## 📊 Xem Coverage Report

**Auto-open**: Report tự động mở sau khi chạy với coverage

**Manual open** (từ project root):
```powershell
start .\CoverageReport\index.html
```

**Location**: `[project-root]/CoverageReport/index.html`

---

## 💡 Workflow Đề Xuất

### Daily Development
```powershell
cd tasks\run-tests
.\run-tests-quick.ps1 -Watch
```

### Before Commit
```powershell
.\run-tests-with-coverage.ps1
# Check: Coverage >= 70%? ✅
```

### Debugging
```powershell
.\run-tests-quick.ps1 -Filter "FailingTestName"
```

---

## 🆘 Troubleshooting

### ReportGenerator Not Found
```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool
```

### Cannot Find Test Projects
- Đảm bảo chạy từ `tasks/run-tests/` hoặc project root
- Scripts tự động tìm đến `../../tests/`

### Coverage Files Not Found
- Check file `coverlet.runsettings` ở project root
- Check folder `TestResults/` ở project root

---

## 📚 Full Documentation

Xem **README-Testing.md** trong cùng folder để biết:
- Detailed options
- Filter patterns
- Visual Studio integration
- CI/CD setup
- Troubleshooting guide

---

**Updated**: 2026-02-06
