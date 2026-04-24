# Automation & CICD Structure - Overall

## 🎯 Overview
Tài liệu này hệ thống lại toàn bộ các kịch bản tự động hóa (Automation), quy trình Build, Deploy và CICD của dự án Lifes. Các script này giúp giảm thiểu thao tác thủ công và đảm bảo tính nhất quán giữa các môi trường.

Tham chiếu cấu trúc Backend: [be-all-structure.md](./be-all-structure.md)  
Tham chiếu chi tiết Electron: [fea-electron-structure.md](./fea-electron-structure.md)

---

## 📁 File Organization

Toàn bộ các task automation được tổ chức trong thư mục gốc `tasks/` và các file script điều hướng tại thư mục root.

### 1. Root Entry Points
| File | Nhiệm vụ |
|------|----------|
| `run.ps1` | **Menu chính**: Giao diện điều khiển tập trung tất cả các task. |
| `test.ps1` | Shortcut để chạy unit test với báo cáo Coverage chi tiết. |
| `test-quick.ps1` | Shortcut để chạy test nhanh (thường dùng trong lúc code). |

### 2. Task-Specific Directories (`tasks/`)

#### 📦 Build & Deployment (`tasks/build-deploy/`)
Quản lý việc đóng gói và triển khai ứng dụng (WPF & Electron).

| File | Nhiệm vụ |
|------|----------|
| `build-electron.ps1` | Build .NET WebApi + Angular Frontend và đóng gói Electron thành thư mục `win-unpacked`. |
| `build-deploy-electron.ps1` | Quy trình full: Build -> Backup bản cũ -> Deploy bản mới vào thư mục đích. |
| `deploy-config-electron.json` | Cấu hình tham số deploy (đường dẫn đích, đường dẫn backup). |
| `build.ps1` | (Legacy) Build ứng dụng WPF. |
| `build-deploy.ps1` | (Legacy) Build và deploy ứng dụng WPF. |
| `deploy-config.json` | (Legacy) Cấu hình deploy cho WPF. |

#### 🧪 Test Automation (`tasks/run-tests/`)
Quản lý việc thực thi test và báo cáo chất lượng code.

| File | Nhiệm vụ |
|------|----------|
| `run-tests-with-coverage.ps1` | Chạy toàn bộ test suite, thu thập dữ liệu và tạo báo cáo HTML (ReportGenerator). |
| `run-tests-quick.ps1` | Chạy test không lấy coverage, hỗ trợ chế độ `-Watch` (tự động chạy lại khi code thay đổi). |
| `run-tests-specific.ps1` | Cho phép chọn chạy test cho một project cụ thể. |
| `coverlet.runsettings` | (Tại root) Cấu hình các folder/file loại trừ khỏi báo cáo coverage. |

---

## 🛠 Workflow Logic

### 1. Electron Deployment Flow
1. **Source Control**: Code được commit.
2. **Build**: Gọi `build-electron.ps1`
   - Dotnet Publish (Backend) -> `backend-dist/`
   - Angular Build (Frontend) -> `dist/frontend/` sử dụng `--base-href ./` để load resource tương đối.
   - Electron Builder -> `dist-electron/win-unpacked/` (Sử dụng flag `--dir` để đóng gói nhanh).
3. **Deploy**: Gọi `build-deploy-electron.ps1`
   - Đọc `deploy-config-electron.json` để xác định đích.
   - Nén bản đang chạy thành `.zip` lưu vào folder `Backups`.
   - Copy đè nội dung `win-unpacked` vào thư mục chạy chính.

### 2. Backend Lifecycle trong Electron
- **Startup**: `main.js` dùng `child_process.spawn` khởi chạy backend.
- **Logging**: 
  - Backend ghi vào `logs/backend-.log`.
  - Electron ghi vào `logs/main.log`.
  - Electron bắt `stdout/stderr` của backend và pipe vào log của mình.
- **Shutdown**: Khi đóng cửa sổ, Electron gửi tín hiệu `kill()` để đảm bảo backend không bị treo process.

---

## 📝 Guidelines cho Automation mới
- **Thư mục**: Mỗi nhóm task mới phải nằm trong folder con của `tasks/`.
- **Độc lập**: Script phải tự động tìm đường dẫn gốc của project (`projectRoot`) để có thể chạy từ bất cứ đâu.
- **Cấu hình**: Các tham số biến đổi (đường dẫn, môi trường) phải để trong file `.json` thay vì fix cứng trong `.ps1`.
- **Menu**: Sau khi tạo script mới, hãy cập nhật vào `run.ps1` để user dễ dàng tiếp cận.
