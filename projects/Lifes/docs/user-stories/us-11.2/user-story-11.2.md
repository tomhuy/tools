# User Story: US-11.2

## Story Information
- **ID**: US-11.2
- **Title**: Implement Sprint Board UI in Electron
- **Priority**: High
- **Estimate**: 12 hours
- **Sprint**: Next Sprint

## User Story
- **As a** Project Manager / Developer
- **I want to** use a visual Sprint Board in the Electron UI
- **So that** I can track task progress across features and team members with intuitive drag-and-drop interactions.

## Acceptance Criteria
1. The Sprint Board UI must be ported from the provided raw prototype (`us-11.2.raw.md`) into a standalone Angular component. (DONE)
2. The UI must support:
   - Feature-based horizontal grouping (rows). (DONE)
   - Assignee-based vertical columns (Huy, Tuấn, Bằng, Hòa). (DONE)
   - "Làm trước" (Pre-requisite) column for initial tasks. (DONE)
   - Summary pills at the top showing task counts per person. (DONE)
3. Functional requirements:
   - Click a task to toggle "Done" status. (DONE)
   - Drag a task between cells to reassign person or change feature. (DONE)
   - Click "Thêm task" or "Feature" to add new items. (DONE)
   - Toggle "Hiện task done" to filter completed tasks. (DONE)
   - Click summary pills to highlight/filter tasks for a specific person. (DONE)
4. The component must follow the project's Angular guidelines (Signals, `inject()`, one-attribute-per-line). (DONE)
5. Data must be served via the C# WebAPI (mocked or persistent JSON). (POSTPONED - using fake data in service for now)

## Technical Design

### Clean Architecture Layers
- **Presentation (Electron/Angular)**:
  - `SprintBoardComponent`: Main view for the matrix.
  - `SprintBoardService`: Frontend service to handle state using Signals.
- **Presentation (WebApi)**:
  - `SprintBoardController`: (TBD in future US)
- **Application**:
  - `GetSprintBoardQuery` / `UpdateSprintBoardCommand`: (TBD in future US)

### Files Created/Modified
- [x] `src/Lifes.Presentation.Electron/src/app/models/sprint-board.model.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/sprint-board/sprint-board.service.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/sprint-board/sprint-board.component.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/sprint-board/sprint-board.component.html`
- [x] `src/Lifes.Presentation.Electron/src/app/features/sprint-board/sprint-board.component.css`
- [x] `src/Lifes.Presentation.Electron/src/app/app.routes.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/app.component.html`
- [x] `src/Lifes.Presentation.Electron/src/app/app.component.css`

## Tasks Breakdown
- [x] Task 1: Định nghĩa models cho Sprint Board.
- [x] Task 2: Tạo `SprintBoardService` chứa fake data và logic xử lý (Signals).
- [x] Task 3: Tạo `SprintBoardComponent` và port logic xử lý (D&D, render).
- [x] Task 4: Port CSS và HTML từ prototype sang Angular.
- [x] Task 5: Cấu hình Routing và Navigation cho Electron shell.
- [x] Task 6: Kiểm tra build và hiệu chỉnh UI (Scrollbars, Borders, CSP).

## Final Status
- **Status**: ✅ Completed (UI Porting)
- **Completed Date**: 2026-04-22
- **Approved By**: bmhuy
- **Note**: Backend integration (WebApi/Application layers) was postponed per user request. UI currently uses fake data in service.

## Definition of Done
- [x] Sprint Board is accessible from the Electron navigation menu.
- [x] All interactions (drag, toggle, filter) work as expected.
- [x] UI matches the provided high-fidelity prototype.
- [x] Code follows `angular_rule.md`.
