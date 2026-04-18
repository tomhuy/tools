# User Story 9.4: Edit Parent Mementos (Topics)

## Overview
Cho phép người dùng chỉnh sửa thông tin của các Topic (Parent Mementos) trực tiếp từ giao diện Monthly Calendar, bao gồm Tiêu đề, Màu sắc và các Tags phân loại.

## Completed Tasks
- [x] Làm cho tiêu đề dòng (Row Header) có thể click được.
- [x] Hiển thị popup chỉnh sửa khi click vào Topic.
- [x] Hiển thị danh sách Tag phân loại trong popup khi sửa Topic.
- [x] Lưu thay đổi xuống file JSON.

## Technical Details
- **Trigger**: Click vào cột Title của dòng.
- **UI Logic**: Sử dụng `IsShowEditingTag = true` để hiển thị phần chọn Tag.
- **Persistence**: Cập nhật `MementoModel.TagIds` và lưu qua `JsonMementoRepository`.

---

# User Story 9.5: Edit Child Mementos (Supplemental Concepts)

## Overview
Cho phép người dùng thêm mới và chỉnh sửa các ghi chú con (Child Mementos) trên lưới lịch, tập trung vào Tiêu đề và Màu sắc.

## Completed Tasks
- [x] Click vào ô trống để thêm ghi chú con mới (mặc định tiêu đề là "X").
- [x] Click vào thanh màu để sửa ghi chú con hiện có.
- [x] Đảm bảo phần chọn Tag được ẩn đi khi sửa ghi chú con để tinh gọn UI.
- [x] Lưu thay đổi xuống file JSON.

## Technical Details
- **Trigger**: Click vào grid cell hoặc gantt bar.
- **UI Logic**: Sử dụng `IsShowEditingTag = false` để ẩn phần chọn Tag.
