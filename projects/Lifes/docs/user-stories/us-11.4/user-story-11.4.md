# User Story: US-11.4

## Story Information
- **ID**: US-11.4
- **Title**: Complete Sprint Board Features & API Integration
- **Priority**: High
- **Estimate**: 16 hours
- **Sprint**: Current Sprint
- **Status**: ✅ Completed
- **Completed Date**: 2026-05-06

## User Story
- **As a** Project Manager/Developer
- **I want to** have a fully functional Sprint Board with advanced management features and data persistence
- **So that** I can track progress, manage team workload, and organize tasks efficiently within the application.

## Acceptance Criteria
1. **Epic Management**:
   - Ability to add new Epics (Title, Color).
   - Ability to edit existing Epics (Title, Color, Status).
   - Ability to archive/unarchive Epics.
   - Epics are grouped by status (In Progress, Backlog).
2. **Task Management**:
   - Ability to add/edit/delete tasks within an Epic.
   - Support for "Top Priority" flag (Star icon), which sorts tasks to the top.
   - Support for "Done" status toggle.
   - "Show Done" global toggle to filter completed tasks.
3. **User Management**:
   - Ability to manage users (Add/Delete/Edit Name, Initials, Color).
   - User workload summary bar (Done vs Active tasks).
4. **Interactive UI**:
   - Drag and Drop tasks between user columns and the "Pre" (Làm trước) column.
   - Feature-based matrix layout with vertical accent borders.
   - Responsive and premium aesthetics matching the prototype.
5. **Backend Integration**:
   - Full CRUD operations via REST API (`Lifes.Presentation.WebApi`).
   - Data persistence in JSON format (`Lifes.Infrastructure`).
   - Clean Architecture implementation (Domain entities, Application commands/queries).

## Technical Design

### Clean Architecture Layers
- **Core**: 
  - `ISprintBoardRepository`: Interface for data persistence.
  - `SprintBoardModels`: Shared DTOs and entities.
- **Domain**:
  - `Epic`: Entity with tasks, color, and status.
  - `SprintTask`: Entity with label, name, assignee, done status, and priority.
  - `SprintUser`: Entity for team members.
- **Application**:
  - `GetSprintBoardQuery`: Fetch current board state.
  - `SaveEpicCommand`, `DeleteEpicCommand`.
  - `SaveTaskCommand`, `DeleteTaskCommand`.
  - `SaveUserCommand`, `DeleteUserCommand`.
- **Infrastructure**:
  - `JsonSprintBoardRepository`: Implementation using local JSON files.
- **Presentation**:
  - `SprintBoardController`: API endpoints.
  - `SprintBoardComponent` (Angular): Enhanced UI with modals and full feature set.
  - `SprintBoardService` (Angular): API integration.

### Files to Create/Modify
- [x] `src/Lifes.Core/Interfaces/ISprintBoardRepository.cs` [NEW]
- [x] `src/Lifes.Core/Models/SprintBoardModels.cs` [NEW]
- [x] `src/Lifes.Infrastructure/Features/SprintBoard/Repositories/JsonSprintBoardRepository.cs` [NEW]
- [x] `src/Lifes.Presentation.WebApi/Controllers/SprintBoardController.cs` [NEW]
- [x] `src/Lifes.Presentation.Electron/src/app/features/sprint-board/` [MODIFY]
- [x] `docs/structures/fea-sprint-board-structure.md` [MODIFY]

## Tasks Breakdown
- [x] Task 1: Define Domain Entities and Core Interfaces.
- [x] Task 2: Implement Infrastructure Layer (JSON Persistence).
- [x] Task 3: Implement Application Layer (Commands/Queries).
- [x] Task 4: Create Web API Controller.
- [x] Task 5: Enhance Angular Frontend (Modals, User Management, Archived view).
- [x] Task 6: Integrate Frontend with API.
- [x] Task 7: Final Polish and UI Refinement.

## Dependencies
- Depends on: US-11.2 (Base Sprint Board UI)
- Blocked by: None

## Definition of Done
- [x] All features from `Sprint Board.html` are implemented and functional.
- [x] Data is persisted correctly across application restarts.
- [x] Code follows Clean Architecture principles.
- [x] UI is responsive and premium (Google Sans, consistent color palette).
- [x] Documentation updated (`fea-sprint-board-structure.md`).
- [x] User Story marked as complete.
