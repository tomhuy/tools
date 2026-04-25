# User Story: US-12.5

## Story Information
- **ID**: US-12.5
- **Title**: Filter Monthly Calendar by Tags with Recursive Children
- **Priority**: High
- **Estimate**: 4 hours
- **Sprint**: Current Sprint

## User Story
- **As a** End User
- **I want to** lọc Monthly Calendar theo Tag đã chọn và có tùy chọn "Include the children" trong dropdown Tags.
- **So that** em chỉ xem được các chủ đề (Topics) quan tâm và các chủ đề con liên quan, giúp giao diện lịch gọn gàng hơn.

## Acceptance Criteria
1. **Tag Selection Logic**:
   - Trong dropdown "Tags" của Monthly Calendar, khi người dùng tích chọn các Tag, danh sách Memento hiển thị trên lịch phải được lọc theo các Tag đó.
   - Nếu không chọn Tag nào, hiển thị tất cả (mặc định hiện tại).
2. **"Include the children" Checkbox**:
   - Thêm checkbox "Include the children" vào trong dropdown "Tags".
   - Nếu tích chọn: Load cả các Memento con của các Memento có chứa Tag đã chọn.
   - Nếu không tích: Chỉ load các Memento có chứa ít nhất một trong các Tag đã chọn.
3. **Reactive Loading**:
   - Dữ liệu trên Monthly Calendar tự động tải lại (reload từ API) khi thay đổi danh sách Tag được chọn hoặc trạng thái checkbox "Include the children".
4. **UI Consistency**:
   - Checkbox "Include the children" nằm trong dropdown menu Tags, vị trí dễ thấy (ví dụ: ngay dưới dropdown header hoặc trong footer).
   - Trạng thái các tag checkbox phải được bind đúng với state hiện tại.

## Technical Design

### Presentation Layer
- **[MODIFY] [monthly-calendar-page.component.ts](../../../src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component.ts)**:
  - Thêm signal `selectedTagIds = signal<number[]>([])`.
  - Thêm signal `includeChildren = signal(false)`.
  - Thêm `effect` để gọi `service.loadMementos()` khi `selectedTagIds` hoặc `includeChildren` thay đổi.
- **[MODIFY] [monthly-calendar-page.component.html](../../../src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component.html)**:
  - Bind `input[type="checkbox"]` của từng tag với `selectedTagIds`.
  - Thêm checkbox cho "Include the children".

### Application Layer
- **[MODIFY] [monthly-calendar.service.ts](../../../src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-calendar.service.ts)**:
  - Thêm method `loadMementos(query: MementoQuery)`.
  - Cập nhật `loadTags` để hỗ trợ fetch tag độc lập.

## Tasks Breakdown
- [x] Task 1: Cập nhật `MonthlyCalendarService` để hỗ trợ load với tag filters.
- [x] Task 2: Thêm signals lọc vào `MonthlyCalendarPageComponent`.
- [x] Task 3: Cập nhật UI dropdown Tags trong HTML để hỗ trợ đa chọn và checkbox phụ trợ.
- [x] Task 4: Implement logic `effect` để tự động reload khi filter thay đổi.
- [x] Task 5: Implement "Ghost Memento" visual rendering trong `MonthlyGridComponent`.
- [x] Task 6: Verify filter hoạt động đúng trên Monthly Calendar.

## Dependencies
- **Depends on**: US-12.1 (Base Monthly Calendar).

## Definition of Done
- [x] Lọc theo Tag trên Monthly Calendar hoạt động chính xác.
- [x] Checkbox "Include children" hoạt động đúng logic (load thêm con hoặc chỉ load memento có tag).
- [x] UI dropdown mượt mà, đồng nhất.
- [x] Ghost Memento hiển thị đúng thiết kế.
- [x] User review & approve.

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-26
- **Approved By**: huy
