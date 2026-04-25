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
- Filter theo Keyword (Title), StartDate và EndDate.
- Edit field `Order` inline.
- Tạo mới Topic trực tiếp từ lưới.


## Acceptance Criteria

1. **Route / Navigation**: Truy cập qua menu, route `/memento-management`.
2. **Table Columns**: Title, Tags (badges), StartDate, EndDate, Order, Actions.
3. **Filtering**: 
   - Multi-select tag filter; áp dụng cascade.
   - Filter theo Keyword (search text).
   - Filter theo khoảng thời gian (StartDate/EndDate).
4. **Edit Order**: Click field Order → inline edit → save → Monthly Calendar reflect resort (verify reactive).
5. **Reactive Contract**: Mọi thay đổi đi qua `MonthlyCalendarService` immutable update; `topicRows()` computed tự resort; Monthly Calendar render lại DOM với `track id` ở đúng thứ tự mới.
6. Tuân thủ `angular_rule.md`.

## Technical Design
- Component: `memento-management.component.ts` standalone (Smart Component).
- Service: `MementoManagementService` chuyên biệt để fetch dữ liệu độc lập với scope của Calendar.
- Filtering: Sử dụng `effect()` để theo dõi filter signals và reload dữ liệu tự động.
- Table: `memento-table.component.ts` (Passive Component) hỗ trợ hiển thị, sắp xếp và inline editing.
- Inline edit Order: Blur/enter trigger `saveTopic` qua API.

## Tasks Breakdown
- [x] Task 1: Tạo `MementoManagementComponent` với table skeleton.
- [x] Task 2: Implement tag filter (multi-select) với Signals.
- [x] Task 3: Inline edit Order + validation (số nguyên ≥ 0).
- [x] Task 4: Routing + navigation.
- [x] Task 5: Verify: đổi Order → Monthly Calendar resort trực tiếp, không reload.
- [x] Task 6: Manual test flow filter + reorder.

## Dependencies
- **Depends on**: US-12.1 (service + `topicRows` computed), US-12.3 (Tag list sẵn cho filter).

## Out of Scope
- Drag-and-drop reorder (có thể làm sau US riêng).
- Bulk edit / bulk delete.
- Export / import.

## Definition of Done
- [x] Memento Management UI hoạt động.
- [x] Filter + edit Order hoạt động.
- [x] Monthly Calendar reflect đổi Order immediate.
- [x] Code tuân thủ `angular_rule.md`.
- [x] User review & approve.

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-25
- **Approved By**: bmhuy
