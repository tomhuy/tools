# User Story: US-9.6

## Story Information
- **ID**: US-9.6
- **Title**: Thêm Chủ đề mới (Parent Memento)
- **Priority**: High
- **Estimate**: 4 hours
- **Sprint**: 6

## User Story
- **As a** Người dùng ứng dụng Lifes
- **I want to** có một form để tạo mới các Topic (Chủ đề) cha
- **So that** tôi có thể phân loại và theo dõi các mảng nội dung/dự án mới trong cuộc sống một cách có tổ chức.

## Acceptance Criteria
1. [x] **Given** người dùng đang ở màn hình Monthly Calendar
   **When** người dùng nhấn vào nút "Thêm Chủ đề"
   **Then** một Popup nhập liệu xuất hiện với các trường: Tiêu đề, Ngày bắt đầu, Ngày kết thúc, Bảng màu.

2. [x] **Given** Popup đang mở
   **When** người dùng nhập Tiêu đề, chọn màu và thiết lập khoảng ngày (StartDate, EndDate), sau đó nhấn "Lưu"
   **Then** dữ liệu được lưu xuống `mementos.json` với `ParentId = null` và các mốc thời gian chính xác.

3. [x] **Given** dữ liệu vừa được lưu thành công
   **When** Popup đóng lại
   **Then** danh sách Topic ở cột bên trái được cập nhật ngay lập tức và hiển thị đúng trong các tháng tương ứng của Topic đó.

4. [x] **Given** người dùng nhập Ngày kết thúc nhỏ hơn Ngày bắt đầu
   **When** quan sát nút "Lưu"
   **Then** nút "Lưu" bị vô hiệu hóa (Disabled) và không cho phép thực hiện hành động.

## Technical Design

### Clean Architecture Layers
- **Presentation**: 
    - `AddTopicView.xaml`: UserControl chứa form nhập liệu Topic.
    - `AddTopicViewModel.cs`: ViewModel đóng gói logic khởi tạo Topic mới, chọn màu và gọi Service để lưu.
    - `MonthlyCalendarView.xaml`: Hosting component cho `AddTopicView`.
- **Application**: 
    - Logic nghiệp vụ được thực hiện trực tiếp trong `AddTopicViewModel` bằng cách gọi `ICalendarService`.
- **Domain**: 
    - Thực thể `MementoModel` được khởi tạo với `ParentId = null`.
- **Infrastructure**: 
    - `JsonMementoRepository` thực hiện ghi dữ liệu xuống `mementos.json`.
- **Core**: 
    - Sử dụng `ICalendarService` và `ITagRepository`.

### Communication & Design
- **Approach**: Self-contained Component (Phương án A).
- **Event-driven**: Component phát ra sự kiện `TopicAdded` (Action delegate) khi lưu thành công.
- **Parent Responsibility**: `MonthlyCalendarViewModel` lắng nghe sự kiện để làm mới bộ lọc và danh sách Topic hiển thị.

### Files to Create/Modify
- [ ] [MODIFY] [MonthlyCalendarView.xaml](file:///c:/Users/bmhuy/OneDrive/Desktop/14%20days/learns/this-is-my-life/tools/projects/Lifes/src/Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarView.xaml)
- [ ] [MODIFY] [MonthlyCalendarViewModel.cs](file:///c:/Users/bmhuy/OneDrive/Desktop/14%20days/learns/this-is-my-life/tools/projects/Lifes/src/Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarViewModel.cs)
- [ ] [MODIFY] [user-story-9.6.md](file:///c:/Users/bmhuy/OneDrive/Desktop/14%20days/learns/this-is-my-life/tools/projects/Lifes/docs/user-stories/us-9.6/user-story-9.6.md) (Updating progress)

## Tasks Breakdown
- [x] Task 1: Thiết kế giao diện nút bấm và Popup trong XAML.
- [x] Task 2: Khai báo các thuộc tính và Commands cần thiết trong ViewModel.
- [x] Task 3: Triển khai logic lưu dữ liệu và thông báo cập nhật kết quả.
- [x] Task 4: Kiểm thử thủ công với dữ liệu thực tế.

## Dependencies
- Depends on: US-9.1 (Unified Memento Model), US-9.3 (Monthly CRUD).

## Definition of Done
- [x] Code implemented following Clean Architecture
- [x] Code reviewed
- [x] Documentation updated
- [x] User Story marked as complete

---

### 4. Ghi chú Tái cấu trúc (Refactoring)
- **Centralized Palette**: Bảng màu của Form này đã được chuyển vào `Lifes.Presentation.WPF.Constants.UIConstants` để dùng chung với các Form khác (TagManager, MonthlyCalendar), đảm bảo tính nhất quán trên toàn ứng dụng.
- **Visual Improvements**: Sử dụng `Ellipse` cho bảng màu để tạo các ô chọn màu hình tròn mềm mại và hiện đại hơn.
