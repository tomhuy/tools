# User Story: US-6.1

## Story Information
- **ID**: US-6.1
- **Title**: Dashboard Chart Layout Form (Clone)
- **Priority**: High
- **Estimate**: 5 Story Points
- **Sprint**: TBD

## User Story
- **As a** User
- **I want to** mở một màn hình mới có giao diện mô phỏng một lá số tử vi (với cấu trúc 1 lưới 4x4, trung tâm 2x2 trống / chứa thông tin gốc, xung quanh là 12 khối nhỏ)
- **So that** tôi có thể có một khung hiển thị cố định về tỉ lệ chia tỷ lệ để sau này tôi tuỳ ý nhúng (implement) các UI nhỏ vào từng khối riêng biệt.

## Acceptance Criteria
1. **Cấu trúc lưới (Grid Structure)**:
   - Sử dụng một hệ lưới tỷ lệ vuông/chữ nhật hoàn hảo với 4 cột x 4 hàng (hoặc Uniform tương đương).
   - Ô ở giữa (chiếm Hàng 1, Hàng 2 và Cột 1, Cột 2) đóng vai trò là Trung tâm Thông tin (Thiên bàn).
   - Có 12 ô xung quanh làm không gian hiển thị cho các "Cung".
   - Kích thước các ô xung quanh phải bằng nhau. Tỉ lệ scale phải được duy trì hoàn toàn giống hình gốc khi resize ứng dụng.

2. **Cơ chế Load dữ liệu thứ tự**:
   - Có khả năng cấu hình danh sách 12 khối hình chữ nhật bằng collection (e.g. `ObservableCollection<BlockItem>`).
   - Thứ tự load bắt buộc phải xuất phát từ **Góc trên cùng bên trái** (Top-Left, index = 0) và xếp lần lượt theo **chiều kim đồng hồ**.
   - Vị trí tính như sau:
     - 0: Top-Left (Tỵ / Cung Tỵ)
     - 1: Top-Mid-Left (Ngọ)
     - 2: Top-Mid-Right (Mùi)
     - 3: Top-Right (Thân)
     - 4: Right-Mid-Top (Dậu)
     - 5: Right-Mid-Bottom (Tuất)
     - 6: Bot-Right (Hợi)
     - 7: Bot-Mid-Right (Tý)
     - 8: Bot-Mid-Left (Sửu)
     - 9: Bot-Left (Dần)
     - 10: Left-Mid-Bot (Mão)
     - 11: Left-Mid-Top (Thìn)

3. **Placeholder & Dịch Tiếng Việt**:
   - Dữ liệu demo phải bám sát cấu trúc của hình lá số tử vi (bản tiếng Trung) nhưng được chuyển ngữ sang Tiếng Việt.
   - **Khối trung tâm**:
     - Tiêu đề: Lá Số Tử Vi
     - Dương lịch: 2/8/1961
     - Âm lịch: 21/6/1961
     - Can chi: Tân Sửu, Ất Mùi, Đinh Mão, Nhâm Dần
     - Giờ sinh: Dần (03:00~05:00)
     - Giới tính: Nam
     - Mệnh: Bích Thượng Thổ
     - Cục: Thủy Nhị Cục
     - Mệnh Chủ: Cự Môn, Thân Chủ: Thiên Tướng
     - Sinh tiêu (Con giáp): Trâu (Sửu)
     - Cung Hoàng Đạo: Sư Tử
   - **12 Khối xung quanh**:
     - Hiển thị tên Tiếng Việt dựa vào mẫu: Mệnh, Phụ Mẫu, Phúc Đức, Điền Trạch, Quan Lộc, Nô Bộc, Thiên Di, Tật Ách, Tài Bạch, Tử Tức, Phu Thê, Huynh Đệ.

4. **Khả năng mở rộng (Extensibility):**
   - View hiển thị khối chữ nhật phải là dạng **Dynamic Injection (Polymorphism)**.
   - Presentation Layer sẽ tự động load Custom View tương ứng với thuộc tính `BlockType` (thông qua interface `IDashboardBlockView` và `[DashboardBlockAttribute]`). Nếu không có custom view nào khớp, mặc định fallback về `DefaultDashboardBlockView`. Form layout đóng vai trò là **Container/Host**.

## Technical Design

### Clean Architecture Layers
- **Presentation**: 
  - `DashboardChartView.xaml` (Giao diện chứa cấu trúc Grid 4x4).
  - `DashboardChartViewModel.cs` (Binding data và điều hướng NavigationMenu).
  - `DashboardViewRegistry` & `DashboardBlockHost` (Điều phối hiển thị động các UI dựa theo Polymorphism `IDashboardBlockView`).
- **Application**: Commands để lấy danh sách block mẫu. DTO cho `BlockItem` (Index, Tên, Title, Elements...).
- **Domain**: Entity `DashboardBlock` (bổ sung trường `BlockType` và `Data` object) và `DashboardCenterInfo` định nghĩa các thông tin business rule.
- **Infrastructure**: (Hiện tại chưa cần DB / API, mock data trực tiếp hoặc qua 1 `MockChartService`. Tương lai sẽ có service lấy thông tin lá số thật).

### Structure Implementation Guideline (UI)
Sử dụng WPF `Grid` (4x4):
```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="*"/>
        <RowDefinition Height="*"/>
        <RowDefinition Height="*"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    
    <!-- Central Block -->
    <Border Grid.Row="1" Grid.Column="1" Grid.RowSpan="2" Grid.ColumnSpan="2" ... />
</Grid>
```
Đưa vào ItemsControl kết hợp Custom `GridPanel` hoặc attached properties định nghĩa toạ độ để map Index -> (Row, Column) tự động theo chiều kim đồng hồ.

### Files to Create/Modify
- [x] `docs/user-stories/us-6.1/user-story-6.1.md` (Document này)
- [x] `src/Lifes.Presentation.WPF/Features/DashboardChart/DashboardChartView.xaml`
- [x] `src/Lifes.Presentation.WPF/Features/DashboardChart/DashboardChartViewModel.cs`
- [x] Các class hỗ trợ Polymorphism: `DashboardBlockHost.cs`, `DashboardViewRegistry.cs`, `DashboardBlockAttribute.cs`, `IDashboardBlockView.cs`.
- [x] Cập nhật: `ToolIds.cs`, `App.xaml.cs`, `MainWindow.xaml.cs` (App Navigation Integration).

## Tasks Breakdown
- [x] T1: Khởi tạo Entity models cho Chart Info (Domain)
- [x] T2: Chuyển toàn bộ dữ liệu mock tiếng Trung trong hình sáng cấu trúc đối tượng tiếng Việt (Application logic/Mocking).
- [x] T3: Khởi tạo View/ViewModel, phân chia Grid 4x4, cài đặt container (Presentation).
- [x] T4: Viết custom mapping từ item.Index (0-11) trả về Grid.Row và Grid.Column.
- [x] T5: Kiểm tra UI Scale khi window resizing.
- [x] T6: Đăng ký Navigation service để mở Form Dashboard (US-5.1 Integration).
- [x] R1: Refactor UI thành Polymorphism - nạp View qua `DashboardBlockHost` và Registry tự động quét `IDashboardBlockView`.

## Dependencies
- Blocked by: Không.

## Definition of Done
- [x] `user-story-6.1.md` được tạo.
- [x] Layout grid 4x4 được render chính xác như hình với khoảng cách đều nhau.
- [x] Binding được danh sách 12 box theo thứ tự chỉ định (chiều kim đồng hồ từ góc Top-Left).
- [x] Data mockup chuyển thành công sang tiếng Việt.
- [x] Code UI không fix cứng nội dung mà sử dụng dynamic Items (Polymorphic View Loading qua `DashboardBlockHost`).
- [x] Tích hợp Hamburger Navigation Menu để truy cập Form.
