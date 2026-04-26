# User Story 14.2: Dynamic Chart Visualization

## Phân tích yêu cầu
Mục tiêu là xây dựng một hệ thống biểu đồ động cho phép kết hợp nhiều Topic khác nhau lên cùng một hệ tọa độ (Overlay Chart). Mỗi Topic có thể có cách biểu diễn khác nhau (Line, Bar, Scatter) dựa trên cấu hình tùy chỉnh.

## Mô hình dữ liệu (Models)

### LabelValueModel
Dùng để convert các tiêu đề của memento con (text) thành giá trị số (number) để vẽ.
- `label`: string (ví dụ: "D", "C-", "Giấc ngủ tốt")
- `value`: number (ví dụ: 0, 1, 2)
- `color`: string (màu sắc riêng cho label đó)

### ChartSettingModel
- `chartType`: 'line' | 'bar' | 'scatter'
- `labelValues`: LabelValueModel[] (Bảng mapping)

### ChartModel
- `topicId`: number
- `topicTitle`: string
- `setting`: ChartSettingModel
- `order`: number (Thứ tự hiển thị hoặc phân tầng dữ liệu)
- `mementos`: Memento[] (Dữ liệu thực tế của topic và các con)

## Các tính năng chính

### 1. Cấu hình Topic (Topic Configuration Popup)
- Khi click vào topic đã chọn, hiển thị popup.
- Cho phép chọn loại biểu đồ.
- Quản lý Grid mapping (Label -> Value -> Color).
- Thiết lập `order`.
- Lưu cấu hình vào Local State.

### 2. Trục tọa độ (Coordinate System)
- **Trục X**: Hiển thị theo ngày trong tháng đã chọn (1 -> 28/30/31).
- **Trục Y**: Tự động điều chỉnh theo dải giá trị của `labelValues`.

### 3. Data Fetching & Sync
- Khi một Topic được chọn vào danh sách Analysis, hệ thống sẽ gọi API để lấy dữ liệu chi tiết của Topic đó (bao gồm các memento con) cho khoảng thời gian đang chọn (Month/Year).
- Dữ liệu này được lưu trữ tập trung để phục vụ việc chuyển đổi sang giá trị số trước khi vẽ.

### 4. Rendering Engine
- Nút **"Render"** để kích hoạt việc vẽ.
- Hỗ trợ vẽ đè (overlay) nhiều loại chart.
- Hiển thị chú thích (Legend) và các ghi chú đặc biệt (Annotation) như trong ảnh mẫu.

## Tasks Breakdown
- [x] Task 1: Định nghĩa các Interface và Model trong TypeScript (`chart.model.ts`).
- [x] Task 2: Phát triển `TopicConfigPopupComponent` với Grid Mapping.
- [x] Task 3: Nâng cấp `ViewChartService` để quản lý local state của configurations.
- [x] Task 4: Xây dựng Core Visualization Engine (SVG based - Premium Design).
- [x] Task 5: Xử lý logic biến đổi dữ liệu (Data Transformation) từ Memento sang Point Coordinates.
- [x] Task 6: Tích hợp mẫu D3.js Visualization Engine (Bonus).

## Implementation Progress
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-26
- **Approved By**: User
- **Notes**: Đã triển khai xong cả 2 engine (SVG Native & D3.js Sample) với thiết kế phân tầng (Stacked Layout) chuyên nghiệp.
