# User Story: US-9.2

## Story Information
- **ID**: US-9.2
- **Title**: Tagging System and Hierarchical Filtering for Mementos
- **Priority**: High
- **Estimate**: 10 hours
- **Sprint**: Phase 5
- **Last Updated**: 2026-04-18

## User Story
- **As a** User
- **I want to** categorize my mementos using tags and filter the calendar views by tags
- **So that** I can easily focus on specific areas of my life (e.g., Work, Learning, Personal) with the option to see all related sub-tasks automatically.

## Acceptance Criteria
1. **Tag Model**: 
   - Create a `TagModel` with `Id`, `Name`, and `Color`.
2. **Tag Association**:
   - `MementoModel` should support multiple tags (Many-to-Many).
3. **Filtering Mechanism**:
   - Views (Monthly, Annual, Heatmap) must provide a way to filter by one or more Tags.
4. **Hierarchical Filtering (Cascade)**:
   - Provide an option "Include child mementos".
   - If enabled: When a parent Memento matches the Tag filter, all its children (and sub-children) must be displayed even if they don't have that specific Tag.
5. **Mock Data**:
   - `MockCalendarService` must seed tags and associate them with existing mementos.

## Technical Design

### Clean Architecture Layers
- **Core (Domain)**:
    - `TagModel.cs`, `MementoQueryModel.cs` [NEW]
    - `MementoModel.cs` [MODIFY]: Add `TagIds`.
    - `IMementoRepository.cs`, `ITagRepository.cs` [NEW]
- **Infrastructure**:
    - `MockMementoRepository.cs`, `MockTagRepository.cs` [NEW]: Moving data seeding here.
    - `CalendarService.cs` [NEW]: Logical orchestration and hierarchical recursion.
- **Presentation**:
    - `SelectableTagViewModel.cs` [NEW]
    - Add Tag selection UI in Header (Popup with Checkboxes).
    - Add "Bao gồm ghi chú con" CheckBox.
    - Logic update in `LoadDataAsync` of all 3 ViewModels.

## Implementation Progress

### Files Created/Modified
- [x] `src/Lifes.Core/Models/TagModel.cs`
- [x] `src/Lifes.Core/Models/MementoQueryModel.cs`
- [x] `src/Lifes.Core/Interfaces/IMementoRepository.cs`
- [x] `src/Lifes.Core/Interfaces/ITagRepository.cs`
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Repositories/MockMementoRepository.cs`
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Repositories/MockTagRepository.cs`
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Services/CalendarService.cs`
- [x] `src/Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarViewModel.cs`
- [x] `src/Lifes.Presentation.WPF/Features/AnnualCalendar/AnnualCalendarViewModel.cs`
- [x] `src/Lifes.Presentation.WPF/Features/AnnualCalendar/ActivityHeatmapViewModel.cs`
- [x] `src/Lifes.Presentation.WPF/Models/SelectableTagViewModel.cs`

## Definition of Done
- [x] Tagging system implemented with Repository pattern
- [x] Hierarchical filtering logic (Cascade) implemented in Service layer
- [x] UI updated with Tag selection and "Include Children" controls
- [x] Documentation updated (Structure, be-all, PRD)
- [x] Build verified and successful
