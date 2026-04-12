# 🧪 Lifes Testing Guide

Hướng dẫn chạy tests và xem coverage reports cho Lifes project.

---

## 📋 Quick Reference

**Location**: Scripts nằm trong `tasks/run-tests/`

### Chạy Tất Cả Tests Với Coverage Report

```powershell
# Từ project root
.\tasks\run-tests\run-tests-with-coverage.ps1

# Hoặc từ tasks/run-tests/ folder
cd tasks\run-tests
.\run-tests-with-coverage.ps1

# Chạy với filter cụ thể
.\run-tests-with-coverage.ps1 -Filter "VersionInfo"

# Skip build step (nếu đã build rồi)
.\run-tests-with-coverage.ps1 -SkipBuild

# Chỉ run tests, không generate report
.\run-tests-with-coverage.ps1 -SkipReport
```

### Chạy Tests Nhanh (Không Coverage)

```powershell
# Chạy tất cả tests
.\tasks\run-tests\run-tests-quick.ps1

# Chạy với filter
.\tasks\run-tests\run-tests-quick.ps1 -Filter "ScanProjects"

# Watch mode - tự động chạy khi code thay đổi
.\tasks\run-tests\run-tests-quick.ps1 -Watch
```

### Chạy Test Project Cụ Thể

```powershell
# Chạy Domain tests
.\tasks\run-tests\run-tests-specific.ps1 -Project Domain

# Chạy Application tests với coverage
.\tasks\run-tests\run-tests-specific.ps1 -Project Application -WithCoverage

# Chạy Infrastructure tests với filter
.\tasks\run-tests\run-tests-specific.ps1 -Project Infrastructure -Filter "ProjectScanner"

# Available projects: Domain, Application, Infrastructure, Presentation, Integration, All
```

---

## 🎯 Workflow Recommendations

### Development Workflow

```powershell
# 1. Quick check khi đang code (từ project root)
.\tasks\run-tests\run-tests-quick.ps1

# 2. Watch mode khi đang implement feature mới
.\tasks\run-tests\run-tests-quick.ps1 -Watch

# 3. Test specific area đang làm việc
.\tasks\run-tests\run-tests-specific.ps1 -Project Application -Filter "ScanProjects"

# 4. Hoặc cd vào tasks/run-tests để gõ ngắn hơn
cd tasks\run-tests
.\run-tests-quick.ps1 -Watch
```

### Before Commit

```powershell
# Full test với coverage report
.\tasks\run-tests\run-tests-with-coverage.ps1

# Kiểm tra coverage >= 70%
# Report sẽ tự động mở trong browser
```

### Debugging Failed Tests

```powershell
# Run specific test với filter
.\tasks\run-tests\run-tests-quick.ps1 -Filter "IncrementVersion_SameDay"

# Hoặc trong Visual Studio:
# 1. Mở Test Explorer (Ctrl + E, T)
# 2. Right-click test → Debug
```

---

## 📊 Xem Coverage Report

### HTML Report (Recommended)

```powershell
# Generate và mở report
.\tasks\run-tests\run-tests-with-coverage.ps1

# Report sẽ mở tự động tại: [project-root]/CoverageReport/index.html
# Manual open (từ project root):
start .\CoverageReport\index.html
```

### Visual Studio

```
1. Menu: Test → Analyze Code Coverage for All Tests
2. View → Other Windows → Code Coverage Results
3. Xem coverage % cho từng assembly/class
4. Blue/Red highlighting trong code editor
```

### Console Summary

```powershell
# View summary trong console (từ project root)
cat .\CoverageReport\Summary.txt
```

---

## 🔍 Filter Patterns

### Filter By Test Name

```powershell
# Run tests có "VersionInfo" trong tên
.\tasks\run-tests\run-tests-quick.ps1 -Filter "VersionInfo"

# Run tests có "IncrementVersion" và "SameDay"
.\tasks\run-tests\run-tests-quick.ps1 -Filter "IncrementVersion&SameDay"
```

### Filter By Namespace

```powershell
# Run tests trong namespace cụ thể
.\tasks\run-tests\run-tests-quick.ps1 -Filter "FullyQualifiedName~Lifes.Domain.Tests"
```

### Filter By Category/Trait

```powershell
# Run tests có trait cụ thể (nếu có)
.\tasks\run-tests\run-tests-quick.ps1 -Filter "Category=Unit"
```

---

## 📁 Output Locations

**Note**: Tất cả output được tạo ở project root, không phải trong `tasks/run-tests/`

| File/Folder | Description |
|-------------|-------------|
| `[root]/TestResults/` | Raw test results và coverage data |
| `[root]/CoverageReport/` | HTML coverage report |
| `[root]/CoverageReport/index.html` | Main coverage report page |
| `[root]/CoverageReport/Summary.txt` | Text summary of coverage |
| `[root]/coverlet.runsettings` | Coverage configuration file |

---

## 🎨 Coverage Report Features

### Summary Page
- Overall line/branch/method coverage
- Coverage by assembly
- Coverage by namespace
- Filterable và sortable tables

### Detail Pages
- Line-by-line coverage
- 🟩 Green = Covered
- 🟥 Red = Not covered
- 🟨 Yellow = Partially covered
- Branch coverage details

### Badges
- Coverage badges (SVG)
- Use trong README hoặc documentation

---

## ⚡ Performance Tips

### Fast Iteration During Development

```powershell
# Use watch mode
.\tasks\run-tests\run-tests-quick.ps1 -Watch

# Only test what you're working on
.\tasks\run-tests\run-tests-specific.ps1 -Project Application -Filter "YourFeature"

# Tip: cd vào tasks/run-tests để command ngắn hơn
cd tasks\run-tests
.\run-tests-quick.ps1 -Watch
```

### Skip Unnecessary Steps

```powershell
# Skip build if already built
.\tasks\run-tests\run-tests-with-coverage.ps1 -SkipBuild

# Skip report if only checking test pass/fail
.\tasks\run-tests\run-tests-with-coverage.ps1 -SkipReport
```

---

## 🐛 Troubleshooting

### ReportGenerator Not Found

```powershell
# Install reportgenerator tool
dotnet tool install --global dotnet-reportgenerator-globaltool

# Update if already installed
dotnet tool update --global dotnet-reportgenerator-globaltool
```

### No Coverage Files Found

```powershell
# Ensure coverlet.runsettings exists at project root
# Ensure test projects have coverlet packages installed
# Check [root]/TestResults folder for coverage.cobertura.xml

# Verify runsettings location
ls .\coverlet.runsettings
```

### Tests Failing

```powershell
# Run with detailed output (từ project root)
dotnet test --verbosity detailed

# Check specific test
.\tasks\run-tests\run-tests-quick.ps1 -Filter "FailingTestName"

# Debug in Visual Studio
# Test Explorer → Right-click test → Debug
```

### Permission Issues

```powershell
# Run PowerShell as Administrator
# Or set execution policy
Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process
```

---

## 📚 Additional Resources

### Documentation
- [Testing Structure](./docs/structures/testing-structure.md)
- [User Story US-2.1](./docs/user-stories/us-2.1/user-story-2.1.md)

### Tools
- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator Documentation](https://danielpalme.github.io/ReportGenerator/)

---

## 🎯 Coverage Goals

| Layer | Target Coverage | Current |
|-------|----------------|---------|
| Domain | >= 80% | TBD |
| Application | >= 70% | TBD |
| Infrastructure | >= 60% | TBD |
| Presentation | >= 50% | TBD |
| **Overall** | **>= 70%** | **TBD** |

---

## 💡 Best Practices

1. ✅ **Run tests frequently** - Use watch mode during development
2. ✅ **Check coverage before commit** - Ensure >= 70% overall
3. ✅ **Use filters** - Focus on what you're working on
4. ✅ **Fix failing tests immediately** - Don't let them accumulate
5. ✅ **Review coverage report** - Identify untested code paths
6. ✅ **Test edge cases** - Not just happy paths
7. ✅ **Keep tests fast** - Unit tests < 100ms

---

**Last Updated**: 2026-02-06  
**Version**: 1.0.0
