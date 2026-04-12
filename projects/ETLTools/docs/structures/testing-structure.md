# Testing Layer Infrastructure

This document describes the testing architecture and strategy for the ETLTools project.

## Testing Architecture

The project follows a pyramid testing strategy with three main layers:

1.  **Unit Tests (60%)**
    *   **Location**: `tests/ETLTools.Domain.Tests`, `tests/ETLTools.Application.Tests`
    *   **Focus**: Domain logic, value objects, application services, and commands.
    *   **Isolation**: Fully isolated using mocks for all external dependencies.
    *   **Frameworks**: xUnit, Moq, FluentAssertions.

2.  **Integration Tests (30%)**
    *   **Location**: `tests/ETLTools.Infrastructure.Tests`
    *   **Focus**: Infrastructure implementations (ProjectScanner, ProjectFileService).
    *   **Dependencies**: Real file system (using temporary test directories), real XML parsing.
    *   **Helpers**: `FileSystemTestHelper` for managing test files.

3.  **UI/Presentation Tests**
    *   **Location**: `tests/ETLTools.Presentation.WPF.Tests`
    *   **Focus**: ViewModels, commands, and UI logic.
    *   **Frameworks**: xUnit, Moq (mocking Application layer commands).

4.  **End-to-End Integration Tests (10%)**
    *   **Location**: `tests/ETLTools.Integration.Tests`
    *   **Focus**: Full workflows from Application to Infrastructure.

## Test Project Structure

```text
tests/
├── ETLTools.Domain.Tests/                      # Unit Tests - Domain
│   ├── Features/VersionIncrease/
│   │   ├── Entities/ProjectFileTests.cs
│   │   └── ValueObjects/VersionInfoTests.cs
│   └── TestUtilities/DomainTestFixtures.cs
│
├── ETLTools.Application.Tests/                 # Unit Tests - Application
│   ├── Features/VersionIncrease/
│   │   └── Commands/
│   │       ├── ScanProjectsCommandTests.cs
│   │       └── UpdateVersionsCommandTests.cs
│   └── TestUtilities/AppMockFactory.cs
│
├── ETLTools.Infrastructure.Tests/              # Integration Tests - Infrastructure
│   ├── Features/VersionIncrease/
│   │   └── Services/
│   │       ├── ProjectScannerTests.cs
│   │       ├── ProjectFileServiceTests.cs
│   │       └── VersionServiceTests.cs
│   └── TestUtilities/
│       ├── FileSystemTestHelper.cs
│       └── InfrastructureTestFixtures.cs
│
└── ETLTools.Presentation.WPF.Tests/            # UI Tests - Presentation
    └── Features/VersionIncrease/
        └── VersionIncreaseViewModelTests.cs
```

## How to Run Tests

### From Visual Studio
Use **Test Explorer** (`Ctrl+E, T`) to run and debug tests.

### From Command Line
Run all tests:
```powershell
dotnet test
```

Run tests with code coverage:
```powershell
dotnet test --settings coverlet.runsettings
```

## Code Coverage

Code coverage is collected using **Coverlet**. Configuration is managed in `coverlet.runsettings`.

### Coverage Goals
| Layer | Target Coverage |
|-------|----------------|
| Domain | >= 80% |
| Application | >= 70% |
| Infrastructure | >= 60% |
| Presentation | >= 50% |

### Viewing Coverage Reports
1. Run tests with the settings file.
2. Coverage files (`coverage.cobertura.xml`) are generated in the `TestResults` folder of each test project.
3. Use a tool like **ReportGenerator** to convert XML to HTML:
   ```powershell
   dotnet tool install -g dotnet-reportgenerator-globaltool
   reportgenerator "-reports:tests/**/coverage.cobertura.xml" "-targetdir:coveragereport" -reporttypes:Html
   ```
