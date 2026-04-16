# User Story: US-8.4

## Story Information
- **ID**: US-8.4
- **Title**: Advanced Calendar Selection & Event Phases
- **Priority**: High
- **Estimate**: 8 hours
- **Sprint**: Phase 5

## User Story
- **As a** Project Manager
- **I want to** select multiple months in the Monthly Calendar and define multiple execution phases for a single event
- **So that** I can visualize long-running, multi-stage projects across a broad timeline.

## Acceptance Criteria
1. **Multi-Month Selection**:
   - Monthly Calendar allows selecting multiple months from a dropdown.
   - Selected months are rendered sequentially in a scrollable view.
2. **Event Phases**:
   - `CalendarEventModel` supports a list of `Phases`.
   - Each phase has its own `Title`, `StartDate`, `EndDate`, and optional `Category`.
3. **Timeline Rendering**:
   - Event rows display all phases as distinct colored bars.
   - Tooltips or labels display phase titles.
   - Row heights are synchronized across all months for visual consistency.

## Technical Design

### Clean Architecture Layers
- **Core**: Updated `CalendarEventModel` and added `CalendarEventPhaseModel`.
- **Infrastructure**: Updated `MockCalendarService` to provide multi-phase demo data.
- **Presentation**: 
    - Updated `MonthlyCalendarViewModel` with `SelectableMonthViewModel` for multi-selection.
    - Updated `AnnualCalendarViewModel` to handle multi-phase rendering.
    - Refactored XAML Views to use `ItemsControl` for Gantt-style bars within event rows.

## Implementation Progress

### Files Created
- [x] `Lifes.Core/Models/CalendarEventModel.cs` (Updated)

### Files Modified
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/AnnualCalendarView.xaml`
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/AnnualCalendarViewModel.cs`
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarView.xaml`
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarViewModel.cs`
- [x] `Lifes.Infrastructure/Features/AnnualCalendar/Services/MockCalendarService.cs`

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-16
- **Approved By**: User
