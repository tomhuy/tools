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
- `src/app/features/view-chart/view-chart-page.component.*`: Main UI.

## Technical Decisions
- **Local Providers**: Quyết định không sử dụng `providedIn: 'root'` cho các service này để tránh việc dữ liệu bị "nhiễm" từ các trang khác (ví dụ: Management) và đảm bảo state được dọn dẹp sạch sẽ khi rời trang.
- **Email-box Layout**: Tăng cường trải nghiệm người dùng trong việc lọc và chọn dữ liệu phức tạp.
