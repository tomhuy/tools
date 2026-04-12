# User Story: US-1.4

## Story Information
- **ID**: US-1.4
- **Title**: Logging và Error Handling
- **Priority**: High
- **Estimate**: 4 hours
- **Sprint**: Sprint 1 - Version Increase Tool
- **Status**: 📋 Pending

## User Story
- **As a** Developer
- **I want to** xem chi tiết logs của tất cả operations trong real-time
- **So that** tôi có thể theo dõi progress và debug khi có vấn đề

## Acceptance Criteria

1. **Given** application đang chạy
   **When** any operation executes
   **Then** logs hiển thị real-time trong Log window

2. **Given** log window
   **When** displaying logs
   **Then** mỗi log entry hiển thị:
   - **Time**: HH:mm:ss format
   - **Level**: INFO, WARNING, ERROR, SUCCESS
   - **Message**: Descriptive message

3. **Given** log entries
   **When** displayed
   **Then** color-coded by level:
   - **INFO**: Blue (#4299E1)
   - **WARNING**: Orange (#FFB347)
   - **ERROR**: Red (#F05252)
   - **SUCCESS**: Green (#3ECF8E)

4. **Given** operations đang execute
   **When** logging events
   **Then** log các events:
   - Application startup
   - Scanning files: "Scanning projects in directory: {path}"
   - Found files: "Found {count} candidate projects"
   - Reading version: "Reading version from {fileName}"
   - Current version: "{fileName}: Current version {version}"
   - Updating version: "Updating {fileName}: {oldVersion} → {newVersion}"
   - File saved: "✅ Saved {fileName}"
   - Git staging: "Staging {count} files for commit"
   - Git commit: "✅ Committed: {commitSha}"
   - Git push: "Pushing to {remote}/{branch}"
   - Git push success: "✅ Pushed to {remote}/{branch}"
   - Errors và warnings with details

5. **Given** log window với nhiều entries
   **When** user scrolls
   **Then** auto-scroll to bottom when new logs arrive (unless user scrolled up manually)

6. **Given** user selects log entries
   **When** user clicks "Copy Logs" button
   **Then** selected logs copied to clipboard với format:
   ```
   [14:30:15] INFO: Scanning projects in directory...
   [14:30:15] SUCCESS: Found 12 candidate projects
   ```

7. **Given** log window has entries
   **When** user clicks "Clear Logs" button
   **Then** all logs cleared from view (but not from file)

8. **Given** user clicks "Export Logs" button
   **When** save dialog opens
   **Then** logs exported to .txt file với timestamp filename:
   - Format: `etltools-logs-{yyyy-MM-dd-HHmmss}.txt`

9. **Given** log file writing
   **When** application runs
   **Then** logs also written to file:
   - Location: `logs/etltools-{yyyy-MM-dd}.txt`
   - Rolling daily (new file each day)
   - Retains last 30 days

10. **Given** any error occurs
    **When** operation fails
    **Then** error handling:
    - User-friendly error message displayed
    - Technical details logged to file
    - Operation can continue or gracefully fail
    - No application crashes

## Technical Design

### Clean Architecture Layers

#### **Presentation Layer** (`ETLTools.Presentation.WPF`)
- `Features/VersionIncrease/VersionIncreaseView.xaml` (extend)
  - Add Log section:
    - ListView with 3 columns: Time, Level, Message
    - Buttons: Copy Logs, Clear Logs, Export Logs
  
- `Features/VersionIncrease/VersionIncreaseViewModel.cs` (extend)
  - Properties:
    - `ObservableCollection<LogEntryViewModel> LogEntries`
  - Commands:
    - `CopyLogsCommand`
    - `ClearLogsCommand`
    - `ExportLogsCommand`
  - Methods:
    - `AddLogEntry(string time, string level, string message)`

- `Features/VersionIncrease/Models/LogEntryViewModel.cs`
  - Properties:
    - `string Time`
    - `string Level`
    - `string Message`
    - `SolidColorBrush LevelColor` - Color based on level

#### **Infrastructure Layer** (`ETLTools.Infrastructure`)
- `Common/Logging/WpfListViewSink.cs`
  - Custom Serilog sink for WPF
  - Implements: `ILogEventSink`
  - Dispatches log events to UI thread
  - Sends log to ViewModel's `AddLogEntry()` method
  
- `Common/Logging/LoggingConfiguration.cs`
  - Static class to configure Serilog
  - Sets up:
    - File sink (rolling daily)
    - WPF ListView sink
    - Console sink (debug)
    - Log level filtering

#### **Core Layer** (`ETLTools.Core`)
- `Models/LogEntry.cs`
  - Properties:
    - `DateTime Timestamp`
    - `string Level`
    - `string Message`
    - `Exception Exception` (optional)

### Files to Create/Modify

#### Presentation Layer
- [x] `src/ETLTools.Presentation.WPF/Features/VersionIncrease/VersionIncreaseView.xaml` (extend)
- [x] `src/ETLTools.Presentation.WPF/Features/VersionIncrease/VersionIncreaseViewModel.cs` (extend)
- [ ] `src/ETLTools.Presentation.WPF/Features/VersionIncrease/Models/LogEntryViewModel.cs`

#### Infrastructure Layer
- [ ] `src/ETLTools.Infrastructure/Common/Logging/WpfListViewSink.cs`
- [ ] `src/ETLTools.Infrastructure/Common/Logging/LoggingConfiguration.cs`

#### Core Layer
- [ ] `src/ETLTools.Core/Models/LogEntry.cs`

#### Application Startup
- [x] `src/ETLTools.Presentation.WPF/App.xaml.cs` (extend)
  - Configure Serilog on startup

## Tasks Breakdown

### Task 1: Setup Serilog Infrastructure (1 hour)
- [ ] Add NuGet packages:
  - [ ] `Serilog` (v4.3.0)
  - [ ] `Serilog.Sinks.File` (latest)
  - [ ] `Serilog.Sinks.Console` (latest)
- [ ] Create `LoggingConfiguration` class
- [ ] Configure file sink:
  - Rolling daily logs
  - Path: `logs/etltools-{Date}.txt`
  - Retain 30 days
- [ ] Configure console sink (debug only)
- [ ] Set minimum log levels
- [ ] Initialize in `App.xaml.cs` startup

### Task 2: Custom WPF Sink (1.5 hours)
- [ ] Create `WpfListViewSink` class
- [ ] Implement `ILogEventSink`
- [ ] Implement `Emit(LogEvent logEvent)`:
  - [ ] Extract timestamp, level, message
  - [ ] Dispatch to UI thread using `Dispatcher.Invoke()`
  - [ ] Call ViewModel's `AddLogEntry()`
  - [ ] Handle exceptions gracefully
- [ ] Register sink in `LoggingConfiguration`
- [ ] Pass ViewModel reference to sink
- [ ] Write unit tests

### Task 3: ViewModel Integration (1 hour)
- [ ] Create `LogEntryViewModel` model
  - [ ] Time, Level, Message properties
  - [ ] `LevelColor` computed property
  - [ ] Color mapping: INFO→Blue, WARNING→Orange, ERROR→Red, SUCCESS→Green
- [ ] Extend `VersionIncreaseViewModel`:
  - [ ] Add `ObservableCollection<LogEntryViewModel> LogEntries`
  - [ ] Implement `AddLogEntry()` method
  - [ ] Add to collection on UI thread
  - [ ] Limit collection size (max 1000 entries, remove oldest)
- [ ] Implement commands:
  - [ ] `CopyLogsCommand` - Copy selected to clipboard
  - [ ] `ClearLogsCommand` - Clear collection
  - [ ] `ExportLogsCommand` - Save to file

### Task 4: UI Implementation (0.5 hours)
- [ ] Extend `VersionIncreaseView.xaml`
- [ ] Add Log section with Card style
- [ ] Add ListView:
  - [ ] 3 columns: Time (120px), Level (100px), Message (*)
  - [ ] Color-coded rows based on level
  - [ ] Multi-select enabled
  - [ ] Virtualization enabled
- [ ] Add action buttons
- [ ] Style with dark theme
- [ ] Test auto-scroll behavior

### Task 5: Integration & Testing (1 hour)
- [ ] Add logging to all services:
  - [ ] `ProjectScanner`: Log scanning start, files found
  - [ ] `VersionService`: Log version parsing, increment logic
  - [ ] `ProjectFileService`: Log file read/write operations
  - [ ] `GitService`: Log Git commands and results
- [ ] Test logging levels:
  - [ ] Info for normal operations
  - [ ] Success for completed operations
  - [ ] Warning for non-critical issues
  - [ ] Error for failures
- [ ] Test UI:
  - [ ] Colors display correctly
  - [ ] Auto-scroll works
  - [ ] Copy logs to clipboard
  - [ ] Clear logs
  - [ ] Export logs to file
- [ ] Test file logging:
  - [ ] File created in logs/ folder
  - [ ] Rolling daily works
  - [ ] Old logs cleaned up

## Dependencies
- **Depends on**: US-1.1, US-1.2, US-1.3 (logging spans all features)
- **Blocked by**: None
- **Required NuGet Packages**:
  - `Serilog` (v4.3.0)
  - `Serilog.Sinks.File`
  - `Serilog.Sinks.Console`

## Definition of Done
- [ ] Code implemented following Clean Architecture
- [ ] Serilog configured properly
- [ ] Custom WPF sink implemented
- [ ] Unit tests written for:
  - [ ] WpfListViewSink log dispatching
  - [ ] LoggingConfiguration setup
  - [ ] ViewModel log entry management
- [ ] Integration test: End-to-end logging workflow
- [ ] Manual testing checklist:
  - [ ] Logs appear in real-time
  - [ ] Colors display correctly
  - [ ] Time format correct (HH:mm:ss)
  - [ ] Copy logs works
  - [ ] Clear logs works
  - [ ] Export logs creates file with correct name
  - [ ] File logging works (check logs/ folder)
  - [ ] Rolling logs work (test with date change)
  - [ ] Auto-scroll works (and stops when user scrolls up)
  - [ ] Limit of 1000 entries enforced
  - [ ] All operations logged properly:
    - [ ] Scan projects
    - [ ] Read versions
    - [ ] Update versions
    - [ ] Git operations
    - [ ] Errors
- [ ] Code reviewed and approved
- [ ] Documentation updated
- [ ] Committed with message: `feat(us-1.4): implement real-time logging and error handling`

## Notes

### Log Level Guidelines

| Level | Use Case | Example |
|-------|----------|---------|
| **INFO** | Normal operations | "Scanning projects in directory..." |
| **SUCCESS** | Successful completion | "✅ Updated 5 projects successfully" |
| **WARNING** | Non-critical issues | "⚠️ File locked, retrying..." |
| **ERROR** | Failures that stop operation | "❌ Failed to read file: Access denied" |

### Log Messages Examples

```csharp
// Application startup
_logger.LogInformation("ETL Tools started - Version Increase Tool");

// Scanning
_logger.LogInformation("Scanning projects in directory: {Path}", basePath);
_logger.LogInformation("Found {Count} candidate projects", projects.Count);

// Reading versions
_logger.LogInformation("Reading version from {FileName}", fileName);
_logger.LogInformation("{FileName}: Current version {Version}", fileName, version);

// Updating versions
_logger.LogInformation("Updating {FileName}: {OldVersion} → {NewVersion}", 
    fileName, oldVersion, newVersion);
_logger.LogInformation("✅ Saved {FileName}", fileName);

// Git operations
_logger.LogInformation("Staging {Count} files for commit", files.Count);
_logger.LogInformation("✅ Committed: {CommitSha}", commitSha);
_logger.LogInformation("Pushing to {Remote}/{Branch}", remote, branch);
_logger.LogInformation("✅ Pushed to {Remote}/{Branch}", remote, branch);

// Errors
_logger.LogError(ex, "Failed to update {FileName}", fileName);
_logger.LogWarning("File {FileName} is locked, skipping", fileName);
```

### Custom Serilog Sink Implementation

```csharp
public class WpfListViewSink : ILogEventSink
{
    private readonly VersionIncreaseViewModel _viewModel;
    private readonly IFormatProvider? _formatProvider;

    public WpfListViewSink(VersionIncreaseViewModel viewModel, IFormatProvider? formatProvider = null)
    {
        _viewModel = viewModel;
        _formatProvider = formatProvider;
    }

    public void Emit(LogEvent logEvent)
    {
        var timestamp = logEvent.Timestamp.ToString("HH:mm:ss", _formatProvider);
        var level = logEvent.Level.ToString().ToUpper();
        var message = logEvent.RenderMessage(_formatProvider);

        _viewModel.AddLogEntry(timestamp, level, message);
    }
}

// Extension method for configuration
public static class WpfListViewSinkExtensions
{
    public static LoggerConfiguration WpfListView(
        this LoggerSinkConfiguration sinkConfiguration,
        VersionIncreaseViewModel viewModel,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
        IFormatProvider? formatProvider = null)
    {
        return sinkConfiguration.Sink(
            new WpfListViewSink(viewModel, formatProvider), 
            restrictedToMinimumLevel);
    }
}
```

### Performance Considerations
- Limit log entries to 1000 in UI (memory management)
- Use `ObservableCollection` for reactive updates
- Dispatcher calls batched if many logs at once
- File logging async (non-blocking)
- ListView virtualization enabled

### Error Handling Strategy
- Never throw exceptions from log sink
- Gracefully handle Dispatcher failures
- Log file write failures don't crash app
- User-friendly error messages in UI
- Technical details in log file only

## Implementation Progress

### Files Created
- [ ] (None yet)

### Current Status
- **Status**: 📋 Pending
- **Completed**: 0%
- **Blockers**: None (can be developed in parallel)
- **Notes**: Can start alongside other user stories

## Final Status
- **Status**: 📋 Pending
- **Completed Date**: TBD
- **Approved By**: TBD
