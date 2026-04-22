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

## Data Flow
1. **User Action**: Người dùng nhấn nút "Scan" trên giao diện Angular.
2. **Angular Service**: `AppComponent` gọi `apiService.scanProjects(path)`.
3. **HTTP Request**: Angular gửi request tới `http://localhost:5110/api/...`.
4. **WebApi Controller**: `VersionIncreaseController` nhận request, gọi `IScanProjectsCommand` thực thi logic.
5. **Application Layer**: Logic nghiệp vụ được thực thi và trả về `Result<List<ProjectFileDto>>`.
6. **Response**: WebApi trả về JSON cho Angular.
7. **Signal Update**: Angular cập nhật `projects.set(data)`, UI tự động render lại.

## Key Decisions
- **Option A (Local API Server)**: Chọn giải pháp này để tận dụng tối đa code .NET hiện có mà không cần port sang Node.js hoàn toàn.
- **Angular 19 + Signals**: Sử dụng công nghệ mới nhất của Angular để đảm bảo hiệu suất và dễ bảo trì.
- **CORS Configuration**: Phải cấu hình CORS trong WebAPI để cho phép Origin từ Electron (localhost:4200 hoặc file protocol).
