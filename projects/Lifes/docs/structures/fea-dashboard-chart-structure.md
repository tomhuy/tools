# Feature: Dashboard Chart Layout (US-6.1)

## Overview
Dashboard Chart là một host layout linh hoạt (container) được thiết kế dựa theo tỉ lệ lưới (4x4 Grid). Chức năng này hỗ trợ mở rộng bằng cách sử dụng nguyên lý đa hình (Polymorphism) để tải động các view tùy chỉnh cho từng "khối chữ nhật" tương ứng.

## Architecture

### Presentation Layer
- `DashboardChartView.xaml` - Giao diện chính chứa phân vùng Grid 4x4 sử dụng `DashboardBlockHost`.
- `DashboardChartViewModel.cs` - Quản lý Navigation Menu và truy xuất danh sách Block thông qua Service.
- `Controls/DashboardBlockHost.cs` - Tùy chỉnh `ContentControl` chịu trách nhiệm render view tự động bằng cách tra cứu View Registry.
- `Registries/DashboardViewRegistry.cs` - Bộ máy đăng ký hoạt động bằng cách quét System Assembly lúc ứng dụng khởi tạo để gom nhặt các Class có cờ hiệu `[DashboardBlock(Type)]`.
- `Interfaces/IDashboardBlockView.cs` - Interface đánh dấu (marker interface) cho phép các Custom View ép kiểu dữ liệu `Data` thành Context tương ứng.
- `Views/AstrologyCellView.xaml` - Custom UI View hiển thị layout ô tử vi cho BlockType `AstrologyCell`.
- `Views/DefaultDashboardBlockView.xaml` - View cơ bản (fallback) nếu BlockType không tìm thấy View phù hợp.

### Application Layer
- Cung cấp DTO / Interfaces nếu ứng dụng cần API Fetching.

### Domain Layer
- `Entities/DashboardBlock.cs` - Entity cấu trúc chung cho một ô Grid, chứa properties `BlockType` và Object `Data`.
- `Entities/DashboardCenterInfo.cs` - Domain class chứa Data logic hiển thị khu vực Center của Dashboard (Trung tâm Info).
- `Entities/AstrologyBlockData.cs` - Payload DTO chuyên biệt cho UI `AstrologyCellView`.

### Infrastructure Layer
- `Services/MockDashboardDataService.cs` - Implement Interface DataService. Tạm thời sử dụng dữ liệu Mock Tiếng Việt để test khả năng scale UI và hoạt động của kiến trúc lưới.

## Key Decisions
**Polymorphic View Loading**:
- Thay vì sử dụng `DataTemplateSelector` quá phức tạp, chúng ta thiết kế `DashboardBlockHost`. Đây là một control cực sạch sẽ cho phép tách biệt "Presentation" và "Business Data" một cách triệt để. Bất kỳ thành viên nào trong Team cần bổ sung UI mới đều chỉ việc tạo mới một UserControl kế thừa `IDashboardBlockView`, đánh dấu `[DashboardBlock("TenLoai")]`, không cần phải edit bất kỳ file XAML container nào.
