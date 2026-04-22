# Feature: Electron UI Integration

## Overview
Dự án tích hợp lớp hiển thị (Presentation Layer) mới sử dụng công nghệ Web (Electron + Angular) để dần thay thế hoặc song hành cùng WPF. Kiến trúc sử dụng mô hình **Local API Server** (Option A) để giao tiếp giữa Frontend và Backend .NET hiện có.

## Architecture

### Presentation Layer (Frontend)
- **Lifes.Presentation.Electron**: 
  - `main.js`: Quy trình chính của Electron, quản lý cửa sổ ứng dụng và load URL/File.
  - `preload.js`: Cầu nối bảo mật (Context Bridge) giữa Node.js và trình duyệt.
  - `src/app/api.service.ts`: Service Angular chịu trách nhiệm gọi REST API tới Backend.
  - `src/app/app.component.ts`: Component chính sử dụng **Angular Signals** để quản lý trạng thái phản xạ (reactive state).

### Presentation Layer (Backend Bridge)
- **Lifes.Presentation.WebApi**:
  - `Program.cs`: Cấu hình WebAPI, CORS (cho phép Electron truy cập), và Dependency Injection (DI).
  - `Controllers/VersionIncreaseController.cs`: Endpoint RESTful điều hướng các yêu cầu từ UI tới Application Layer (Commands/Queries).

## Key Components

### ApiService (Angular)
**Location**: `src/app/api.service.ts`
**Purpose**: Đóng gói các logic gọi API bằng `HttpClient` của Angular. Sử dụng hàm `inject()` thay vì constructor injection theo best practice mới.

### VersionIncreaseController (WebAPI)
**Location**: `Lifes.Presentation.WebApi/Controllers/`
**Purpose**: Expose các chức năng của `ScanProjectsCommand` ra bên ngoài dưới dạng API endpoint `GET /api/VersionIncrease/scan`.

### SprintBoard (Angular Feature)
**Location**: `src/app/features/sprint-board/`
**Purpose**: Cung cấp giao diện ma trận Kanban cho Sprint Board. Sử dụng native HTML5 Drag & Drop và Angular Signals để quản lý hiệu năng render ma trận lớn.

## Data Flow
1. **User Action**: Người dùng nhấn nút "Scan" hoặc thực hiện kéo thả Task.
2. **Angular Service**: Component gọi service tương ứng (`apiService` hoặc `sprintBoardService`).
3. **Reactive state update**: Angular Signals nhận dữ liệu mới và tự động kích hoạt quá trình render UI hiệu quả.
4. **WebApi Interaction**: Request được gửi tới WebApi Bridge (`Lifes.Presentation.WebApi`) để thực hiện các Application Commands (đối với các tính năng đã đấu nối backend).

## Key Decisions
- **Option A (Local API Server)**: Chọn giải pháp này để tận dụng tối đa code .NET hiện có mà không cần port sang Node.js hoàn toàn.
- **Angular 19 + Signals**: Sử dụng công nghệ mới nhất của Angular để đảm bảo hiệu suất và dễ bảo trì.
- **CORS Configuration**: Phải cấu hình CORS trong WebAPI để cho phép Origin từ Electron (localhost:4200 hoặc file protocol).
- **CSS Hierarchy Over Shell Overflow**: Khóa hiện tượng cuộn của Application Shell và giao quyền cuộn dọc cho Component lớn (như SprintBoard) để tránh hiện tượng có quá nhiều thanh cuộn (3 thanh cuộn) trên một màn hình.
