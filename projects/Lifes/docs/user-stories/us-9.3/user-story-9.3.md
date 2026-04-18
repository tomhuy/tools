# User Story: US-9.3

## Story Information
- **ID**: US-9.3
- **Title**: CRUD on Monthly Calendar
- **Priority**: High
- **Estimate**: 8 hours
- **Sprint**: 1

## User Story
- **As a** user
- **I want to** add, edit, and change colors for mementos directly on the Monthly Calendar grid
- **So that** I can quickly log and organize my life events without leaving the calendar view.

## Acceptance Criteria

### AC 1: Add/Edit via Popup
- **Given** I am in the Monthly Calendar view
- **When** I click on a cell in the daily grid
- **Then** a popup appears containing a text input and a "Save" button.

### AC 2: Save Data
- **Given** the Edit Popup is open
- **When** I enter text and click "Save"
- **Then** a new child memento is created for that date and parent row (if none exists) or the existing one is updated.
- **And** the child memento inherits the color from its parent (for new entries).
- **And** the calendar refreshes to show the new/updated entry.

### AC 3: Hover Interaction
- **Given** I mouse over a cell in the daily grid
- **Then** a small down-arrow button appears in the corner of the cell.

### AC 4: Color Picker
- **Given** the hover arrow is visible
- **When** I click the arrow
- **Then** a color picker popup appears with a set of predefined colors.
- **When** I select a color
- **Then** the corresponding memento's color is updated.
- **And** the UI refreshes.

## Technical Design

### Clean Architecture Layers
- **Presentation**: 
    - `MonthlyCalendarView.xaml`: Add Popups, Hover logic using Triggers/VisualStates.
    - `MonthlyCalendarViewModel.cs`: Add `SaveMementoCommand`, `OpenColorPickerCommand`, `SetColorCommand`, and popup state properties.
    - `MonthlyEventRowViewModel`: Update to store row context (Parent Id).
- **Application**:
    - `SaveMementoCommand`: Orchestrates adding/updating mementos.
- **Domain**:
    - `MementoModel`: Ensure fields for Title and Color are mutable.
- **Infrastructure**:
    - `MockMementoRepository`: Implement `SaveAsync` to persist changes in memory.
- **Core**:
    - `IMementoRepository`: Add `SaveAsync(MementoModel memento)`.
    - `ICalendarService`: Add `SaveMementoAsync`.

### Files to Create/Modify
- [x] `docs/user-stories/us-9.3/user-story-9.3.md` [NEW]
- [x] `src/Lifes.Core/Interfaces/IMementoRepository.cs` [MODIFY]
- [x] `src/Lifes.Core/Interfaces/ICalendarService.cs` [MODIFY]
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Repositories/MockMementoRepository.cs` [MODIFY]
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Services/CalendarService.cs` [MODIFY]
- [x] `src/Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarViewModel.cs` [MODIFY]
- [x] `src/Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarView.xaml` [MODIFY]
- [x] `src/Lifes.Presentation.WPF/Converters/EditCellArgsConverter.cs` [NEW]
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Repositories/JsonMementoRepository.cs` [NEW]
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Repositories/JsonTagRepository.cs` [NEW]

## Tasks Breakdown
- [x] Task 1: Initialize User Story documentation
- [x] Task 2: Update Core Interfaces (Repository & Service)
- [x] Task 3: Implement persistence logic in MockMementoRepository
- [x] Task 4: Add ViewModel logic for Popups and Commands
- [x] Task 5: Implement UI updates in MonthlyCalendarView (Clickable cells, Popups, Hover arrow)
- [x] Task 6: Implement Color Picker component/popup
- [x] Task 7: Verification and final documentation

## Definition of Done
- [x] CRUD operations working on Monthly Calendar
- [x] Color picker updates correctly
- [x] Popup UI matches requirements
- [x] Code follows Clean Architecture
- [x] Documentation updated
