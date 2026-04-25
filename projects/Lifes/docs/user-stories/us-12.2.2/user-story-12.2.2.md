# User Story: US-12.2.2

## Story Information
- **ID**: US-12.2.2
- **Title**: Quick Phase CRUD via Grid Interaction
- **Priority**: Medium
- **Estimate**: 4 hours
- **Sprint**: Next Sprint

## User Story
- **As a** End User
- **I want to** nhanh chóng tạo hoặc xóa các giai đoạn (child mementos) bằng cách click trực tiếp vào các ô trên lưới (grid cells)
- **So that** em có thể lập lịch nhanh cho các đầu việc nhỏ mà không cần mở form editor phức tạp.

## Acceptance Criteria
1. **Quick Creation/Edit Popup**: Khi click vào một ô trên lưới (ô trống hoặc ô của phase bar hiện có), hiển thị một popup nhỏ.
   - **Input Title**: Mặc định là "X".
   - **Delete Icon**: Nằm bên phải input, cho phép xóa phase (nếu đang edit) mà không cần confirm.
   - **Update Button**: Nằm ở dưới cùng để lưu các thay đổi (hoặc tạo mới).
2. **Phase Range**: Mặc định `startDate` và `endDate` là ngày của ô được click.
3. **Quick Color Edit**: Giữ nguyên tính năng hover hiện mũi tên để đổi màu nhanh.
4. **Instant Feedback**: Giao diện cập nhật ngay lập tức.
5. **Integration**: Sử dụng các phương thức `addChild`, `updateMemento` và `deleteMemento`.

## Technical Design
- **MonthlyGridComponent**:
  - Implement một `QuickPhasePopup` (div overlay).
  - Khi click vào `.grid-cell` hoặc `.phase-bar`, xác định phase đang chọn (hoặc tạo mới) và hiển thị popup tại vị trí click.
  - Form trong popup: Input `title` (autofocus), icon `delete` (SVG), và button `Update`.
  - Giữ lại icon mũi tên đổi màu trên `.phase-bar` như một hành động nhanh độc lập.
  - Xử lý `event.stopPropagation()` để popup không tự đóng ngay khi vừa mở.
- **MonthlyCalendarService**:
  - Đảm bảo `addChild` và `deleteMemento` hoạt động mượt mà với API.
- **UX**:
  - Đảm bảo việc click xóa không bị nhầm lẫn với việc click tạo mới (dùng `event.stopPropagation()` trên phase bar).

## Tasks Breakdown
- [x] Task 1: Cập nhật `MonthlyGridComponent.html` để thêm sự kiện click cho `.grid-cell`.
- [x] Task 2: Implement logic tạo mới phase nhanh trong `MonthlyGridComponent.ts`.
- [x] Task 3: Cập nhật logic click trên `.phase-bar` để xóa thay vì mở editor.
- [x] Task 4: Verify tính năng tạo và xóa hoạt động với Backend API.

## Definition of Done
- [x] Click ô trống -> Tạo phase "X".
- [x] Click phase bar -> Xóa phase (không confirm).
- [x] Dữ liệu được persist vào backend.
- [x] Không có lỗi console hoặc UI glitch.

### Current Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-25
- **Notes**: Đã implement Quick Phase Popup và Quick Color Picker. Tối ưu hóa workflow cho người dùng khi lập lịch các phase nhỏ.
