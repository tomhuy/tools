# Backend Structure - Overall

## Overview

ETL Deployment Tools Suite follows **Clean Architecture** with **feature-based organization**. The application is structured into 5 layers, with each feature organized in its own folder within each layer.

---

## Project Structure

```
src/
├── Lifes.Core/                  # Shared interfaces and models
│   ├── Interfaces/
│   │   ├── IProjectScanner.cs
│   │   ├── IProjectFileService.cs
│   │   ├── IVersionService.cs
│   │   ├── ISettingsService.cs     # US-1.2.1
│   │   ├── IGitService.cs          # US-1.3
│   │   ├── INavigationService.cs   # US-5.1
│   │   ├── ICalendarService.cs     # US-9.1
│   │   ├── IMementoRepository.cs   # US-9.2
│   │   └── ITagRepository.cs       # US-9.2
│   └── Models/
│       ├── Result.cs               # Result<T> pattern
│       ├── ToolDefinition.cs       # US-5.1 — tool metadata
│       ├── ToolNavigatedEventArgs.cs # US-5.1 — navigation event args
│       ├── CalendarEventModel.cs   # US-8.4 — Multi-phase event entity
│       ├── CalendarEventPhaseModel.cs # US-8.4 — event phase detail
│       ├── MementoModel.cs         # US-9.1 — Recursive hierarchy
│       ├── TagModel.cs             # US-9.2 — Tagging system
│       └── MementoQueryModel.cs    # US-9.2 — Filtering model
│
├── Lifes.Domain/                # Business logic and entities
│   ├── Common/                     # US-1.2.1
│   │   └── ValueObjects/
│   │       └── AppSettings.cs      # Settings validation logic
│   └── Features/
│       └── VersionIncrease/
│           ├── Entities/
│           │   └── ProjectFile.cs
│           ├── ValueObjects/
│           │   ├── VersionInfo.cs  # Core version increment logic
│           │   └── GitCommitInfo.cs # US-1.3
│           └── Enums/
│               └── ProjectStatus.cs
│
├── Lifes.Infrastructure/        # External services
│   ├── Common/                     # US-1.2.1
│   │   └── Configuration/
│   │       └── SettingsService.cs  # JSON file persistence
│   └── Features/
│       ├── VersionIncrease/
│       │   ├── Services/
│       │   │   ├── ProjectScanner.cs
│       │   │   ├── ProjectFileService.cs
│       │   │   └── VersionService.cs
│       │   └── Git/                # US-1.3
│       │       └── GitService.cs   # LibGit2Sharp implementation
│       └── AnnualCalendar/
│           ├── Services/
│           │   └── CalendarService.cs     # US-9.2 — logical orchestration
│           └── Repositories/
│               ├── MockMementoRepository.cs # US-9.2 (Deprecated)
│               ├── MockTagRepository.cs     # US-9.2 (Deprecated)
│               ├── JsonMementoRepository.cs # US-9.3
│               └── JsonTagRepository.cs     # US-9.3
│
├── Lifes.Application/           # Use cases and commands
│   ├── Common/                     # US-1.2.1
│   │   ├── Commands/
│   │   │   ├── LoadSettingsCommand.cs
│   │   │   └── SaveSettingsCommand.cs
│   │   └── DTOs/
│   │       └── AppSettingsDto.cs
│   ├── Services/                   # US-5.1
│   │   └── NavigationService.cs    # INavigationService implementation
│   └── Features/
│       └── VersionIncrease/
│           ├── Commands/
│           │   ├── ScanProjectsCommand.cs
│           │   ├── UpdateVersionsCommand.cs
│           │   └── CommitChangesCommand.cs    # US-1.3
│           └── DTOs/
│               ├── ProjectFileDto.cs
│               ├── CommitChangesDto.cs        # US-1.3
│               ├── CommitResultDto.cs         # US-1.3
│               ├── ProjectUpdateDto.cs
│               └── VersionUpdateResultDto.cs
│
├── Lifes.Presentation.WPF/     # WPF UI
│   ├── Constants/                  # US-5.1
│   │   └── ToolIds.cs              # Well-known tool ID constants
│   ├── Controls/                   # US-5.1
│   │   ├── NavigationMenuButton.xaml    # Hamburger button + hover dropdown
│   │   └── NavigationMenuButton.xaml.cs
│   ├── Models/                     # US-5.1
│   │   └── ToolMenuItem.cs         # ViewModel cho từng item trong dropdown
│   ├── Styles/                     # US-5.1
│   │   └── NavigationMenuStyles.xaml   # NavButtonStyle, NavMenuItemStyle
│   ├── Features/
│   │   ├── DashboardChart/             # US-6.1
│   │   │   ├── Controls/
│   │   │   │   └── DashboardBlockHost.cs
│   │   │   ├── Registries/
│   │   │   │   └── DashboardViewRegistry.cs
│   │   │   ├── Views/
│   │   │   │   ├── AstrologyCellView.xaml
│   │   │   │   └── DefaultDashboardBlockView.xaml
│   │   │   ├── DashboardChartView.xaml
│   │   │   └── DashboardChartViewModel.cs
│   │   ├── AnnualCalendar/             # US-8.4, US-8.5, US-8.6
│   │   │   ├── AnnualCalendarView.xaml
│   │   │   ├── AnnualCalendarViewModel.cs
│   │   │   ├── MonthlyCalendarView.xaml
│   │   │   ├── MonthlyCalendarViewModel.cs
│   │   │   ├── ActivityHeatmapView.xaml    # US-8.6
│   │   │   ├── ActivityHeatmapViewModel.cs # US-8.6
│   │   │   └── Models/
│   │   │       └── SelectableTagViewModel.cs # US-9.2
│   │   ├── DocumentManagement/         # US-7.1
│   │   │   ├── DocumentManagementView.xaml
│   │   │   └── DocumentManagementViewModel.cs
│   │   └── VersionIncrease/
│   │       ├── VersionIncreaseView.xaml      # Search UI + Commit button + Hamburger (US-1.2.1, US-1.3, US-8.5)
│   │       ├── VersionIncreaseViewModel.cs   # Search + Settings + Git + Navigation (US-1.2.1, US-1.3, US-8.5)
│   │       ├── Helpers/                      # US-1.2.1
│   │       │   └── ProjectFilterHelper.cs    # Search/filter logic
│   │       ├── Models/
│   │       │   └── ProjectFileViewModel.cs
│   │       ├── ViewModels/                   # US-1.3
│   │       │   └── GitCommitDialogViewModel.cs
│   │       └── Views/                        # US-1.3
│   │           ├── GitCommitDialog.xaml
│   │           └── GitCommitDialog.xaml.cs
│   ├── App.xaml                    # DI config + Hamburger styles (US-8.5)
│   ├── App.xaml.cs                 # Registers NavigationService + tools (US-5.1)
│   └── MainWindow.xaml.cs          # Handles ToolNavigated → swap views (US-5.1)
│
└── tests/                          # Testing Layer (US-2.1)
    ├── Lifes.Domain.Tests/      # Unit tests for Domain
    │   ├── Features/VersionIncrease/
    │   │   ├── ValueObjects/
    │   │   └── Entities/
    │   └── TestUtilities/
    │       └── DomainTestFixtures.cs
    │
    ├── Lifes.Application.Tests/ # Unit tests for Application
    │   ├── Features/VersionIncrease/
    │   │   └── Commands/
    │   └── TestUtilities/
    │       └── AppMockFactory.cs
    │
    ├── Lifes.Infrastructure.Tests/ # Integration tests
    │   ├── Features/VersionIncrease/
    │   │   └── Services/
    │   └── TestUtilities/
    │       ├── FileSystemTestHelper.cs
    │       └── InfrastructureTestFixtures.cs
    │
    ├── Lifes.Presentation.WPF.Tests/ # UI tests
    │   └── Features/VersionIncrease/
    │       └── VersionIncreaseViewModelTests.cs
    │
    └── Lifes.Integration.Tests/ # E2E tests
        └── Features/VersionIncrease/
```

---

## Architecture Layers

### Dependency Flow

```
┌─────────────────────────────────────────┐
│  Presentation (WPF)                     │
│  - Views, ViewModels                   │
└──────────────┬──────────────────────────┘
               │ depends on
               ▼
┌─────────────────────────────────────────┐
│  Application                            │
│  - Commands, Queries, DTOs             │
└─────┬────────────────────────┬──────────┘
      │ depends on              │ depends on
      ▼                        ▼
┌──────────────┐    ┌──────────────────────┐
│   Domain     │    │   Infrastructure     │
│  (Pure)      │    │   (Implementations)  │
└──────────────┘    └──────────────────────┘
      ▲                        ▲
      └────────────┬───────────┘
                   │ implements
┌──────────────────┴───────────────────────┐
│  Core (Shared)                           │
│  - Interfaces, Models                    │
└──────────────────────────────────────────┘
```

### Layer Responsibilities

| Layer | Purpose | Dependencies |
|-------|---------|--------------|
| **Core** | Shared contracts | None |
| **Domain** | Business logic | Core |
| **Infrastructure** | External services | Core, Domain |
| **Application** | Use cases | Core, Domain |
| **Presentation** | UI | Core, Application |
| **Testing** | Quality assurance | All layers (for testing) |

---

## Features

### 1. Version Increase Tool
**Status**: ✅ Partially Completed (US-1.1, US-1.2, US-1.2.1, US-1.3 Done)
**User Stories**:
- ✅ US-1.1: Load và Hiển thị Danh Sách Projects
- ✅ US-1.2: Tăng Version Number Tự Động
- ✅ US-1.2.1: Search và Save Last Directory (Enhancement)
- ✅ US-1.3: Git Commit và Push Changes - Completed 2026-02-08
- 📋 US-1.4: Logging và Error Handling
- ⏳ US-1.5: Advanced Settings và Configuration

**Documentation**: [fea-version-increase-structure.md](./fea-version-increase-structure.md)

### 2. Dashboard Chart Layout
**Status**: ✅ Completed
**User Stories**: US-6.1
**Documentation**: [fea-dashboard-chart-structure.md](./fea-dashboard-chart-structure.md)

**Key Components**:
- `DashboardBlockHost` & `DashboardViewRegistry` (UI Injection)
- `DashboardBlock` (Dynamic data domain object)
- `IDashboardBlockView` & `DashboardBlockAttribute`

### 6. Document Management Tracker
**Status**: ✅ Completed
**User Stories**: US-7.1
**Documentation**: [fea-document-management-structure.md](./fea-document-management-structure.md)

### 7. Annual & Monthly Calendar
**Status**: ✅ Completed
**User Stories**: US-8.4, US-8.5, US-8.6, US-8.7
**Documentation**: [fea-calendar-structure.md](./fea-calendar-structure.md)

**Key Components**:
- `AnnualCalendarViewModel` & `MonthlyCalendarViewModel`
- `ActivityHeatmapViewModel` (Event-centric dot grid)
- `CalendarEventModel` with `Phases` (Multi-phase tracking)
- **Tagging & Hierarchical Filtering** (US-9.2)
- **Monthly Calendar CRUD & Color Picker** (US-9.3)
- Gantt-style timeline rendering in XAML
- Hamburger Navigation integration

**Key Components Overview**:
### 9. Core Architecture Refactor (Memento)
**Status**: ✅ Completed
**User Stories**: US-9.1
**Documentation**: [fea-calendar-structure.md](./fea-calendar-structure.md)

**Key Components**:
- MementoModel (Recursive Hierarchy)
- ICalendarService (Refactored)
- **Tagging System** (TagModel, Repositories)

## Shared Components

### Core Layer
- `MementoModel` - Unified hierarchical note/event model.
- `Result<T>` - Result pattern for error handling.
- `ICalendarService` - Central service for time-based data.

#### Domain Layer
- `ProjectFile` - Entity with filtering business rules
- `VersionInfo` - Value object with increment logic
- `GitCommitInfo` - Value object for commit information (US-1.3)
- `ProjectStatus` - Enum for processing status
- `AppSettings` - Value object with validation (US-1.2.1)

#### Infrastructure Layer
- `ProjectScanner` - Scans file system, filters ETL projects
- `ProjectFileService` - XML manipulation (XDocument)
- `VersionService` - Version parsing and formatting
- `SettingsService` - JSON file persistence (US-1.2.1)
- `GitService` - Git operations using LibGit2Sharp (US-1.3)

#### Application Layer
- `ScanProjectsCommand` - Orchestrates scan workflow
- `UpdateVersionsCommand` - Orchestrates update workflow
- `CommitChangesCommand` - Orchestrates Git commit & push workflow (US-1.3)
- `LoadSettingsCommand`, `SaveSettingsCommand` - Settings commands (US-1.2.1)
- `ProjectFileDto`, `VersionUpdateResultDto`, `AppSettingsDto` - Data transfer objects

#### Presentation Layer
- `VersionIncreaseView.xaml` - Modern dark theme UI with search + nav button (US-1.2.1, US-5.1)
- `VersionIncreaseViewModel` - MVVM with search & settings & navigation menu (US-1.2.1, US-1.3, US-5.1)
- `ProjectFileViewModel` - Individual project representation
- `ProjectFilterHelper` - Search/filter logic (US-1.2.1)

**Features Implemented**:
- ✅ Scan .csproj files recursively
- ✅ Filter: EndsWith "ETL", NOT StartsWith "Share"
- ✅ Read current version from XML
- ✅ Display in DataGrid with checkboxes
- ✅ Select All/Deselect All (works with filtered list - US-1.2.1)
- ✅ **Search/filter projects** in real-time (US-1.2.1)
- ✅ **Save last directory** between sessions (US-1.2.1)
- ✅ Version increment logic: same day +1, new day reset to 1
- ✅ Batch update multiple projects
- ✅ Progress tracking with progress bar
- ✅ Update both AssemblyVersion and FileVersion tags
- ✅ Preserve XML formatting

---

### 4. Navigation Menu System _(Shared — US-5.1)_
**Status**: ✅ Completed — 2026-04-07  
**User Stories**: US-5.1  
**Documentation**: [fea-navigation-menu-structure.md](./fea-navigation-menu-structure.md)

**Purpose**: Cross-cutting UI/UX enhancement — hamburger button + hover dropdown cho phép chuyển đổi giữa các tool forms mà không cần quay về main menu.

**Key Components**:

#### Core Layer
- `INavigationService` - Contract: register, list, navigate, current tool, ToolNavigated event
- `ToolDefinition` - Tool metadata model (Id, Name, Description, IconPath)
- `ToolNavigatedEventArgs` - EventArgs cho navigation event

#### Application Layer
- `NavigationService` - Thread-safe singleton; Dictionary-backed tool registry

#### Presentation Layer
- `NavigationMenuButton` (UserControl) - Hamburger button + Popup với fade animation (180ms open / 160ms close), 300ms hover-leave delay
- `ToolMenuItem` - ObservableObject cho dropdown item (IsActive, NavigateCommand)
- `NavigationMenuStyles.xaml` - `NavButtonStyle`, `NavMenuItemStyle` (dark theme)
- `ToolIds` - Constants class tránh magic strings

**Design Highlights**:
- Hover-to-open, không cần click
- Active tool highlighted màu `#3ECF8E` (green accent) với dấu ✓
- Extensible: thêm tool mới chỉ cần 3 bước (register, handle event, build menu)

---

### 2. Build & Deploy Tool
**Status**: 📋 Planned  
**User Stories**: US-2.x (Not yet defined)  
**Documentation**: TBD

**Planned Features**:
- Build multiple projects in parallel
- Deploy to multiple environments
- Health check after deployment
- Rollback capability

---

### 3. Database Migration Tool
**Status**: 📋 Planned  
**User Stories**: US-3.x (Not yet defined)  
**Documentation**: TBD

**Planned Features**:
- Run EF migrations across databases
- Migration rollback
- Database backup before migration
- Migration history tracking

---

## Shared Components

### Core Layer

#### INavigationService (US-5.1)
```csharp
public interface INavigationService
{
    void RegisterTool(ToolDefinition tool);
    IEnumerable<ToolDefinition> GetAllTools();
    void NavigateTo(string toolId);
    ToolDefinition? GetCurrentTool();
    event EventHandler<ToolNavigatedEventArgs>? ToolNavigated;
}
```
**Used by**: `App.xaml.cs` (registration), `VersionIncreaseViewModel` (build menu), `MainWindow` (subscribe event)  
**Implemented by**: `NavigationService` (Application layer)  
**Purpose**: Decoupled navigation giữa các tool forms; MainWindow lắng nghe event và swap view

---

#### ISettingsService (US-1.2.1)
```csharp
public interface ISettingsService
{
    Task<Result<AppSettings>> LoadAsync();
    Task<Result> SaveAsync(AppSettings settings);
}
```
**Used by**: LoadSettingsCommand, SaveSettingsCommand  
**Implemented by**: SettingsService  
**Purpose**: Persist application settings to JSON file

#### IProjectScanner
```csharp
public interface IProjectScanner
{
    Task<Result<IEnumerable<string>>> ScanProjectsAsync(
        string basePath,
        string filePattern = "*ETL.csproj",
        string excludePattern = "Share*");
}
```
**Used by**: ScanProjectsCommand  
**Implemented by**: ProjectScanner

#### IProjectFileService
```csharp
public interface IProjectFileService
{
    Task<Result<string>> ReadVersionAsync(string filePath);
    Task<Result> UpdateVersionAsync(string filePath, string newVersion);
}
```
**Used by**: ScanProjectsCommand, UpdateVersionsCommand  
**Implemented by**: ProjectFileService

#### IVersionService
```csharp
public interface IVersionService
{
    Result<VersionInfo> ParseVersion(string versionString);
    Result<string> FormatVersion(VersionInfo version);
    Result<VersionInfo> IncrementVersion(VersionInfo current, DateTime targetDate);
}
```
**Used by**: UpdateVersionsCommand  
**Implemented by**: VersionService

#### Result<T>
**Purpose**: Result pattern for clean error handling  
**Usage**: All service methods return Result or Result<T>  
**Benefits**:
- Explicit error handling
- No exception overhead
- Forces developers to handle errors

```csharp
// Example usage
var result = await service.ScanProjectsAsync(path);
if (!result.IsSuccess)
{
    ShowError(result.Error);
    return;
}
var projects = result.Value;
```

---

## Testing Layer

### Overview

**Status**: ✅ Completed (US-2.1 Done)  
**Documentation**: [testing-structure.md](./testing-structure.md)

The Testing Layer provides comprehensive test coverage across all layers of the application, following the Testing Pyramid pattern with unit tests, integration tests, and end-to-end tests.

### Test Projects Structure

| Test Project | Purpose | Test Type | Coverage Goal |
|--------------|---------|-----------|---------------|
| **Lifes.Domain.Tests** | Test pure business logic | Unit Tests | >= 80% |
| **Lifes.Application.Tests** | Test use cases and workflows | Unit Tests (with mocks) | >= 70% |
| **Lifes.Infrastructure.Tests** | Test infrastructure implementations | Integration Tests | >= 60% |
| **Lifes.Presentation.WPF.Tests** | Test ViewModels and UI logic | Unit Tests (with mocks) | >= 50% |
| **Lifes.Integration.Tests** | Test end-to-end workflows | E2E Tests | Key scenarios |

### Testing Technology Stack

| Component | Technology | Version | Purpose |
|-----------|-----------|---------|---------|
| **Framework** | xUnit | 2.6.6 | Testing framework |
| **Mocking** | Moq | 4.20.70 | Mock dependencies |
| **Assertions** | FluentAssertions | 6.12.0 | Readable assertions |
| **Coverage** | Coverlet | 6.0.0 | Code coverage |

### Testing Pyramid

```
         /\
        /E2E\           E2E Tests (10%)
       /------\         - Full workflows
      /        \        - Real dependencies
     /Integration\      
    /------------\      Integration Tests (30%)
   /              \     - Infrastructure layer
  /  Infrastructure\    - File system, XML
 /------------------\   
                        Unit Tests (60%)
                        - Domain & Application
                        - Fast, isolated
```

### Key Test Utilities

#### AppMockFactory.cs
**Location**: `tests/Lifes.Application.Tests/TestUtilities/`  
**Purpose**: Centralized mock creation for common interfaces
- Mock for `IProjectScanner`
- Mock for `IProjectFileService`
- Mock for `IVersionService`
- Mock for `ISettingsService`

#### TestFixtures.cs
**Location**: `tests/Lifes.Domain.Tests/TestUtilities/`  
**Purpose**: Test data generation
- Create test `ProjectFile` instances
- Create test `VersionInfo` instances
- Create test project lists

#### FileSystemTestHelper.cs
**Location**: `tests/Lifes.Infrastructure.Tests/TestUtilities/`  
**Purpose**: File system test fixtures and cleanup
- Create temporary test directories
- Create test .csproj files
- Automatic cleanup after tests

### Sample Tests Coverage

**Domain Tests (US-2.1)**:
- `VersionInfoTests.cs` - Version parsing and increment logic
- `ProjectFileTests.cs` - Entity validation and business rules

**Application Tests (US-2.1)**:
- `ScanProjectsCommandTests.cs` - Scan workflow with mocked dependencies
- `UpdateVersionsCommandTests.cs` - Update workflow orchestration

**Infrastructure Tests (US-2.1)**:
- `ProjectScannerTests.cs` - File scanning with test fixtures
- `ProjectFileServiceTests.cs` - XML read/write operations
- `VersionServiceTests.cs` - Version service implementation

**Presentation Tests (US-2.1)**:
- `VersionIncreaseViewModelTests.cs` - ViewModel commands and properties

### Running Tests

**Visual Studio**:
- View → Test Explorer → Run All Tests

**Command Line**:
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage-report -reporttypes:Html
```

### Testing Best Practices

1. **Naming Convention**: `MethodName_Scenario_ExpectedResult`
2. **AAA Pattern**: Arrange, Act, Assert
3. **Test Independence**: No test interdependencies
4. **Fast Unit Tests**: < 100ms execution time
5. **Mock External Dependencies**: Keep tests isolated
6. **Test Edge Cases**: Not just happy path

---

## Technology Stack

| Component | Technology | Version |
|-----------|-----------|---------|
| Framework | .NET | 6.0 |
| UI | WPF (XAML) | 6.0 |
| MVVM | CommunityToolkit.Mvvm | 8.3.2 |
| DI | Microsoft.Extensions.DependencyInjection | 8.0.0 |
| Logging | Serilog | 4.3.0 |
| XML Processing | System.Xml.Linq | Built-in |

---

## Design Patterns Used

### 1. Clean Architecture
- Dependencies flow inward
- Domain layer is pure (no external dependencies)
- Infrastructure implements Core interfaces

### 2. MVVM (Model-View-ViewModel)
- Views (XAML) → ViewModels → Application Commands
- Data binding with ObservableCollection
- Commands with CommunityToolkit.Mvvm

### 3. Result Pattern
- Replace exceptions with Result<T>
- Explicit success/failure handling
- Better performance and code clarity

### 4. Command Pattern
- Application layer uses Command objects
- Encapsulate use cases
- Easy to test with mocks

### 5. Dependency Injection
- Constructor injection throughout
- Configured in App.xaml.cs
- Easy to swap implementations for testing

---

## SOLID Principles

### Single Responsibility
- Each class has one reason to change
- `ProjectScanner` only scans files
- `VersionService` only handles version operations

### Open/Closed
- Open for extension (add new features)
- Closed for modification (existing features don't change)
- Feature-based organization supports this

### Liskov Substitution
- Implementations can replace interfaces
- All IProjectScanner implementations behave consistently

### Interface Segregation
- Small, focused interfaces
- IProjectScanner, IProjectFileService, IVersionService
- Not one large IProjectService

### Dependency Inversion
- Depend on abstractions (interfaces), not concretions
- Infrastructure implements Core interfaces
- Application depends on Core interfaces

---

**Document Version**: 1.4.0  
**Last Updated**: 2026-04-18  
**Status**: ✅ Active (85% Complete - US-1.1, US-1.2, US-1.2.1, US-1.3, US-2.1, US-5.1, US-7.1, US-8.4, US-8.5, US-9.1, US-9.2, US-9.3 Done)
