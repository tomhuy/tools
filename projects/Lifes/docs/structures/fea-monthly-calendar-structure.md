# Feature: Monthly Calendar (Electron Migration)

## Overview
Di chuyển giao diện Lịch tháng (Monthly Calendar) từ WPF sang Electron/Angular. Mục tiêu là đạt được sự tương đồng 100% về UI (pixel-parity) và logic hiển thị Gantt/Dot, đồng thời xây dựng nền tảng reactive bằng Angular Signals cho các tính năng CRUD tiếp theo.

## Architecture

### Presentation Layer (Angular)
- `monthly-calendar-page.component.ts` - Container component quản lý toolbar, state của tháng được chọn, và chế độ hiển thị. Thực hiện load dữ liệu ban đầu qua `ngOnInit`.
- `monthly-grid.component.ts` - Pure presentational component chịu trách nhiệm render lưới Gantt, vạch Today, và các phase bars.
- `monthly-grid.component.css` - Chứa logic CSS Grid 31 cột và các style hiển thị (Gantt, Dot, Pure Dot).

### Application Layer (Services & API)
- `calendar-api.service.ts` - [NEW] Service cấp thấp xử lý giao tiếp HTTP với WebApi, unwrap `ApiResponse`.
- `monthly-calendar.service.ts` - Quản lý state bằng Angular Signals. Đã refactor để fetch dữ liệu thực từ API thay vì dữ liệu mẫu.

### Domain Layer (Models & Utils)
- `memento.model.ts` - Định nghĩa cấu trúc phân cấp của Memento (Topic và Child).
- `tag.model.ts` - Định nghĩa cấu trúc Tag.
- `api-response.model.ts` - [NEW] Cấu trúc envelope chuẩn cho dữ liệu trả về từ Backend.
- `color-utils.ts` - [NEW] Port các hàm tính toán màu sắc (Bg, Fg, Contrast) từ bản WPF sang TypeScript.
- `display-mode.model.ts` - Các chế độ hiển thị: `gantt`, `dot`, `pure-dot`.
- `selectable-month.model.ts` - Model cho việc chọn tháng (bao gồm year và month).

## Key Components

### MonthlyGridComponent
**Location**: `src/app/features/monthly-calendar/monthly-grid/`
**Purpose**: Render lưới lịch 31 cột. Sử dụng CSS Grid để định vị các phase bars dựa trên `startDate` và `endDate`.
**Key Logic**:
- `isToday(day, month, year)`: Xác định ngày hiện tại để vẽ vạch đỏ.
- `getStartCol` / `getEndCol`: Tính toán vị trí cột dựa trên ngày và tháng.

### MonthlyCalendarService
**Location**: `src/app/features/monthly-calendar/`
**Purpose**: Single source of truth cho dữ liệu lịch. Sử dụng Signal-based state để đảm bảo chỉ những row bị thay đổi mới re-render.
**Key Logic**:
- `loadInitial(year)`: Gọi API để lấy mementos và tags đồng thời qua `forkJoin`.
- `addChild` / `updateMemento` / `deleteMemento`: Các phương thức mutation đã được kết nối với Backend API qua `CalendarApiService`.

### CalendarController (Backend)
**Location**: `src/Lifes.Presentation.WebApi/Controllers/`
**Purpose**: Cung cấp các REST endpoints (`GET`, `POST`, `DELETE`) để thao tác với `ICalendarService`. Trả về dữ liệu bọc trong `ApiResponse<T>`.

## Data Flow
1. `MonthlyCalendarPageComponent` kích hoạt `loadInitial` trong `ngOnInit`.
2. `MonthlyCalendarService` gọi `CalendarApiService` để thực hiện request tới WebApi.
3. WebApi trả về dữ liệu bọc trong `ApiResponse<T>`.
4. Service cập nhật các Signals, kích hoạt re-render tự động trên Grid.
5. Các thao tác thêm/sửa/xóa được gửi lên API và cập nhật lại state cục bộ ngay khi có phản hồi thành công.

## Key Decisions
- **CSS Grid (31 columns)**: Sử dụng grid cố định 31 cột (mỗi cột 30px) để đảm bảo các thanh Gantt khớp chính xác với tiêu đề ngày.
- **Signal-based Computing**: Sử dụng `computed` để map child mementos theo parent ID, giúp việc render bars trong từng row cực kỳ hiệu quả.
- **Multi-Year Support**: Toàn bộ logic hiển thị và tính toán weekday đều nhận tham số `year` để hỗ trợ hiển thị lịch qua nhiều năm.
- **ApiResponse Envelope**: Sử dụng một cấu trúc phản hồi chuẩn để quản lý lỗi và dữ liệu một cách nhất quán giữa .NET và Angular.
