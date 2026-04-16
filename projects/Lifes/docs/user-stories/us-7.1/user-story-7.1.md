# User Story: US-7.1

## Story Information
- **ID**: US-7.1
- **Title**: Document Management Visual Tracker Form
- **Priority**: High
- **Estimate**: 5 Story Points
- **Sprint**: TBD

## User Story
- **As a** user
- **I want to** manage documents and their sub-tasks using a visual grid form accessed via "Document Management" menu
- **So that** I can accurately track documents and task statuses across the days of the current month, exactly matching the provided tracking interface.

## Acceptance Criteria
1. **Database / Entity Structure**:
   - Entities for documents and subdocuments (tasks) are created with the following fields using C# conventions: `Id`, `ParentId`, `Title`, `Description`, `StartDate`, `EndDate`, `Status`.
   - A subdocument is treated as a task and links to a parent document via `ParentId`.
   - The form will filter documents to only show those where `StartDate` and `EndDate` overlap with the currently displayed month.
   - *Phase 1 Requirement*: Display mock data only; database integration is not required at this phase.
2. **UI Implementation**:
   - A new WPF form (`DocumentManagementView`) is created.
   - The UI correctly utilizes an `ItemsControl` and custom `Grid` to perfectly clone the reference layout, bypassing standard `DataGrid` limitation.
   - The UI contains a grid/table where the first column is `Task/ngày`.
   - The following columns represent days of the month (1 to 31 depending on month length) along with their corresponding day of the week (T2, T3, T4, T5, T6, T7, CN).
   - Default view loads the current month.
   - Columns corresponding to the current day (Hôm nay) have a red background color indicator.
3. **Behavior**:
   - Users can view documents and tasks on the left.
   - Users can visually mark tasks for specific days, indicated in the grid with an 'X'.

## Technical Design

### Clean Architecture Layers
- **Presentation**: `DocumentManagementView.xaml`, `DocumentManagementViewModel.cs`
- **Application**: 
  - `Queries/GetDocumentsTrackerQuery.cs`
  - `DTOs/DocumentDto.cs`
- **Infrastructure**: 
  - `Services/MockDocumentService.cs` (Returning static layout/records for Phase 1)
- **Core**: 
  - `Interfaces/IDocumentService.cs`

### Files to Create/Modify
- [x] `docs/user-stories/us-7.1/user-story-7.1.md`
- [x] `src/Lifes.Presentation.WPF/Features/DocumentManagement/DocumentManagementView.xaml`
- [x] `src/Lifes.Presentation.WPF/Features/DocumentManagement/DocumentManagementViewModel.cs`
- [x] `src/Lifes.Constants/ToolIds.cs` (or equivalent to register into menu)

## Tasks Breakdown
- [x] Task 1: Update Application mapping & Menu to navigate to `Document Management`.
- [x] Task 2: Create `DocumentDto` models and `MockDocumentService` to serve static test items including overlapping `StartDate`/`EndDate`.
- [x] Task 3: Setup WPF ViewModel `DocumentManagementViewModel` to compute current month's days, map mock data to columns, and detect the current day index.
- [x] Task 4: Design intricate `ItemsControl` Grid in `DocumentManagementView.xaml` precisely matching the reference screenshot.

## Definition of Done
- [x] UI perfectly matches the given image mockup, utilizing ItemsControl+Grid.
- [x] Mock data is integrated to simulate records correctly aligning to days.
- [x] Current day red highlight renders correctly.
- [x] Documentation updated
- [x] User Story marked as complete

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-16
- **Approved By**: bmHuy
