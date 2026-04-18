# User Story: US-9.1

## Story Information
- **ID**: US-9.1
- **Title**: Refactor Event Model to Unified Memento Model
- **Priority**: High
- **Estimate**: 12 hours
- **Sprint**: Phase 5
- **Last Updated**: 2026-04-18

## User Story
- **As a** Developer / Information Architect
- **I want to** refactor the existing `CalendarEventModel` into a unified, hierarchical `MementoModel`
- **So that** I can represent complex relationships between topic-level notes and supplemental concept-level notes using a single, extensible data structure.

## Acceptance Criteria
1. **Model Definition**:
   - `MementoModel` must replace `CalendarEventModel` and `CalendarEventPhaseModel`.
   - Fields: `Id`, `Title`, `StartDate`, `EndDate`, `ParentId`, `Order`, `CreatedDate`, `Color`, `Description`.
2. **Hierarchical Logic**:
   - If `ParentId` is `null`, the memento is a "Topic Note".
   - If `ParentId` is not `null`, it is a "Supplemental Concept Note" belonging to the parent.
3. **Core Interface Migration**:
   - `ICalendarService` and all implementations must return `MementoModel`.
4. **ViewModel Compatibility**:
   - `MonthlyCalendarViewModel`, `AnnualCalendarViewModel`, and `ActivityHeatmapViewModel` must be updated to consume `MementoModel`.
   - Hierarchical rendering (Topic -> Supplemental) must be maintained in the UI.
5. **Data Source Update**:
   - `MockCalendarService` must be updated to produce `MementoModel` data instead of legacy event models.

## Technical Design

### Clean Architecture Layers
- **Core (Domain)**:
    - `MementoModel.cs`: New unified model.
    - `ICalendarService.cs`: Updated interface.
- **Infrastructure**:
    - `MockCalendarService.cs`: Refactored to generate mementos.
- **Presentation**:
    - `AnnualCalendarViewModel.cs`, `MonthlyCalendarViewModel.cs`, `ActivityHeatmapViewModel.cs`: Updated to handle the new data structure.

## Implementation Progress

### Files to Create/Modify
- [x] `src/Lifes.Core/Models/MementoModel.cs` [NEW]
- [x] `src/Lifes.Core/Models/CalendarEventModel.cs` [DELETE]
- [x] `src/Lifes.Core/Interfaces/ICalendarService.cs` [MODIFY]
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Services/MockCalendarService.cs` [MODIFY]
- [x] `src/Lifes.Presentation.WPF/Features/AnnualCalendar/AnnualCalendarViewModel.cs` [MODIFY]
- [x] `src/Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarViewModel.cs` [MODIFY]
- [x] `src/Lifes.Presentation.WPF/Features/AnnualCalendar/ActivityHeatmapViewModel.cs` [MODIFY]

## Definition of Done
- [x] Code implemented following Clean Architecture
- [x] Unit tests updated (Verified via Build)
- [x] Documentation updated (updoc)
- [x] User Story marked as complete

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-18
- **Approved By**: User
