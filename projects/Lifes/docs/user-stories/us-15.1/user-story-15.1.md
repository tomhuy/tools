# User Story: US-15.1

## Story Information
- **ID**: US-15.1
- **Title**: Thêm trạng thái IsAchieved cho Topic/Memento
- **Priority**: High
- **Estimate**: 4 hours
- **Sprint**: Current Sprint

## User Story
- **As a** End User
- **I want to** đánh dấu một Topic/Memento là "Đã hoàn thành" (IsAchieved) trong Electron
- **So that** tôi có thể ẩn các Topic này khỏi Monthly Calendar và Memento Management.

## Acceptance Criteria
1. **Model Update**: `MementoModel` (Backend) và `Memento` (Frontend) có thêm field `IsAchieved` (boolean).
2. **Persistence**: Trạng thái `IsAchieved` được lưu trữ bền vững trong `mementos.json`.
3. **Editor UI (Electron)**: Trong component Topic Editor của Angular, có checkbox "Đã hoàn thành".
4. **Filtering (Electron)**: Monthly Calendar và Memento Management mặc định ẩn các Topic đã hoàn thành.

## Technical Design

### Clean Architecture Layers

#### Domain / Core
- **[MODIFY] MementoModel.cs**: Add `public bool IsAchieved { get; set; }`.
- **[MODIFY] MementoQueryModel.cs**: Add `ShowAchieved` (bool?) to support filtering.

#### Application / Infrastructure
- **[MODIFY] JsonMementoRepository.cs**: Cập nhật logic mapping/save và filtering (mặc định ẩn nếu `ShowAchieved != true`).
- **[MODIFY] CalendarApiService.ts**: Thêm tham số `showAchieved` vào `MementoQuery` interface.

#### Presentation (Electron/Angular)
- **[MODIFY] memento.model.ts**: Add `isAchieved: boolean`.
- **[MODIFY] topic-editor.component.ts/html/css**: Thêm checkbox "Đã hoàn thành" vào form CRUD.
- **[MODIFY] monthly-calendar-page.component.ts/html**: Thêm toggle "Show completed topics" sử dụng local signal `showAchieved`.
- **[MODIFY] memento-management.component.ts/html**: Thêm toggle "Show Completed" sử dụng local signal `showAchieved`.

## Tasks Breakdown
- [x] Task 1: Update `MementoModel` in Core and Electron model.
- [x] Task 2: Update Infrastructure repository to persist `IsAchieved`.
- [x] Task 3: Implement UI in Electron Topic Editor.
- [x] Task 4: Apply filtering logic in Monthly Calendar and Memento Management.
- [x] Task 5: Renamed `isAchieved` query parameter to `showAchieved` for better clarity.

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-26
- **Approved By**: User

## Definition of Done
- [x] Code follows Clean Architecture.
- [x] Data is saved correctly to `mementos.json`.
- [x] UI reflects the state of `IsAchieved`.
- [x] Achieved topics are hidden from the board by default.
- [x] Manual test passed on Electron.
