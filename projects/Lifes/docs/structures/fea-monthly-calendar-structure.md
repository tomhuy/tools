# Feature: Monthly Calendar (Electron Migration)

## Overview
Di chuyển giao diện Lịch tháng (Monthly Calendar) từ WPF sang Electron/Angular. Mục tiêu là đạt được sự tương đồng 100% về UI (pixel-parity) và logic hiển thị Gantt/Dot, đồng thời xây dựng nền tảng reactive bằng Angular Signals cho các tính năng CRUD tiếp theo.

## Architecture

### Presentation Layer (Angular)
- `monthly-calendar-page.component.ts` - Container component quản lý toolbar, state của tháng được chọn, và chế độ hiển thị.
- `monthly-grid.component.ts` - Pure presentational component chịu trách nhiệm render lưới Gantt, vạch Today, và các phase bars.
- `monthly-grid.component.css` - Chứa logic CSS Grid 31 cột và các style hiển thị (Gantt, Dot, Pure Dot).

### Application Layer (Services)
- `monthly-calendar.service.ts` - Quản lý state của mementos, tags và cấu hình hiển thị bằng Angular Signals. Cung cấp các computed signals để tối ưu hóa re-render (`topicRows`, `childrenByParent`).

### Domain Layer (Models)
- `memento.model.ts` - Định nghĩa cấu trúc phân cấp của Memento (Topic và Child).
- `tag.model.ts` - Định nghĩa cấu trúc Tag.
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
- `toggleMonth(month, year)`: Cập nhật danh sách tháng hiển thị.
- `seedFakeData()`: Cung cấp dữ liệu mẫu tương đồng với bản WPF để kiểm thử.

## Data Flow
1. `MonthlyCalendarService` giữ state trong các Signals.
2. `MonthlyCalendarPageComponent` inject service và truyền data xuống Grid qua signal inputs.
3. `MonthlyGridComponent` render UI dựa trên inputs.
4. User click trên Grid phát output sự kiện lên Page.

## Key Decisions
- **CSS Grid (31 columns)**: Sử dụng grid cố định 31 cột (mỗi cột 30px) để đảm bảo các thanh Gantt khớp chính xác với tiêu đề ngày.
- **Signal-based Computing**: Sử dụng `computed` để map child mementos theo parent ID, giúp việc render bars trong từng row cực kỳ hiệu quả.
- **Multi-Year Support**: Toàn bộ logic hiển thị và tính toán weekday đều nhận tham số `year` để hỗ trợ hiển thị lịch qua nhiều năm.
