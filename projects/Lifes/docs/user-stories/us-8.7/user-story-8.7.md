# User Story: US-8.7

## Story Information
- **ID**: US-8.7
- **Title**: Hybrid Monthly Calendar Display Modes (Gantt vs Dots)
- **Priority**: Medium
- **Estimate**: 8 hours
- **Sprint**: Phase 5
- **Last Updated**: 2026-04-18

## User Story
- **As a** Project Manager / User
- **I want to** toggle between a Gantt-style (continuous bars) and a Dot-style (discrete markers) view in the Monthly Calendar
- **So that** I can visualize long-running task duration or high-density activity frequency as needed.

## Acceptance Criteria
1. **Display Mode Switcher**:
   - A ComboBox is added to the Monthly Calendar header.
   - Options: "Gantt" (Bar view), "Dot" (Dot + Border), and "PureDot" (Just dots).
2. **Gantt Mode**:
   - Existing behavior: Bars stretch from start to end column with solid background and titles.
3. **Dot Mode**:
   - Discrete markers (10x10px dots) with a faint background and border representing the task span.
4. **Pure Dot Mode**:
   - Only discrete markers (10x10px dots) are displayed, with no background or border around the task span.
5. **Interactive Persistence**:
   - Switching modes updates the view immediately without data reload.

## Technical Design

### Clean Architecture Layers
- **Presentation**: 
    - `MonthlyCalendarViewModel`: Add `DisplayMode` enum and property.
    - `MonthlyCalendarView.xaml`: 
        - Add `ComboBox` for mode selection.
        - Refactor the bar rendering to use a `DataTrigger` or a different `ItemTemplate` based on the selected mode.

## Implementation Progress

### Files Modified
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarView.xaml`
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarViewModel.cs`

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-18
- **Approved By**: User
