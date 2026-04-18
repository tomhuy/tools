# User Story: US-8.6

## Story Information
- **ID**: US-8.6
- **Title**: Event-Centric Activity Tracker (Heatmap Redesign)
- **Priority**: High
- **Estimate**: 12 hours
- **Sprint**: Phase 5
- **Last Updated**: 2026-04-18

## User Story
- **As a** user
- **I want to** view my activities grouped by event in a high-density "Dot Grid" layout
- **So that** I can easily track progress and frequency across multiple months in a single view.

## Acceptance Criteria
1. **Event-Centric Hierarchy**:
   - Activities are grouped by `EventTitle` first.
   - Each event has a bold "Action Label" banner (e.g., "hành động: [tên sự kiện]").
2. **Minimalist Dot Grid**:
   - The grid uses squares (`34x34px`) without internal borders between days for a clean look.
   - Days are displayed as columns (01-31), and months as rows.
3. **Dot Visualization**:
   - Activities are rendered as small colored dots (`10x10px`) centered within the cells.
   - Hovering over a dot displays a Tooltip with the event detail.
4. **Multi-Month Rows**:
   - The view supports listing all active months for an event as vertical rows.
   - Invalid days (e.g., Feb 30) are visually grayed out/hidden.

## Technical Design

### Clean Architecture Layers
- **Presentation**: 
    - `ActivityHeatmapView.xaml`: Implements the fixed-size dot grid with `StackPanel` (Horizontal) for pixel-perfect alignment.
    - `ActivityHeatmapViewModel.cs`: Handles event grouping logic and day-number generation.
- **Core**: Uses `CalendarEventModel` for data structure.
- **Infrastructure**: Consumes `MockCalendarService` for demonstration data.

## Implementation Progress

### Files Created/Modified
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/ActivityHeatmapView.xaml`
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/ActivityHeatmapViewModel.cs`

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-18
- **Approved By**: User
