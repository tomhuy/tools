# User Story: US-12.4

## Story Information
- **ID**: US-12.4
- **Title**: Memento Management (Central Ordering + Filtering) in Electron
- **Priority**: Medium
- **Estimate**: 6 hours
- **Sprint**: Next Sprint

## User Story
- **As a** End User
- **I want to** quản lý tập trung tất cả Mementos (Topics) dưới dạng bảng, lọc theo Tag và chỉnh sửa `Order` (thứ tự hiển thị)
- **So that** em tùy chỉnh được ưu tiên hiển thị trên Monthly Calendar, tương đương tính năng của `MementoManagementView` bản WPF (US-9.8).

## Scope
- Table/grid view liệt kê Topic mementos (`parentId == null`).
- Filter theo 1 hoặc nhiều Tag (cascade).
- Edit field `Order` inline.
- Reflects về Monthly Calendar: đổi Order → row resort ngay (re-render qua `topicRows()` computed trong US-12.1).

## Acceptance Criteria

1. **Route / Navigation**: Truy cập qua menu, route `/memento-management`.
2. **Table Columns**: Title, Tags (badges), StartDate, EndDate, Order, Actions.
3. **Filter by Tag**: Multi-select tag filter; áp dụng cascade (topic phải có ít nhất 1 tag thỏa).
4. **Edit Order**: Click field Order → inline edit → save → Monthly Calendar reflect resort (verify reactive).
5. **Reactive Contract**: Mọi thay đổi đi qua `MonthlyCalendarService` immutable update; `topicRows()` computed tự resort; Monthly Calendar render lại DOM với `track id` ở đúng thứ tự mới.
6. Tuân thủ `angular_rule.md`.

## Technical Design
- Component: `memento-management.component.ts` standalone.
- Dùng `MonthlyCalendarService.topicRows()` làm data source + local filter signal cho tag filter.
- Inline edit Order: `[(ngModel)]` trên số + blur/enter trigger `updateMemento` immutable.
- Có thể dùng Angular CDK Table hoặc plain `<table>` — quyết định khi implement.

## Tasks Breakdown
- [ ] Task 1: Tạo `MementoManagementComponent` với table skeleton.
- [ ] Task 2: Implement tag filter (multi-select) với Signals.
- [ ] Task 3: Inline edit Order + validation (số nguyên ≥ 0).
- [ ] Task 4: Routing + navigation.
- [ ] Task 5: Verify: đổi Order → Monthly Calendar resort trực tiếp, không reload.
- [ ] Task 6: Manual test flow filter + reorder.

## Dependencies
- **Depends on**: US-12.1 (service + `topicRows` computed), US-12.3 (Tag list sẵn cho filter).

## Out of Scope
- Drag-and-drop reorder (có thể làm sau US riêng).
- Bulk edit / bulk delete.
- Export / import.

## Definition of Done
- [ ] Memento Management UI hoạt động.
- [ ] Filter + edit Order hoạt động.
- [ ] Monthly Calendar reflect đổi Order immediate.
- [ ] Code tuân thủ `angular_rule.md`.
- [ ] User review & approve.
