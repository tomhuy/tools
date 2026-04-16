# User Story 8.1: Annual Linear Calendar

## Objective
As a user, I want an annual linear calendar view so that I can visualize my tasks, events, and plans throughout the entire year across a continuous timebound grid.

## Requirements
1. **Linear Grid Layout**: 
   - Each row represents a month (Jan to Dec).
   - Columns represent the days of the week (`Su Mo Tu We Th Fr Sa`), repeating sequentially to accommodate up to 37 days (max days in a month + initial offsets).
   - Dates (`01`, `02`, ..., `31`) align correctly under their corresponding day of the week for that specific month in the year 2026.
2. **Gantt-like Task Visualization**:
   - Tasks/events should be displayed as colored horizontal bars spanning their respective `StartDate` to `EndDate`.
   - The bars must sit underneath the date numbers within the same month's section.
   - Non-overlapping task stacking: if multiple tasks overlap during the same dates, they should drop down to the next row within that month.
3. **Categories & Colors**:
   - The UI must support color-coded tags indicating categories such as `Work`, `Personal`, `Health`, `Learning`, `Travel`, `Event`, `Review`, `Planning`, `Conference`, `Competition`, `Release`.
   - A legend row should be shown at the top.
4. **Current Day Highlight**:
   - Today's date (if applicable) can be highlighted similar to the mockup (red stroke / background).
5. **WPF Implementation Details**:
   - Utilize standard `ItemsControl` and dynamic `Grid` with `Grid.ColumnSpan` to place the colored bars.
   - Separate ViewModel (`AnnualCalendarViewModel`) and mock service (`MockCalendarService`).
   - Add access to this screen from the main navigation menu as `Annual Calendar`.

## Mockup Reference
Visual clone of "2026 Linear Calendar", featuring a minimalist, high-contrast, edge-to-edge layout, with left and right vertical month labels, fading non-applicable days.
