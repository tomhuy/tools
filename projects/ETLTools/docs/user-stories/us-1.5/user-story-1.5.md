# User Story: US-1.5

## Story Information
- **ID**: US-1.5
- **Title**: Settings và Configuration
- **Priority**: Medium
- **Estimate**: 3 hours
- **Sprint**: Sprint 1 - Version Increase Tool (Enhancement)
- **Status**: ⏳ Future Enhancement

## User Story
- **As a** Developer
- **I want to** configure tool settings theo preferences của tôi
- **So that** tool hoạt động phù hợp với workflow và environment của tôi

## Acceptance Criteria

1. **Given** user opens application
   **When** clicking Settings icon/button
   **Then** Settings dialog/panel opens

2. **Given** Settings dialog open
   **When** displaying settings
   **Then** shows configurable options:
   - **Base Directory**: Default directory to scan (with Browse button)
   - **File Filter Pattern**: Pattern for .csproj files (default: `*ETL.csproj`)
   - **Exclude Pattern**: Pattern to exclude files (default: `Share*`)
   - **Git Auto-Push**: Automatically push after commit (default: `true`)
   - **Log Level**: Minimum log level (Verbose, Debug, Information, Warning, Error)
   - **Log Retention Days**: Number of days to keep log files (default: 30)

3. **Given** user modifies settings
   **When** user changes values
   **Then** UI validates input:
   - Base directory must exist
   - Patterns must be valid glob patterns
   - Log retention must be 1-365 days

4. **Given** user clicks "Save" button
   **When** saving settings
   **Then**:
   - Settings saved to `appsettings.user.json`
   - Success message displayed
   - Settings applied immediately (no restart required)

5. **Given** user clicks "Reset to Defaults" button
   **When** confirming reset
   **Then**:
   - All settings reset to default values
   - `appsettings.user.json` deleted or cleared
   - UI updates with defaults

6. **Given** application starts
   **When** loading
   **Then**:
   - Load settings from `appsettings.user.json` if exists
   - Fallback to defaults from `appsettings.json` if not found
   - Display loaded settings in UI

7. **Given** settings loaded
   **When** user uses Version Increase Tool
   **Then**:
   - Base directory pre-populated with saved value
   - File scanning uses configured patterns
   - Git operations follow auto-push setting
   - Logging respects configured level

## Technical Design

### Clean Architecture Layers

#### **Presentation Layer** (`Lifes.Presentation.WPF`)
- `Shared/Views/SettingsDialog.xaml`
  - Base directory TextBox + Browse button
  - File pattern TextBox
  - Exclude pattern TextBox
  - Git auto-push CheckBox
  - Log level ComboBox
  - Log retention NumberBox
  - Save, Cancel, Reset buttons
  
- `Shared/ViewModels/SettingsViewModel.cs`
  - Properties (bound to UI):
    - `string BaseDirectory`
    - `string FilePattern`
    - `string ExcludePattern`
    - `bool GitAutoPush`
    - `LogLevel LogLevel`
    - `int LogRetentionDays`
  - Commands:
    - `SaveCommand`
    - `CancelCommand`
    - `ResetCommand`
    - `BrowseDirectoryCommand`
  - Methods:
    - `LoadSettings()`
    - `SaveSettings()`
    - `ResetToDefaults()`
    - `ValidateSettings()`

#### **Application Layer** (`Lifes.Application`)
- `Common/Commands/LoadSettingsCommand.cs`
  - Input: None
  - Output: `Result<AppSettingsDto>`
  
- `Common/Commands/SaveSettingsCommand.cs`
  - Input: `AppSettingsDto`
  - Output: `Result`

- `Common/DTOs/AppSettingsDto.cs`
  - Properties:
    - `string BaseDirectory`
    - `string FilePattern`
    - `string ExcludePattern`
    - `bool GitAutoPush`
    - `string LogLevel`
    - `int LogRetentionDays`

#### **Domain Layer** (`Lifes.Domain`)
- `Common/ValueObjects/AppSettings.cs`
  - Properties:
    - `string BaseDirectory`
    - `string FilePattern`
    - `string ExcludePattern`
    - `bool GitAutoPush`
    - `LogLevel LogLevel`
    - `int LogRetentionDays`
  - Methods:
    - `static AppSettings CreateDefault()`
    - `bool IsValid()` - Validation rules
    - `Result Validate()` - Detailed validation

#### **Infrastructure Layer** (`Lifes.Infrastructure`)
- `Common/Configuration/SettingsService.cs`
  - Implements: `ISettingsService`
  - Uses: `Microsoft.Extensions.Configuration`
  - Methods:
    - `Task<Result<AppSettings>> LoadSettingsAsync()`
      - Read from `appsettings.user.json`
      - Fallback to `appsettings.json`
      - Merge configurations
    - `Task<Result> SaveSettingsAsync(AppSettings settings)`
      - Serialize to JSON
      - Write to `appsettings.user.json`
    - `Task<Result> ResetSettingsAsync()`
      - Delete `appsettings.user.json`

#### **Core Layer** (`Lifes.Core`)
- `Interfaces/ISettingsService.cs`
  ```csharp
  public interface ISettingsService
  {
      Task<Result<AppSettings>> LoadSettingsAsync();
      Task<Result> SaveSettingsAsync(AppSettings settings);
      Task<Result> ResetSettingsAsync();
      AppSettings GetCurrentSettings();
  }
  ```

### Files to Create/Modify

#### Presentation Layer
- [ ] `src/Lifes.Presentation.WPF/Shared/Views/SettingsDialog.xaml`
- [ ] `src/Lifes.Presentation.WPF/Shared/ViewModels/SettingsViewModel.cs`
- [x] `src/Lifes.Presentation.WPF/Shared/MainWindow.xaml` (add Settings button)

#### Application Layer
- [ ] `src/Lifes.Application/Common/Commands/LoadSettingsCommand.cs`
- [ ] `src/Lifes.Application/Common/Commands/SaveSettingsCommand.cs`
- [ ] `src/Lifes.Application/Common/DTOs/AppSettingsDto.cs`

#### Domain Layer
- [ ] `src/Lifes.Domain/Common/ValueObjects/AppSettings.cs`

#### Infrastructure Layer
- [ ] `src/Lifes.Infrastructure/Common/Configuration/SettingsService.cs`

#### Core Layer
- [ ] `src/Lifes.Core/Interfaces/ISettingsService.cs`

#### Configuration Files
- [x] `appsettings.json` (default settings)
- [ ] `appsettings.user.json` (user-specific, git-ignored)

## Tasks Breakdown

### Task 1: Setup Configuration Infrastructure (1 hour)
- [ ] Add NuGet packages (if not already):
  - `Microsoft.Extensions.Configuration`
  - `Microsoft.Extensions.Configuration.Json`
- [ ] Create default `appsettings.json`:
  ```json
  {
    "AppSettings": {
      "BaseDirectory": "",
      "FilePattern": "*ETL.csproj",
      "ExcludePattern": "Share*",
      "GitAutoPush": true,
      "LogLevel": "Information",
      "LogRetentionDays": 30
    }
  }
  ```
- [ ] Add `appsettings.user.json` to `.gitignore`
- [ ] Create `ISettingsService` interface in Core
- [ ] Implement `SettingsService` in Infrastructure
- [ ] Write unit tests for load/save/reset

### Task 2: Domain Model (0.5 hours)
- [ ] Create `AppSettings` value object
- [ ] Implement validation rules:
  - Base directory exists (or empty for default)
  - Patterns are valid glob syntax
  - Log retention 1-365 days
- [ ] Implement `CreateDefault()` factory method
- [ ] Write unit tests for validation

### Task 3: Application Commands (0.5 hours)
- [ ] Implement `LoadSettingsCommand`
- [ ] Implement `SaveSettingsCommand`
- [ ] Write unit tests with settings service mock

### Task 4: Settings Dialog UI (1 hour)
- [ ] Create `SettingsDialog.xaml`:
  - Form layout with labels and inputs
  - Apply dark theme styling
  - Validation error displays
- [ ] Create `SettingsViewModel`:
  - Bind all properties
  - Implement Save command
  - Implement Reset command
  - Implement Browse command
- [ ] Add Settings button to MainWindow
- [ ] Manual testing

## Dependencies
- **Depends on**: None (independent feature)
- **Blocked by**: None
- **Required**: `Microsoft.Extensions.Configuration` (already in use)

## Definition of Done
- [ ] Code implemented following Clean Architecture
- [ ] Configuration system working
- [ ] Unit tests written for:
  - [ ] AppSettings validation
  - [ ] SettingsService load/save/reset
  - [ ] Application commands
- [ ] Integration test: End-to-end settings workflow
- [ ] Manual testing checklist:
  - [ ] Settings dialog opens from MainWindow
  - [ ] Default values loaded on first launch
  - [ ] All fields editable
  - [ ] Browse button opens folder dialog
  - [ ] Validation works (invalid directory, patterns)
  - [ ] Save creates `appsettings.user.json`
  - [ ] Settings applied immediately after save
  - [ ] Reset deletes user settings file
  - [ ] Settings persist across app restarts
  - [ ] Version Increase Tool uses saved settings:
    - [ ] Base directory pre-filled
    - [ ] Scanning uses file patterns
    - [ ] Git auto-push respected
    - [ ] Log level applied
- [ ] Code reviewed and approved
- [ ] Documentation updated
- [ ] Committed with message: `feat(us-1.5): implement settings and configuration`

## Notes

### Settings File Structure

**appsettings.json** (Default, committed to Git):
```json
{
  "AppSettings": {
    "BaseDirectory": "",
    "FilePattern": "*ETL.csproj",
    "ExcludePattern": "Share*",
    "GitAutoPush": true,
    "LogLevel": "Information",
    "LogRetentionDays": 30
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information"
    }
  }
}
```

**appsettings.user.json** (User-specific, git-ignored):
```json
{
  "AppSettings": {
    "BaseDirectory": "F:\\Sources\\MyProjects",
    "GitAutoPush": false,
    "LogLevel": "Debug"
  }
}
```

### Configuration Merge Strategy
1. Load `appsettings.json` (defaults)
2. Load `appsettings.user.json` if exists
3. Merge: user settings override defaults
4. Only modified values saved to user file

### Validation Rules

| Setting | Validation |
|---------|------------|
| **BaseDirectory** | Must exist or be empty (empty = use current) |
| **FilePattern** | Valid glob pattern (not empty) |
| **ExcludePattern** | Valid glob pattern (can be empty) |
| **GitAutoPush** | Boolean (no validation needed) |
| **LogLevel** | One of: Verbose, Debug, Information, Warning, Error |
| **LogRetentionDays** | Integer between 1 and 365 |

### UI Layout

```
┌────────────────────────────────────────────────────┐
│  Settings                                     [X]  │
├────────────────────────────────────────────────────┤
│                                                    │
│  General Settings                                  │
│  ────────────────────────────────────────────────  │
│                                                    │
│  Base Directory                                    │
│  [F:\Sources\MyProjects              ] [Browse]    │
│                                                    │
│  File Filter Pattern                               │
│  [*ETL.csproj                        ]             │
│                                                    │
│  Exclude Pattern                                   │
│  [Share*                             ]             │
│                                                    │
│  ────────────────────────────────────────────────  │
│                                                    │
│  Git Settings                                      │
│  ────────────────────────────────────────────────  │
│                                                    │
│  [✓] Automatically push to remote after commit     │
│                                                    │
│  ────────────────────────────────────────────────  │
│                                                    │
│  Logging Settings                                  │
│  ────────────────────────────────────────────────  │
│                                                    │
│  Log Level                                         │
│  [Information ▼]                                   │
│                                                    │
│  Log Retention (days)                              │
│  [30                                 ]             │
│                                                    │
│  ────────────────────────────────────────────────  │
│                                                    │
│  [Reset to Defaults]        [Cancel]  [Save]       │
│                                                    │
└────────────────────────────────────────────────────┘
```

### Performance Considerations
- Settings loaded once on startup
- Cached in memory during session
- Save operations are fast (small JSON file)
- No performance impact on main operations

### Future Enhancements (not in this story)
- Import/Export settings
- Multiple setting profiles
- Cloud sync of settings
- Per-project settings override

## Implementation Progress

### Files Created
- [ ] (None yet)

### Current Status
- **Status**: ⏳ Future Enhancement
- **Completed**: 0%
- **Blockers**: None (lower priority)
- **Notes**: Can be implemented after core features (US-1.1 to US-1.4)

## Final Status
- **Status**: ⏳ Future Enhancement
- **Completed Date**: TBD
- **Approved By**: TBD
