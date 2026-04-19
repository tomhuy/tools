# User Story 9.5: Quản lý danh sách Tag (CRUD)

## Overview
Cho phép người dùng hoàn toàn quản lý danh sách các Tag phân loại, không chỉ sử dụng các tag mặc định mà còn có thể tự tạo, sửa đổi màu sắc và xóa bỏ các tag không cần thiết.

## Completed Tasks
- [x] Cập nhật Interface `ITagRepository` và `ICalendarService`.
- [x] Triển khai logic Lưu/Xóa xuống file JSON tại `JsonTagRepository`.
- [x] Thêm nút "Thiết lập Tags" vào giao diện chính của Monthly Calendar.
- [x] **Refactoring (Componentization)**: Tách logic và UI thành `TagManagementView` và `TagManagementViewModel` độc lập.
- [x] Hỗ trợ đổi màu Tag bằng bảng màu đồng nhất (Palette).

## Technical Details
- **Architecture**: Mô hình Component-based.
    - `TagManagementViewModel`: Đóng gói toàn bộ logic CRUD.
    - `TagManagementView`: UserControl độc lập cho phép nhúng vào nhiều View khác nhau.
- **Persistence**: Toàn bộ thay đổi được lưu tại `database/tags.json`.
- **Communication**:
    - Truyền dữ liệu qua `DataContext` (TagManager property).
    - Đồng bộ hóa với View cha thông qua sự kiện `TagsUpdated` (Action delegate).
- **UI Interaction**:
    - Sử dụng `OpenTagManagerCommand` để mở trình quản lý.
    - Cập nhật danh sách Tag ngay lập tức thông qua cơ chế event-driven, đảm bảo thanh lọc dữ liệu ở Calendar View cha luôn khớp với dữ liệu mới nhất.

## Notes
- Khi xóa một Tag, các memento đang gán Tag đó vẫn tồn tại nhưng Tag ID đó sẽ không còn được định nghĩa trong hệ thống (UI sẽ không hiển thị nhãn đó nữa).
