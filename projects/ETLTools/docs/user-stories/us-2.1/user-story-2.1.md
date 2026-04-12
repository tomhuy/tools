# User Story: US-2.1

## Story Information

| Field | Value |
|-------|-------|
| **ID** | US-2.1 |
| **Title** | Add Testing Layer Infrastructure |
| **Priority** | High |
| **Estimate** | 8 hours |
| **Sprint** | Sprint 2 |
| **Created Date** | 2026-02-06 |
| **Status** | ✅ Completed |

---

## User Story

- **As a** Developer
- **I want to** have a comprehensive testing infrastructure with unit tests, integration tests, and test utilities
- **So that** I can ensure code quality, prevent regressions, and maintain confidence when refactoring or adding new features

---

## Business Context

### Problem Statement

Hiện tại dự án ETLTools chưa có testing infrastructure:
- ⚠️ Không có unit tests cho business logic
- ⚠️ Không có integration tests cho infrastructure layer
- ⚠️ Khó phát hiện bugs khi refactor code
- ⚠️ Không đảm bảo code coverage
- ⚠️ Developer không có confidence khi thay đổi code

### Solution

Xây dựng testing infrastructure hoàn chỉnh bao gồm:
- ✅ Test projects cho tất cả layers
- ✅ Unit tests cho Domain và Application layers
- ✅ Integration tests cho Infrastructure layer
- ✅ Test utilities và helpers
- ✅ Mocking framework setup
- ✅ Code coverage reporting
- ✅ Testing best practices documentation

---

## Acceptance Criteria

### AC-1: Test Project Structure
**Given** dự án ETLTools với Clean Architecture  
**When** developer xem solution structure  
**Then** phải có các test projects sau:
- `ETLTools.Domain.Tests` - Unit tests cho Domain layer
- `ETLTools.Application.Tests` - Unit tests cho Application layer
- `ETLTools.Infrastructure.Tests` - Integration tests cho Infrastructure layer
- `ETLTools.Presentation.WPF.Tests` - Tests cho ViewModels
- `ETLTools.Integration.Tests` - End-to-end integration tests

### AC-2: Testing Framework Configuration
**Given** test projects đã được tạo  
**When** developer chạy tests  
**Then**:
- Sử dụng xUnit testing framework
- Sử dụng Moq cho mocking
- Sử dụng FluentAssertions cho assertions
- Code coverage được collect bằng Coverlet
- Tests có thể chạy từ Visual Studio Test Explorer
- Tests có thể chạy từ command line với `dotnet test`

### AC-3: Sample Tests for Version Increase Feature
**Given** Version Increase feature đã được implement (US-1.1, US-1.2)  
**When** developer xem test projects  
**Then** phải có sample tests sau:

**Domain Tests:**
- `VersionInfoTests.cs` - Test version parsing và increment logic
- `ProjectFileTests.cs` - Test ProjectFile entity validation

**Application Tests:**
- `ScanProjectsCommandTests.cs` - Test scan projects workflow với mocked dependencies
- `UpdateVersionsCommandTests.cs` - Test update versions workflow

**Infrastructure Tests:**
- `ProjectScannerTests.cs` - Test file scanning với test fixtures
- `ProjectFileServiceTests.cs` - Test XML file read/write operations
- `VersionServiceTests.cs` - Test version service implementation

**Presentation Tests:**
- `VersionIncreaseViewModelTests.cs` - Test ViewModel logic và commands

### AC-4: Test Utilities and Helpers
**Given** test projects đã được tạo  
**When** developer viết tests  
**Then** có sẵn test utilities:
- `TestFixtures` class cho test data generation
- `MockFactory` class cho common mocks
- `FileSystemTestHelper` class cho file system testing
- Test configuration helpers
- Sample test data files trong `TestData/` folder

### AC-5: Testing Documentation
**Given** testing infrastructure hoàn chỉnh  
**When** developer đọc documentation  
**Then** phải có:
- `docs/structures/testing-structure.md` - Testing architecture overview
- Testing strategy và guidelines
- How to write tests examples
- How to run tests và view coverage
- Testing best practices
- Update `be-all-structure.md` với Testing section

### AC-6: Code Coverage Baseline
**Given** sample tests đã được viết  
**When** developer chạy code coverage  
**Then**:
- Domain layer coverage >= 80%
- Application layer coverage >= 70%
- Infrastructure layer coverage >= 60%
- Coverage report có thể generate dạng HTML
- Coverage metrics được hiển thị trong console

---

## Technical Design

### Clean Architecture Layers

#### Test Projects Structure

```
tests/
├── ETLTools.Domain.Tests/                      # Unit Tests - Domain
│   ├── Features/
│   │   └── VersionIncrease/
│   │       ├── Entities/
│   │       │   └── ProjectFileTests.cs
│   │       ├── ValueObjects/
│   │       │   └── VersionInfoTests.cs
│   │       └── Enums/
│   │           └── ProjectStatusTests.cs
│   └── TestUtilities/
│       └── DomainTestFixtures.cs
│
├── ETLTools.Application.Tests/                # Unit Tests - Application
│   ├── Features/
│   │   └── VersionIncrease/
│   │       ├── Commands/
│   │       │   ├── ScanProjectsCommandTests.cs
│   │       │   └── UpdateVersionsCommandTests.cs
│   │       └── DTOs/
│   │           └── ProjectFileDtoTests.cs
│   └── TestUtilities/
│       ├── MockFactory.cs
│       └── ApplicationTestFixtures.cs
│
├── ETLTools.Infrastructure.Tests/             # Integration Tests - Infrastructure
│   ├── Features/
│   │   └── VersionIncrease/
│   │       └── Services/
│   │           ├── ProjectScannerTests.cs
│   │           ├── ProjectFileServiceTests.cs
│   │           └── VersionServiceTests.cs
│   ├── TestData/                               # Test data files
│   │   ├── SampleProjects/
│   │   │   ├── Test.ETL.csproj
│   │   │   └── ShareProject.ETL.csproj
│   │   └── ExpectedResults/
│   └── TestUtilities/
│       ├── FileSystemTestHelper.cs
│       └── InfrastructureTestFixtures.cs
│
├── ETLTools.Presentation.WPF.Tests/           # UI Tests - Presentation
│   ├── Features/
│   │   └── VersionIncrease/
│   │       └── VersionIncreaseViewModelTests.cs
│   └── TestUtilities/
│       └── ViewModelTestHelper.cs
│
└── ETLTools.Integration.Tests/                # E2E Integration Tests
    ├── Features/
    │   └── VersionIncrease/
    │       └── VersionIncreaseE2ETests.cs
    └── TestUtilities/
        └── IntegrationTestHelper.cs
```

#### NuGet Packages Required

Each test project cần các packages sau:

```xml
<PackageReference Include="xunit" Version="2.6.6" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="Moq" Version="4.20.70" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="coverlet.collector" Version="6.0.0" />
<PackageReference Include="coverlet.msbuild" Version="6.0.0" />
```

### Testing Strategy

#### 1. Unit Tests (Domain & Application)

**Characteristics:**
- Fast execution (< 100ms per test)
- Isolated, no external dependencies
- Use mocks for dependencies
- Test single responsibility

**Example - VersionInfo Tests:**
```csharp
public class VersionInfoTests
{
    [Theory]
    [InlineData("2026.2.3.1", 2026, 2, 3, 1)]
    [InlineData("2026.12.25.5", 2026, 12, 25, 5)]
    public void ParseVersion_ValidFormat_ShouldParseCorrectly(
        string versionString, int year, int month, int day, int build)
    {
        // Act
        var result = VersionInfo.Parse(versionString);
        
        // Assert
        result.Year.Should().Be(year);
        result.Month.Should().Be(month);
        result.Day.Should().Be(day);
        result.Build.Should().Be(build);
    }
    
    [Fact]
    public void IncrementVersion_SameDay_ShouldIncrementBuildNumber()
    {
        // Arrange
        var version = new VersionInfo(2026, 2, 3, 1);
        var targetDate = new DateTime(2026, 2, 3);
        
        // Act
        var result = version.Increment(targetDate);
        
        // Assert
        result.Build.Should().Be(2);
        result.ToString().Should().Be("2026.2.3.2");
    }
    
    [Fact]
    public void IncrementVersion_NewDay_ShouldResetToOne()
    {
        // Arrange
        var version = new VersionInfo(2026, 2, 2, 5);
        var targetDate = new DateTime(2026, 2, 3);
        
        // Act
        var result = version.Increment(targetDate);
        
        // Assert
        result.Build.Should().Be(1);
        result.ToString().Should().Be("2026.2.3.1");
    }
}
```

#### 2. Integration Tests (Infrastructure)

**Characteristics:**
- Test with real dependencies (file system, XML parsing)
- Use test fixtures and setup/teardown
- May be slower than unit tests
- Test actual implementations

**Example - ProjectFileService Tests:**
```csharp
public class ProjectFileServiceTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly ProjectFileService _service;
    
    public ProjectFileServiceTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);
        _service = new ProjectFileService();
    }
    
    [Fact]
    public async Task ReadVersionAsync_ValidCsprojFile_ShouldReturnVersion()
    {
        // Arrange
        var testFile = CreateTestCsprojFile("2026.2.3.1");
        
        // Act
        var result = await _service.ReadVersionAsync(testFile);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("2026.2.3.1");
    }
    
    [Fact]
    public async Task UpdateVersionAsync_ValidFile_ShouldUpdateVersion()
    {
        // Arrange
        var testFile = CreateTestCsprojFile("2026.2.2.5");
        var newVersion = "2026.2.3.1";
        
        // Act
        var result = await _service.UpdateVersionAsync(testFile, newVersion);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        
        // Verify file content
        var updatedVersion = await _service.ReadVersionAsync(testFile);
        updatedVersion.Value.Should().Be(newVersion);
    }
    
    private string CreateTestCsprojFile(string version)
    {
        var filePath = Path.Combine(_testDirectory, "Test.ETL.csproj");
        var content = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <AssemblyVersion>{version}</AssemblyVersion>
    <FileVersion>{version}</FileVersion>
  </PropertyGroup>
</Project>";
        File.WriteAllText(filePath, content);
        return filePath;
    }
    
    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
            Directory.Delete(_testDirectory, recursive: true);
    }
}
```

#### 3. ViewModel Tests (Presentation)

**Characteristics:**
- Mock Application layer dependencies
- Test commands, properties, and UI logic
- Test data binding scenarios
- Test validation logic

**Example - ViewModel Tests:**
```csharp
public class VersionIncreaseViewModelTests
{
    private readonly Mock<IScanProjectsCommand> _mockScanCommand;
    private readonly Mock<IUpdateVersionsCommand> _mockUpdateCommand;
    private readonly VersionIncreaseViewModel _viewModel;
    
    public VersionIncreaseViewModelTests()
    {
        _mockScanCommand = new Mock<IScanProjectsCommand>();
        _mockUpdateCommand = new Mock<IUpdateVersionsCommand>();
        _viewModel = new VersionIncreaseViewModel(
            _mockScanCommand.Object,
            _mockUpdateCommand.Object);
    }
    
    [Fact]
    public async Task ScanProjectsCommand_WhenExecuted_ShouldLoadProjects()
    {
        // Arrange
        var expectedProjects = new List<ProjectFileDto>
        {
            new() { FileName = "Test1.ETL.csproj", CurrentVersion = "2026.2.3.1" },
            new() { FileName = "Test2.ETL.csproj", CurrentVersion = "2026.2.3.2" }
        };
        
        _mockScanCommand
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IEnumerable<ProjectFileDto>>.Success(expectedProjects));
        
        // Act
        await _viewModel.ScanProjectsCommand.ExecuteAsync(null);
        
        // Assert
        _viewModel.ProjectFiles.Should().HaveCount(2);
        _viewModel.ProjectFiles[0].FileName.Should().Be("Test1.ETL.csproj");
    }
    
    [Fact]
    public void SelectedProjectsCount_WhenProjectsSelected_ShouldUpdateCorrectly()
    {
        // Arrange
        _viewModel.ProjectFiles.Add(new ProjectFileViewModel { IsSelected = true });
        _viewModel.ProjectFiles.Add(new ProjectFileViewModel { IsSelected = false });
        _viewModel.ProjectFiles.Add(new ProjectFileViewModel { IsSelected = true });
        
        // Act
        var count = _viewModel.SelectedProjectsCount;
        
        // Assert
        count.Should().Be(2);
    }
}
```

### Test Utilities

#### MockFactory.cs
```csharp
public static class MockFactory
{
    public static Mock<IProjectScanner> CreateProjectScannerMock()
    {
        var mock = new Mock<IProjectScanner>();
        // Setup default behavior
        return mock;
    }
    
    public static Mock<IProjectFileService> CreateProjectFileServiceMock()
    {
        var mock = new Mock<IProjectFileService>();
        // Setup default behavior
        return mock;
    }
    
    public static Mock<IVersionService> CreateVersionServiceMock()
    {
        var mock = new Mock<IVersionService>();
        // Setup default behavior
        return mock;
    }
}
```

#### TestFixtures.cs
```csharp
public static class TestFixtures
{
    public static ProjectFile CreateTestProjectFile(
        string fileName = "Test.ETL.csproj",
        string version = "2026.2.3.1")
    {
        return new ProjectFile
        {
            FileName = fileName,
            FullPath = $"C:\\Projects\\{fileName}",
            RelativePath = $"Projects\\{fileName}",
            CurrentVersion = VersionInfo.Parse(version),
            IsSelected = false,
            Status = ProjectStatus.Pending
        };
    }
    
    public static VersionInfo CreateTestVersionInfo(
        int year = 2026,
        int month = 2,
        int day = 3,
        int build = 1)
    {
        return new VersionInfo(year, month, day, build);
    }
    
    public static List<ProjectFile> CreateTestProjectList(int count = 3)
    {
        var projects = new List<ProjectFile>();
        for (int i = 1; i <= count; i++)
        {
            projects.Add(CreateTestProjectFile($"Test{i}.ETL.csproj", $"2026.2.3.{i}"));
        }
        return projects;
    }
}
```

#### FileSystemTestHelper.cs
```csharp
public class FileSystemTestHelper : IDisposable
{
    public string TestDirectory { get; private set; }
    
    public FileSystemTestHelper()
    {
        TestDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(TestDirectory);
    }
    
    public string CreateTestCsprojFile(string fileName, string version)
    {
        var filePath = Path.Combine(TestDirectory, fileName);
        var content = $@"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <AssemblyVersion>{version}</AssemblyVersion>
    <FileVersion>{version}</FileVersion>
  </PropertyGroup>
</Project>";
        File.WriteAllText(filePath, content);
        return filePath;
    }
    
    public void CreateTestProjectStructure()
    {
        // Create typical project structure for testing
        var projectsDir = Path.Combine(TestDirectory, "Projects");
        Directory.CreateDirectory(projectsDir);
        
        CreateTestCsprojFile(Path.Combine(projectsDir, "Test1.ETL.csproj"), "2026.2.3.1");
        CreateTestCsprojFile(Path.Combine(projectsDir, "Test2.ETL.csproj"), "2026.2.3.2");
        CreateTestCsprojFile(Path.Combine(projectsDir, "ShareProject.ETL.csproj"), "2026.2.3.1");
    }
    
    public void Dispose()
    {
        if (Directory.Exists(TestDirectory))
        {
            Directory.Delete(TestDirectory, recursive: true);
        }
    }
}
```

### Code Coverage Configuration

#### coverlet.runsettings
```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <DataCollectionRunSettings>
    <DataCollectors>
      <DataCollector friendlyName="XPlat code coverage">
        <Configuration>
          <Format>cobertura,opencover</Format>
          <Exclude>[*.Tests]*</Exclude>
          <ExcludeByAttribute>Obsolete,GeneratedCode,CompilerGenerated</ExcludeByAttribute>
        </Configuration>
      </DataCollector>
    </DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

---

## Tasks Breakdown

### Phase 1: Setup Test Projects (2 hours)

- [ ] Task 1.1: Create test project structure
  - [ ] Create `ETLTools.Domain.Tests` project
  - [ ] Create `ETLTools.Application.Tests` project
  - [ ] Create `ETLTools.Infrastructure.Tests` project
  - [ ] Create `ETLTools.Presentation.WPF.Tests` project
  - [ ] Create `ETLTools.Integration.Tests` project

- [ ] Task 1.2: Add NuGet packages
  - [ ] Add xUnit to all test projects
  - [ ] Add Moq to Application and Presentation test projects
  - [ ] Add FluentAssertions to all test projects
  - [ ] Add Coverlet for code coverage

- [ ] Task 1.3: Setup test folder structure
  - [ ] Create `Features/` folders matching source structure
  - [ ] Create `TestUtilities/` folders
  - [ ] Create `TestData/` folder in Infrastructure tests

### Phase 2: Create Test Utilities (1 hour)

- [ ] Task 2.1: Create MockFactory
  - [ ] Mock for IProjectScanner
  - [ ] Mock for IProjectFileService
  - [ ] Mock for IVersionService
  - [ ] Mock for ISettingsService

- [ ] Task 2.2: Create TestFixtures
  - [ ] ProjectFile test data factory
  - [ ] VersionInfo test data factory
  - [ ] Test data generation methods

- [ ] Task 2.3: Create FileSystemTestHelper
  - [ ] Test directory management
  - [ ] Test .csproj file creation
  - [ ] Test project structure creation
  - [ ] Cleanup/disposal logic

### Phase 3: Write Sample Tests (3 hours)

- [ ] Task 3.1: Domain layer tests
  - [ ] VersionInfoTests.cs - Version parsing tests
  - [ ] VersionInfoTests.cs - Version increment tests
  - [ ] ProjectFileTests.cs - Entity validation tests

- [ ] Task 3.2: Application layer tests
  - [ ] ScanProjectsCommandTests.cs - Command execution tests
  - [ ] UpdateVersionsCommandTests.cs - Update workflow tests
  - [ ] Test with mocked dependencies

- [ ] Task 3.3: Infrastructure layer tests
  - [ ] ProjectScannerTests.cs - File scanning tests
  - [ ] ProjectFileServiceTests.cs - XML read/write tests
  - [ ] VersionServiceTests.cs - Version service tests
  - [ ] Use FileSystemTestHelper for test fixtures

- [ ] Task 3.4: Presentation layer tests
  - [ ] VersionIncreaseViewModelTests.cs - ViewModel logic tests
  - [ ] Test commands and properties
  - [ ] Mock application layer dependencies

### Phase 4: Code Coverage Setup (1 hour)

- [ ] Task 4.1: Configure Coverlet
  - [ ] Create `coverlet.runsettings` file
  - [ ] Configure exclusions and formats
  - [ ] Setup coverage collection

- [ ] Task 4.2: Test coverage reporting
  - [ ] Run tests with coverage
  - [ ] Generate HTML coverage report
  - [ ] Verify coverage thresholds met

### Phase 5: Documentation (1 hour)

- [ ] Task 5.1: Create testing-structure.md
  - [ ] Document testing architecture
  - [ ] Document testing strategy
  - [ ] Document how to run tests
  - [ ] Document how to view coverage

- [ ] Task 5.2: Update be-all-structure.md
  - [ ] Add Testing section
  - [ ] Document test project structure
  - [ ] Add testing best practices

- [ ] Task 5.3: Create testing guidelines
  - [ ] How to write unit tests
  - [ ] How to write integration tests
  - [ ] Testing naming conventions
  - [ ] Testing best practices

---

## Files to Create/Modify

### New Test Projects

- [ ] `tests/ETLTools.Domain.Tests/ETLTools.Domain.Tests.csproj`
- [ ] `tests/ETLTools.Application.Tests/ETLTools.Application.Tests.csproj`
- [ ] `tests/ETLTools.Infrastructure.Tests/ETLTools.Infrastructure.Tests.csproj`
- [ ] `tests/ETLTools.Presentation.WPF.Tests/ETLTools.Presentation.WPF.Tests.csproj`
- [ ] `tests/ETLTools.Integration.Tests/ETLTools.Integration.Tests.csproj`

### Test Utilities

- [ ] `tests/ETLTools.Application.Tests/TestUtilities/MockFactory.cs`
- [ ] `tests/ETLTools.Domain.Tests/TestUtilities/DomainTestFixtures.cs`
- [ ] `tests/ETLTools.Infrastructure.Tests/TestUtilities/FileSystemTestHelper.cs`
- [ ] `tests/ETLTools.Infrastructure.Tests/TestUtilities/InfrastructureTestFixtures.cs`

### Sample Tests

**Domain Tests:**
- [ ] `tests/ETLTools.Domain.Tests/Features/VersionIncrease/ValueObjects/VersionInfoTests.cs`
- [ ] `tests/ETLTools.Domain.Tests/Features/VersionIncrease/Entities/ProjectFileTests.cs`

**Application Tests:**
- [ ] `tests/ETLTools.Application.Tests/Features/VersionIncrease/Commands/ScanProjectsCommandTests.cs`
- [ ] `tests/ETLTools.Application.Tests/Features/VersionIncrease/Commands/UpdateVersionsCommandTests.cs`

**Infrastructure Tests:**
- [ ] `tests/ETLTools.Infrastructure.Tests/Features/VersionIncrease/Services/ProjectScannerTests.cs`
- [ ] `tests/ETLTools.Infrastructure.Tests/Features/VersionIncrease/Services/ProjectFileServiceTests.cs`
- [ ] `tests/ETLTools.Infrastructure.Tests/Features/VersionIncrease/Services/VersionServiceTests.cs`

**Presentation Tests:**
- [ ] `tests/ETLTools.Presentation.WPF.Tests/Features/VersionIncrease/VersionIncreaseViewModelTests.cs`

### Test Data

- [ ] `tests/ETLTools.Infrastructure.Tests/TestData/SampleProjects/Test.ETL.csproj`
- [ ] `tests/ETLTools.Infrastructure.Tests/TestData/SampleProjects/ShareProject.ETL.csproj`

### Configuration

- [ ] `coverlet.runsettings` (root level)
- [ ] `.editorconfig` updates for test projects

### Documentation

- [ ] `docs/structures/testing-structure.md` (new)
- [ ] `docs/structures/be-all-structure.md` (update)
- [ ] `docs/testing-guidelines.md` (new)

---

## Dependencies

- **Depends on**: 
  - US-1.1 (Load Projects) - Completed
  - US-1.2 (Version Update) - Completed
  - US-1.2.1 (Search & Settings) - Completed
  
- **Blocked by**: None

- **Enables**:
  - Future US-1.3 (Git Integration) - Can be developed with TDD
  - Future US-1.4 (Logging) - Can be developed with TDD
  - All future features - Testing infrastructure ready

---

## Definition of Done

- [ ] All 5 test projects created and configured
- [ ] NuGet packages installed (xUnit, Moq, FluentAssertions, Coverlet)
- [ ] Test folder structure matches source code structure
- [ ] Test utilities created (MockFactory, TestFixtures, FileSystemTestHelper)
- [ ] Sample tests written for Version Increase feature:
  - [ ] Domain tests (VersionInfo, ProjectFile)
  - [ ] Application tests (Commands)
  - [ ] Infrastructure tests (Services)
  - [ ] Presentation tests (ViewModel)
- [ ] All sample tests passing (green)
- [ ] Code coverage >= 70% for tested components
- [ ] Coverage report can be generated
- [ ] Testing documentation created:
  - [ ] `testing-structure.md`
  - [ ] `testing-guidelines.md`
  - [ ] `be-all-structure.md` updated
- [ ] Tests can be run from:
  - [ ] Visual Studio Test Explorer
  - [ ] Command line with `dotnet test`
  - [ ] CI/CD pipeline ready (configuration provided)
- [ ] Code review completed
- [ ] User Story approved

---

## Testing Strategy Summary

### Testing Pyramid

```
        /\
       /E2E\          Integration Tests (10%)
      /------\        - End-to-end scenarios
     /        \       - Real dependencies
    /Integration\     
   /------------\     Integration Tests (30%)
  /              \    - Infrastructure tests
 /   Unit Tests  \   - With test fixtures
/------------------\  
                     Unit Tests (60%)
                     - Fast, isolated
                     - Domain & Application
```

### Coverage Goals

| Layer | Target Coverage | Priority |
|-------|----------------|----------|
| Domain | >= 80% | High |
| Application | >= 70% | High |
| Infrastructure | >= 60% | Medium |
| Presentation | >= 50% | Low |

### Test Naming Convention

**Format**: `MethodName_Scenario_ExpectedResult`

**Examples:**
- `ParseVersion_ValidFormat_ShouldParseCorrectly`
- `IncrementVersion_SameDay_ShouldIncrementBuildNumber`
- `IncrementVersion_NewDay_ShouldResetToOne`
- `ReadVersionAsync_ValidFile_ShouldReturnVersion`

---

## Notes

### Testing Best Practices

1. **AAA Pattern**: Arrange, Act, Assert
2. **One Assert Per Test**: Focus on single behavior
3. **Test Names Are Documentation**: Clear, descriptive names
4. **Fast Tests**: Unit tests < 100ms
5. **Independent Tests**: No test dependencies
6. **Use Test Fixtures**: Reusable test data
7. **Mock External Dependencies**: Keep tests isolated
8. **Test Edge Cases**: Not just happy path

### Future Enhancements

- [ ] Add performance tests for large project sets (1000+ projects)
- [ ] Add UI automation tests with WPF testing framework
- [ ] Add mutation testing with Stryker.NET
- [ ] Setup continuous testing in CI/CD pipeline
- [ ] Add benchmark tests with BenchmarkDotNet
- [ ] Add architecture tests with NetArchTest

---

### Implementation Progress

#### Files Created
- [x] `tests/ETLTools.Domain.Tests/Features/VersionIncrease/ValueObjects/VersionInfoTests.cs`
- [x] `tests/ETLTools.Domain.Tests/Features/VersionIncrease/Entities/ProjectFileTests.cs`
- [x] `tests/ETLTools.Application.Tests/Features/VersionIncrease/Commands/ScanProjectsCommandTests.cs`
- [x] `tests/ETLTools.Application.Tests/Features/VersionIncrease/Commands/UpdateVersionsCommandTests.cs`
- [x] `tests/ETLTools.Infrastructure.Tests/Features/VersionIncrease/Services/ProjectScannerTests.cs`
- [x] `tests/ETLTools.Infrastructure.Tests/Features/VersionIncrease/Services/ProjectFileServiceTests.cs`
- [x] `tests/ETLTools.Infrastructure.Tests/Features/VersionIncrease/Services/VersionServiceTests.cs`
- [x] `tests/ETLTools.Presentation.WPF.Tests/Features/VersionIncrease/VersionIncreaseViewModelTests.cs`
- [x] `tests/ETLTools.Domain.Tests/TestUtilities/DomainTestFixtures.cs`
- [x] `tests/ETLTools.Application.Tests/TestUtilities/AppMockFactory.cs`
- [x] `tests/ETLTools.Infrastructure.Tests/TestUtilities/FileSystemTestHelper.cs`
- [x] `tests/ETLTools.Infrastructure.Tests/TestUtilities/InfrastructureTestFixtures.cs`
- [x] `coverlet.runsettings`
- [x] `docs/structures/testing-structure.md`
- [x] `docs/guidelines/testing-guidelines.md`

#### Current Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-02-06
- **Percentage**: 100%
- **Notes**: All test projects setup, sample tests written for Version Increase feature, and documentation completed.

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
