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
│       ├── MementoModel.cs         # US-9.1 — Recursive hierarchy (TagIds, IsAchieved support)
│       ├── TagModel.cs             # US-9.2 — Tagging system
│       └── MementoQueryModel.cs    # US-9.2 — Filtering model (ShowAchieved support - US-15.1)
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
├── Lifes.Presentation.WebApi/    # C# Local API Server (US-11.1)
│   ├── Controllers/                # REST endpoints routing to Application Commands
│   └── Program.cs                  # WebAPI Bootstrap
│
├── Lifes.Presentation.Electron/  # Electron UI (US-11.1)
│   ├── main.js                     # Electron main process
│   ├── preload.js                  # IPC Bridge
│   └── src/                        # Frontend UI (Angular)
│       └── app/
│           ├── features/
│           │   ├── sprint-board/    # US-11.2 Sprint Board feature
│           │   │   ├── sprint-board.component.[ts|html|css]
│           │   │   └── sprint-board.service.ts
│           │   ├── monthly-calendar/ # US-12.1 Monthly Calendar feature
│           │   │   ├── monthly-grid/
│           │   │   ├── monthly-calendar-page/
│           │   │   ├── tag-management/  # US-12.3 Tag Management UI
│           │   │   ├── tag.service.ts   # US-12.3 Global tag state
│           │   │   ├── tag.constants.ts # US-12.3 Color palette
│           │   │   └── monthly-calendar.service.ts
│           │   ├── memento-management/  # US-12.4 Memento Management feature
│           │   │   ├── memento-table/   # Passive table component
│           │   │   ├── memento-management.component.[ts|html|css]
│           │   │   └── memento-management.service.ts
│           │   ├── daily-timeline/       # US-16.1 Daily Timeline feature
│           │   │   └── daily-timeline.component.[ts|html|css]
│           │   ├── yearly-stream/        # US-17.1 Yearly Stream feature
│           │   │   ├── yearly-stream-page/
│           │   │   └── yearly-stream.service.ts
│           │   ├── weekly-tracker/       # US-18.1 Weekly Tracker feature
│           │   │   ├── weekly-tracker-page/
│           │   │   ├── content-explorer-page/ # Refinement (Content Mode)
│           │   │   ├── entry-editor/
│           │   │   ├── content-explorer.service.ts # Isolated content data
│           │   │   └── weekly-tracker.service.ts
│           │   └── ...
│           └── models/
│               ├── sprint-board.model.ts # US-11.2 data models
│               ├── memento.model.ts      # US-12.1 (Updated TagIds[])
│               ├── tag.model.ts          # US-12.1
│               ├── display-mode.model.ts # US-12.1
│               ├── selectable-month.model.ts # US-12.1
│               ├── daily-timeline.model.ts   # US-16.1
│               ├── yearly-stream.model.ts    # US-17.1
│               └── weekly-tracker.model.ts   # US-18.1
│
├── Lifes.Presentation.WPF/     # WPF UI
│   ├── Constants/                  # US-5.1
│   │   ├── ToolIds.cs              # Well-known tool ID constants
│   │   └── UIConstants.cs          # Centralized UI assets (Colors, Palette)
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
│   │   ├── AnnualCalendar/             # US-8.4, US-8.5, US-8.6, US-9.6
│   │   │   ├── AnnualCalendarView.xaml
│   │   │   ├── AnnualCalendarViewModel.cs
│   │   │   ├── MonthlyCalendarView.xaml
│   │   │   ├── MonthlyCalendarViewModel.cs
│   │   │   ├── TopicEditorView.xaml        # US-9.6, US-9.7
│   │   │   ├── TopicEditorView.xaml.cs     # US-9.6, US-9.7
│   │   │   ├── TopicEditorViewModel.cs     # US-9.6, US-9.7
│   │   │   ├── TagManagementView.xaml      # US-9.5 (Refactored)
│   │   │   ├── TagManagementView.xaml.cs   # US-9.5
│   │   │   ├── TagManagementViewModel.cs   # US-9.5
│   │   │   ├── ActivityHeatmapView.xaml    # US-8.6
│   │   │   ├── ActivityHeatmapViewModel.cs # US-8.6
│   │   │   └── Models/
│   │   │       └── SelectableTagViewModel.cs # US-9.2
│   │   ├── DocumentManagement/         # US-7.1
│   │   │   ├── DocumentManagementView.xaml
│   │   │   └── DocumentManagementViewModel.cs
│   │   ├── SprintBoard/                # US-10.1
│   │   │   ├── Models/
│   │   │   │   └── SprintBoardModels.cs
│   │   │   ├── SprintBoardView.xaml
│   │   │   └── SprintBoardViewModel.cs
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
├── tasks/                          # Automation & Task Scripts
│   ├── build-deploy/               # US-3.1 & US-11.3
│   │   ├── build-electron.ps1      # Packaging Frontend + Backend
│   │   ├── build-deploy-electron.ps1 # Full deployment + backup logic
│   │   └── deploy-config-electron.json # Deployment settings
│   └── run-tests/                  # US-2.1
│       └── run-tests-quick.ps1     # Fast testing script
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
│  Presentation (WPF / Electron)          │
│  - Views, ViewModels, UI Shell          │
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
| **Presentation** | UI (WPF / Electron) | Core, Application |
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

### 9. Annual Calendar & Memento System
**Status**: ✅ Completed
**User Stories**: US-9.1 -> US-9.9
**Documentation**: [fea-calendar-structure.md](./fea-calendar-structure.md)
**Key Components**:
- AnnualCalendar
- MonthlyCalendar (with Quick Edit Popup)
- TopicEditor (Add/Update Topics)
- TagManagementComponent
- MementoManagement (Ordering & Filtering)
- JsonMementoRepository (Hierarchical Data)
- CalendarService (Unified Query Filter)
- MementoModel (Recursive Hierarchy)
- `ActivityHeatmapViewModel` (Event-centric dot grid)
- `CalendarEventModel` with `Phases` (Multi-phase tracking)
- **Tagging & Hierarchical Filtering** (US-9.2)
- **Monthly Calendar CRUD & Color Picker** (US-9.3)
- **Edit Parent Mementos & Tags** (US-9.4)
- **Tag Manager Refactor (US-9.5)**: Decoupled UI and logic into a dedicated component.
- **Add Topic (US-9.6/US-9.7)**
- **Memento Management (US-9.8)**
- Gantt-style timeline rendering in XAML
- Hamburger Navigation integration

### 10. Sprint Board
**Status**: ✅ Completed
**User Stories**: US-10.1 (WPF), US-11.2 (Electron)
**Documentation**: [fea-sprint-board-structure.md](./fea-sprint-board-structure.md)
**Key Components**:
- **WPF**: `SprintBoardView.xaml` (UniformGrid matrix), code-behind Drag & Drop.
- **Electron/Angular**: `SprintBoardComponent` (Angular 19 + Signals), native HTML5 Drag & Drop.
- **Data Models**: `SprintBoardData`, `SprintFeature`, `SprintTask` (Angular interfaces).
- **Service**: `SprintBoardService` (Reactive state with Signals).

### 11. Electron UI Integration
**Status**: ✅ Completed
**User Stories**: US-11.1, US-11.3
**Documentation**: [fea-electron-structure.md](./fea-electron-structure.md)
**Key Components**:
- `Lifes.Presentation.WebApi`: REST API bridge using ASP.NET Core (Serilog integrated).
- `Lifes.Presentation.Electron`: Electron shell hosting an Angular 19 frontend (electron-log integrated).
- `main.js`: Backend process management (spawn/kill).
- `build-deploy-electron.ps1`: Automated packaging and deployment.

### 12. Monthly Calendar (Electron Shell)
**Status**: ✅ Completed
**User Stories**: US-12.1 -> US-12.6, US-15.1, US-15.2
**Documentation**: [fea-monthly-calendar-structure.md](./fea-monthly-calendar-structure.md), [fea-memento-management-structure.md](./fea-memento-management-structure.md)
**Key Components**:
- **Backend**: `CalendarController`, `ApiResponse<T>`, `ICalendarService`.
- **Frontend**: `MonthlyCalendarService`, `TagService`, `MementoManagementService` (US-12.4), `MonthlyGridComponent`, `TopicEditorComponent`, `MementoTableComponent` (US-12.4).
- **Cascade Delete**: Tự động dọn dẹp tagId khỏi mementos khi xóa tag.
- **Option B (US-15.1)**: Local state manipulation for instant filtering (Hide Achieved) without server reload.
- **Custom Hex Color (US-15.2)**: Support for manual hex color input in Editor and Quick Picker.

### 13. View Chart & Data Analysis (Electron Shell)
**Status**: ✅ Completed
**User Stories**: US-14.1, US-14.2
**Documentation**: [fea-view-chart-structure.md](./fea-view-chart-structure.md)
**Key Components**:
- **Local Services**: `MementoService`, `ViewChartService`.
- **UI Architecture**: `ViewChartPageComponent` (Smart Host) + `ChartContainerComponent` (Passive Coordinator).
- **Visualization Engines**:
    - `ChartVisualizerComponent` (SVG Native): High-performance, premium aesthetics via Angular templates.
    - `D3SampleComponent` (D3.js): Advanced mathematical scaling and dynamic transitions.
- **Config & Data Flow**: 
    - `TopicConfigPopupComponent`: Grid mapping (Label -> Value -> Color).
    - **Multi-row Stacked Layout**: Integrated Events, Emotions, and Sleep data on a single timeline.
    - **Data Transformation**: Recursive mapping of child mementos to parent topics.

### 14. Daily Timeline Feature (Electron Shell)
**Status**: ✅ Completed (UI Prototype)
**User Stories**: US-16.1
**Documentation**: [fea-daily-timeline-structure.md](./fea-daily-timeline-structure.md)
**Key Components**:
- **UI Components**: `DailyTimelinePageComponent`, `EntryEditorComponent`.
- **Logic & Data**: `DailyTimelineService` (Reactive state with Signals), `DailyEntry` model.
- **Experience**: Slide-up/Fade-in animations, Glassmorphism backdrop.

---

### 15. Yearly Stream View (US-17.1)
**Status**: ✅ Completed (UI Prototype)
**User Stories**: US-17.1
**Documentation**: [fea-yearly-stream-structure.md](./fea-yearly-stream-structure.md)

**Purpose**: Cung cấp cái nhìn tổng quan toàn năm (12 tháng x 31 ngày) dưới dạng ma trận. Hỗ trợ theo dõi thói quen đọc sách và các hoạt động khác thông qua giao diện trực quan và bộ lọc Signal-based.

**Key Components**:
- **YearlyStreamPageComponent**: Hiển thị lưới ma trận 12x31 với khả năng tối ưu hóa cho màn hình 4K.
- **YearlyStreamService**: Quản lý dữ liệu mock và logic lọc bài viết/sách.
- **Mailbox Style Reader**: Popup đọc bài viết với bố cục 2 cột (List - Detail) chuyên nghiệp.
- **Future Muting**: Tự động ẩn màu sắc và làm mờ các ô ngày trong tương lai để tập trung vào dữ liệu hiện tại.

---

### 16. Weekly Mood & Activity Tracker (US-18.1)
**Status**: ✅ Completed (UI Prototype)
**User Stories**: US-18.1
**Documentation**: [fea-weekly-tracker-structure.md](./fea-weekly-tracker-structure.md)

**Purpose**: Cung cấp giao diện theo dõi tâm trạng và hoạt động theo từng khung giờ trong tuần (7 ngày x 24 giờ). Tích hợp chế độ **Content Explorer** chuyên dụng cho nội dung văn bản.

**Key Components**:
- **WeeklyTrackerPageComponent**: Hiển thị lưới ma trận 7x24 với thanh điều hướng tuần.
- **ContentExplorerPageComponent**: Chế độ xem nội dung chuyên sâu với bộ lọc danh mục và CSS Isolation.
- **WeeklyTrackerService**: Quản lý trạng thái Tracker tối giản.
- **ContentExplorerService**: Quản lý dữ liệu tin tức/nội dung độc lập (Tech News).
- **WeeklyEntryEditorComponent**: Modal glassmorphism cho việc nhập liệu.

---

### 17. PDF Reader Interface (US-19.1)
**Status**: ✅ Completed (UI Clone)
**User Stories**: US-19.1
**Documentation**: [fea-pdf-reader-structure.md](./fea-pdf-reader-structure.md)

**Purpose**: Trình đọc PDF tích hợp với khả năng chuyển đổi linh hoạt giữa các bố cục khác nhau (Classic, Focus, Contextual) để cá nhân hóa trải nghiệm đọc và ghi chú.

**Key Components**:
- **PdfReaderPageComponent**: Giao diện chính chứa các Panel hiển thị thư viện sách, nội dung PDF, và danh sách ghi chú.
- **Layout Management**: Hỗ trợ 3 chế độ xem khác nhau giúp tối ưu hóa không gian cho từng mục đích (Đọc tập trung, tra cứu ngữ cảnh, v.v.).
- **Reading Tools**: Tích hợp thanh công cụ nổi (Floating Toolbar) khi bôi đen văn bản để highlight hoặc thêm ghi chú nhanh.
- **Theme & Zoom**: Hỗ trợ Dark Mode và khả năng thu phóng nội dung linh hoạt.

---

### 18. Laputa Notes Interface (US-20.1)
**Status**: ✅ Completed (UI Clone)
**User Stories**: US-20.1
**Documentation**: [fea-laputa-notes-structure.md](./fea-laputa-notes-structure.md)

**Purpose**: Giao diện Note-taking tích hợp theo nguyên mẫu `sample.note.app.html`, hỗ trợ soạn thảo Markdown, tổ chức theo tag/section, cùng khả năng chuyển đổi 4 chế độ hiển thị (List, Card, Compact, Grid).

**Key Components**:
- **LaputaNotesPageComponent**: Container chính cho bố cục Layout.
- **LaputaSidebarComponent**: Quản lý Section, Tags và Theme (Dark/Sepia).
- **LaputaNoteListComponent**: Giao diện danh sách ghi chú với hỗ trợ Context Menu và thay đổi kích thước bằng Drag Handle.
- **LaputaEditorComponent**: Trình soạn thảo Markdown chuyên dụng kết hợp với cơ chế Popup Detail Panel tự động kích hoạt ở chế độ Grid.
- **LaputaNotesService**: Quản lý State cục bộ của component và dữ liệu Mock thông qua Angular Signals.

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

**Document Version**: 1.6.0  
**Last Updated**: 2026-05-01  
**Status**: ✅ Active (100% Complete - Phase 16 Finalized with Content Explorer)
