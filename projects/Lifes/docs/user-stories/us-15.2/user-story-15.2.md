# User Story: US-15.2

## Story Information
- **ID**: US-15.2
- **Title**: Cho phép nhập mã màu Hex tùy chỉnh trong Editor và Color Picker
- **Priority**: Medium
- **Estimate**: 3 hours
- **Sprint**: Current Sprint

## User Story
- **As a** End User
- **I want to** có thể nhập mã màu Hex trực tiếp trong Topic Editor và Quick Color Picker
- **So that** tôi có thể chọn bất kỳ màu nào mình thích thay vì chỉ giới hạn trong bảng màu Preset.

## Acceptance Criteria
1. **Topic Editor Update**:
   - Có thêm ô nhập liệu (Text input) cho mã màu Hex.
   - Khi chọn một màu từ palette, ô nhập liệu tự động cập nhật.
   - Khi nhập mã màu vào ô, màu preview và topic color tự động cập nhật.
2. **Quick Color Picker Update**:
   - Thêm ô nhập liệu Hex bên dưới bảng màu swatch.
   - Hỗ trợ phím Enter để lưu nhanh màu sau khi nhập.
3. **Validation**: Kiểm tra định dạng mã màu Hex hợp lệ (ví dụ: #FFFFFF).

## Technical Design

### Presentation (Electron/Angular)

#### Topic Editor Component
- **[MODIFY] topic-editor.component.html**: Thêm ô input cho trường `color`.
- **[MODIFY] topic-editor.component.ts**: Đồng bộ hóa logic giữa palette và custom input.

#### Monthly Grid Component (Quick Color Picker)
- **[MODIFY] monthly-grid.component.html**: Thêm ô input vào `quick-color-picker`.
- **[MODIFY] monthly-grid.component.ts**: Thêm logic xử lý thay đổi màu qua input.
- **[MODIFY] monthly-grid.component.css**: Style cho ô input mới trong popup.

## Tasks Breakdown
- [x] Task 1: Thêm ô nhập mã màu Hex vào Topic Editor.
- [x] Task 2: Thêm ô nhập mã màu Hex vào Quick Color Picker (Monthly Grid).
- [x] Task 3: Cập nhật CSS cho các ô nhập liệu mới.
- [x] Task 4: Kiểm tra tính năng đồng bộ màu sắc.

## Definition of Done
- [x] Người dùng có thể nhập màu tùy chỉnh trong Topic Editor.
- [x] Người dùng có thể nhập màu tùy chỉnh trong Quick Color Picker.
- [x] Màu sắc hiển thị chính xác trên lưới Gantt và trong bảng quản lý.
- [x] Mã màu không hợp lệ được xử lý hoặc có fallback.

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-26
- **Approved By**: User
