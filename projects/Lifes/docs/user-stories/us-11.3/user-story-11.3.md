# User Story 11.3: Electron Build & Backend Auto-Run (One-Click Deploy)

## Status: ✅ Completed (2026-04-24)

## 🎯 Overview
**As a** Developer / User  
**I want to** build the entire Electron application (Frontend + Backend) into a single folder with one command, and have the Electron app automatically start the .NET backend when launched  
**So that** I can easily deploy and run the software as a standalone application without manually starting multiple processes.

## 💡 Description
Hiện tại, để chạy ứng dụng với giao diện Electron mới, ta phải chạy riêng phần backend (`Lifes.Presentation.WebApi`) và phần frontend (`Lifes.Presentation.Electron`). US-11.3 giải quyết vấn đề triển khai (deployment) bằng cách:
1. Cung cấp một script tự động hóa để build cả backend (.NET) và frontend (Angular + Electron) vào một thư mục release duy nhất.
2. Cấu hình file `main.js` của Electron để tự động khởi chạy file thực thi của backend (`WebApi.exe`) khi người dùng mở ứng dụng, và tự động tắt backend khi đóng ứng dụng.

## ✅ Acceptance Criteria
1.  **One-Click Build Script**: Có một automation script (ví dụ: `tasks/build-deploy/build-electron.ps1`) thực hiện build WebApi (Release mode), build Angular, và đóng gói Electron ra một thư mục.
2.  **Auto-Start Backend**: Khi chạy file `Lifes.exe` (Electron executable), nó sẽ tự động chạy file `Lifes.Presentation.WebApi.exe` dưới nền (background process).
3.  **Lifecycle Management**: Khi tắt cửa sổ Electron, tiến trình `WebApi.exe` cũng phải được tự động kill (đóng lại) để tránh rò rỉ tài nguyên.
4.  **Port Configuration**: Đảm bảo Electron biết được WebApi đang chạy ở port nào (có thể thông qua tham số command line hoặc config động).

## 🛠 Technical Design
- **Automation**: Sử dụng PowerShell để gọi `dotnet publish` cho WebApi và `npm run package` (hoặc cấu hình tương đương sử dụng `electron-forge` / `electron-builder`) cho Electron.
- **Node.js `child_process`**: Trong `main.js` của Electron, sử dụng `child_process.spawn()` để gọi `WebApi.exe` với các cờ (flags) ẩn cửa sổ terminal (`windowsHide: true`).
- **Process Cleanup**: Lắng nghe các sự kiện `app.on('will-quit')` hoặc `app.on('window-all-closed')` của Electron để gọi `backendProcess.kill()` nhằm giải phóng tài nguyên.

## 📋 Tasks
- [x] Cấu hình `electron-builder` trong `Lifes.Presentation.Electron/package.json` để copy `backend-dist` và sử dụng `win-unpacked`.
- [x] Viết script `build-electron.ps1` để tự động hóa quy trình build và đóng gói.
- [x] Viết script `build-deploy-electron.ps1` để thực hiện backup và triển khai thực tế.
- [x] Cập nhật `main.js` với logic quản lý vòng đời Backend (spawn/kill) và tích hợp logging.
- [x] Cấu hình `Serilog` cho WebApi và ép buộc dùng `appsettings.{env}.json`.
- [x] Tối ưu hóa build bằng cách chỉ sử dụng thư mục giải nén (`win-unpacked`).
- [x] Test thực nghiệm: App mở lên, Backend tự chạy, log ghi đầy đủ, tắt app process biến mất.

## 📊 Definition of Done
- [x] Tài liệu User Story hoàn tất và được cập nhật đúng ý tưởng.
- [x] Chạy lệnh build thành công ra thư mục `win-unpacked`.
- [x] Chạy file `Lifes.exe`, app hiện nội dung Angular, Backend tự chạy tại `localhost:5110`.
- [x] Hệ thống log (Electron & Backend) hoạt động ổn định.
- [x] Tắt app, kiểm tra Task Manager không còn process của WebApi.
