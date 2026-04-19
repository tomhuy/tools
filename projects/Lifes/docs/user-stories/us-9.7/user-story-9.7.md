# User Story 9.7: Topic Editor (Add or Update)

## Mô tả
Hợp nhất và nâng cấp công cụ "Thêm Chủ đề" thành một bộ soạn thảo Topic hoàn chỉnh. Cho phép người dùng chỉnh sửa thông tin của các Topic hiện có (Tiêu đề, Ngày, Màu sắc, Tags) trực tiếp từ Monthly Calendar.

### 1. Người dùng (User Personas)
- Người dùng cá nhân muốn điều chỉnh kế hoạch hoặc thay đổi phân loại của một chủ đề lớn.

### 2. Tiến Độ (Roadmap)
- [x] **Task 1: Refactor AddTopicViewModel thành AddOrUpdate** (Hỗ trợ Load dữ liệu cũ).
- [x] **Task 2: Cập nhật giao diện Topic Editor** (Thay đổi tiêu đề form linh hoạt).
- [x] **Task 3: Cải thiện Popup Edit của Monthly Calendar** (Bổ sung nút Sửa Chủ đề).
- [x] **Task 4: Tích hợp logic Update** (Lưu thay đổi xuống database JSON).

---

### 3. Tiêu chí chấp nhận (Acceptance Criteria)
- [x] Khi nhấn vào một cell thuộc hàng Topic (cột Tiêu đề), Popup Edit hiện ra nút "✏️ Sửa chi tiết Chủ đề".
- [x] Nhấn "Sửa Chủ đề" sẽ mở ra Form Topic Editor với toàn bộ dữ liệu cũ đã được điền sẵn.
- [x] Form hiển thị tiêu đề "Sửa Chủ đề" khi đang ở chế độ cập nhật.
- [x] Thay đổi thông tin và nhấn "Lưu" sẽ cập nhật đúng Memento cũ thay vì tạo mới.
- [x] Sau khi cập nhật, Monthly Calendar tự động Refresh để hiển thị thông tin mới nhất.
