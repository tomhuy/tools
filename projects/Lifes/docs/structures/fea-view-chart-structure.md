# Feature: View Chart & Data Analysis

## Overview
Cung cấp giao diện phân tích dữ liệu memento dưới dạng biểu đồ. Trang này sử dụng bố cục 2 cột (Email-box style) để tối ưu hóa việc tìm kiếm và chọn lọc dữ liệu trước khi trực quan hóa.

## Architecture

### Local Scoped Services (Angular)
Feature này áp dụng mô hình **Local Scoped Services** để đảm bảo tính cách ly dữ liệu:
- `MementoService`: Chịu trách nhiệm fetch dữ liệu memento từ API. Được khai báo trong `providers` của `ViewChartPageComponent`, tạo ra một instance riêng không chia sẻ với các trang khác.
- `ViewChartService`: Chứa logic nghiệp vụ của trang Chart (chọn Topic, đồng bộ search term). Inject `MementoService` để lấy dữ liệu.

### Presentation Layer
- `view-chart-page.component.ts`: Smart component đóng vai trò là "Host" cung cấp các local services và điều phối dữ liệu giữa các component con.
- `view-chart-page.component.html`: Layout chia làm 2 phần:
    - **Topic Sidebar (Trái)**: Chứa thanh search và danh sách topic card thông qua `TopicSearchListComponent`.
    - **Chart Main (Phải)**: Sử dụng `ChartContainerComponent` — một **Passive/Dumb Component** chỉ nhận dữ liệu qua `@Input` và bắn sự kiện qua `@Output`. Điều này đảm bảo tính tái sử dụng cao cho việc vẽ biểu đồ.

## Key Logic

### Multi-topic Selection
- Logic chọn được quản lý trong `ViewChartService` thông qua mảng `selectedTopics = signal<Memento[]>([])`.
- Hỗ trợ thêm/xóa topic từ cả danh sách bên trái và thanh chip bên phải.

### Search Coordination
- `ViewChartService` sử dụng `effect()` để theo dõi sự thay đổi của `searchTerm`. Khi search term thay đổi, nó sẽ điều phối `MementoService` gọi API `loadMementos` với keyword tương ứng.

## Files Structure
- `src/app/core/services/memento.service.ts`: Local data engine.
- `src/app/features/view-chart/view-chart.service.ts`: Page business logic.
- `src/app/features/view-chart/view-chart-page.component.*`: Main UI Host.
- `src/app/features/view-chart/chart-container/`: Chứa `ChartContainerComponent` và UI điều phối render.
- `src/app/features/view-chart/chart-visualizer/`: `ChartVisualizerComponent` — SVG Engine cho thiết kế Premium.
- `src/app/features/view-chart/d3-sample/`: `D3SampleComponent` — Mẫu engine sử dụng D3.js.
- `src/app/features/view-chart/topic-config-popup/`: Modal cấu hình biểu đồ cho từng Topic.
- `src/app/models/chart.model.ts`: Định nghĩa các interface cấu hình biểu đồ (`LabelValue`, `ChartSetting`).

## Key Logic (Updated US-14.2)

### Multi-row Stacked Layout
Hệ thống hỗ trợ tự động phân tầng dữ liệu thành 3 khu vực (Rows) trên cùng một coordinate system nhưng có các dải Y-axis khác nhau:
1. **Events (Hoạt động)**: Hiển thị dạng Dots/Lines cho các memento không có giá trị số (ví dụ: Học tập, Công việc).
2. **Emotions (Cảm xúc)**: Biểu đồ đường (Line) kết hợp vùng màu nền (Zones: Xanh/Vàng/Đỏ) và nhãn trục Y định tính (A, B, C, D).
3. **Sleep (Giấc ngủ)**: Biểu đồ cột (Bar) với đường mục tiêu "7h ★".

### Data Transformation
Logic chuyển đổi từ Memento Title (Text) sang Chart Value (Number) được thực hiện linh hoạt thông qua bảng mapping trong `ChartSetting`. Điều này cho phép người dùng tự định nghĩa "tốt" là mấy điểm, "xấu" là mấy điểm.

## Technical Decisions
- **Manual SVG vs D3.js**: Hệ thống hiện tại hỗ trợ cả 2 hướng tiếp cận. 
    - **Manual SVG**: Tối ưu cho hiệu suất Angular và dễ dàng styling "Premium" bằng CSS/HTML template.
    - **D3.js**: Cung cấp khả năng tính toán Scale và tọa độ mạnh mẽ cho các yêu cầu phân tích chuyên sâu hơn.
- **Stacked Visualization**: Quyết định chia layer theo chiều dọc giúp quan sát được mối tương quan giữa Tác nhân (Events) -> Phản ứng (Emotions) -> Phục hồi (Sleep).
- **Local Providers**: Quyết định không sử dụng `providedIn: 'root'` cho các service này để tránh việc dữ liệu bị "nhiễm" từ các trang khác (ví dụ: Management) và đảm bảo state được dọn dẹp sạch sẽ khi rời trang.
- **Email-box Layout**: Tăng cường trải nghiệm người dùng trong việc lọc và chọn dữ liệu phức tạp.
