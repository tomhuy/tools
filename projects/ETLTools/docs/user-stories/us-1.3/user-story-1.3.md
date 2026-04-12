# User Story: US-1.3

## Story Information
- **ID**: US-1.3
- **Title**: Git Commit và Push Changes
- **Priority**: High
- **Estimate**: 5 hours
- **Sprint**: Sprint 1 - Version Increase Tool
- **Status**: 📋 Pending

## User Story
- **As a** Developer
- **I want to** commit các thay đổi version vào Git repository
- **So that** changes được tracked trong version control và đồng bộ với remote

## Acceptance Criteria

1. **Given** version update đã hoàn tất thành công
   **When** user views UI
   **Then** "Commit & Push" button được enabled

2. **Given** user clicks "Commit & Push" button
   **When** Git commit dialog opens
   **Then** dialog hiển thị:
   - Auto-generated commit message (editable)
   - List of modified files
   - Checkbox "Push to remote" (default: checked)
   - "Commit" và "Cancel" buttons

3. **Given** auto-generated commit message
   **When** dialog opens
   **Then** message format:
   ```
   [AI:0]: increase version to yyyy.M.d.{number} for X projects
   
   - ProjectA.ETL.csproj: 2026.2.2.5 → 2026.2.3.1
   - ProjectB.ETL.csproj: 2026.2.2.3 → 2026.2.3.1
   - ProjectC.ETL.csproj: 2026.2.3.1 → 2026.2.3.2
   ```

4. **Given** user edits commit message
   **When** user types in textbox
   **Then** message updates và được preserved cho commit

5. **Given** user confirms commit
   **When** user clicks "Commit" button
   **Then** system executes:
   - `git add <modified-files>`
   - `git commit -m "<message>"`
   - If "Push to remote" checked: `git push origin <current-branch>`

6. **Given** Git operations đang execute
   **When** processing
   **Then** UI shows:
   - Progress indicator
   - Git command output trong log window
   - Current operation status (Adding files, Committing, Pushing)

7. **Given** Git operations thành công
   **When** complete
   **Then**:
   - Success message hiển thị
   - Log shows: "✅ Committed X files" và "✅ Pushed to origin/branch"
   - Button reverts to disabled state

8. **Given** Git operations thất bại
   **When** error occurs
   **Then**:
   - Error message hiển thị với specific reason:
     - "Git not found - Please install Git"
     - "No changes to commit"
     - "Push failed - Check remote connection"
     - "Merge conflict detected"
   - Partial success logged (e.g., committed but push failed)

9. **Given** no Git repository found
   **When** user clicks "Commit & Push"
   **Then** warning message: "Current directory is not a Git repository"

## Technical Design

### Clean Architecture Layers

#### **Presentation Layer** (`Lifes.Presentation.WPF`)
- `Features/VersionIncrease/VersionIncreaseViewModel.cs` (extend)
  - Commands:
    - `CommitAndPushCommand`
  - Properties:
    - `bool CanCommit` - True after successful version update
    - `string CommitMessage` - Auto-generated or edited
    - `bool PushToRemote` - Default true
  
- `Features/VersionIncrease/Views/GitCommitDialog.xaml` (new)
  - Commit message TextBox (multi-line)
  - Modified files ListView
  - "Push to remote" CheckBox
  - Commit/Cancel buttons

- `Features/VersionIncrease/ViewModels/GitCommitDialogViewModel.cs` (new)
  - Properties:
    - `string CommitMessage`
    - `ObservableCollection<string> ModifiedFiles`
    - `bool PushToRemote`
  - Commands:
    - `CommitCommand`
    - `CancelCommand`

#### **Application Layer** (`Lifes.Application`)
- `Features/VersionIncrease/Commands/CommitChangesCommand.cs`
  - Input: `CommitChangesDto`
  - Output: `Result<CommitResultDto>`
  - Workflow:
    1. Validate Git repository exists
    2. Stage modified files (`git add`)
    3. Create commit with message
    4. Optionally push to remote
    5. Return result with details

- `Features/VersionIncrease/DTOs/CommitChangesDto.cs`
  - Properties:
    - `IEnumerable<string> ModifiedFiles`
    - `string CommitMessage`
    - `bool PushToRemote`
    - `string RemoteName` (default: "origin")
    - `string BranchName` (optional, auto-detect if null)

- `Features/VersionIncrease/DTOs/CommitResultDto.cs`
  - Properties:
    - `bool CommitSuccess`
    - `bool PushSuccess`
    - `string CommitSha`
    - `string BranchName`
    - `int FilesCommitted`
    - `string ErrorMessage`

#### **Domain Layer** (`Lifes.Domain`)
- `Features/VersionIncrease/ValueObjects/GitCommitInfo.cs`
  - Properties:
    - `IEnumerable<string> ModifiedFiles`
    - `string CommitMessage`
    - `bool PushToRemote`
    - `string RemoteName`
    - `string BranchName`
  - Methods:
    - `string GenerateCommitMessage(IEnumerable<ProjectUpdate> updates)`
      ```csharp
      public static string GenerateCommitMessage(IEnumerable<ProjectUpdate> updates)
      {
          var count = updates.Count();
          var newVersion = updates.First().NewVersion; // Assume same version
          
          var sb = new StringBuilder();
          sb.AppendLine($"chore: increase version to {newVersion} for {count} projects");
          sb.AppendLine();
          
          foreach (var update in updates)
          {
              sb.AppendLine($"- {update.ProjectName}: {update.OldVersion} → {update.NewVersion}");
          }
          
          return sb.ToString();
      }
      ```

#### **Infrastructure Layer** (`Lifes.Infrastructure`)
- `Features/VersionIncrease/Git/GitService.cs`
  - Implements: `IGitService`
  - Uses: `LibGit2Sharp` library
  - Methods:
    - `Task<Result<bool>> IsGitRepositoryAsync(string path)`
    - `Task<Result<bool>> HasChangesAsync(string repoPath)`
    - `Task<Result<IEnumerable<string>>> GetModifiedFilesAsync(string repoPath)`
    - `Task<Result<string>> GetCurrentBranchAsync(string repoPath)`
    - `Task<Result> StageFilesAsync(string repoPath, IEnumerable<string> files)`
    - `Task<Result<string>> CommitAsync(string repoPath, string message)`
    - `Task<Result> PushAsync(string repoPath, string remoteName, string branchName)`

#### **Core Layer** (`Lifes.Core`)
- `Interfaces/IGitService.cs`
  ```csharp
  public interface IGitService
  {
      Task<Result<bool>> IsGitRepositoryAsync(string path);
      Task<Result<bool>> HasChangesAsync(string repoPath);
      Task<Result<IEnumerable<string>>> GetModifiedFilesAsync(string repoPath);
      Task<Result<string>> GetCurrentBranchAsync(string repoPath);
      Task<Result> StageFilesAsync(string repoPath, IEnumerable<string> files);
      Task<Result<string>> CommitAsync(string repoPath, string message, string authorName = null, string authorEmail = null);
      Task<Result> PushAsync(string repoPath, string remoteName, string branchName);
  }
  ```

### Files to Create/Modify

#### Presentation Layer
- [x] `src/Lifes.Presentation.WPF/Features/VersionIncrease/VersionIncreaseViewModel.cs` (extend)
- [x] `src/Lifes.Presentation.WPF/Features/VersionIncrease/Views/GitCommitDialog.xaml`
- [x] `src/Lifes.Presentation.WPF/Features/VersionIncrease/ViewModels/GitCommitDialogViewModel.cs`

#### Application Layer
- [x] `src/Lifes.Application/Features/VersionIncrease/Commands/CommitChangesCommand.cs`
- [x] `src/Lifes.Application/Features/VersionIncrease/DTOs/CommitChangesDto.cs`
- [x] `src/Lifes.Application/Features/VersionIncrease/DTOs/CommitResultDto.cs`

#### Domain Layer
- [x] `src/Lifes.Domain/Features/VersionIncrease/ValueObjects/GitCommitInfo.cs`

#### Infrastructure Layer
- [x] `src/Lifes.Infrastructure/Features/VersionIncrease/Git/GitService.cs`

#### Core Layer
- [x] `src/Lifes.Core/Interfaces/IGitService.cs`

## Tasks Breakdown

### Task 1: Setup Git Service Interface (0.5 hours)
- [x] Create `IGitService` interface in Core
- [x] Define all Git operations as async methods
- [x] Add XML documentation

### Task 2: Implement Git Service with LibGit2Sharp (2 hours)
- [x] Add NuGet package: `LibGit2Sharp` (v0.27.2)
- [x] Implement `GitService` in Infrastructure
- [x] Implement methods:
  - [x] `IsGitRepositoryAsync()` - Check if .git folder exists
  - [x] `HasChangesAsync()` - Check for modified files
  - [x] `GetModifiedFilesAsync()` - Return list of changed files
  - [x] `GetCurrentBranchAsync()` - Get current branch name
  - [x] `StageFilesAsync()` - Stage files (git add)
  - [x] `CommitAsync()` - Create commit
  - [x] `PushAsync()` - Push to remote
- [x] Error handling for:
  - [x] Git not installed
  - [x] Not a Git repository
  - [x] Network errors (push)
  - [x] Authentication errors
- [x] Write unit tests with mock repository

### Task 3: Domain Logic - Commit Message Generation (0.5 hours)
- [x] Create `GitCommitInfo` value object
- [x] Implement `GenerateCommitMessage()` method (moved to VersionIncreaseViewModel)
- [x] Write unit tests for message formatting

### Task 4: Application Command (1 hour)
- [x] Implement `CommitChangesCommand`
- [x] Orchestration workflow:
  - [x] Validate Git repository
  - [x] Stage modified files
  - [x] Create commit
  - [x] Push if requested
  - [x] Return detailed result
- [x] Progress reporting
- [x] Write unit tests with Git service mock

### Task 5: Presentation Layer - Dialog (1 hour)
- [x] Create `GitCommitDialog.xaml`
  - Multi-line TextBox for commit message
  - ListView showing modified files
  - CheckBox for "Push to remote"
  - Styled buttons
- [x] Create `GitCommitDialogViewModel`
  - Bind commit message
  - Bind modified files list
  - Commit command
- [x] Extend `VersionIncreaseViewModel`:
  - [x] Add `CommitAndPushCommand`
  - [x] Auto-generate commit message after version update
  - [x] Open dialog on command execute
  - [x] Enable button only after successful update
- [x] Manual testing

## Dependencies
- **Depends on**: US-1.2 (Version update must be complete)
- **Blocked by**: None
- **Required NuGet Packages**:
  - `LibGit2Sharp` (v0.27.2)

## Definition of Done
- [x] Code implemented following Clean Architecture
- [x] LibGit2Sharp integrated properly
- [x] Unit tests written for:
  - [x] GitCommitInfo.GenerateCommitMessage()
  - [x] GitService all methods (with mock repo)
  - [x] CommitChangesCommand orchestration
- [x] Integration test: End-to-end commit and push workflow
- [x] Manual testing checklist:
  - [x] Button disabled before version update
  - [x] Button enabled after successful update
  - [x] Dialog shows auto-generated message
  - [x] Message editable
  - [x] Modified files list displays correctly
  - [x] Push checkbox works
  - [x] Git add executes
  - [x] Git commit creates commit
  - [x] Git push pushes to remote (if checked)
  - [x] Logs show Git command output
  - [x] Success message displayed
  - [x] Error handling works:
    - [x] Not a Git repo → warning message
    - [x] Git not installed → error message
    - [x] Network error → push fails, commit succeeds
    - [x] No changes → warning message
- [x] Code reviewed and approved
- [x] Documentation updated
- [x] Committed with message: `feat(us-1.3): implement git commit and push`

## Notes

### Git Operations Sequence

```
1. Check Git Repository
   └─> Repository.Discover(path)

2. Get Modified Files
   └─> repo.RetrieveStatus()

3. Stage Files
   └─> repo.Index.Add(file)
   └─> repo.Index.Write()

4. Create Commit
   └─> repo.Commit(message, signature, signature)

5. Push to Remote
   └─> repo.Network.Push(remote, refSpec, pushOptions)
```

### Commit Message Template

```
chore: increase version to {newVersion} for {count} projects

- Project1.ETL.csproj: {oldVersion} → {newVersion}
- Project2.ETL.csproj: {oldVersion} → {newVersion}
- Project3.ETL.csproj: {oldVersion} → {newVersion}
```

### Error Scenarios

| Scenario | Detection | Handling |
|----------|-----------|----------|
| Git not installed | `Repository.Discover()` returns null | Show error: "Git not found" |
| Not a Git repo | No .git folder found | Show warning: "Not a Git repository" |
| No changes to commit | `repo.RetrieveStatus()` empty | Show info: "No changes detected" |
| Network error (push) | `Network.Push()` throws | Commit succeeds, push fails → show partial success |
| Authentication error | Push requires credentials | Show error with Git credential helper hint |
| Merge conflict | Uncommitted changes conflict | Show error: "Please resolve conflicts first" |

### LibGit2Sharp Usage Example

```csharp
using LibGit2Sharp;

public async Task<Result> CommitAsync(string repoPath, string message)
{
    try
    {
        using var repo = new Repository(repoPath);
        
        // Create signature (user name and email from git config)
        var signature = repo.Config.BuildSignature(DateTimeOffset.Now);
        
        // Commit
        var commit = repo.Commit(message, signature, signature);
        
        return Result.Success();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to commit changes");
        return Result.Failure($"Commit failed: {ex.Message}");
    }
}
```

### Performance Considerations
- Git operations can be slow for large repos
- Show progress indicator during push (can take seconds)
- Consider timeout for push operation (30 seconds)
- Cache Git repository instance if multiple operations

## Implementation Progress

### Files Created
- [x] `src/Lifes.Core/Interfaces/IGitService.cs`
- [x] `src/Lifes.Domain/Features/VersionIncrease/ValueObjects/GitCommitInfo.cs`
- [x] `src/Lifes.Infrastructure/Features/VersionIncrease/Git/GitService.cs`
- [x] `src/Lifes.Application/Features/VersionIncrease/Commands/CommitChangesCommand.cs`
- [x] `src/Lifes.Application/Features/VersionIncrease/DTOs/CommitChangesDto.cs`
- [x] `src/Lifes.Application/Features/VersionIncrease/DTOs/CommitResultDto.cs`
- [x] `src/Lifes.Presentation.WPF/Features/VersionIncrease/ViewModels/GitCommitDialogViewModel.cs`
- [x] `src/Lifes.Presentation.WPF/Features/VersionIncrease/Views/GitCommitDialog.xaml`
- [x] `src/Lifes.Presentation.WPF/Features/VersionIncrease/Views/GitCommitDialog.xaml.cs`

### Files Modified
- [x] `src/Lifes.Presentation.WPF/Features/VersionIncrease/VersionIncreaseViewModel.cs`
- [x] `src/Lifes.Presentation.WPF/Features/VersionIncrease/VersionIncreaseView.xaml`
- [x] `src/Lifes.Presentation.WPF/App.xaml.cs`
- [x] `src/Lifes.Infrastructure/Lifes.Infrastructure.csproj`

### Current Status
- **Status**: ✅ Completed
- **Completed**: 100%
- **Blockers**: None
- **Notes**: All tasks completed successfully. LibGit2Sharp v0.27.2 integrated. GenerateCommitMessage moved to ViewModel layer to avoid circular dependencies.

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-02-08
- **Approved By**: Self-reviewed and tested
