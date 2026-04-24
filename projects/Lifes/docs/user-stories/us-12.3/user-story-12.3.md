# User Story: US-12.3

## Story Information
- **ID**: US-12.3
- **Title**: Tag Management UI in Electron
- **Priority**: Medium
- **Estimate**: 5 hours
- **Sprint**: Next Sprint

## User Story
- **As a** End User
- **I want to** quản lý Tags (CRUD + chọn màu từ palette) trong Electron
- **So that** em gán/lọc được tags cho Mementos mà không cần WPF.

## Scope
- UI CRUD Tag: list + form (name, color).
- Color palette picker.
- Reactive propagation: khi tag thay đổi, Monthly Calendar + Topic Editor (US-12.2) tự refresh danh sách tag.
- Giao tiếp qua signal `tags` đã khai trong service từ US-12.1.

## Acceptance Criteria

1. **Tag List**: Hiển thị tất cả tag hiện có, mỗi tag show name + color swatch.
2. **Create**: Form nhập name, chọn color từ palette → add vào signal `tags`.
3. **Update**: Click tag → edit form → save update immutable.
4. **Delete**: Confirm → xóa tag. Memento đang dùng tag này phải được xử lý (xóa tagId khỏi memento, hoặc confirm).
5. **Color Palette**: Predefined palette (ví dụ 12-16 màu) + custom hex input (optional).
6. **Reactive**: Monthly Calendar (nếu đang hiển thị tag badges) và Topic Editor (US-12.2) tự update khi `tags` signal thay đổi — KHÔNG cần event thủ công như `TagsUpdated` bản WPF.
7. Tuân thủ `angular_rule.md`.

## Technical Design
- Component: `tag-management.component.ts` standalone, mở dạng modal hoặc dedicated route `/tags`.
- Service: dùng `MonthlyCalendarService.tags` signal (hoặc tách ra `TagService` riêng nếu muốn separation — quyết định khi implement).
- Cascade khi delete tag: default strip tagId khỏi tất cả memento (immutable update).

## Tasks Breakdown
- [ ] Task 1: Định nghĩa color palette constants.
- [ ] Task 2: Tạo `TagManagementComponent` với list + form.
- [ ] Task 3: Bổ sung method `addTag/updateTag/deleteTag` vào service (immutable + cascade xóa tagId khỏi mementos).
- [ ] Task 4: Navigation / modal trigger từ Monthly Calendar.
- [ ] Task 5: Verify reactive update — đổi tag color thấy Topic row (nếu hiển thị tag badge) update ngay.
- [ ] Task 6: Manual test.

## Dependencies
- **Depends on**: US-12.1 (service + `tags` signal).
- **Related**: US-12.2 (Topic Editor dùng tag list này để gán).

## Out of Scope
- Backend persistence tags (nếu WebApi chưa có endpoint → fake data).
- Tag grouping / nested tags.

## Definition of Done
- [ ] CRUD tags hoạt động.
- [ ] Color palette picker hoạt động.
- [ ] Các view khác tự refresh khi tag đổi.
- [ ] Code tuân thủ `angular_rule.md`.
- [ ] User review & approve.
