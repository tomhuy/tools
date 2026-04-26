# Feature: Memento Management (Electron)

## Overview
Tính năng Memento Management cung cấp giao diện quản lý tập trung cho tất cả các Mementos (Topics). Người dùng có thể tìm kiếm, lọc theo Tag, khoảng thời gian và sắp xếp thứ tự hiển thị của các Topic một cách trực quan thông qua bảng dữ liệu.

## Architecture

### Presentation Layer (Angular)
- **`memento-management.component.ts`** - Smart Component quản lý logic lọc, tải dữ liệu và điều phối các hành động (Edit/Delete).
- **`memento-table/memento-table.component.ts`** - Passive Component chịu trách nhiệm hiển thị dữ liệu dưới dạng bảng, xử lý sắp xếp cột và inline editing cho trường `Order`.
- **`memento-management.component.html`** - Chứa layout cho bộ lọc (Search, Date Range, Tag Filter) và tích hợp bảng dữ liệu.
- **`memento-management.component.css`** - Định nghĩa style cho trang quản lý, tuân thủ `fe_design_rule.md` (full-width layout, responsive design).

### Application Layer (Angular Service)
- **`memento-management.service.ts`** - Service chuyên biệt để tương tác với `CalendarApiService`. Nó quản lý việc fetch dữ liệu mementos dựa trên các tham số lọc phức tạp (keyword, date range, tags) độc lập với phạm vi (scope) của Calendar hiện tại.

### Infrastructure Layer (C# API)
- **`CalendarController.cs`** - Endpoint `GetMementos` được mở rộng để hỗ trợ các filter mới: `Keyword`, `StartDate`, `EndDate`.
- **`CalendarService.cs`** - Logic lọc dữ liệu được tập trung hóa, cho phép bỏ qua filter theo Year/Month nếu có `StartDate`/`EndDate`.

## Key Classes & Components

### MementoManagementComponent (Smart)
**Purpose**: Quản lý trạng thái của các bộ lọc thông qua Angular Signals và tự động kích hoạt việc tải dữ liệu bằng `effect()`.
**Key Actions**:
- `clearFilters()`: Reset toàn bộ bộ lọc về trạng thái mặc định.
- `onUpdateOrder()`: Xử lý sự kiện cập nhật thứ tự từ bảng và gọi API lưu trữ.
- **ShowAchieved Toggle**: Signal cục bộ điều khiển việc hiển thị các topic đã hoàn thành.

### MementoTableComponent (Passive)
**Purpose**: Hiển thị danh sách mementos và cung cấp các tương tác tại chỗ.
**Features**:
- **Column Sorting**: Cho phép sắp xếp theo Title, StartDate, EndDate, và Order.
- **Inline Editing**: Ô nhập liệu `Order` hỗ trợ cập nhật nhanh giá trị ưu tiên.
- **Action Buttons**: Cung cấp nút Edit (bút chì) và Delete (thùng rác) với style tròn hiện đại.

## Data Flow
1. Người dùng thay đổi bộ lọc (Keyword, Tags, hoặc Date Range).
2. `MementoManagementComponent` cập nhật các Signals tương ứng.
3. `effect()` phát hiện thay đổi và gọi `managementService.loadMementos()`.
4. `MementoManagementService` gửi request lên WebApi thông qua `CalendarApiService`.
5. WebApi (C#) lọc dữ liệu từ JSON repository và trả về.
6. Dữ liệu được hiển thị trong `MementoTableComponent`.
7. Khi người dùng sửa `Order`, luồng đi ngược lại từ Table -> Management Component -> Service -> API.

## Key Decisions (ADR 2)
- **Dedicated Service**: Sử dụng `MementoManagementService` thay vì dùng chung `MonthlyCalendarService` để tránh việc dữ liệu bị giới hạn trong phạm vi Năm/Tháng của lịch.
- **Backend Filter Override**: Nếu truyền `StartDate/EndDate`, API sẽ ưu tiên lọc theo khoảng thời gian này và bỏ qua Year/Month context.
- **ShowAchieved Clarity**: Đổi tên tham số query từ `isAchieved` thành `showAchieved` để phản ánh đúng ý nghĩa "bao gồm các mục đã hoàn thành" thay vì lọc chính xác theo trạng thái.
