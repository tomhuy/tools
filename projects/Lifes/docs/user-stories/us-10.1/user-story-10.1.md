# User Story: US-10.1

## Story Information
- **ID**: US-10.1
- **Title**: Build Sprint Board Dashboard UI
- **Priority**: High
- **Estimate**: 5 Story Points
- **Sprint**: Sprint 12
- **Feature Area**: Sprint Board

## User Story
- **As an** agile team member or manager
- **I want to** view a matrix-style Sprint Board showing features as rows and assignees as columns
- **So that** I can easily visualize which tasks belong to which feature and who they are assigned to in the current sprint.

## Acceptance Criteria
1. **Top Header Bar**:
   - Must display "Sprint Board" title and "Sprint \d" badge.
   - Must contain a "Hiện task done" (Show done tasks) checkbox and a "+ Feature" button.
2. **Assignee Summary Row**:
   - Must display dynamic user summary cards (e.g., Huy, Tuấn, Bằng, Hòa).
   - Each card has an avatar placeholder, name, and tasks summary (e.g., "0 done - 1 active").
   - Each persona should have a cohesive unique theme color.
3. **Matrix Layout**:
   - Features will be depicted as rows (e.g., "Refactor PS Mode", "Add validation dataloader").
   - Columns map directly to each assignee.
   - Inside the grid intersections, task cards should be displayed detailing the task info (task ID, title, status indicator).
4. **Visual Style**:
   - The UI must look premium, modern, and aligned with proper design principles (rounded corners `CornerRadius`, box shadows, clear typography).
   - Custom item layouts must be scalable and responsive.

## Technical Design

### Clean Architecture Layers
- **Presentation**: 
  - `SprintBoardView.xaml`: Main user interface.
  - `SprintBoardViewModel.cs`: Core logic, mocking the initial static layout as seen in the design schema.
  - Models: `BoardFeatureModel`, `BoardAssigneeModel`, `BoardTaskModel`.
- **Application**: `NavigationService` (registering the new Tool).
- **Domain**: Future phase - standardizing sprint and board entities.
- **Infrastructure**: Not applicable for initial static UI clone.
- **Core**: Update `ToolIds` with `SprintBoard`.

### Files to Create/Modify
- [x] `src/Lifes.Presentation.WPF/Features/SprintBoard/SprintBoardView.xaml`
- [x] `src/Lifes.Presentation.WPF/Features/SprintBoard/SprintBoardViewModel.cs`
- [x] `src/Lifes.Presentation.WPF/Features/SprintBoard/Models/BoardFeatureModel.cs`
- [x] `src/Lifes.Presentation.WPF/Features/SprintBoard/Models/BoardAssigneeModel.cs`
- [x] `src/Lifes.Presentation.WPF/Features/SprintBoard/Models/BoardTaskModel.cs`
- [x] `src/Lifes.Presentation.WPF/Constants/ToolIds.cs` (Modify)
- [x] `src/Lifes.Presentation.WPF/App.xaml.cs` (Modify - register tool)

## Tasks Breakdown
- [x] Task 1: Create the US documentation (this file).
- [x] Task 2: Update navigation structure so the tool can be routed to in `App.xaml.cs`.
- [x] Task 3: Develop ViewModels and mock data models representing the complex grid state.
- [x] Task 4: Construct WPF UI using a robust combination of Grids with `SharedSizeGroup` to achieve the matrix appearance.
- [x] Task 5: Apply premium styles, smooth aesthetics, rounded corners, subtle gradients for colored tags.

## Dependencies
- Depends on: US-5.1 Navigation System (Core).
- Blocked by: None.

## Definition of Done
- [x] UI is pixel-perfect as compared to the required wireframe/reference image.
- [x] Matrix is flexible enough to accept dynamic inputs.
- [x] Navigation works and properly swaps main view.
- [x] Documentation updated (`docs/structures/be-all-structure.md` and feature-specific struct file).
