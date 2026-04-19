# Feature: Annual & Monthly Calendar

## Overview
Công cụ Calendar cung cấp hai chế độ hiển thị: **Annual View** (nhìn tổng thể cả năm) và **Monthly View** (chi tiết từng tháng). Nó hỗ trợ hiển thị các sự kiện dạng timeline, Gantt chart với nhiều giai đoạn (phases) và khả năng chọn nhiều tháng để theo dõi đồng thời.

## Architecture

### Presentation Layer
- `AnnualCalendarView.xaml` - Hiển thị biểu đồ Gantt cho toàn bộ năm (12 tháng). Sử dụng ItemsControl để render các dải màu sự kiện.
- `AnnualCalendarViewModel.cs` - Quản lý dữ liệu sự kiện năm, xử lý điều hướng thông qua `INavigationService`.
- `MonthlyCalendarView.xaml` - Hiển thị chi tiết theo hàng dọc cho một hoặc nhiều tháng được chọn.
- `MonthlyCalendarViewModel.cs` - Quản lý danh sách tháng được chọn (`AvailableMonths`), logic multi-select và render event phases. Điều phối hiển thị TagManager.
- `TagManagementView.xaml` - Giao diện UserControl quản lý Tag, bao gồm danh sách tag, form chỉnh sửa và bảng màu palette.
- `TagManagementViewModel.cs` - Đóng gói toàn bộ logic CRUD cho Tag, bảng màu và thông báo cập nhật qua sự kiện.
- `AddTopicView.xaml` - Form "Thêm Chủ đề mới" với giao diện Light Theme, cho phép nhập tiêu đề, chọn ngày và gán Tags (US-9.6).
- `AddTopicViewModel.cs` - Quản lý logic nghiệp vụ cho việc tạo Topic, bao gồm validation ngày tháng và liên kết TagIds.
- `ActivityHeatmapView.xaml` - Hiển thị "Activity Tracker" dạng lưới ô vuông (Dot Grid), gom nhóm dữ liệu theo sự kiện (Event-centric).
- `ActivityHeatmapViewModel.cs` - Phân loại dữ liệu heatmap theo đầu mục hành động, quản lý hiển thị 31 ngày đồng bộ cho nhiều tháng.
- `SelectableTagViewModel.cs` - View model phụ trợ cho việc chọn tag trong bộ lọc UI.

### Domain Layer (Core)
- `MementoModel.cs` - Entity chính hỗ trợ phân cấp (Topic/Concept) và mã hóa Tags.
- `TagModel.cs` - Entity đại diện cho nhãn phân loại.
- `MementoQueryModel.cs` - Query object để lọc dữ liệu linh hoạt (Tags, Date Range).
- `IMementoRepository.cs`, `ITagRepository.cs` - Interface cho việc truy xuất dữ liệu thô.

### Infrastructure Layer
- `JsonMementoRepository.cs`, `JsonTagRepository.cs` - Implementations lưu trữ dữ liệu bền vững dưới dạng file JSON tại thư mục `database/`.
- `MockMementoRepository.cs`, `MockTagRepository.cs` - (Deprecated) Các lớp mẫu chỉ dùng cho mục đích unit test hoặc dev-mode nhanh.
- `CalendarService.cs` - Lớp Service điều phối, thực hiện logic lọc đệ quy (recursive filtering) và chuẩn bị dữ liệu cho View.

## Key Classes

### MonthlyCalendarViewModel
- **Location**: `src/Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarViewModel.cs`
- **Purpose**: Điều phối việc hiển thị nhiều tháng. Mỗi tháng được bọc trong một `SelectableMonthViewModel` để hỗ trợ chọn/bỏ chọn.
- **Key Logic**: Tính toán vị trí (Offset) và độ dài (Width) của các phase bars dựa trên Grid column width.

### CalendarEventModel
- **Location**: `src/Lifes.Core/Models/CalendarEventModel.cs`
- **Purpose**: Chứa logic dữ liệu cơ bản. Thuộc tính `Phases` cho phép một sự kiện không chỉ là một khối duy nhất mà là một tập hợp các công việc con.

### TagManagementViewModel
- **Location**: `src/Lifes.Presentation.WPF/Features/AnnualCalendar/TagManagementViewModel.cs`
- **Purpose**: Đóng gói logic CRUD Tag. Nó phát ra sự kiện `TagsUpdated` để thông báo cho các View cha (như Monthly Calendar) thực hiện làm mới bộ lọc UI khi dữ liệu Tag thay đổi.

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
- **Tagging & Repository Pattern**: Áp dụng mô hình Repository + Service để tách biệt việc truy vấn dữ liệu thô và logic nghiệp vụ. Hỗ trợ nhiều Tags cho mỗi Memento và cung cấp tính năng **Cascade Filtering**.
- **JSON File Persistence**: Toàn bộ dữ liệu Mementos và Tags được lưu trữ tại `[AppPath]/database/mementos.json` và `tags.json`. Hệ thống tự động khởi tạo dữ liệu mẫu (seed data) nếu file chưa tồn tại, đảm bảo trải nghiệm người dùng liền mạch từ trạng thái mock sang trạng thái lưu trữ thật.
- **Self-contained AddTopic Component**: Tính năng thêm Topic mới (US-9.6) được thiết kế như một thành phần độc lập (standalone), giao tiếp với View cha thông qua sự kiện `TopicAdded`. Điều này giúp giữ cho code-behind của Monthly Calendar sạch sẽ và dễ bảo trì.
