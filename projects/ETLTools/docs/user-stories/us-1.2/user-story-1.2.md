# User Story: US-1.2

## Story Information
- **ID**: US-1.2
- **Title**: Tăng Version Number Tự Động
- **Priority**: High
- **Estimate**: 6 hours
- **Sprint**: Sprint 1 - Version Increase Tool
- **Status**: 📋 Pending

## User Story
- **As a** Developer
- **I want to** tăng version number cho các projects đã chọn
- **So that** version được update nhất quán theo chuẩn công ty

## Acceptance Criteria

1. **Given** user đã chọn ít nhất 1 project từ danh sách
   **When** user clicks "Increase Version" button
   **Then** confirmation dialog hiển thị với thông tin:
   - Số lượng projects sẽ được update
   - New version format: `yyyy.M.d.{auto}`

2. **Given** user confirms version update
   **When** system processes each selected project
   **Then** version được tăng theo logic:
   - **Format**: `yyyy.M.d.{number}`
     - `yyyy`: Năm hiện tại (vd: 2026)
     - `M`: Tháng hiện tại không có leading zero (vd: 2)
     - `d`: Ngày hiện tại không có leading zero (vd: 3)
     - `{number}`: Số thứ tự tăng dần trong ngày
   
3. **Given** current version có cùng ngày với hôm nay
   **When** increment version
   **Then** tăng `{number}` lên 1
   - Example: `2026.2.3.1` → `2026.2.3.2`

4. **Given** current version khác ngày với hôm nay
   **When** increment version
   **Then** reset về số đầu tiên của ngày mới
   - Example: `2026.2.2.5` → `2026.2.3.1` (today is Feb 3)

5. **Given** system đang update version
   **When** modifying .csproj file
   **Then** update cả 2 tags với cùng giá trị:
   - `<AssemblyVersion>yyyy.M.d.{number}</AssemblyVersion>`
   - `<FileVersion>yyyy.M.d.{number}</FileVersion>`
   - Preserve XML formatting và indentation

6. **Given** update đang progress
   **When** processing files
   **Then** UI shows:
   - Progress bar (X of Y projects)
   - Current file being processed
   - Log entry cho mỗi file: "Updated ProjectName: oldVersion → newVersion"

7. **Given** all updates hoàn tất
   **When** process completes
   **Then** summary hiển thị:
   - "Updated 5 of 5 projects successfully"
   - DataGrid cập nhật với new versions
   - "Commit & Push" button enabled

8. **Given** không có projects nào được chọn
   **When** user views "Increase Version" button
   **Then** button is disabled

## Technical Design

### Clean Architecture Layers

#### **Presentation Layer** (`Lifes.Presentation.WPF`)
- `Features/VersionIncrease/VersionIncreaseViewModel.cs` (extend existing)
  - Commands:
    - `IncreaseVersionCommand` - Main update command
  - Properties:
    - `int ProcessedCount`
    - `int TotalCount`
    - `double ProgressPercentage`
    - `bool IsProcessing`
  - Methods:
    - `async Task IncreaseVersionAsync()`
    - `bool CanIncreaseVersion()` - At least 1 selected

#### **Application Layer** (`Lifes.Application`)
- `Features/VersionIncrease/Commands/UpdateVersionsCommand.cs`
  - Input: `IEnumerable<ProjectFileDto>` (selected projects)
  - Output: `Result<VersionUpdateResultDto>`
  - Workflow:
    1. Validate selected projects list
    2. Get current date
    3. For each project:
       - Read current version
       - Calculate new version
       - Update .csproj file
       - Log result
    4. Return summary
  
- `Features/VersionIncrease/DTOs/VersionUpdateResultDto.cs`
  - Properties:
    - `int TotalProjects`
    - `int SuccessCount`
    - `int FailedCount`
    - `List<ProjectUpdateDto> Updates`

- `Features/VersionIncrease/DTOs/ProjectUpdateDto.cs`
  - Properties:
    - `string ProjectName`
    - `string OldVersion`
    - `string NewVersion`
    - `bool Success`
    - `string ErrorMessage`

#### **Domain Layer** (`Lifes.Domain`)
- `Features/VersionIncrease/ValueObjects/VersionInfo.cs` (extend existing)
  - Methods:
    - `VersionInfo Increment(DateTime targetDate)` - Core business logic
      ```csharp
      public VersionInfo Increment(DateTime targetDate)
      {
          var currentDate = new DateTime(Year, Month, Day);
          
          if (currentDate.Date == targetDate.Date)
          {
              // Same day: increment build number
              return new VersionInfo
              {
                  Year = targetDate.Year,
                  Month = targetDate.Month,
                  Day = targetDate.Day,
                  Build = Build + 1
              };
          }
          else
          {
              // Different day: reset to 1
              return new VersionInfo
              {
                  Year = targetDate.Year,
                  Month = targetDate.Month,
                  Day = targetDate.Day,
                  Build = 1
              };
          }
      }
      ```
    - `bool IsSameDate(DateTime date)`

#### **Infrastructure Layer** (`Lifes.Infrastructure`)
- `Features/VersionIncrease/Services/VersionService.cs`
  - Implements: `IVersionService`
  - Methods:
    - `Result<VersionInfo> ParseVersion(string versionString)`
    - `Result<string> FormatVersion(VersionInfo version)`
    - `Result<VersionInfo> IncrementVersion(VersionInfo current, DateTime targetDate)`

- `Features/VersionIncrease/Services/ProjectFileService.cs` (extend existing)
  - Methods:
    - `async Task<Result> UpdateVersionAsync(string filePath, string newVersion)`
      - Load XML using XDocument
      - Find `<AssemblyVersion>` and `<FileVersion>` tags
      - Update both tags with new version
      - Preserve formatting
      - Save file

#### **Core Layer** (`Lifes.Core`)
- `Interfaces/IVersionService.cs`
  ```csharp
  public interface IVersionService
  {
      Result<VersionInfo> ParseVersion(string versionString);
      Result<string> FormatVersion(VersionInfo version);
      Result<VersionInfo> IncrementVersion(VersionInfo current, DateTime targetDate);
  }
  ```

### Files to Create/Modify

#### Presentation Layer
- [x] `src/Lifes.Presentation.WPF/Features/VersionIncrease/VersionIncreaseViewModel.cs` (extend)

#### Application Layer
- [ ] `src/Lifes.Application/Features/VersionIncrease/Commands/UpdateVersionsCommand.cs`
- [ ] `src/Lifes.Application/Features/VersionIncrease/DTOs/VersionUpdateResultDto.cs`
- [ ] `src/Lifes.Application/Features/VersionIncrease/DTOs/ProjectUpdateDto.cs`

#### Domain Layer
- [x] `src/Lifes.Domain/Features/VersionIncrease/ValueObjects/VersionInfo.cs` (extend)

#### Infrastructure Layer
- [ ] `src/Lifes.Infrastructure/Features/VersionIncrease/Services/VersionService.cs`
- [x] `src/Lifes.Infrastructure/Features/VersionIncrease/Services/ProjectFileService.cs` (extend)

#### Core Layer
- [ ] `src/Lifes.Core/Interfaces/IVersionService.cs`

## Tasks Breakdown

### Task 1: Domain Logic - Version Increment (1.5 hours)
- [ ] Extend `VersionInfo` value object with `Increment()` method
- [ ] Implement business rules:
  - Same day: increment build number
  - Different day: reset to 1
- [ ] Write comprehensive unit tests:
  - [ ] Same day increment: `2026.2.3.1` → `2026.2.3.2`
  - [ ] Different day reset: `2026.2.2.5` → `2026.2.3.1`
  - [ ] Edge cases: last day of month, last day of year

### Task 2: Version Service (1 hour)
- [ ] Create `IVersionService` interface in Core
- [ ] Implement `VersionService` in Infrastructure
- [ ] Implement methods:
  - [ ] `ParseVersion()` - Parse "yyyy.M.d.{n}" format
  - [ ] `FormatVersion()` - Format VersionInfo to string
  - [ ] `IncrementVersion()` - Delegate to domain logic
- [ ] Write unit tests for parsing edge cases

### Task 3: Project File Update Service (1.5 hours)
- [ ] Extend `ProjectFileService` with `UpdateVersionAsync()`
- [ ] Implement XML manipulation:
  - [ ] Load .csproj using XDocument
  - [ ] Find PropertyGroup containing version tags
  - [ ] Update `<AssemblyVersion>` tag
  - [ ] Update `<FileVersion>` tag
  - [ ] Preserve indentation and formatting
  - [ ] Save file atomically
- [ ] Error handling:
  - [ ] File not found
  - [ ] Invalid XML
  - [ ] Missing version tags
  - [ ] File locked/read-only
- [ ] Write unit tests with sample .csproj files

### Task 4: Update Versions Command (1 hour)
- [ ] Implement `UpdateVersionsCommand` in Application layer
- [ ] Orchestration workflow:
  - [ ] Validate input
  - [ ] Get current date (injectable for testing)
  - [ ] Process each project in parallel (with semaphore)
  - [ ] Collect results
  - [ ] Return summary DTO
- [ ] Progress reporting using IProgress<T>
- [ ] Write unit tests with mocks

### Task 5: ViewModel Integration (1 hour)
- [ ] Add `IncreaseVersionCommand` to ViewModel
- [ ] Implement progress tracking:
  - [ ] Update `ProcessedCount` reactively
  - [ ] Update `ProgressPercentage`
  - [ ] Update DataGrid with new versions
- [ ] Implement confirmation dialog
- [ ] Implement summary message
- [ ] Disable button logic (CanExecute)
- [ ] Manual testing

## Dependencies
- **Depends on**: US-1.1 (Project scanning must be complete)
- **Blocked by**: None
- **Required**: Same NuGet packages as US-1.1

## Definition of Done
- [ ] Code implemented following Clean Architecture
- [ ] Domain logic isolated and pure (no external dependencies)
- [ ] Unit tests written for:
  - [ ] VersionInfo.Increment() with all scenarios
  - [ ] VersionService parsing and formatting
  - [ ] ProjectFileService.UpdateVersionAsync()
  - [ ] UpdateVersionsCommand orchestration
- [ ] Integration test: End-to-end version update workflow
- [ ] Manual testing checklist:
  - [ ] Button disabled when no selection
  - [ ] Confirmation dialog shows correct info
  - [ ] Same-day increment works: 1→2, 5→6
  - [ ] New-day reset works: 2026.2.2.5 → 2026.2.3.1
  - [ ] Both AssemblyVersion and FileVersion updated
  - [ ] XML formatting preserved
  - [ ] Progress bar updates smoothly
  - [ ] Logs show old→new version for each file
  - [ ] Summary displays correct counts
  - [ ] DataGrid updates with new versions
  - [ ] Handles errors gracefully (locked files, etc.)
- [ ] Code reviewed and approved
- [ ] Documentation updated
- [ ] Committed with message: `feat(us-1.2): implement automatic version increment`

## Notes

### Version Format Logic

**Format**: `yyyy.M.d.{number}`

**Examples**:
```
// Today is 2026-02-03

// Scenario 1: Same day increment
Current: 2026.2.3.1
New:     2026.2.3.2

Current: 2026.2.3.5
New:     2026.2.3.6

// Scenario 2: Different day reset
Current: 2026.2.2.5    (yesterday)
New:     2026.2.3.1    (today)

Current: 2026.1.15.3   (last month)
New:     2026.2.3.1    (today)

// Scenario 3: First version of the day
Current: 2025.12.31.10 (last year)
New:     2026.2.3.1    (today)
```

### XML Update Example

**Before** (`MyProject.ETL.csproj`):
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <AssemblyVersion>2026.2.2.5</AssemblyVersion>
    <FileVersion>2026.2.2.5</FileVersion>
  </PropertyGroup>
</Project>
```

**After** (preserving formatting):
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <AssemblyVersion>2026.2.3.1</AssemblyVersion>
    <FileVersion>2026.2.3.1</FileVersion>
  </PropertyGroup>
</Project>
```

### Performance Considerations
- Process files in parallel with max concurrency limit (e.g., 10)
- Use async/await for file I/O
- Implement cancellation token support
- Progress reporting throttled (max 10 updates/second)

### Error Handling Strategy
- Continue processing other files if one fails
- Collect all errors in summary
- Display individual file errors in log
- Don't rollback successful updates on partial failure

## Implementation Progress

### Files Created
- [ ] (None yet)

### Current Status
- **Status**: 📋 Pending
- **Completed**: 0%
- **Blockers**: Requires US-1.1 to be completed first
- **Notes**: Ready to start after US-1.1

## Final Status
- **Status**: 📋 Pending
- **Completed Date**: TBD
- **Approved By**: TBD
