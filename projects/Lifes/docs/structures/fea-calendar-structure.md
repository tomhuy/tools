# Feature: Annual & Monthly Calendar

## Overview
Công cụ Calendar cung cấp hai chế độ hiển thị: **Annual View** (nhìn tổng thể cả năm) và **Monthly View** (chi tiết từng tháng). Nó hỗ trợ hiển thị các sự kiện dạng timeline, Gantt chart với nhiều giai đoạn (phases) và khả năng chọn nhiều tháng để theo dõi đồng thời.

## Architecture

### Presentation Layer
- `AnnualCalendarView.xaml` - Hiển thị biểu đồ Gantt cho toàn bộ năm (12 tháng). Sử dụng ItemsControl để render các dải màu sự kiện.
- `AnnualCalendarViewModel.cs` - Quản lý dữ liệu sự kiện năm, xử lý điều hướng thông qua `INavigationService`.
- `MonthlyCalendarView.xaml` - Hiển thị chi tiết theo hàng dọc cho một hoặc nhiều tháng được chọn.
- `MonthlyCalendarViewModel.cs` - Quản lý danh sách tháng được chọn (`AvailableMonths`), logic multi-select và render event phases.
- `ActivityHeatmapView.xaml` - Hiển thị "Activity Tracker" dạng lưới ô vuông (Dot Grid), gom nhóm dữ liệu theo sự kiện (Event-centric).
- `ActivityHeatmapViewModel.cs` - Phân loại dữ liệu heatmap theo đầu mục hành động, quản lý hiển thị 31 ngày đồng bộ cho nhiều tháng.

### Domain Layer (Core)
- `CalendarEventModel.cs` - Entity chính của sự kiện, chứa list các `Phases`.
- `CalendarEventPhaseModel` - Đại diện cho một giai đoạn trong sự kiện.

### Infrastructure Layer
- `MockCalendarService.cs` - Cung cấp dữ liệu mẫu phong phú với các sự kiện có nhiều giai đoạn chồng chéo và kéo dài qua nhiều tháng.

## Key Classes

### MonthlyCalendarViewModel
- **Location**: `src/Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarViewModel.cs`
- **Purpose**: Điều phối việc hiển thị nhiều tháng. Mỗi tháng được bọc trong một `SelectableMonthViewModel` để hỗ trợ chọn/bỏ chọn.
- **Key Logic**: Tính toán vị trí (Offset) và độ dài (Width) của các phase bars dựa trên Grid column width.

### CalendarEventModel
- **Location**: `src/Lifes.Core/Models/CalendarEventModel.cs`
- **Purpose**: Chứa logic dữ liệu cơ bản. Thuộc tính `Phases` cho phép một sự kiện không chỉ là một khối duy nhất mà là một tập hợp các công việc con.

## Data Flow
1. `MockCalendarService` trả về danh sách `CalendarEventModel`.
2. `MonthlyCalendarViewModel` lọc các sự kiện thuộc về các tháng đã được chọn.
3. Trong XAML, một `ItemsControl` lồng trong row của event sẽ render các `Border` tương ứng với từng phase bằng cách sử dụng `Margin` để định vị.

## Key Decisions
- **Row Synchronizing**: Các sự kiện được hiển thị trong các hàng có độ rộng header cố định và được render lại khi kéo dãn để đảm bảo giao diện đồng bộ.
- **Gantt Visualization**: Thay vì sử dụng DataGrid phức tạp, chúng dùng `Grid` với các cột định nghĩa theo ngày (1 cột = 1 ngày) để đạt được hiệu năng và độ linh hoạt cao nhất trong styling.
- **Hamburger Navigation**: Thay thế Tab navigation để tối ưu diện tích hiển thị cho các biểu đồ Gantt vốn cần nhiều không gian ngang.
- **Event-Centric Activity Tracking**: Chuyển đổi từ hiển thị theo tháng sang hiển thị theo đầu mục hành động (Event). Điều này giúp tập trung vào thói quen và tần suất thực hiện một hành động cụ thể xuyên suốt cả năm thay vì chỉ nhìn vào một mốc thời gian cố định.
- **Hybrid Display Modes**: Monthly Calendar cho phép chuyển đổi linh hoạt giữa 3 chế độ: **Gantt** (thanh đặc), **Dot** (chấm có viền mờ), và **Pure Dot** (chỉ có chấm). Hệ thống sử dụng `DataTriggers` trong XAML để thay đổi hiển thị ngay lập tức mà không cần tính toán lại dữ liệu ở ViewModel.
- **Unified Memento Model**: Thay thế mô hình Event/Phase bằng cấu trúc `MementoModel` phân cấp (recursive). Một Memento có `ParentId == null` được coi là một "Topic Note" (Ghi chú chủ đề) với Id là kiểu `int`, trong khi memento có `ParentId` trỏ về Topic được coi là "Supplemental Concept Note" (Ghi chú khái niệm bổ sung).
