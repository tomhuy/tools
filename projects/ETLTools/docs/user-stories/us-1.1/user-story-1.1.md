# User Story: US-1.1

## Story Information
- **ID**: US-1.1
- **Title**: Load và Hiển thị Danh Sách Projects
- **Priority**: High
- **Estimate**: 8 hours
- **Sprint**: Sprint 1 - Version Increase Tool
- **Status**: 📋 Pending

## User Story
- **As a** Developer
- **I want to** xem danh sách các .csproj files cần tăng version
- **So that** tôi có thể chọn projects nào cần update

## Acceptance Criteria

1. **Given** user mở Version Increase Tool
   **When** user chọn base directory và click "Scan Projects"
   **Then** system scans tất cả .csproj files trong directory và subdirectories

2. **Given** system đang scan projects
   **When** tìm thấy .csproj files
   **Then** system filters theo điều kiện:
   - File name phải kết thúc bằng "ETL" (vd: `MyProject.ETL.csproj`)
   - File extension phải là `.csproj`
   - File name không được bắt đầu bằng "Share" (case-insensitive)

3. **Given** scan hoàn tất
   **When** hiển thị kết quả
   **Then** ListView/DataGrid shows:
   - Checkbox để select từng project
   - Project name (filename)
   - Current version (đọc từ AssemblyVersion tag)
   - Status (Ready/Processing/Updated)
   - File path (relative path)

4. **Given** danh sách projects được hiển thị
   **When** user clicks "Select All"
   **Then** tất cả checkboxes được chọn

5. **Given** có projects được chọn
   **When** user clicks "Deselect All"
   **Then** tất cả checkboxes bị bỏ chọn

6. **Given** scan hoàn tất
   **When** hiển thị summary
   **Then** shows:
   - "Found: X projects"
   - "Selected: Y projects"

## Technical Design

### Clean Architecture Layers

#### **Presentation Layer** (`Lifes.Presentation.WPF`)
- `Features/VersionIncrease/VersionIncreaseView.xaml`
  - Base directory TextBox with Browse button
  - "Scan Projects", "Select All", "Deselect All" buttons
  - DataGrid to display project list
  - Status text showing found/selected count
  
- `Features/VersionIncrease/VersionIncreaseViewModel.cs`
  - Properties:
    - `string BaseDirectory`
    - `ObservableCollection<ProjectFileViewModel> ProjectFiles`
    - `int FoundCount`
    - `int SelectedCount`
  - Commands:
    - `ScanProjectsCommand`
    - `SelectAllCommand`
    - `DeselectAllCommand`
    - `BrowseDirectoryCommand`
  
- `Features/VersionIncrease/Models/ProjectFileViewModel.cs`
  - Properties:
    - `bool IsSelected`
    - `string FileName`
    - `string CurrentVersion`
    - `string Status`
    - `string RelativePath`
    - `string FullPath`

#### **Application Layer** (`Lifes.Application`)
- `Features/VersionIncrease/Commands/ScanProjectsCommand.cs`
  - Input: `string basePath`
  - Output: `Result<IEnumerable<ProjectFileDto>>`
  - Orchestrates scanning workflow:
    1. Validate base path
    2. Call ProjectScanner service
    3. Read version from each project
    4. Map to DTOs
  
- `Features/VersionIncrease/Queries/GetProjectVersionQuery.cs`
  - Input: `string projectFilePath`
  - Output: `Result<string>` (version string)
  
- `Features/VersionIncrease/DTOs/ProjectFileDto.cs`
  - Properties:
    - `string FileName`
    - `string FullPath`
    - `string RelativePath`
    - `string CurrentVersion`

#### **Domain Layer** (`Lifes.Domain`)
- `Features/VersionIncrease/Entities/ProjectFile.cs`
  - Pure domain entity representing a project file
  - Properties:
    - `string FileName`
    - `string FullPath`
    - `string RelativePath`
    - `VersionInfo CurrentVersion`
  - Methods:
    - `bool MatchesFilter()` - Business rule for filtering

- `Features/VersionIncrease/ValueObjects/VersionInfo.cs`
  - Value object representing version number
  - Properties:
    - `int Year`
    - `int Month`
    - `int Day`
    - `int Build`
  - Methods:
    - `static VersionInfo Parse(string version)`
    - `string ToString()` -> "yyyy.M.d.{build}"
    - `DateTime GetDate()`

- `Features/VersionIncrease/Enums/ProjectStatus.cs`
  - `Ready`, `Processing`, `Updated`, `Failed`, `Skipped`

#### **Infrastructure Layer** (`Lifes.Infrastructure`)
- `Features/VersionIncrease/Services/ProjectScanner.cs`
  - Implements: `IProjectScanner`
  - Scans file system for .csproj files
  - Filters based on naming conventions
  - Returns list of matching project files
  
- `Features/VersionIncrease/Services/ProjectFileService.cs`
  - Implements: `IProjectFileService`
  - Reads .csproj XML file
  - Parses `<AssemblyVersion>` tag
  - Returns version string

#### **Core Layer** (`Lifes.Core`)
- `Interfaces/IProjectScanner.cs`
  ```csharp
  public interface IProjectScanner
  {
      Task<Result<IEnumerable<ProjectFile>>> ScanProjectsAsync(
          string basePath,
          string filePattern = "*ETL.csproj",
          string excludePattern = "Share*"
      );
  }
  ```

- `Interfaces/IProjectFileService.cs`
  ```csharp
  public interface IProjectFileService
  {
      Task<Result<string>> ReadVersionAsync(string filePath);
      Task<Result> UpdateVersionAsync(string filePath, string newVersion);
  }
  ```

- `Models/Result.cs`
  - Result pattern for error handling
  - `Result` (no value)
  - `Result<T>` (with value)

### Files to Create/Modify

#### Presentation Layer
- [ ] `src/Lifes.Presentation.WPF/Features/VersionIncrease/VersionIncreaseView.xaml`
- [ ] `src/Lifes.Presentation.WPF/Features/VersionIncrease/VersionIncreaseViewModel.cs`
- [ ] `src/Lifes.Presentation.WPF/Features/VersionIncrease/Models/ProjectFileViewModel.cs`

#### Application Layer
- [ ] `src/Lifes.Application/Features/VersionIncrease/Commands/ScanProjectsCommand.cs`
- [ ] `src/Lifes.Application/Features/VersionIncrease/Queries/GetProjectVersionQuery.cs`
- [ ] `src/Lifes.Application/Features/VersionIncrease/DTOs/ProjectFileDto.cs`

#### Domain Layer
- [ ] `src/Lifes.Domain/Features/VersionIncrease/Entities/ProjectFile.cs`
- [ ] `src/Lifes.Domain/Features/VersionIncrease/ValueObjects/VersionInfo.cs`
- [ ] `src/Lifes.Domain/Features/VersionIncrease/Enums/ProjectStatus.cs`

#### Infrastructure Layer
- [ ] `src/Lifes.Infrastructure/Features/VersionIncrease/Services/ProjectScanner.cs`
- [ ] `src/Lifes.Infrastructure/Features/VersionIncrease/Services/ProjectFileService.cs`

#### Core Layer
- [ ] `src/Lifes.Core/Interfaces/IProjectScanner.cs`
- [ ] `src/Lifes.Core/Interfaces/IProjectFileService.cs`
- [ ] `src/Lifes.Core/Models/Result.cs`

## Tasks Breakdown

### Task 1: Setup Core Interfaces (2 hours)
- [ ] Create `IProjectScanner` interface
- [ ] Create `IProjectFileService` interface
- [ ] Implement `Result` and `Result<T>` pattern
- [ ] Add XML documentation

### Task 2: Domain Layer Implementation (1.5 hours)
- [ ] Create `ProjectFile` entity with business rules
- [ ] Create `VersionInfo` value object with parsing logic
- [ ] Create `ProjectStatus` enum
- [ ] Write unit tests for domain logic

### Task 3: Infrastructure Services (2 hours)
- [ ] Implement `ProjectScanner` service
  - Directory scanning with recursive search
  - File filtering logic (EndsWith "ETL", not StartsWith "Share")
  - Error handling
- [ ] Implement `ProjectFileService`
  - XML parsing using XDocument
  - Read AssemblyVersion tag
  - Error handling for malformed XML
- [ ] Write unit tests with mock file system

### Task 4: Application Commands/Queries (1 hour)
- [ ] Implement `ScanProjectsCommand`
  - Orchestrate scanning workflow
  - Map domain entities to DTOs
- [ ] Implement `GetProjectVersionQuery`
- [ ] Write unit tests

### Task 5: Presentation Layer (1.5 hours)
- [ ] Create `ProjectFileViewModel` model
- [ ] Implement `VersionIncreaseViewModel`
  - Bind to ScanProjectsCommand
  - Implement SelectAll/DeselectAll logic
  - Update counts reactively
- [ ] Create `VersionIncreaseView.xaml`
  - Base directory section with Browse button
  - Action buttons row
  - DataGrid with columns
  - Status text
- [ ] Apply ModernWPF styling
- [ ] Manual testing

## Dependencies
- Depends on: None (First user story)
- Blocked by: None
- Required NuGet Packages:
  - `CommunityToolkit.Mvvm` (v8.3.2)
  - `ModernWPF` (v0.9.6)
  - `Microsoft.Extensions.DependencyInjection` (v7.0.0)
  - `Serilog` (v4.3.0)

## Definition of Done
- [x] Code implemented following Clean Architecture
- [ ] All files created and organized by feature
- [ ] Dependencies flow inward (Presentation → Application → Domain ← Infrastructure)
- [ ] Unit tests written for:
  - [ ] Domain logic (VersionInfo parsing, ProjectFile filtering)
  - [ ] Application commands
  - [ ] Infrastructure services (with mocks)
- [ ] Integration test: End-to-end scan workflow
- [ ] Manual testing checklist:
  - [ ] Browse button opens folder dialog
  - [ ] Scan finds correct .csproj files
  - [ ] Files ending with "ETL" are included
  - [ ] Files starting with "Share" are excluded
  - [ ] Current version displayed correctly
  - [ ] Select All/Deselect All works
  - [ ] Counts update correctly
  - [ ] UI responsive during scan
- [ ] Code reviewed and approved
- [ ] Documentation updated
- [ ] Committed with message: `feat(us-1.1): implement project scanning and display`

## Notes

### Filter Logic Examples
✅ **Included:**
- `MyProject.ETL.csproj`
- `DataSync.ETL.csproj`
- `TransformETL.csproj` (ends with ETL)

❌ **Excluded:**
- `ShareUtil.ETL.csproj` (starts with "Share")
- `SHAREDLIB.ETL.csproj` (starts with "share", case-insensitive)
- `MyProject.csproj` (doesn't end with ETL)
- `MyProject.API.csproj` (doesn't end with ETL)

### Performance Considerations
- Use `Parallel.ForEachAsync` for reading versions from multiple files
- Implement cancellation token support for long scans
- Show progress bar if scanning > 100 files
- ListView virtualization enabled for large lists

### UI Behavior
- Disable "Scan" button while scanning
- Show spinner/progress indicator during scan
- Display error message if directory not found
- Remember last used directory in settings

## Implementation Progress

### Files Created
- [ ] (None yet)

### Current Status
- **Status**: 📋 Pending
- **Completed**: 0%
- **Blockers**: None
- **Notes**: Ready to start implementation

## Final Status
- **Status**: 📋 Pending
- **Completed Date**: TBD
- **Approved By**: TBD
