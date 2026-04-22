# User Story: US-9.8

## Story Information
- **ID**: US-9.8
- **Title**: Memento Management & Ordering (General Query Approach)
- **Priority**: High
- **Estimate**: 4 hours

## User Story
- **As a** user
- **I want to** manage my topics using a filterable grid (by tag and hierarchy)
- **So that** I can easily adjust their display order and organize my calendar structure.

## Acceptance Criteria
1. Given "Memento Management" tool
   When loaded, it should support filtering by **Tags** and a toggle for **Include Children**.
2. Given the Management Grid
   When "Parent Only" is active, it should only show Mementos where `ParentId` is null.
3. Given the grid columns
   Then I should see Title, Description, Created Date, Start Date, End Date, and **Order**.
4. Given the "Order" field
   When I modify it and save, it should persist to the JSON database.
5. Given the Monthly Calendar
   When rendering rows, they must be sorted by `Order` (Ascending).

## Technical Design

### Clean Architecture Layers
- **Core**: 
    - Update `MementoQueryModel` to include `bool? ParentOnly`.
    - Update `ICalendarService` to expose a generic `GetMementosAsync(MementoQueryModel query, bool includeChildren)`.
- **Infrastructure**: 
    - Implement `ParentOnly` logic in `JsonMementoRepository`.
- **Presentation**: 
    - `MementoManagementView.xaml` with Tag Filter UI and Order column.
    - `MonthlyCalendarViewModel.cs` update for sorting.

## Tasks Breakdown
- [x] Task 1: Create `MementoManagementView` and `ViewModel`.
- [x] Task 2: Extend `ICalendarService` to support Topic retrieval.
- [x] Task 3: Implement Grid loading and saving logic in `MementoManagementViewModel`.
- [x] Task 4: Register the new tool in `ToolIds` and `App.xaml.cs`.
- [x] Task 5: Implement sorting logic in `MonthlyCalendarViewModel`.

## Dependencies
- Depends on: US-9.7

## Definition of Done
- [x] Code implemented following Clean Architecture
- [x] Unit tests written and passing
- [x] Code reviewed
- [x] Documentation updated (PRD, Feature Structure)
- [x] User Story marked as complete

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-19
- **Approved By**: User
