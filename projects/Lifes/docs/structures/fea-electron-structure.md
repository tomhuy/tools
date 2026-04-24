# Feature: Electron UI Integration & Deployment

## Overview
Tính năng này tích hợp Electron làm shell cho ứng dụng Angular frontend, đồng thời quản lý một Backend .NET chạy ngầm (local bridge). Hệ thống hỗ trợ quy trình build và deploy tự động thông qua PowerShell.

## Architecture

### Presentation Layer (Electron Shell)
- `main.js` - Quy trình chính của Electron, chịu trách nhiệm:
  - Khởi tạo cửa sổ BrowserWindow.
  - Quản lý vòng đời tiến trình Backend (.NET WebApi).
  - Tích hợp `electron-log` để ghi nhận nhật ký hệ thống.
- `preload.js` - Cầu nối IPC an toàn giữa Frontend và Electron Main.
- `src/app/` - Angular 19 Frontend sử dụng Signals và RxJS.

### Backend Layer (WebApi Bridge)
- `Lifes.Presentation.WebApi` - ASP.NET Core API server:
  - Cung cấp REST endpoints cho Frontend.
  - Chuyển tiếp yêu cầu tới Application Layer.
  - Tích hợp `Serilog` ghi log vào thư mục `logs/` cục bộ.
  - Tự động thiết lập Working Directory về thư mục chứa file thực thi.

### Automation Layer (Build & Deploy)
- `tasks/build-deploy/build-electron.ps1` - Script đóng gói:
  - Publish .NET Backend vào thư mục tạm.
  - Build Angular Frontend với `--base-href ./`.
  - Sử dụng `electron-builder` để tạo bản phân phối `win-unpacked`.
- `tasks/build-deploy/build-deploy-electron.ps1` - Script triển khai:
  - Backup phiên bản cũ (nếu có).
  - Copy thư mục `win-unpacked` vào thư mục đích.

## Key Decisions

### 1. Unpacked Distribution
Thay vì đóng gói thành một file `.exe` duy nhất (portable/installer), chúng tôi sử dụng thư mục `win-unpacked`. Điều này giúp:
- Tăng tốc độ build (skip bước nén tốn thời gian).
- Khởi động ứng dụng nhanh hơn.
- Dễ dàng cập nhật/thay thế các file tài nguyên lẻ (như `appsettings.json`).

### 2. Mandatory Environment Settings
Backend được thiết lập để luôn yêu cầu file `appsettings.{ASPNETCORE_ENVIRONMENT}.json`. Điều này đảm bảo tính nhất quán giữa môi trường Development và Production.

### 3. Integrated Logging
Hệ thống sử dụng hai cơ chế log song song:
- `electron-log`: Lưu log của shell Electron và log được pipe từ stdout/stderr của Backend.
- `Serilog`: Lưu log chi tiết của Backend vào thư mục `logs/backend-.log`.

## Data Flow
1. User mở `Lifes.exe`.
2. `main.js` khởi chạy, gọi `spawn()` để chạy `Lifes.Presentation.WebApi.exe`.
3. Electron mở cửa sổ và load `index.html` của Angular.
4. Angular Frontend gọi API qua `http://localhost:5110`.
5. Backend xử lý logic và trả kết quả.
6. Khi đóng ứng dụng, `main.js` gửi tín hiệu `kill()` tới tiến trình Backend.
