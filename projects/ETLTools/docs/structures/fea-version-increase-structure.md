# Feature: Version Increase Tool

## Overview

Version Increase Tool là công cụ đầu tiên trong ETL Deployment Tools Suite, giúp tự động tăng version number cho các .csproj files theo chuẩn `yyyy.M.d.{number}`. Tool scan, filter, và update version cho multiple projects cùng lúc, với UI hiện đại dark theme.

**Status**: ✅ Partially Completed (US-1.1, US-1.2, US-1.2.1, US-1.3 Done | Testing Layer US-2.1 Done)

**User Stories Completed**:
- US-1.1: Load và Hiển thị Danh Sách Projects
- US-1.2: Tăng Version Number Tự Động
- US-1.2.1: Search và Save Last Directory (Enhancement)
- US-1.3: Git Commit và Push Changes - Completed 2026-02-08

**User Stories Planned**:
- US-1.4: Logging và Error Handling
- US-1.5: Settings và Configuration (Advanced)

---

## Architecture

Version Increase Tool được xây dựng theo **Clean Architecture** với 5 layers:

```
┌─────────────────────────────────────────┐
│  Presentation Layer (WPF)               │
│  - VersionIncreaseView.xaml            │
│  - VersionIncreaseViewModel            │
│  - ProjectFileViewModel                │
└──────────────┬──────────────────────────┘
               │ depends on
               ▼
┌─────────────────────────────────────────┐
│  Application Layer                      │
│  - ScanProjectsCommand                 │
│  - UpdateVersionsCommand               │
│  - DTOs (ProjectFileDto, etc.)         │
└─────┬────────────────────────┬──────────┘
      │ depends on              │ depends on
      ▼                        ▼
┌──────────────┐    ┌──────────────────────┐
│   Domain     │    │   Infrastructure     │
│  - Entities  │    │  - Services          │
│  - VOs       │    │    * ProjectScanner  │
│  - Enums     │    │    * VersionService  │
└──────────────┘    │    * ProjectFileServ │
                    └──────────────────────┘
      ▲                        ▲
      └────────────┬───────────┘
                   │ implements
┌──────────────────┴───────────────────────┐
│  Core Layer (Shared)                     │
│  - Interfaces (IProjectScanner, etc.)    │
│  - Result<T> pattern                     │
└──────────────────────────────────────────┘
```

---

## File Structure

### Presentation Layer
**Location**: `src/ETLTools.Presentation.WPF/Features/VersionIncrease/`

#### VersionIncreaseView.xaml
- **Purpose**: Main UI for Version Increase Tool
- **Components**:
  - Base directory selection with Browse button (auto-saved)
  - Action buttons: Scan Projects, Select All, Deselect All
  - **Search section** (US-1.2.1):
    - Search TextBox with placeholder "Search projects..."
    - Clear button (✕) visible when text entered
    - Filter status text "Showing X of Y projects"
  - DataGrid showing filtered project list with checkboxes
  - Version Update section with:
    - **Increase Version button** (primary)
    - **Commit & Push button** (secondary, enabled after version update) - US-1.3
  - Progress bar for batch operations
- **Design**: Modern dark theme (Supabase-inspired)
- **Dependencies**: VersionIncreaseViewModel (DataContext)
- **Data Binding**: DataGrid binds to `FilteredProjectFiles` (US-1.2.1)

#### VersionIncreaseViewModel.cs
- **Purpose**: ViewModel orchestrating UI logic and commands
- **Properties**:
  - `BaseDirectory` - Current scan directory (auto-saved)
  - `ProjectFiles` - ObservableCollection of all projects
  - `FilteredProjectFiles` - ObservableCollection of filtered projects (US-1.2.1)
  - `SearchText` - Current search text (US-1.2.1)
  - `FilterStatusText` - "Showing X of Y projects" (US-1.2.1)
  - `FoundCount`, `SelectedCount` - Project counts
  - `ProcessedCount`, `TotalCount`, `ProgressPercentage` - Progress tracking
  - `IsProcessing`, `StatusMessage` - UI state
  - `HasSearchText` - Computed property for UI binding (US-1.2.1)
  - `CanCommit` - Enabled after successful version update (US-1.3)
  - `_lastUpdateResults` - Stores update results for commit message generation (US-1.3)
- **Commands**:
  - `BrowseDirectoryCommand` - Open folder picker (auto-saves directory)
  - `ScanProjectsCommand` - Trigger project scan
  - `SelectAllCommand`, `DeselectAllCommand` - Checkbox operations (works with filtered list)
  - `IncreaseVersionCommand` - Trigger version update
  - `ClearSearchCommand` - Clear search text (US-1.2.1)
  - `CommitAndPushCommand` - Trigger Git commit & push workflow (US-1.3)
- **Dependencies**:
  - `ScanProjectsCommand` (Application)
  - `UpdateVersionsCommand` (Application)
  - `CommitChangesCommand` (Application) - US-1.3
  - `LoadSettingsCommand` (Application) - US-1.2.1
  - `SaveSettingsCommand` (Application) - US-1.2.1
  - `IGitService` - US-1.3
  - `ILogger<VersionIncreaseViewModel>`
- **Methods** (US-1.2.1):
  - `LoadLastDirectoryAsync()` - Load saved directory on startup
  - `SaveLastDirectoryAsync()` - Save directory on change
  - `ApplyFilter()` - Filter projects based on search text
- **Methods** (US-1.3):
  - `CommitAndPushAsync()` - Orchestrates Git workflow:
    1. Validate Git repository
    2. Get modified files
    3. Generate commit message from `_lastUpdateResults`
    4. Show GitCommitDialog for user review/edit
    5. Execute commit & push via CommitChangesCommand
    6. Update UI and reset state
  - `GenerateCommitMessage()` - Static method generates formatted commit message:
    - Format: "[AI:0] increase version to X.X.X.X for N projects"
    - Lists all project updates with old → new versions
  - `OnCanCommitChanged()` - Notifies command CanExecute
  - `OnIsProcessingChanged()` - Notifies command CanExecute

#### Models/ProjectFileViewModel.cs
- **Purpose**: UI representation of a project file
- **Properties**:
  - `IsSelected` - Checkbox state
  - `FileName`, `CurrentVersion`, `Status`, `RelativePath`, `FullPath`
- **Observability**: Uses `ObservableObject` from CommunityToolkit.Mvvm

#### Helpers/ProjectFilterHelper.cs (US-1.2.1)
- **Purpose**: Static helper for filtering projects based on search
- **Methods**:
  - `Filter(projects, searchText)` - Filters projects (case-insensitive)
    - Searches in: FileName, RelativePath, CurrentVersion
    - Returns IEnumerable<ProjectFileViewModel>
  - `GetFilterStatusText(filteredCount, totalCount, isFiltered)` - Generates status text
- **Performance**: O(n) complexity, acceptable for < 1000 projects

#### ViewModels/GitCommitDialogViewModel.cs (US-1.3)
- **Purpose**: ViewModel for Git Commit Dialog
- **Properties**:
  - `CommitMessage` - Editable commit message (pre-filled from auto-generation)
  - `ModifiedFiles` - ObservableCollection of file paths to commit
  - `PushToRemote` - Checkbox for push after commit (default: false)
  - `IsProcessing` - Processing state indicator
  - `StatusMessage` - Status message for user feedback
  - `DialogResult` - Boolean result (true = commit, false = cancel)
  - `CloseAction` - Action to close dialog window
- **Commands**:
  - `CommitCommand` - Confirms commit (sets DialogResult = true, closes dialog)
    - CanExecute: CommitMessage not empty && !IsProcessing
  - `CancelCommand` - Cancels operation (sets DialogResult = false, closes dialog)
- **Pattern**: Uses CommunityToolkit.Mvvm with `[ObservableProperty]` and `[RelayCommand]`
- **Validation**: Commit button disabled when message is empty

#### Views/GitCommitDialog.xaml (US-1.3)
- **Purpose**: Dialog window for reviewing and editing Git commit
- **UI Components**:
  - **Header**: "Git Commit & Push" title
  - **Commit Message Section**:
    - Label: "Commit Message"
    - Multi-line TextBox (4 rows, editable, pre-filled)
    - Character counter (optional)
  - **Modified Files Section**:
    - Label: "Modified Files ({count})"
    - ListView showing file paths (read-only)
    - Scrollable list with modern styling
  - **Options Section**:
    - CheckBox: "Push to remote after commit" (default: unchecked)
  - **Action Buttons**:
    - "Commit" button (PrimaryButtonStyle, bound to CommitCommand)
    - "Cancel" button (SecondaryButtonStyle, bound to CancelCommand)
- **Design**:
  - Modern dark theme matching main UI
  - Dialog size: 700x600 pixels
  - WindowStartupLocation: CenterOwner
  - Owner: MainWindow
- **Data Context**: GitCommitDialogViewModel
- **Behavior**:
  - Modal dialog (ShowDialog)
  - Returns DialogResult from ViewModel
  - Allows editing commit message before confirming

#### Views/GitCommitDialog.xaml.cs (US-1.3)
- **Purpose**: Code-behind for GitCommitDialog
- **Constructor**: Accepts `GitCommitDialogViewModel` parameter
  - Sets DataContext to ViewModel
  - Wires up ViewModel.CloseAction to Close() method
- **Public Property**: `ViewModel` - Typed access to DataContext
- **Pattern**: Minimal code-behind, logic in ViewModel

---

### Application Layer
**Location**: `src/ETLTools.Application/Features/VersionIncrease/`

#### Commands/ScanProjectsCommand.cs
- **Purpose**: Orchestrates project scanning workflow
- **Input**: `string basePath`
- **Output**: `Result<IEnumerable<ProjectFileDto>>`
- **Workflow**:
  1. Call `IProjectScanner.ScanProjectsAsync()`
  2. For each found file, call `IProjectFileService.ReadVersionAsync()`
  3. Map to `ProjectFileDto` objects
  4. Return results
- **Dependencies**:
  - `IProjectScanner`
  - `IProjectFileService`
  - `ILogger<ScanProjectsCommand>`

#### Commands/UpdateVersionsCommand.cs
- **Purpose**: Orchestrates version update workflow
- **Input**: `IEnumerable<ProjectFileDto>`, `DateTime targetDate`
- **Output**: `Result<VersionUpdateResultDto>`
- **Workflow**:
  1. For each project:
     - Parse current version using `IVersionService`
     - Increment version using domain logic
     - Update file using `IProjectFileService`
     - Report progress
  2. Collect results and return summary
- **Dependencies**:
  - `IProjectFileService`
  - `IVersionService`
  - `ILogger<UpdateVersionsCommand>`
- **Features**: Progress reporting with `IProgress<T>`

#### DTOs/ProjectFileDto.cs
- **Purpose**: Data transfer object for project information
- **Properties**: `FileName`, `FullPath`, `RelativePath`, `CurrentVersion`

#### DTOs/VersionUpdateResultDto.cs
- **Purpose**: Summary of batch update operation
- **Properties**: `TotalProjects`, `SuccessCount`, `FailedCount`, `Updates` (list)
- **Computed**: `IsSuccess`, `Summary` message

#### DTOs/ProjectUpdateDto.cs
- **Purpose**: Individual project update result
- **Properties**: `ProjectName`, `OldVersion`, `NewVersion`, `Success`, `ErrorMessage`

#### Commands/CommitChangesCommand.cs (US-1.3)
- **Purpose**: Orchestrates Git commit and push workflow
- **Input**: `CommitChangesDto`, `IProgress<string>` (optional)
- **Output**: `Result<CommitResultDto>`
- **Workflow**:
  1. Validate Git repository exists
  2. Get current branch (if not specified in DTO)
  3. Stage modified files using `IGitService.StageFilesAsync()`
  4. Create commit using `IGitService.CommitAsync()`
  5. Optionally push to remote using `IGitService.PushAsync()`
  6. Report progress at each step
  7. Return comprehensive result with success/failure details
- **Dependencies**:
  - `IGitService`
  - `ILogger<CommitChangesCommand>`
- **Features**:
  - Progress reporting for UI updates
  - Partial success handling (commit OK, push failed)
  - Validation before operations
  - Detailed error messages
- **Error Handling**:
  - Not a Git repo → Early return with error
  - Staging fails → Return error, no commit
  - Commit fails → Return error
  - Push fails → Commit succeeded, return partial success
- **Example**:
  ```csharp
  var dto = new CommitChangesDto
  {
      RepositoryPath = basePath,
      ModifiedFiles = files,
      CommitMessage = "chore: increase version",
      PushToRemote = true,
      RemoteName = "origin"
  };

  var result = await command.ExecuteAsync(dto, progress);
  ```

#### DTOs/CommitChangesDto.cs (US-1.3)
- **Purpose**: Request DTO for commit operation
- **Properties**:
  - `ModifiedFiles` - List of files to stage and commit
  - `CommitMessage` - Commit message text
  - `PushToRemote` - Whether to push after commit (default: true)
  - `RemoteName` - Remote name (default: "origin")
  - `BranchName` - Branch name (nullable, uses current branch if null)
  - `RepositoryPath` - Path to Git repository

#### DTOs/CommitResultDto.cs (US-1.3)
- **Purpose**: Result DTO for commit operation
- **Properties**:
  - `CommitSuccess` - Whether commit succeeded
  - `PushSuccess` - Whether push succeeded
  - `CommitSha` - SHA of created commit
  - `BranchName` - Branch name that was committed/pushed
  - `FilesCommitted` - Number of files in commit
  - `ErrorMessage` - Error message if operation failed
- **Computed Properties**:
  - `IsSuccess` - Overall success (commit succeeded)
  - `Summary` - User-friendly summary message with emojis
    - ✅ Success: "Successfully committed X files and pushed to branch"
    - ⚠️ Partial: "Committed X files but push failed: {error}"
    - ❌ Failure: "Commit failed: {error}"

---

### Application Layer - Common (US-1.2.1)
**Location**: `src/ETLTools.Application/Common/`

#### Commands/LoadSettingsCommand.cs
- **Purpose**: Load application settings from persistence
- **Input**: None
- **Output**: `Result<AppSettingsDto>`
- **Workflow**:
  1. Call `ISettingsService.LoadAsync()`
  2. Map Core.AppSettings → AppSettingsDto
  3. Return result (defaults on failure)
- **Dependencies**:
  - `ISettingsService`
  - `ILogger<LoadSettingsCommand>`
- **Error Handling**: Returns default settings on any error (graceful fallback)

#### Commands/SaveSettingsCommand.cs
- **Purpose**: Save application settings to persistence
- **Input**: `AppSettingsDto`
- **Output**: `Result`
- **Workflow**:
  1. Validate DTO
  2. Map AppSettingsDto → Core.AppSettings
  3. Call `ISettingsService.SaveAsync()`
  4. Return result
- **Dependencies**:
  - `ISettingsService`
  - `ILogger<SaveSettingsCommand>`
- **Error Handling**: Logs errors but doesn't crash app

#### DTOs/AppSettingsDto.cs
- **Purpose**: Data transfer object for application settings
- **Properties**: 
  - `LastDirectory` - Last used base directory
  - `Version` - Settings format version ("1.0")

---

### Domain Layer
**Location**: `src/ETLTools.Domain/Features/VersionIncrease/`

#### Entities/ProjectFile.cs
- **Purpose**: Domain entity representing a project file
- **Properties**: `FileName`, `FullPath`, `RelativePath`, `CurrentVersion`, `Status`
- **Business Logic**:
  - `MatchesFilter()` - Implements filtering rules:
    - Must end with "ETL" (case-insensitive)
    - Must have `.csproj` extension
    - Must NOT start with "Share" (case-insensitive)
- **Dependencies**: `VersionInfo` (ValueObject), `ProjectStatus` (Enum)

#### ValueObjects/VersionInfo.cs
- **Purpose**: Value object for version number with business logic
- **Properties**: `Year`, `Month`, `Day`, `Build`
- **Methods**:
  - `Parse(string)` - Parse version string "yyyy.M.d.{n}"
  - `ToString()` - Format to "yyyy.M.d.{n}"
  - `GetDate()` - Get DateTime from version
  - **`Increment(DateTime targetDate)`** - Core business logic:
    - Same day: increment build number
    - Different day: reset build to 1
  - `IsSameDate(DateTime)` - Date comparison helper
- **Example**:
  ```csharp
  var current = VersionInfo.Parse("2026.2.3.5");
  var incremented = current.Increment(DateTime.Now); // 2026.2.4.1 (if new day)
  ```

#### Enums/ProjectStatus.cs
- **Purpose**: Status enumeration for project processing
- **Values**: `Ready`, `Processing`, `Updated`, `Failed`, `Skipped`

#### ValueObjects/GitCommitInfo.cs (US-1.3)
- **Purpose**: Value object representing Git commit information
- **Properties**:
  - `ModifiedFiles` - List of files to commit
  - `CommitMessage` - Commit message text
  - `PushToRemote` - Whether to push after commit
  - `RemoteName` - Remote name (default: "origin")
  - `BranchName` - Branch name to push to
- **Design**: Immutable value object following DDD principles
- **Usage**: Contains all information needed for Git commit & push operation

---

### Domain Layer - Common (US-1.2.1)
**Location**: `src/ETLTools.Domain/Common/ValueObjects/`

#### AppSettings.cs
- **Purpose**: Domain value object for application settings with validation
- **Properties**:
  - `LastDirectory` - Last used directory (private setter for immutability)
  - `Version` - Settings format version
- **Factory Methods**:
  - `CreateDefault()` - Returns settings with current directory
  - `Create(lastDirectory, version)` - Creates with validation
  - `FromPersistenceModel(Core.AppSettings)` - Maps from Core model
- **Methods**:
  - `Validate()` - Validates settings (directory path format)
  - `WithLastDirectory(newDirectory)` - Immutable update
  - `ToPersistenceModel()` - Maps to Core model
- **Design**: Immutable value object following DDD principles
- **Validation**:
  - LastDirectory không được empty
  - Path phải valid format (không cần tồn tại)
  - Returns `Result<AppSettings>` cho error handling

---

### Infrastructure Layer
**Location**: `src/ETLTools.Infrastructure/Features/VersionIncrease/Services/`

#### ProjectScanner.cs
- **Purpose**: Scans file system for .csproj files
- **Implements**: `IProjectScanner`
- **Logic**:
  - Recursive directory scanning
  - Applies filters (EndsWith "ETL", NOT StartsWith "Share")
  - Returns list of matching file paths
- **Dependencies**: `ILogger<ProjectScanner>`
- **Performance**: Async with `Task.Run` for CPU-bound operations

#### ProjectFileService.cs
- **Purpose**: Reads and updates .csproj XML files
- **Implements**: `IProjectFileService`
- **Methods**:
  - `ReadVersionAsync()` - Parse XML, extract `<AssemblyVersion>` tag
  - `UpdateVersionAsync()` - Update both `<AssemblyVersion>` and `<FileVersion>` tags
- **Features**:
  - XML manipulation using `System.Xml.Linq.XDocument`
  - Preserves XML formatting and indentation
  - Creates `<FileVersion>` tag if not exists
- **Dependencies**: `ILogger<ProjectFileService>`

#### VersionService.cs
- **Purpose**: Version parsing and increment operations
- **Implements**: `IVersionService`
- **Methods**:
  - `ParseVersion()` - Delegates to domain `VersionInfo.Parse()`
  - `FormatVersion()` - Converts `VersionInfo` to string
  - `IncrementVersion()` - Delegates to domain `VersionInfo.Increment()`
- **Pattern**: Thin wrapper around domain logic
- **Dependencies**: `ILogger<VersionService>`

#### Git/GitService.cs (US-1.3)
- **Purpose**: Git operations using LibGit2Sharp library
- **Implements**: `IGitService`
- **Location**: `src/ETLTools.Infrastructure/Features/VersionIncrease/Git/`
- **Methods**:
  - `IsGitRepositoryAsync()` - Uses `Repository.Discover()` to check for Git repo
  - `HasChangesAsync()` - Uses `repo.RetrieveStatus()` to check for dirty working tree
  - `GetModifiedFilesAsync()` - Filters status entries (excludes Ignored, Unaltered)
  - `GetCurrentBranchAsync()` - Returns `repo.Head.FriendlyName`
  - `StageFilesAsync()` - Uses `Commands.Stage()` for each file
  - `CommitAsync()` - Creates commit with signature (custom or from git config)
  - `PushAsync()` - Pushes using `repo.Network.Push()` with credentials provider
- **Features**:
  - Full error handling with specific error types (RepositoryNotFoundException, EmptyCommitException, etc.)
  - Comprehensive logging for all operations
  - Authentication support with `DefaultCredentials` provider
  - Network error handling (connection issues, auth failures)
  - Partial success handling (commit succeeded but push failed)
- **Dependencies**:
  - `LibGit2Sharp` v0.27.2 NuGet package
  - `ILogger<GitService>`
- **Error Handling**:
  - Repository not found → Result.Failure
  - No changes to commit → EmptyCommitException handled
  - Network errors → Specific error messages
  - Authentication errors → User-friendly messages
- **Example Usage**:
  ```csharp
  // Stage files
  await _gitService.StageFilesAsync(repoPath, modifiedFiles);

  // Commit
  var commitResult = await _gitService.CommitAsync(repoPath, "chore: increase version");

  // Push to remote
  await _gitService.PushAsync(repoPath, "origin", "master");
  ```

---

### Infrastructure Layer - Common (US-1.2.1)
**Location**: `src/ETLTools.Infrastructure/Common/Configuration/`

#### SettingsService.cs
- **Purpose**: Persists application settings to JSON file
- **Implements**: `ISettingsService`
- **Methods**:
  - `LoadAsync()` - Read from `appsettings.user.json`
  - `SaveAsync(AppSettings)` - Write to `appsettings.user.json`
- **File Location**: Application base directory / `appsettings.user.json`
- **Serialization**: `System.Text.Json` with indented formatting
- **Error Handling**:
  - File not found → Return default AppSettings
  - Invalid JSON → Return default AppSettings, log warning
  - IO errors → Return defaults / failure result, log error
- **Features**:
  - Automatic directory creation
  - UTF-8 encoding
  - Pretty-printed JSON (WriteIndented = true)
- **Dependencies**: `ILogger<SettingsService>`
- **Pattern**: Graceful degradation - never crashes, always returns valid settings

---

### Core Layer
**Location**: `src/ETLTools.Core/`

#### Interfaces/IProjectScanner.cs
- **Purpose**: Contract for project scanning
- **Method**: `Task<Result<IEnumerable<string>>> ScanProjectsAsync(string basePath, string filePattern, string excludePattern)`

#### Interfaces/IProjectFileService.cs
- **Purpose**: Contract for .csproj file operations
- **Methods**:
  - `Task<Result<string>> ReadVersionAsync(string filePath)`
  - `Task<Result> UpdateVersionAsync(string filePath, string newVersion)`

#### Interfaces/IVersionService.cs
- **Purpose**: Contract for version operations
- **Methods**:
  - `Result<VersionInfo> ParseVersion(string versionString)`
  - `Result<string> FormatVersion(VersionInfo version)`
  - `Result<VersionInfo> IncrementVersion(VersionInfo current, DateTime targetDate)`

#### Interfaces/ISettingsService.cs (US-1.2.1)
- **Purpose**: Contract for settings persistence
- **Methods**:
  - `Task<Result<AppSettings>> LoadAsync()` - Load settings from storage
  - `Task<Result> SaveAsync(AppSettings settings)` - Save settings to storage
- **Model**: `AppSettings` class with `LastDirectory` and `Version` properties

#### Interfaces/IGitService.cs (US-1.3)
- **Purpose**: Contract for Git operations using LibGit2Sharp
- **Methods**:
  - `Task<Result<bool>> IsGitRepositoryAsync(string path)` - Check if path is a Git repo
  - `Task<Result<bool>> HasChangesAsync(string repoPath)` - Check for uncommitted changes
  - `Task<Result<IEnumerable<string>>> GetModifiedFilesAsync(string repoPath)` - Get modified files list
  - `Task<Result<string>> GetCurrentBranchAsync(string repoPath)` - Get current branch name
  - `Task<Result> StageFilesAsync(string repoPath, IEnumerable<string> files)` - Stage files (git add)
  - `Task<Result<string>> CommitAsync(string repoPath, string message, string? authorName, string? authorEmail)` - Create commit
  - `Task<Result> PushAsync(string repoPath, string remoteName, string branchName)` - Push to remote
- **Features**:
  - Repository validation
  - File staging with selective file support
  - Commit with custom author or default git config
  - Push with authentication support
  - Comprehensive error handling for network/auth errors

#### Models/Result.cs
- **Purpose**: Result pattern for error handling without exceptions
- **Types**:
  - `Result` - Operation without return value
  - `Result<T>` - Operation with return value
- **Properties**: `IsSuccess`, `Error`, `Value` (for Result<T>)
- **Factory Methods**: `Success()`, `Success(T)`, `Failure(string)`

---

## Data Flow

### Scan Projects Flow (US-1.1)

```
User clicks "Scan Projects"
    ↓
VersionIncreaseViewModel.ScanProjectsCommand
    ↓
ScanProjectsCommand.ExecuteAsync(basePath)
    ↓
IProjectScanner.ScanProjectsAsync()
    ↓
ProjectScanner → File System
    ↓ (for each .csproj file)
IProjectFileService.ReadVersionAsync()
    ↓
ProjectFileService → XDocument.Load()
    ↓
Map to ProjectFileDto
    ↓
Return to ViewModel
    ↓
Update ObservableCollection<ProjectFileViewModel>
    ↓
UI DataGrid updates automatically
```

### Update Versions Flow (US-1.2)

```
User selects projects → clicks "Increase Version"
    ↓
Confirmation dialog
    ↓
VersionIncreaseViewModel.IncreaseVersionCommand
    ↓
UpdateVersionsCommand.ExecuteAsync(projects, targetDate)
    ↓ (for each project)
IVersionService.ParseVersion(currentVersion)
    ↓
VersionInfo.Parse() → Domain logic
    ↓
IVersionService.IncrementVersion(current, targetDate)
    ↓
VersionInfo.Increment() → Domain business logic
    ↓
IProjectFileService.UpdateVersionAsync(path, newVersion)
    ↓
ProjectFileService → XDocument manipulation
    ↓
Report progress via IProgress<T>
    ↓
Collect results → VersionUpdateResultDto
    ↓
Return to ViewModel
    ↓
Update UI: CurrentVersion, Status, ProgressBar
    ↓
Show summary MessageBox
```

### Search/Filter Flow (US-1.2.1)

```
User types in search box
    ↓
VersionIncreaseViewModel.SearchText property changes
    ↓
PropertyChanged event triggers
    ↓
ApplyFilter() method called
    ↓
ProjectFilterHelper.Filter(ProjectFiles, SearchText)
    ↓
Filter logic (case-insensitive):
  - Search in FileName
  - Search in RelativePath
  - Search in CurrentVersion
    ↓
FilteredProjectFiles updated
    ↓
FilterStatusText updated ("Showing X of Y projects")
    ↓
DataGrid re-binds automatically to FilteredProjectFiles
    ↓
UI shows filtered results

User clicks Clear (✕) button
    ↓
ClearSearchCommand executes
    ↓
SearchText = empty string
    ↓
ApplyFilter() → all projects shown
```

### Settings Flow - Load on Startup (US-1.2.1)

```
Application starts
    ↓
VersionIncreaseViewModel constructor
    ↓
LoadLastDirectoryAsync() called
    ↓
LoadSettingsCommand.ExecuteAsync()
    ↓
ISettingsService.LoadAsync()
    ↓
SettingsService reads appsettings.user.json
    ↓
If file exists → Deserialize JSON
If not exists → Return default AppSettings
    ↓
Map Core.AppSettings → AppSettingsDto
    ↓
Return to ViewModel
    ↓
BaseDirectory property updated
    ↓
UI TextBox shows saved directory
```

### Settings Flow - Save on Change (US-1.2.1)

```
User browses directory OR manually edits BaseDirectory
    ↓
BaseDirectory property changes
    ↓
PropertyChanged event triggers
    ↓
SaveLastDirectoryAsync() called
    ↓
Create AppSettingsDto with new directory
    ↓
SaveSettingsCommand.ExecuteAsync(dto)
    ↓
Validate DTO
    ↓
Map AppSettingsDto → Core.AppSettings
    ↓
ISettingsService.SaveAsync(settings)
    ↓
SettingsService serializes to JSON
    ↓
Write to appsettings.user.json
    ↓
If error → Log warning (graceful degradation)
If success → Log debug info
```

### Git Commit & Push Flow (US-1.3)

```
User completes version update → CanCommit = true
    ↓
User clicks "Commit & Push" button
    ↓
VersionIncreaseViewModel.CommitAndPushCommand executes
    ↓
IGitService.IsGitRepositoryAsync(BaseDirectory)
    ↓
If NOT Git repo → Show warning, exit
    ↓
IGitService.GetModifiedFilesAsync(BaseDirectory)
    ↓
If NO modified files → Show info, exit
    ↓
GenerateCommitMessage(_lastUpdateResults)
    ↓
Create GitCommitDialogViewModel:
  - CommitMessage = generated message (editable)
  - ModifiedFiles = list of changed files
  - PushToRemote = false (default)
    ↓
Show GitCommitDialog (modal)
    ↓
User reviews/edits commit message
User checks/unchecks "Push to remote"
User clicks "Commit" or "Cancel"
    ↓
If Cancel → DialogResult = false, exit
    ↓
If Commit → DialogResult = true
    ↓
Create CommitChangesDto:
  - RepositoryPath = BaseDirectory
  - ModifiedFiles = files to commit
  - CommitMessage = edited message from dialog
  - PushToRemote = checkbox value
  - RemoteName = "origin"
    ↓
CommitChangesCommand.ExecuteAsync(dto, progress)
    ↓
Progress: "Validating Git repository..."
    ↓
IGitService.IsGitRepositoryAsync() [validate again]
    ↓
Progress: "Getting current branch..."
    ↓
IGitService.GetCurrentBranchAsync() → BranchName
    ↓
Progress: "Staging N files..."
    ↓
IGitService.StageFilesAsync(files)
    ↓
For each file: Commands.Stage(repo, file)
    ↓
Progress: "Creating commit..."
    ↓
IGitService.CommitAsync(repoPath, message)
    ↓
repo.Commit() → CommitSha
    ↓
If PushToRemote:
    Progress: "Pushing to origin/branch..."
    ↓
    IGitService.PushAsync(repoPath, "origin", branchName)
    ↓
    repo.Network.Push() with credentials
    ↓
    If push fails → Log error, partial success
    ↓
Return CommitResultDto:
  - CommitSuccess = true
  - PushSuccess = true/false
  - CommitSha = SHA
  - FilesCommitted = count
  - Summary = formatted message
    ↓
Update UI:
  - StatusMessage = result.Summary
  - CanCommit = false (reset)
  - _lastUpdateResults.Clear()
    ↓
Show MessageBox with result.Summary:
  - ✅ "Successfully committed X files and pushed to branch"
  - ⚠️ "Committed X files but push failed: {error}"
  - ❌ "Commit failed: {error}"
```

---

## Key Design Decisions

### 1. Clean Architecture
**Decision**: Use Clean Architecture with feature-based organization

**Rationale**:
- Clear separation of concerns
- Testable business logic (Domain layer pure)
- Easy to add new tools without affecting existing code
- Dependencies flow inward (Infrastructure → Domain ← Application)

**Trade-offs**:
- More files and folders (initial complexity)
- Learning curve for new developers
- Worth it for long-term maintainability

### 2. Result Pattern
**Decision**: Use Result<T> pattern instead of exceptions for expected errors

**Rationale**:
- More explicit error handling
- Better performance (no exception overhead)
- Forces developers to handle errors
- Returns success/failure state with error messages

**Example**:
```csharp
var result = await _projectScanner.ScanProjectsAsync(basePath);
if (!result.IsSuccess)
{
    // Handle error gracefully
    ShowError(result.Error);
    return;
}
var projects = result.Value;
```

### 3. Version Increment Logic in Domain
**Decision**: Place version increment logic in `VersionInfo` value object (Domain)

**Rationale**:
- Core business rule: "Same day +1, new day reset to 1"
- Independent of infrastructure (file system, database, etc.)
- Easy to unit test without mocks
- Can be reused across different contexts

**Implementation**:
```csharp
public VersionInfo Increment(DateTime targetDate)
{
    if (GetDate().Date == targetDate.Date)
        return new VersionInfo { ..., Build = Build + 1 };
    else
        return new VersionInfo { ..., Build = 1 };
}
```

### 4. MVVM with CommunityToolkit.Mvvm
**Decision**: Use CommunityToolkit.Mvvm for MVVM implementation

**Rationale**:
- Source generators reduce boilerplate
- `[ObservableProperty]` and `[RelayCommand]` attributes
- Built-in support for `INotifyPropertyChanged`
- Modern, performant, actively maintained

**Example**:
```csharp
[ObservableProperty]
private string _baseDirectory;

[RelayCommand(CanExecute = nameof(CanScanProjects))]
private async Task ScanProjectsAsync() { ... }
```

### 5. XML Manipulation with XDocument
**Decision**: Use `System.Xml.Linq.XDocument` for .csproj file manipulation

**Rationale**:
- Built-in .NET library (no external dependencies)
- LINQ query support for easy XML navigation
- Preserves formatting when saving
- Better than string manipulation or regex

**Example**:
```csharp
var doc = XDocument.Load(filePath);
var assemblyVersion = doc.Descendants("AssemblyVersion").FirstOrDefault();
assemblyVersion.Value = newVersion;
doc.Save(filePath);
```

### 6. Progress Reporting with IProgress<T>
**Decision**: Use `IProgress<T>` for progress reporting during batch operations

**Rationale**:
- Standard .NET pattern for async progress
- Automatically marshals to UI thread
- Decouples business logic from UI updates
- Easy to mock for testing

**Usage**:
```csharp
var progress = new Progress<(int current, int total)>(update =>
{
    ProgressPercentage = (double)update.current / update.total * 100;
});

await _updateVersionsCommand.ExecuteAsync(projects, DateTime.Now, progress);
```

---

## Testing Strategy

### Unit Tests
**Domain Layer**:
- ✅ `VersionInfo.Parse()` - Valid/invalid formats
- ✅ `VersionInfo.Increment()` - Same day, new day, edge cases
- ✅ `ProjectFile.MatchesFilter()` - Various file names

**Infrastructure Layer**:
- ✅ `ProjectScanner` - Mock file system (Integration tests)
- ✅ `ProjectFileService` - Sample .csproj files (Integration tests)
- ✅ `VersionService` - Parse/format edge cases

**Application Layer**:
- ✅ `ScanProjectsCommand` - Mock dependencies
- ✅ `UpdateVersionsCommand` - Mock services

**Presentation Layer**:
- ✅ `VersionIncreaseViewModel` - Mock application commands and UI logic

### Integration Tests
- ✅ End-to-end: Scan → Update → Verify files (Covered by Infrastructure Tests)
- ✅ Multiple projects in parallel
- ✅ Error scenarios (locked files, invalid XML)

### Manual Testing Checklist
**Core Features**:
- ✅ Browse for directory
- ✅ Scan finds correct .csproj files
- ✅ Filtering works (ETL included, Share excluded)
- ✅ Select All/Deselect All
- ✅ Version increment logic (same day +1, new day reset)
- ✅ Batch update multiple projects
- ✅ Progress bar updates
- ✅ UI remains responsive

**US-1.2.1 Features**:
- ✅ Search filters projects in real-time
- ✅ Search works on FileName, RelativePath, CurrentVersion
- ✅ Clear button (✕) removes filter
- ✅ Filter status text shows "Showing X of Y"
- ✅ Select All affects only filtered projects
- ✅ Last directory loads on startup
- ✅ Directory saves when browsed
- ✅ Directory saves when manually changed
- ✅ Missing appsettings.user.json doesn't crash

**Unit Tests** (US-1.2.1 & US-2.1):
- ✅ `ProjectFilterHelper.Filter()` - Various search terms
- ✅ `AppSettings.Create()` - Validation tests
- ✅ `AppSettings.Validate()` - Invalid paths
- ✅ `SettingsService.LoadAsync()` - Missing file, invalid JSON
- ✅ `SettingsService.SaveAsync()` - IO errors
- ✅ `LoadSettingsCommand` - Error handling
- ✅ `SaveSettingsCommand` - Validation

---

## Known Limitations

1. **No Undo**: Once versions are updated, cannot rollback (user should commit before use)
2. **No Dry-Run**: Cannot preview changes before applying (planned for US-1.5)
3. **No Logging UI Yet**: Console/file logging only (US-1.4 will add UI)
4. **Basic Settings Only**: Only LastDirectory saved, advanced settings in US-1.5
5. **Windows Only**: WPF application, no cross-platform support
6. **Search Performance**: O(n) filter on each keystroke (acceptable for < 1000 projects)

---

## Future Enhancements

**~~US-1.3: Git Integration~~** ✅ Done (2026-02-08)
- ~~Auto-commit with generated message~~ ✅ Implemented
- ~~Push to remote~~ ✅ Implemented
- ~~Dialog for commit message review~~ ✅ Implemented
- Handle merge conflicts - Future enhancement

**US-1.4: Logging UI**
- Real-time log viewer with color coding
- Export logs to file
- Copy/clear logs functionality

**US-1.5: Advanced Settings**
- ~~Persist base directory~~ ✅ Done in US-1.2.1
- Custom file patterns
- Log level configuration
- Git auto-push toggle
- Recent directories dropdown
- Settings dialog UI

**Beyond v1.0**:
- Preview changes before applying
- Undo/rollback last update
- Version history tracking
- Custom version formats
- Parallel processing optimization

---

## Performance Characteristics

**Scan Performance** (100 projects):
- Time: ~2 seconds
- Memory: ~50MB
- Parallel file reading with async/await

**Update Performance** (100 projects):
- Time: ~10-15 seconds (sequential writes)
- Memory: ~70MB
- Progress reporting throttled

**UI Responsiveness**:
- All I/O operations async
- UI thread never blocked
- Smooth progress bar updates

---

**Document Version**: 1.3.0
**Last Updated**: 2026-02-08
**Status**: ✅ Active (US-1.1, US-1.2, US-1.2.1, US-1.3, US-2.1 Completed)
