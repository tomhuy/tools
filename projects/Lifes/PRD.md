# Product Requirements Document (PRD)
## ETL Deployment Tools Suite

---

## 📋 Document Information

| Field | Value |
|-------|-------|
| **Product Name** | ETL Deployment Tools Suite |
| **Version** | 1.0.0 |
| **Document Status** | Draft - Initial Version |
| **Last Updated** | 2026-02-06 |
| **Author** | Development Team |
| **Stakeholders** | DevOps Team, Developers, Release Managers |

---

## 🎯 Executive Summary

**ETL Deployment Tools Suite** là một bộ công cụ desktop được xây dựng để hỗ trợ quy trình deploy các dự án ETL một cách tự động, nhanh chóng và đáng tin cậy. Ứng dụng tập hợp nhiều công cụ khác nhau, mỗi công cụ có một màn hình chính riêng biệt, giúp developer và DevOps team thực hiện các tác vụ deployment phổ biến.

**Công nghệ:**
- **UI Pattern:** MVVM (Model-View-ViewModel)
- **Framework:** WPF (XAML) với .NET 6.0
- **Design:** Modern dark theme với Fluent Design

### Mục tiêu chính:
- ✅ Tự động hóa các tác vụ deployment thủ công
- ✅ Tăng version số cho các project ETL một cách nhất quán
- ✅ Commit và push changes lên Git repository
- ✅ Cung cấp logging real-time cho tất cả operations
- ✅ Dễ dàng mở rộng thêm công cụ mới trong tương lai

### Danh sách Tools:
1. **Version Increase Tool** - Tự động tăng version số cho các .csproj files
2. _(More tools to be added in future versions)_

---

## 🏢 Business Context

### Problem Statement

Trong quá trình deploy các dự án ETL, việc thực hiện thủ công:
- ⚠️ Tăng version số cho nhiều projects rất tốn thời gian
- ⚠️ Dễ quên hoặc sai sót khi update version
- ⚠️ Version format không nhất quán giữa các projects
- ⚠️ Phải mở từng file .csproj để edit thủ công
- ⚠️ Khó theo dõi các file đã được modified

### Solution

ETL Deployment Tools Suite cung cấp:
- ✅ Giao diện trực quan với checkbox list để chọn projects
- ✅ Tự động scan và filter các .csproj files phù hợp
- ✅ Tăng version theo chuẩn yyyy.M.d.{number} tự động
- ✅ Logic thông minh để tăng số thứ tự trong ngày
- ✅ Logging chi tiết cho mọi thao tác
- ✅ Kiến trúc mở rộng dễ dàng cho các tools mới

### Success Metrics

| Metric | Target | Current |
|--------|--------|---------|
| Time saved per deployment | > 80% | TBD |
| Error rate in versioning | < 1% | TBD |
| User satisfaction | > 4.5/5 rating | TBD |
| Tool adoption rate | > 90% of team | TBD |

---

## 👥 Target Users

### Primary Users

1. **Developers**
   - Update version trước khi commit code
   - Deploy ETL projects lên production
   - Maintain version consistency

2. **DevOps Engineers**
   - Automate deployment workflows
   - Manage release versions
   - Track deployment history

3. **Release Managers**
   - Coordinate release schedules
   - Verify version numbers
   - Approve deployments

### Secondary Users

1. **Team Leads**
   - Monitor deployment activities
   - Review version changes
   - Enforce versioning standards

---

## 🎨 User Stories

### Tool 1: Version Increase Tool

**US-001: Load và Hiển thị Danh Sách Projects**
- **As a** Developer
- **I want to** xem danh sách các .csproj files cần tăng version
- **So that** tôi có thể chọn projects nào cần update

**Acceptance Criteria:**
- ✅ Scan tất cả .csproj files trong solution/workspace
- ✅ Filter theo điều kiện:
  - File name phải kết thúc bằng "ETL" (vd: `MyProject.ETL.csproj`)
  - File extension phải là `.csproj`
  - File name không được bắt đầu bằng "Share" (case-insensitive)
- ✅ Hiển thị danh sách files trong ListView/DataGrid với checkbox
- ✅ Mỗi item hiển thị:
  - Checkbox để select
  - Project name
  - Current version (đọc từ AssemblyVersion)
  - File path (relative)
- ✅ Có button "Select All" và "Deselect All"
- ✅ Hiển thị số lượng files tìm thấy và số lượng đã chọn

---

**US-002: Tăng Version Number Tự Động**
- **As a** Developer
- **I want to** tăng version number cho các projects đã chọn
- **So that** version được update nhất quán theo chuẩn công ty

**Acceptance Criteria:**

**Version Format:** `yyyy.M.d.{number}`
- `yyyy`: Năm hiện tại (vd: 2026)
- `M`: Tháng hiện tại không có leading zero (vd: 2 cho February)
- `d`: Ngày hiện tại không có leading zero (vd: 3)
- `{number}`: Số thứ tự tăng dần trong ngày

**Logic Tăng Version:**
- ✅ Đọc ngày hiện tại từ system clock
- ✅ Parse version hiện tại từ `<AssemblyVersion>` và `<FileVersion>` tags
- ✅ Nếu version hiện tại là ngày hôm nay (vd: `2026.2.3.1`):
  - Tăng `{number}` lên 1 → `2026.2.3.2`
- ✅ Nếu version hiện tại không phải ngày hôm nay (vd: `2026.2.2.5`):
  - Reset về `2026.2.3.1` (số đầu tiên của ngày mới)
- ✅ Update cả 2 tags: `<AssemblyVersion>` và `<FileVersion>` với giá trị giống nhau
- ✅ Preserve XML formatting và indentation của file

**Example:**
```xml
<!-- Before (today is 2026-02-03) -->
<AssemblyVersion>2026.2.2.5</AssemblyVersion>
<FileVersion>2026.2.2.5</FileVersion>

<!-- After -->
<AssemblyVersion>2026.2.3.1</AssemblyVersion>
<FileVersion>2026.2.3.1</FileVersion>

<!-- If run again on same day -->
<AssemblyVersion>2026.2.3.2</AssemblyVersion>
<FileVersion>2026.2.3.2</FileVersion>
```

**UI Behavior:**
- ✅ Button "Increase Version" chỉ enabled khi có ít nhất 1 project được chọn
- ✅ Hiển thị confirmation dialog trước khi modify files
- ✅ Hiển thị progress bar khi đang process
- ✅ Log mỗi file được update với version mới
- ✅ Hiển thị summary khi hoàn thành: "Updated 5 of 5 projects successfully"

---

**US-003: Git Commit Changes**
- **As a** Developer
- **I want to** commit các thay đổi version vào Git
- **So that** changes được tracked trong version control

**Acceptance Criteria:**
- ✅ Button "Commit & Push" available sau khi increase version thành công
- ✅ Auto-generate commit message:
  ```
  chore: increase version to yyyy.M.d.{number} for X projects
  
  - ProjectA.ETL.csproj: 2026.2.2.5 → 2026.2.3.1
  - ProjectB.ETL.csproj: 2026.2.2.3 → 2026.2.3.1
  - ProjectC.ETL.csproj: 2026.2.3.1 → 2026.2.3.2
  ```
- ✅ User có thể edit commit message trước khi commit
- ✅ Thực hiện Git commands:
  1. `git add <modified-files>`
  2. `git commit -m "<message>"`
  3. `git push origin <current-branch>` (optional)
- ✅ Hiển thị Git output trong log window
- ✅ Handle errors (no git, no changes, conflicts, etc.)
- ✅ Checkbox "Push to remote" (default: checked)

---

**US-004: Logging và Error Handling**
- **As a** Developer
- **I want to** xem chi tiết logs của tất cả operations
- **So that** tôi có thể debug khi có vấn đề

**Acceptance Criteria:**
- ✅ Log window hiển thị real-time logs với 3 cột: Time, Level, Message
- ✅ Logs có màu sắc theo level:
  - Info: Blue
  - Warning: Orange
  - Error: Red
  - Success: Green
- ✅ Log các events:
  - Scanning files
  - Found X candidate files
  - Reading current version from file
  - Updating version
  - File saved successfully
  - Git commands executed
  - Errors và warnings
- ✅ Button "Copy Logs" để copy selected logs
- ✅ Button "Clear Logs" để xóa logs
- ✅ Button "Export Logs" để save logs ra file .txt

---

**US-005: Settings và Configuration**
- **As a** Developer
- **I want to** configure tool settings
- **So that** tool hoạt động phù hợp với workflow của tôi

**Acceptance Criteria:**
- ⏳ Settings dialog/panel bao gồm:
  - Base directory to scan (default: current solution directory)
  - File filter pattern (default: "*ETL.csproj")
  - Exclude pattern (default: "Share*")
  - Git auto-push (default: true)
  - Log level (default: Information)
- ⏳ Save settings to JSON file: `appsettings.user.json`
- ⏳ Load settings on startup

**Status:** Future Enhancement

---

## 🏗️ Technical Architecture

### Technology Stack

| Layer | Technology | Version | Purpose |
|-------|-----------|---------|---------|
| **Framework** | .NET | 6.0 | Core platform |
| **UI** | WPF (XAML) | 6.0 | User interface |
| **MVVM** | CommunityToolkit.Mvvm | 8.3.2 | MVVM implementation |
| **UI Framework** | ModernWPF (Fluent Design) | 0.9.6 | Modern UI styling |
| **DI** | Microsoft.Extensions.DependencyInjection | 7.0.0 | Dependency injection |
| **Hosting** | Microsoft.Extensions.Hosting | 7.0.0 | Application lifetime |
| **Logging** | Serilog | 4.3.0 | Logging framework |
| **Configuration** | Microsoft.Extensions.Configuration | 7.0.0 | Settings management |
| **XML Processing** | System.Xml.Linq | Built-in | .csproj manipulation |
| **Git** | LibGit2Sharp | 0.27.2 | Git operations |

### Architecture Overview

#### Clean Architecture + Feature-based Organization

**Philosophy:**

Ứng dụng được xây dựng theo **Clean Architecture** (Uncle Bob) kết hợp với **Feature-based folder organization** thay vì traditional layered approach.

**Why Clean Architecture?**
- ✅ **Independent of Frameworks**: Business logic không phụ thuộc vào WPF, EF, hay library nào
- ✅ **Testable**: Core business logic có thể test mà không cần UI, database, hay external services
- ✅ **Independent of UI**: Có thể swap WPF → Console → Web mà không thay đổi business logic
- ✅ **Independent of Database**: Có thể swap file system → SQL → NoSQL
- ✅ **Independent of External Services**: Business logic không biết về Git, HTTP, hay service nào

---

### 5. Dashboard Chart Layout (US-6.1)
**Status**: ✅ Completed (US-6.1 Done)
**User Stories**: US-6.1
**Documentation**: [fea-dashboard-chart-structure.md](./docs/structures/fea-dashboard-chart-structure.md)

**Purpose**: Hỗ trợ layout dashboard và injection UI đa hình thông qua DashboardBlockHost cho các cell của lưới trực quan.

**Key Components**:
- `IDashboardBlockView` & `DashboardBlockAttribute`: Định nghĩa Custom View cho Dashboard.
- `DashboardBlockHost`: Custom ContentControl tự động nạp View tương ứng dựa theo `BlockType` của data object.
- `DashboardViewRegistry`: Tự động quét Assembly lúc Application Startup để map UI theo loại data.
- `DashboardChartViewModel` & `DashboardBlock` entity: Liên kết dữ liệu UI động và chứa cấu trúc `Data` linh hoạt.

---

**Why Feature-based Organization?**
- ✅ **High Cohesion**: Code liên quan đến một feature nằm gần nhau
- ✅ **Easy Navigation**: Tìm code nhanh hơn - nhìn feature thay vì tìm trong nhiều layer folders
- ✅ **Parallel Development**: Nhiều dev làm features khác nhau không conflict
- ✅ **Easy to Delete**: Xóa feature = xóa folder, không cần tìm trong nhiều nơi
- ✅ **Scalability**: Thêm tool mới không làm phình to folders hiện tại

**Traditional Layered vs Feature-based:**

```
❌ Traditional (Layer-first):           ✅ Feature-based (Feature-first):

Services/                               Features/
├── ProjectScanner.cs                   ├── VersionIncrease/
├── VersionService.cs                   │   ├── Services/
├── GitService.cs                       │   │   ├── ProjectScanner.cs
├── BuildService.cs                     │   │   └── VersionService.cs
├── DeployService.cs                    │   └── Git/
└── MigrationService.cs                 │       └── GitService.cs
    (hard to find related code)         │
                                        ├── BuildDeploy/
Models/                                 │   └── Services/
├── ProjectFile.cs                      │       ├── BuildService.cs
├── VersionInfo.cs                      │       └── DeployService.cs
├── BuildConfig.cs                      │
├── DeploymentResult.cs                 └── DatabaseMigration/
└── MigrationScript.cs                      └── Services/
    (models mixed together)                     └── MigrationService.cs
                                            (easy to navigate by feature)
```

**4 Layers in Clean Architecture:**

| Layer | Purpose | Dependencies | Contains |
|-------|---------|--------------|----------|
| **Domain** | Core business logic | NONE (pure) | Entities, Value Objects, Business Rules |
| **Application** | Use cases | Domain, Core | Commands, Queries, DTOs, Workflows |
| **Infrastructure** | External concerns | Domain, Application, Core | File I/O, Git, Database, APIs |
| **Presentation** | User interface | Application, Core | Views, ViewModels, UI Logic |

**Core (Shared across all layers):**
- Interfaces và contracts
- DTOs và Result types
- Extension methods
- Constants
- **NO implementations**

---

### Architectural Patterns

#### 1. MVVM (Model-View-ViewModel) ⭐

**Separation of Concerns:**
- **View (XAML)**: Pure UI, no business logic
  - `MainWindow.xaml`: Main tool selector
  - `VersionIncreaseView.xaml`: Version increase tool UI
  - `Views/` folder cho các tools khác
  
- **ViewModel**: UI logic và state management
  - `MainViewModel.cs`: Navigation và tool selection
  - `VersionIncreaseViewModel.cs`: Version tool business logic
  - `ViewModels/` folder cho các tools khác
  - Sử dụng `ObservableObject` base class
  - Properties với `[ObservableProperty]` attribute
  - Commands với `[RelayCommand]` attribute
  
- **Model**: Business entities và data
  - `ProjectFile.cs`: Represents a .csproj file
  - `VersionInfo.cs`: Version parsing và logic
  - `Models/` folder

**Benefits:**
- Clean separation of UI and business logic
- Easy unit testing (test ViewModels without UI)
- Two-way data binding reduces boilerplate code
- Commands handle user actions declaratively

#### 2. Service Pattern with Clean Architecture

**Service Interfaces (in Core):**

```csharp
// Lifes.Core/Interfaces/IProjectScanner.cs
public interface IProjectScanner
{
    Task<Result<IEnumerable<ProjectFile>>> ScanProjectsAsync(string basePath);
}

// Lifes.Core/Interfaces/IVersionService.cs
public interface IVersionService
{
    Result<VersionInfo> ParseVersion(string versionString);
    Result<string> IncrementVersion(VersionInfo current, DateTime targetDate);
}

// Lifes.Core/Interfaces/IProjectFileService.cs
public interface IProjectFileService
{
    Task<Result<string>> ReadVersionAsync(string filePath);
    Task<Result> UpdateVersionAsync(string filePath, string newVersion);
}

// Lifes.Core/Interfaces/IGitService.cs
public interface IGitService
{
    Task<Result<bool>> HasChangesAsync();
    Task<Result> CommitAsync(string message, IEnumerable<string> files);
    Task<Result> PushAsync(string remoteName, string branchName);
}

// Lifes.Core/Models/Result.cs - Result Pattern
public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    
    public static Result Success() => new Result(true, string.Empty);
    public static Result Failure(string error) => new Result(false, error);
}

public class Result<T> : Result
{
    public T Value { get; }
    
    public static Result<T> Success(T value) => new Result<T>(true, value, string.Empty);
    public static new Result<T> Failure(string error) => new Result<T>(false, default, error);
}
```

**Service Implementations (in Infrastructure):**

```csharp
// Lifes.Infrastructure/Features/VersionIncrease/Services/ProjectScanner.cs
public class ProjectScanner : IProjectScanner
{
    private readonly ILogger<ProjectScanner> _logger;
    
    public async Task<Result<IEnumerable<ProjectFile>>> ScanProjectsAsync(string basePath)
    {
        try
        {
            // Implementation...
            return Result<IEnumerable<ProjectFile>>.Success(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to scan projects");
            return Result<IEnumerable<ProjectFile>>.Failure(ex.Message);
        }
    }
}
```

**Service Registration (in Presentation/App.xaml.cs):**

```csharp
// Clean Architecture - Register from outer to inner layers
private void ConfigureServices(IServiceCollection services)
{
    // Infrastructure Services (implements Core interfaces)
    services.AddSingleton<IProjectScanner, ProjectScanner>();
    services.AddSingleton<IVersionService, VersionService>();
    services.AddSingleton<IProjectFileService, ProjectFileService>();
    services.AddSingleton<IGitService, GitService>();
    
    // Application Services (Use Cases)
    services.AddTransient<ScanProjectsCommand>();
    services.AddTransient<UpdateVersionsCommand>();
    services.AddTransient<CommitChangesCommand>();
    
    // Presentation ViewModels
    services.AddTransient<MainViewModel>();
    services.AddTransient<VersionIncreaseViewModel>();
    
    // Logging
    services.AddLogging(builder =>
    {
        builder.AddSerilog();
    });
}
```

#### 3. Clean Architecture with Feature-based Organization

**Architecture Principles:**
- **Clean Architecture**: Dependencies flow inward (Presentation → Application → Domain → Infrastructure)
- **Feature-based**: Code organized by feature, not by technical layer
- **Core for shared code**: Common utilities, models, interfaces in Core project

**Project Structure:**

```
Lifes.sln
│
├── src/
│   ├── Lifes.Presentation.WPF/        # 🎨 Presentation Layer (WPF)
│   │   ├── Features/                      # Feature-based organization
│   │   │   ├── VersionIncrease/           # Version Increase Tool
│   │   │   │   ├── VersionIncreaseView.xaml
│   │   │   │   ├── VersionIncreaseViewModel.cs
│   │   │   │   └── Models/                # Feature-specific ViewModels
│   │   │   │       └── ProjectFileViewModel.cs
│   │   │   │
│   │   │   ├── BuildDeploy/               # Future: Build & Deploy Tool
│   │   │   │   └── (to be added)
│   │   │   │
│   │   │   └── DatabaseMigration/         # Future: DB Migration Tool
│   │   │       └── (to be added)
│   │   │
│   │   ├── Shared/                        # Shared UI components
│   │   │   ├── MainWindow.xaml
│   │   │   └── MainViewModel.cs
│   │   │
│   │   ├── Styles/                        # Design system
│   │   │   ├── Colors.xaml
│   │   │   ├── Spacing.xaml
│   │   │   ├── Typography.xaml
│   │   │   └── Controls.xaml
│   │   │
│   │   ├── App.xaml
│   │   └── App.xaml.cs                    # DI configuration
│   │
│   ├── Lifes.Application/              # 💼 Application Layer
│   │   ├── Features/                      # Use cases per feature
│   │   │   ├── VersionIncrease/
│   │   │   │   ├── Commands/
│   │   │   │   │   ├── ScanProjectsCommand.cs
│   │   │   │   │   ├── UpdateVersionsCommand.cs
│   │   │   │   │   └── CommitChangesCommand.cs
│   │   │   │   ├── Queries/
│   │   │   │   │   └── GetProjectVersionQuery.cs
│   │   │   │   └── DTOs/
│   │   │   │       ├── ProjectFileDto.cs
│   │   │   │       └── VersionUpdateResultDto.cs
│   │   │   │
│   │   │   ├── BuildDeploy/               # Future
│   │   │   └── DatabaseMigration/         # Future
│   │   │
│   │   └── Common/                        # Shared application logic
│   │       ├── Interfaces/
│   │       └── Behaviors/
│   │
│   ├── Lifes.Domain/                   # 🎯 Domain Layer (Core Business Logic)
│   │   ├── Features/                      # Domain models per feature
│   │   │   ├── VersionIncrease/
│   │   │   │   ├── Entities/
│   │   │   │   │   ├── ProjectFile.cs
│   │   │   │   │   └── VersionInfo.cs
│   │   │   │   ├── ValueObjects/
│   │   │   │   │   └── VersionNumber.cs
│   │   │   │   └── Enums/
│   │   │   │       └── VersionUpdateStatus.cs
│   │   │   │
│   │   │   ├── BuildDeploy/               # Future
│   │   │   └── DatabaseMigration/         # Future
│   │   │
│   │   └── Common/                        # Shared domain concepts
│   │       ├── Interfaces/
│   │       └── Exceptions/
│   │
│   ├── Lifes.Infrastructure/           # 🔧 Infrastructure Layer
│   │   ├── Features/                      # Infrastructure per feature
│   │   │   ├── VersionIncrease/
│   │   │   │   ├── Services/
│   │   │   │   │   ├── ProjectScanner.cs
│   │   │   │   │   ├── VersionService.cs
│   │   │   │   │   └── ProjectFileService.cs
│   │   │   │   └── Git/
│   │   │   │       └── GitService.cs
│   │   │   │
│   │   │   ├── BuildDeploy/               # Future
│   │   │   └── DatabaseMigration/         # Future
│   │   │
│   │   └── Common/                        # Shared infrastructure
│   │       ├── Logging/
│   │       │   └── WpfListViewSink.cs
│   │       ├── FileSystem/
│   │       └── Configuration/
│   │
│   └── Lifes.Core/                     # 🔄 Shared Across All Layers
│       ├── Interfaces/                    # Shared contracts
│       │   ├── IProjectScanner.cs
│       │   ├── IVersionService.cs
│       │   ├── IProjectFileService.cs
│       │   └── IGitService.cs
│       │
│       ├── Models/                        # Shared DTOs
│       │   └── Result.cs                  # Result pattern
│       │
│       ├── Extensions/                    # Extension methods
│       │   ├── StringExtensions.cs
│       │   └── DateTimeExtensions.cs
│       │
│       └── Constants/                     # Shared constants
│           └── AppConstants.cs
│
└── tests/                                 # Test projects mirror src structure
    ├── Lifes.Application.Tests/
    ├── Lifes.Domain.Tests/
    └── Lifes.Infrastructure.Tests/
```

**Dependency Flow (Clean Architecture):**
```
┌─────────────────────────────────────────────────────┐
│  Presentation Layer (WPF)                           │
│  - Views, ViewModels, UI Logic                     │
│  - Depends on: Application, Core                   │
└─────────────────────┬───────────────────────────────┘
                      │ References
                      ▼
┌─────────────────────────────────────────────────────┐
│  Application Layer                                  │
│  - Use Cases (Commands, Queries)                   │
│  - Business workflows, DTOs                        │
│  - Depends on: Domain, Core                        │
└─────────────────────┬───────────────────────────────┘
                      │ References
                      ▼
┌─────────────────────────────────────────────────────┐
│  Domain Layer (Pure Business Logic)                │
│  - Entities, Value Objects, Domain Services        │
│  - NO external dependencies                        │
│  - Depends on: Core (interfaces only)              │
└─────────────────────────────────────────────────────┘
                      ▲
                      │ Implements
┌─────────────────────┴───────────────────────────────┐
│  Infrastructure Layer                               │
│  - External services (File, Git, Database)         │
│  - Implements interfaces from Core                 │
│  - Depends on: Domain, Core                        │
└─────────────────────────────────────────────────────┘
```

**Core Project (Shared):**
- Interfaces cho tất cả services
- Result pattern và common DTOs
- Extension methods và utilities
- Constants và configuration models
- **NO implementations** - chỉ contracts

**Benefits of This Architecture:**

| Benefit | Description |
|---------|-------------|
| **Feature Isolation** | Mỗi tool độc lập, dễ maintain và test |
| **Scalability** | Thêm tool mới không ảnh hưởng code cũ |
| **Testability** | Domain logic pure, dễ unit test |
| **Reusability** | Core chứa shared code tái sử dụng |
| **Clear Dependencies** | Dependencies flow inward, dễ hiểu |
| **Team Collaboration** | Nhiều dev làm các features khác nhau song song |

**Example: Version Increase Feature Organization:**

```
Features/VersionIncrease/
│
├── Presentation (WPF)
│   ├── VersionIncreaseView.xaml        # UI
│   ├── VersionIncreaseViewModel.cs     # VM with commands
│   └── Models/
│       └── ProjectFileViewModel.cs     # UI-specific model
│
├── Application
│   ├── Commands/
│   │   ├── ScanProjectsCommand.cs      # Use case: Scan projects
│   │   └── UpdateVersionsCommand.cs    # Use case: Update versions
│   └── DTOs/
│       └── ProjectFileDto.cs           # Data transfer object
│
├── Domain
│   ├── Entities/
│   │   ├── ProjectFile.cs              # Domain entity
│   │   └── VersionInfo.cs              # Domain entity
│   └── ValueObjects/
│       └── VersionNumber.cs            # Value object (yyyy.M.d.{n})
│
└── Infrastructure
    ├── Services/
    │   ├── ProjectScanner.cs           # Implements IProjectScanner
    │   └── VersionService.cs           # Implements IVersionService
    └── Git/
        └── GitService.cs               # Implements IGitService
```

#### 4. XAML Resource Dictionaries

**Design System:**
- `Colors.xaml`: 16-color dark theme palette
- `Spacing.xaml`: 11-value spacing system
- `Typography.xaml`: Font styles (Header, SubHeader, Body)
- `Controls.xaml`: Styled buttons, textboxes, checkboxes

**Benefits:**
- Centralized styling
- Easy theme changes
- Consistent UI across tools
- Reusable components

#### 5. Observer Pattern (Built-in WPF)

```csharp
// Auto-implemented by CommunityToolkit.Mvvm
[ObservableProperty]
private ObservableCollection<ProjectFileViewModel> _projectFiles;

// UI updates automatically when collection changes
ProjectFiles.Add(newProject);
```

### SOLID Principles in Clean Architecture

| Principle | Implementation | Example |
|-----------|---------------|---------|
| **Single Responsibility** | Mỗi class có một lý do duy nhất để thay đổi | `ProjectScanner`: chỉ scan files<br>`VersionService`: chỉ xử lý version logic<br>`GitService`: chỉ Git operations |
| **Open/Closed** | Open for extension, closed for modification | Thêm tool mới (BuildDeploy) không sửa VersionIncrease code<br>Feature-based organization cho phép extend dễ dàng |
| **Liskov Substitution** | Có thể thay thế implementations mà không phá vỡ code | Swap `ProjectScanner` → `MockProjectScanner` trong tests<br>Interface contracts đảm bảo behavior |
| **Interface Segregation** | Interfaces nhỏ, focused, không ép client implement không cần | `IProjectScanner`: chỉ scan<br>`IVersionService`: chỉ version<br>Không có `IProjectService` với nhiều methods |
| **Dependency Inversion** | Depend on abstractions (interfaces), not concretions | `ViewModel` → `IProjectScanner` (interface in Core)<br>`ProjectScanner` (implementation in Infrastructure)<br>Dependencies point inward toward Domain |

**Clean Architecture enforces SOLID:**
- ✅ **Domain** = Pure business logic, no dependencies
- ✅ **Application** = Use cases depend on Domain abstractions
- ✅ **Infrastructure** = Implements interfaces from Core
- ✅ **Presentation** = Depends on Application & Core interfaces

**Example Dependency Flow:**
```
VersionIncreaseViewModel (Presentation)
    ↓ depends on
IProjectScanner (Core interface)
    ↑ implemented by
ProjectScanner (Infrastructure)
```

Presentation và Infrastructure **không biết nhau**, chỉ biết Core interfaces.

---

## 📊 Data Models

### ProjectFile Model

Represents a .csproj file candidate for version update.

```csharp
public class ProjectFile
{
    public string FileName { get; set; }           // "MyProject.ETL.csproj"
    public string FullPath { get; set; }           // Absolute path
    public string RelativePath { get; set; }       // Relative to solution
    public VersionInfo CurrentVersion { get; set; } // Parsed version
    public bool IsSelected { get; set; }           // User selection
    public VersionUpdateStatus Status { get; set; } // Processing status
}

public enum VersionUpdateStatus
{
    Pending,
    Processing,
    Success,
    Failed,
    Skipped
}
```

### VersionInfo Model

Represents parsed version information.

```csharp
public class VersionInfo
{
    public int Year { get; set; }        // 2026
    public int Month { get; set; }       // 2 (February)
    public int Day { get; set; }         // 3
    public int Build { get; set; }       // 1, 2, 3...
    
    public DateTime Date => new DateTime(Year, Month, Day);
    
    public override string ToString() 
        => $"{Year}.{Month}.{Day}.{Build}";
    
    public static VersionInfo Parse(string version)
    {
        // Parse "2026.2.3.1" format
    }
    
    public VersionInfo Increment(DateTime targetDate)
    {
        // Logic to increment version
        if (Date == targetDate.Date)
            return new VersionInfo { ... Build = Build + 1 };
        else
            return new VersionInfo { ... Build = 1 };
    }
}
```

### GitCommitInfo Model

Represents Git commit information.

```csharp
public class GitCommitInfo
{
    public IEnumerable<string> ModifiedFiles { get; set; }
    public string CommitMessage { get; set; }
    public bool PushToRemote { get; set; }
    public string RemoteName { get; set; } = "origin";
    public string BranchName { get; set; }
}
```

### LogEntry Model

Represents a log entry for the UI.

```csharp
public class LogEntry
{
    public string Time { get; set; }      // "HH:mm:ss"
    public string Level { get; set; }     // "INFO", "WARNING", "ERROR", "SUCCESS"
    public string Message { get; set; }   // Log message
    public string LevelColor { get; set; } // Color based on level
}
```

---

## 🔄 Version Increase Flow

### Sequence Diagram

```
User          ViewModel              Services                    FileSystem       Git
  │                │                     │                            │           │
  │── Click Scan ──>│                     │                            │           │
  │                │─── ScanProjects ───>│                            │           │
  │                │                     │─── Read Directory ────────>│           │
  │                │                     │<─── File List ─────────────│           │
  │                │                     │─── Filter Files ────>      │           │
  │                │                     │─── Read Version ──────────>│           │
  │                │<─── Projects ───────│                            │           │
  │<── Display ────│                     │                            │           │
  │                │                     │                            │           │
  │── Select Items >│                     │                            │           │
  │── Click Update >│                     │                            │           │
  │                │─ IncrementVersion ─>│                            │           │
  │                │                     │─── Update File ───────────>│           │
  │                │                     │<─── Success ───────────────│           │
  │                │<─── Updated ────────│                            │           │
  │<── Display ────│                     │                            │           │
  │                │                     │                            │           │
  │── Click Commit >│                     │                            │           │
  │                │──── CommitAsync ───>│                            │           │
  │                │                     │──── git add ───────────────────────────>│
  │                │                     │──── git commit ────────────────────────>│
  │                │                     │──── git push ──────────────────────────>│
  │                │<──── Result ────────│                            │           │
  │<── Display ────│                     │                            │           │
```

### Version Increment Logic Flowchart

```
┌─────────────────────┐
│  Read Current       │
│  Version from       │
│  <AssemblyVersion>  │
└──────────┬──────────┘
           │
           v
┌─────────────────────┐
│  Parse Version:     │
│  yyyy.M.d.{number}  │
└──────────┬──────────┘
           │
           v
┌─────────────────────┐
│  Get Today's Date   │
│  from System Clock  │
└──────────┬──────────┘
           │
           v
      ┌────────────┐
      │ Is current │ No    ┌──────────────────┐
      │ version    │──────>│ Set new version  │
      │ today's    │       │ to yyyy.M.d.1    │
      │ date?      │       └────────┬─────────┘
      └────┬───────┘                │
           │ Yes                    │
           v                        │
┌─────────────────────┐             │
│ Increment {number}  │             │
│ by 1                │             │
└──────────┬──────────┘             │
           │                        │
           └────────────┬───────────┘
                        │
                        v
           ┌──────────────────────┐
           │ Update both tags:    │
           │ <AssemblyVersion>    │
           │ <FileVersion>        │
           └──────────┬───────────┘
                      │
                      v
           ┌──────────────────────┐
           │ Save .csproj file    │
           │ (preserve formatting)│
           └──────────────────────┘
```

---

## 🎨 UI/UX Specifications

### Design System

The application uses a **Supabase-inspired modern dark theme** with a comprehensive design system implemented in XAML Resource Dictionaries.

#### Color Palette (Colors.xaml)

| Color Name | Hex Code | RGB | Usage |
|------------|----------|-----|-------|
| **BackgroundDark** | #1A1A1A | (26, 26, 26) | Main background |
| **BackgroundMedium** | #1E1E1E | (30, 30, 30) | Cards, panels |
| **BackgroundLight** | #282828 | (40, 40, 40) | Input fields |
| **AccentGreen** | #3ECF8E | (62, 207, 142) | Primary actions |
| **AccentGreenHover** | #34C57E | (52, 197, 126) | Hover state |
| **AccentGreenDark** | #2ABB7A | (42, 187, 122) | Active state |
| **TextPrimary** | #F0F0F0 | (240, 240, 240) | Main text |
| **TextSecondary** | #A0A0A0 | (160, 160, 160) | Secondary text |
| **TextMuted** | #787878 | (120, 120, 120) | Muted text |
| **BorderDark** | #323232 | (50, 50, 50) | Borders |
| **BorderLight** | #464646 | (70, 70, 70) | Light borders |
| **Success** | #3ECF8E | (62, 207, 142) | Success states |
| **Error** | #F05252 | (240, 82, 82) | Error states |
| **Warning** | #FFB347 | (255, 179, 71) | Warning states |
| **Info** | #4299E1 | (66, 153, 225) | Info states |

#### Spacing System (Spacing.xaml)

| Constant | Value (px) | Usage |
|----------|-----------|--------|
| **ExtraSmall** | 8 | Minimal spacing |
| **Small** | 12 | Compact elements |
| **Medium** | 20 | Standard spacing |
| **Large** | 30 | Content padding |
| **ExtraLarge** | 40 | Major separators |
| **ComponentGap** | 15 | Between form elements |
| **SectionGap** | 25 | Between sections |
| **CardGap** | 20 | Between cards |
| **CardPadding** | 25 | Inside cards |
| **ContentPadding** | 30 | Main content area |
| **LabelInputGap** | 10 | Label to input |

#### Typography (Typography.xaml)

| Style | Font | Size | Weight | Usage |
|-------|------|------|--------|-------|
| **HeaderTextStyle** | Segoe UI | 24 | Bold | Page headers |
| **SubHeaderTextStyle** | Segoe UI | 16 | SemiBold | Section headers |
| **BodyTextStyle** | Segoe UI | 14 | Regular | Body text |
| **SmallTextStyle** | Segoe UI | 12 | Regular | Small text |

### Main Window Layout - Version Increase Tool

```
┌────────────────────────────────────────────────────────────────────────┐
│  ETL Deployment Tools                           1200x750    [_][□][X]  │
├────────────────────────────────────────────────────────────────────────┤
│  Version Increase Tool                                                 │
│  Automatically increment version numbers for ETL projects              │
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Project Selection                                                  ││
│  │                                                                    ││
│  │ Base Directory: F:\Sources\support_linkit_hub\coding              ││
│  │ [_________________________________________________]  [Browse...]  ││
│  │                                                                    ││
│  │ [Scan Projects]  [Select All]  [Deselect All]                     ││
│  │                                                                    ││
│  │ Found: 12 projects | Selected: 5 projects                         ││
│  └────────────────────────────────────────────────────────────────────┘│
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Projects                                                           ││
│  │                                                                    ││
│  │ ☑ Project Name              Current Version    Status     Path    ││
│  │ ─────────────────────────────────────────────────────────────────  ││
│  │ ☑ MyProject.ETL.csproj      2026.2.2.5         Ready      ...     ││
│  │ ☑ DataSync.ETL.csproj       2026.2.3.1         Ready      ...     ││
│  │ ☐ Transform.ETL.csproj      2026.1.15.3        Ready      ...     ││
│  │ ☑ LoadData.ETL.csproj       2026.2.1.2         Ready      ...     ││
│  │ ☑ ExportETL.csproj          2026.2.2.8         Ready      ...     ││
│  │ ☑ ImportETL.csproj          2026.2.3.1         Ready      ...     ││
│  │                             ↑ Scrollable, sortable               ││
│  │                                                                    ││
│  └────────────────────────────────────────────────────────────────────┘│
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Version Update                                                     ││
│  │                                                                    ││
│  │ New Version: 2026.2.3.{auto}                                       ││
│  │                                                                    ││
│  │ [Increase Version]  [Commit & Push]  [Cancel]                     ││
│  └────────────────────────────────────────────────────────────────────┘│
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────────┐│
│  │ Activity Log                                                       ││
│  │                                                                    ││
│  │ Time      Level       Message                                     ││
│  │ ────────  ─────────   ──────────────────────────────────────────  ││
│  │ 14:30:15  INFO        Scanning projects in directory...           ││
│  │ 14:30:15  SUCCESS     Found 12 candidate projects                 ││
│  │ 14:30:20  INFO        Increasing version for MyProject.ETL        ││
│  │ 14:30:20  SUCCESS     Updated: 2026.2.2.5 → 2026.2.3.1            ││
│  │ 14:30:21  INFO        Increasing version for DataSync.ETL         ││
│  │ 14:30:21  SUCCESS     Updated: 2026.2.3.1 → 2026.2.3.2            ││
│  │ 14:30:22  SUCCESS     All 5 projects updated successfully         ││
│  │                                                                    ││
│  │ [Copy Logs]  [Clear Logs]  [Export Logs]                          ││
│  └────────────────────────────────────────────────────────────────────┘│
│                                                                         │
│  Ready                                                                 │
└────────────────────────────────────────────────────────────────────────┘
```

### Layout Structure (WPF Grid)

```
MainWindow (1200x750, MinSize: 1000x600)
└─ Grid (Margin: 30px)
   ├─ Row 0: Header (Auto)
   │  ├─ Title: "Version Increase Tool" (HeaderTextStyle)
   │  └─ Subtitle: "Automatically increment..." (BodyTextStyle, Muted)
   │
   ├─ Row 1: Project Selection Card (Auto)
   │  └─ Border (Card style, Padding: 25px, Margin: 20px bottom)
   │     ├─ Base Directory TextBox + Browse Button
   │     ├─ Action Buttons Row (Scan, Select All, Deselect All)
   │     └─ Status Text: "Found X | Selected Y"
   │
   ├─ Row 2: Projects DataGrid (*) - GROWS ✨
   │  └─ Border (Card style)
   │     └─ DataGrid with Columns:
   │        - Checkbox (40px)
   │        - Project Name (2*)
   │        - Current Version (*)
   │        - Status (*)
   │        - Path (2*)
   │
   ├─ Row 3: Version Update Card (Auto)
   │  └─ Border (Card style)
   │     ├─ New Version Display
   │     └─ Action Buttons (Increase, Commit, Cancel)
   │
   ├─ Row 4: Activity Log Card (2*) - EXPANDS
   │  └─ Border (Card style)
   │     ├─ ListView (3 columns: Time, Level, Message)
   │     └─ Log Action Buttons
   │
   └─ Row 5: Status Bar (Auto)
      └─ TextBlock: Status message
```

### XAML Components

#### Card Border Style

```xml
<Style x:Key="CardBorderStyle" TargetType="Border">
    <Setter Property="Background" Value="{StaticResource BackgroundMediumBrush}"/>
    <Setter Property="BorderBrush" Value="{StaticResource BorderDarkBrush}"/>
    <Setter Property="BorderThickness" Value="1"/>
    <Setter Property="CornerRadius" Value="8"/>
    <Setter Property="Padding" Value="{StaticResource CardPaddingThickness}"/>
    <Setter Property="Margin" Value="0,0,0,{StaticResource CardGap}"/>
</Style>
```

#### Primary Button Style

```xml
<Style x:Key="PrimaryButtonStyle" TargetType="Button">
    <Setter Property="Background" Value="{StaticResource AccentGreenBrush}"/>
    <Setter Property="Foreground" Value="#1A1A1A"/>
    <Setter Property="FontWeight" Value="SemiBold"/>
    <Setter Property="Height" Value="38"/>
    <Setter Property="Padding" Value="20,0"/>
    <Setter Property="Margin" Value="0,0,15,0"/>
</Style>
```

#### DataGrid Style

```xml
<Style x:Key="ModernDataGridStyle" TargetType="DataGrid">
    <Setter Property="Background" Value="{StaticResource BackgroundMediumBrush}"/>
    <Setter Property="Foreground" Value="{StaticResource TextPrimaryBrush}"/>
    <Setter Property="BorderThickness" Value="0"/>
    <Setter Property="RowBackground" Value="{StaticResource BackgroundMediumBrush}"/>
    <Setter Property="AlternatingRowBackground" Value="{StaticResource BackgroundLight}"/>
    <Setter Property="GridLinesVisibility" Value="Horizontal"/>
    <Setter Property="HeadersVisibility" Value="Column"/>
    <Setter Property="AutoGenerateColumns" Value="False"/>
    <Setter Property="CanUserAddRows" Value="False"/>
    <Setter Property="SelectionMode" Value="Extended"/>
</Style>
```

### ViewModel Binding Examples

```xml
<!-- Base Directory TextBox -->
<TextBox Text="{Binding BaseDirectory, UpdateSourceTrigger=PropertyChanged}"
         Style="{StaticResource ModernTextBoxStyle}"/>

<!-- Scan Button -->
<Button Content="Scan Projects"
        Command="{Binding ScanProjectsCommand}"
        Style="{StaticResource PrimaryButtonStyle}"/>

<!-- Projects DataGrid -->
<DataGrid ItemsSource="{Binding ProjectFiles}"
          Style="{StaticResource ModernDataGridStyle}">
    <DataGrid.Columns>
        <DataGridCheckBoxColumn Binding="{Binding IsSelected}" Width="40"/>
        <DataGridTextColumn Header="Project Name" Binding="{Binding FileName}" Width="2*"/>
        <DataGridTextColumn Header="Current Version" Binding="{Binding CurrentVersion}" Width="*"/>
        <DataGridTextColumn Header="Status" Binding="{Binding Status}" Width="*"/>
        <DataGridTextColumn Header="Path" Binding="{Binding RelativePath}" Width="2*"/>
    </DataGrid.Columns>
</DataGrid>

<!-- Log ListView -->
<ListView ItemsSource="{Binding LogEntries}"
          Style="{StaticResource ModernListViewStyle}">
    <ListView.View>
        <GridView>
            <GridViewColumn Header="Time" DisplayMemberBinding="{Binding Time}" Width="120"/>
            <GridViewColumn Header="Level" DisplayMemberBinding="{Binding Level}" Width="100"/>
            <GridViewColumn Header="Message" DisplayMemberBinding="{Binding Message}" Width="600"/>
        </GridView>
    </ListView.View>
</ListView>
```

---

## 💡 Implementation Examples

### Custom Serilog Sink for WPF

Để hiển thị logs real-time trong WPF ListView, ta cần custom Serilog sink:

```csharp
public class WpfListViewSink : ILogEventSink
{
    private readonly VersionIncreaseViewModel _viewModel;
    private readonly IFormatProvider? _formatProvider;

    public void Emit(LogEvent logEvent)
    {
        var timestamp = logEvent.Timestamp.ToString("HH:mm:ss", _formatProvider);
        var level = logEvent.Level.ToString().ToUpper();
        var message = logEvent.RenderMessage(_formatProvider);

        _viewModel.AddLogEntry(timestamp, level, message);
    }
}

// In ViewModel
public void AddLogEntry(string time, string level, string message)
{
    System.Windows.Application.Current.Dispatcher.Invoke(() =>
    {
        LogEntries.Add(new LogEntry
        {
            Time = time,
            Level = level,
            Message = message,
            LevelColor = GetLevelColor(level)
        });

        if (LogEntries.Count > 1000)
            LogEntries.RemoveAt(0);
    });
}

private string GetLevelColor(string level)
{
    return level switch
    {
        "INFO" => "#4299E1",      // Info blue
        "WARNING" => "#FFB347",   // Warning orange
        "ERROR" => "#F05252",     // Error red
        "SUCCESS" => "#3ECF8E",   // Success green
        _ => "#F0F0F0"            // Default
    };
}
```

---

## 🔐 Security & Performance

### Security Considerations

| Area | Requirement | Implementation |
|------|------------|----------------|
| **File Access** | Only modify .csproj files in workspace | ✅ Path validation |
| **Git Credentials** | Use system Git credentials | ✅ LibGit2Sharp uses system config |
| **Input Validation** | Validate file paths and versions | ✅ Path and version format validation |
| **Error Handling** | Safe error messages, no sensitive data | ✅ Generic error messages to UI |
| **Backup** | No automatic backup (Git handles it) | ⚠️ Users should commit before use |

### Performance Requirements

| Metric | Target | Notes |
|--------|--------|-------|
| **File Scan Time** | < 2 seconds | For ~100 projects |
| **Version Update** | < 500ms per file | XML parse, update, save |
| **UI Responsiveness** | < 100ms | UI should never freeze |
| **Git Operations** | < 5 seconds | Commit + Push |
| **Memory Usage** | < 200MB | Lightweight tool |
| **Log Display** | < 50ms | Per log entry |

### Performance Optimizations

- ✅ Async/await for all I/O operations (file, Git)
- ✅ Parallel file scanning with `Parallel.ForEachAsync`
- ✅ ListView virtualization for large project lists
- ✅ Debounced UI updates to reduce overhead
- ✅ Background threads for long operations
- ✅ Progress reporting for better UX

---

## 🧪 Testing Strategy

### Unit Testing

| Component | Test Coverage | Priority |
|-----------|--------------|----------|
| **VersionService** | 100% | High |
| **ProjectScanner** | 95% | High |
| **ProjectFileService** | 95% | High |
| **GitService** | 80% | Medium |
| **ViewModels** | 90% | High |

**Key Test Cases:**
- Version parsing: valid and invalid formats
- Version increment: same day vs new day
- File filtering: EndsWith "ETL", StartsWith "Share"
- XML manipulation: preserve formatting
- Git operations: commit, push, error handling

### Integration Testing

- ✅ End-to-end: Scan → Update → Commit → Push
- ✅ File system operations
- ✅ Git repository integration
- ✅ Error recovery scenarios
- ✅ Large project sets (100+ projects)

### Manual Testing Checklist

- ✅ UI responsiveness (no freezing)
- ✅ DataGrid sorting and filtering
- ✅ Checkbox selection (Select All, Deselect All)
- ✅ Button enable/disable states
- ✅ Progress indicators
- ✅ Log color coding
- ✅ Copy logs functionality
- ✅ Error messages are user-friendly
- ✅ Git commit message generation
- ✅ Version display updates correctly

---

## 🚀 Deployment

### System Requirements

| Component | Requirement |
|-----------|------------|
| **Operating System** | Windows 10/11 |
| **Framework** | .NET 6.0 Runtime or SDK |
| **Git** | Git for Windows 2.30+ |
| **Memory** | 2GB RAM minimum |
| **Disk Space** | 100MB |
| **Permissions** | Read/Write access to workspace folder |

### Installation Steps

1. Install .NET 6.0 Runtime (if not installed)
   - Download: https://dotnet.microsoft.com/download/dotnet/6.0
2. Install Git for Windows (if not installed)
   - Download: https://git-scm.com/download/win
3. Extract application files to desired location
4. Run `Lifes.exe` (or `Lifes.WPF.exe`)
5. Configure base directory on first launch

### Configuration

**appsettings.json:**
```json
{
  "AppSettings": {
    "BaseDirectory": "F:\\Sources\\support_linkit_hub\\coding",
    "FilePattern": "*ETL.csproj",
    "ExcludePattern": "Share*",
    "GitAutoPush": true
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/Lifes-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

**appsettings.user.json** (User-specific, git-ignored):
```json
{
  "AppSettings": {
    "BaseDirectory": "C:\\MyWorkspace\\Projects"
  }
}
```

---

## 🗺️ Roadmap

### Phase 1: Version Increase Tool - v1.0.0 (Current Sprint)

**Tool 1: Version Increase Tool**
- ✅ Project scanning và filtering (US-1.1) - Completed 2026-02-04
- ✅ Version parsing và increment logic (yyyy.M.d.{number}) (US-1.2) - Completed 2026-02-04
- ✅ Batch update multiple .csproj files (US-1.2) - Completed 2026-02-04
- ✅ Search/Filter projects (US-1.2.1) - Completed 2026-02-04
- ✅ Save last directory (US-1.2.1) - Completed 2026-02-04
- ✅ Git integration (commit & push) (US-1.3) - Completed 2026-02-08
- 📋 Real-time logging with color-coded levels (US-1.4) - Planned
- ✅ MVVM architecture with WPF - Completed 2026-02-04
- ✅ Modern dark theme UI (Supabase-inspired) - Completed 2026-02-04
- 📋 Export logs to file (US-1.4) - Planned
- ⏳ Settings persistence (US-1.5) - Future Enhancement

**Status:** 🚧 In Development (65% Complete - US-1.1, US-1.2, US-1.2.1 Done)

---

### Phase 2: Testing & Infrastructure - v1.1.0 (Current Sprint)

- ✅ Add Testing Layer Infrastructure (US-2.1) - Completed 2026-02-06
- ✅ Unit & Integration Tests setup - Completed 2026-02-06
- ✅ Code Coverage reporting configuration - Completed 2026-02-06
- ✅ Documentation & Testing Guidelines - Completed 2026-02-06

**Status:** ✅ Completed (100% Complete)

---

### Phase 5 (Sprint 5): UI/UX & Calendar Enhancements - v1.1.0

- ✅ Tool Navigation Menu with hover dropdown (US-5.1) - Completed 2026-04-07
- ✅ Dashboard Chart Layout with Polymorphism (US-6.1) - Completed 2026-04-12
- ✅ Document Management Tracker (US-7.1) - Completed 2026-04-14
- ✅ Advanced Calendar Selection & Event Phases (US-8.4) - Completed 2026-04-16
- ✅ Hamburger Navigation Menu (US-8.5) - Completed 2026-04-16

**Status:** ✅ Completed (100% Complete)

---

### Phase 3: Additional Tools - v1.2.0 (Next Sprint)

**Tool 2: Build & Deploy Tool**
- 📋 Build multiple projects in parallel
- 📋 Deploy to multiple environments
- 📋 Health check after deployment
- 📋 Rollback capability
- 📋 Build logs và deployment history

**Tool 3: Database Migration Tool**
- 📋 Run EF migrations across multiple databases
- 📋 Migration rollback
- 📋 Database backup before migration
- 📋 Migration history tracking

**Status:** 📝 Planning

---

### Phase 4: Enhanced Features - v1.3.0 (Future)

**Version Tool Enhancements:**
- 📋 Preview changes before applying
- 📋 Dry-run mode
- 📋 Version history per project
- 📋 Undo last version update
- 📋 Custom version format support
- 📋 Batch operations history

**Tool 4: Configuration Manager**
- 📋 Update appsettings.json across projects
- 📋 Environment-specific configs
- 📋 Connection string management
- 📋 Secret management integration

### Phase 6: Memento Tagging & Hierarchical Filtering - v2.1.0 (Current)
- ✅ **Tagging System**: Categorize mementos with multiple tags (Work, Health, Personal, etc.)
- ✅ **Hierarchical (Cascade) Filtering**: Automatically include all child notes if the parent topic matches the selected tag.
- ✅ **Advanced Search**: Filter calendar views (Monthly, Annual, Heatmap) by tags.
- ✅ **Repository Pattern Migration**: Decoupled data access from business logic.

**Tool 5: Dependency Updater**
- 📋 Scan NuGet package versions
- 📋 Bulk update packages
- 📋 Check for vulnerabilities
- 📋 Generate dependency report

**Status:** 💡 Ideas

---

### Phase 5: Automation & CI/CD - v2.0.0 (Future)

**CLI Version:**
- 📋 Command-line interface for all tools
- 📋 Support for CI/CD pipelines
- 📋 Azure DevOps / GitHub Actions integration
- 📋 Automated deployment workflows

**Reporting & Analytics:**
- 📋 Deployment dashboard
- 📋 Version history analytics
- 📋 Build success rate tracking
- 📋 Performance metrics

**Status:** 🔮 Vision

---

## 📝 Appendix

### Glossary

| Term | Definition |
|------|------------|
| **ETL** | Extract, Transform, Load - quy trình xử lý data |
| **.csproj** | C# project file chứa metadata và configuration |
| **AssemblyVersion** | Version number của assembly (DLL/EXE) |
| **FileVersion** | File version hiển thị trong Windows properties |
| **MVVM** | Model-View-ViewModel - UI design pattern cho WPF |
| **ObservableProperty** | Attribute tạo property với change notification tự động |
| **RelayCommand** | Command pattern implementation cho MVVM |
| **LibGit2Sharp** | .NET library để tương tác với Git repositories |
| **Semantic Versioning** | Version format: Major.Minor.Patch.Build |
| **Build Number** | Số thứ tự build trong ngày (yyyy.M.d.{number}) |

### Version Format Explained

**Format:** `yyyy.M.d.{number}`

| Component | Description | Example |
|-----------|-------------|---------|
| `yyyy` | 4-digit year | 2026 |
| `M` | Month without leading zero | 2 (February), 12 (December) |
| `d` | Day without leading zero | 3, 15, 28 |
| `{number}` | Build number (increments daily) | 1, 2, 3... |

**Examples:**
- `2026.2.3.1` - First build on Feb 3, 2026
- `2026.2.3.5` - Fifth build on Feb 3, 2026
- `2026.12.25.1` - First build on Dec 25, 2026

### References

**Technical Documentation:**
- [MVVM Pattern](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/data/data-binding-overview)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)
- [LibGit2Sharp Documentation](https://github.com/libgit2/libgit2sharp)
- [Serilog Documentation](https://serilog.net/)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

**.NET Documentation:**
- [WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [System.Xml.Linq (XDocument)](https://docs.microsoft.com/en-us/dotnet/api/system.xml.linq.xdocument)
- [Dependency Injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

### Contact

| Role | Name | Email |
|------|------|-------|
| Product Owner | Development Team | - |
| Tech Lead | TBD | - |
| DevOps Lead | TBD | - |

---

**Document Version:** 1.5.1
**Last Updated:** 2026-04-12
**Status:** ✅ Active (85% Complete - US-1.1, US-1.2, US-1.2.1, US-1.3, US-2.1, US-5.1, US-6.1 Done)
**Document Version:** 2.1.0
**Last Updated:** 2026-04-18
**Status:** ✅ Active (Unified Memento Architecture & Tagging implemented - US-9.1, US-9.2)
