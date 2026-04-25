# User Story: US-12.6

## Story Information
- **ID**: US-12.6
- **Title**: Switch Axes (Vertical Timeline) in Monthly Calendar Grid
- **Priority**: Medium
- **Estimate**: 6 hours
- **Sprint**: Current Sprint

## User Story
- **As a** End User
- **I want to** có thể hoán đổi trục X và Y của lưới lịch tháng.
- **So that** tôi có thể xem các chủ đề theo chiều dọc (Vertical Timeline), giúp so sánh song song các Topic trong một tháng dễ dàng hơn.

## Acceptance Criteria
1. **Switch Axis Button**:
   - Thêm button "Switch Axes" vào toolbar của Monthly Calendar Page.
2. **Vertical Layout Logic**:
   - Khi bật chế độ Vertical:
     - Trục hoành (X): Hiển thị danh sách các Topics.
     - Trục tung (Y): Hiển thị các ngày trong tháng (1-31).
   - Các thanh Phase bar phải xoay dọc và chiếm đúng vị trí ngày trên trục Y.
3. **Responsive Design**:
   - Đảm bảo lưới vẫn hiển thị tốt và có thanh cuộn nếu danh sách Topic quá dài khi ở chế độ dọc.
4. **Consistency**:
   - Các tính năng hiện có (Today indicator, Ghost Memento, Color Picker) vẫn hoạt động đúng trong giao diện mới.

## Technical Design

### Presentation Layer
- **[MODIFY] [monthly-calendar-page.component.ts](../../../src/app/features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component.ts)**:
  - Thêm signal `isVerticalView = signal(false)`.
- **[MODIFY] [monthly-grid.component.ts](../../../src/app/features/monthly-calendar/monthly-grid/monthly-grid.component.ts)**:
  - Thêm input `isVertical = input(false)`.
- **[MODIFY] [monthly-grid.component.html](../../../src/app/features/monthly-calendar/monthly-grid/monthly-grid.component.html)**:
  - Cấu trúc lại template để hỗ trợ class `vertical-mode`.
- **[MODIFY] [monthly-grid.component.css](../../../src/app/features/monthly-calendar/monthly-grid/monthly-grid.component.css)**:
  - Thêm CSS Grid cho chế độ dọc (Topic columns).

## Tasks Breakdown
- [x] Task 1: Tạo signal và button switch trong `MonthlyCalendarPageComponent`.
- [x] Task 2: Cập nhật `MonthlyGridComponent` để nhận input `isVertical`.
- [x] Task 3: Implement CSS Grid và styling cho chế độ dọc (`.vertical-mode`).
- [x] Task 4: Cập nhật logic tính toán vị trí bar trong template HTML cho cả 2 chế độ.
- [x] Task 5: Verify và fix lỗi hiển thị (nếu có).

## Definition of Done
- [x] Hoán đổi trục X/Y mượt mà qua button.
- [x] Vị trí thời gian của các bar vẫn chính xác 100%.
- [x] Không phá vỡ giao diện hiện tại của chế độ Gantt ngang.
