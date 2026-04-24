# User Story: US-12.2

## Story Information
- **ID**: US-12.2
- **Title**: Topic Editor (Add/Update/Delete Topic Memento) in Electron
- **Priority**: High
- **Estimate**: 6 hours
- **Sprint**: Next Sprint

## User Story
- **As a** End User
- **I want to** thêm mới, cập nhật và xóa Topic memento (parentId == null) trực tiếp trên giao diện Electron
- **So that** em quản lý được các chủ đề chính hiển thị trên Monthly Calendar mà không cần mở WPF.

## Scope
- Form (modal/panel) để **Add** Topic: title, date range, color, tags assignment.
- Form để **Edit** Topic hiện có.
- Hành động **Delete** Topic (+ xóa child mementos liên quan, hoặc confirm).
- Gọi vào service stubs đã có ở US-12.1 (`addChild` / `updateMemento` / `deleteMemento`) — chỉnh sửa thành phù hợp cho Topic (parentId null).
- Tận dụng re-render contract từ US-12.1: chỉ row tương ứng re-render, không reload cả Gantt.

## Acceptance Criteria

1. **Add Topic**: Nút "Add Topic" trên Monthly Calendar mở form. Submit → topic mới xuất hiện làm row mới trên Gantt (không reload cả view).
2. **Edit Topic**: Click row hoặc nút edit → form pre-fill. Save → chỉ row đó update (title/color/date range refresh).
3. **Delete Topic**: Confirm dialog → topic bị xóa khỏi UI. Child mementos của topic đó được xử lý (xóa cascade, hoặc block nếu còn children — chọn 1 behavior và document).
4. **Validation**: StartDate ≤ EndDate. Title không rỗng. Tags optional.
5. **Reactive**: Dùng `MonthlyCalendarService` từ US-12.1. Không tự tạo state riêng.
6. Tuân thủ `angular_rule.md`.

## Technical Design
- Component: `topic-editor.component.ts` (standalone, dùng Angular Reactive Forms hoặc template-driven với Signals).
- Mở dạng modal overlay hoặc slide-in panel — chọn pattern đã dùng ở Sprint Board (US-11.2) nếu có.
- Gọi `monthlyCalendarService.addTopic/updateTopic/deleteTopic` (rename/thêm method nếu cần).
- Cascade delete: mặc định xóa luôn children; có thể đổi sau dựa trên feedback user.

## Tasks Breakdown
- [ ] Task 1: Thiết kế form schema (title, startDate, endDate, color, tagIds[]).
- [ ] Task 2: Tạo `TopicEditorComponent` + form validation.
- [ ] Task 3: Integrate mở/đóng form từ `MonthlyCalendarComponent`.
- [ ] Task 4: Bổ sung method `addTopic/updateTopic/deleteTopic` vào service (immutable).
- [ ] Task 5: Verify re-render scope (chỉ row thay đổi re-render).
- [ ] Task 6: Manual test.

## Dependencies
- **Depends on**: US-12.1 (service + rendering contract sẵn có).
- **Related**: US-12.3 (Tag selection trong form dựa vào tag list đã CRUD được).

## Out of Scope
- Tag CRUD → US-12.3.
- Drag-and-drop để reorder → US-12.4 hoặc US sau.

## Definition of Done
- [ ] Add/Edit/Delete Topic hoạt động trên UI.
- [ ] Validation pass.
- [ ] Re-render scope đúng contract US-12.1.
- [ ] Code tuân thủ `angular_rule.md`.
- [ ] User review & approve.
