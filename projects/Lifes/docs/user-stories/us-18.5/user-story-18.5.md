# User Story: US-18.5

## Story Information

| Field | Value |
|-------|-------|
| **ID** | US-18.5 |
| **Title** | Mood Tracker — Metadata Customization & Dynamic Form Fields |
| **Priority** | High |
| **Estimate** | 8 story points |
| **Sprint** | TBD |

## User Story

- **As a** người dùng Mood Tracker
- **I want to** tự tùy biến cấu hình các trường thông tin phụ (metadata fields) và nhập liệu dynamic theo từng khung giờ tâm trạng
- **So that** tôi có thể mở rộng dữ liệu ghi chép một cách linh hoạt (như lượng nước uống, hoạt động tập thể thao, mức độ tập trung) mà không bị giới hạn bởi các trường cố định có sẵn.

---

## Acceptance Criteria

### AC-1: Metadata Definition Model & API Endpoints
- **Given**: Cấu hình metadata cần được quản lý bền vững
- **When**: Gọi API từ frontend
- **Then**:
  - Có API `GET /api/moodmetadata` để lấy tất cả cấu hình metadata đã tạo.
  - Có API `POST /api/moodmetadata` để lưu mới hoặc cập nhật một định nghĩa metadata.
  - Có API `DELETE /api/moodmetadata/{key}` để xóa cấu hình metadata.
  - Dữ liệu được lưu trữ dạng JSON tại `database/mood_metadata_definitions.json`.

### AC-2: Metadata Configuration Screen (Manage Metadata UI)
- **Given**: Người dùng ở màn hình Mood Tracker
- **When**: Mở trình quản lý cấu hình metadata
- **Then**:
  - Giao diện kính mờ (glassmorphism) hiển thị danh sách các metadata fields hiện có.
  - Mỗi field hiển thị thông tin gồm: `Label Display`, `Key`, `Description`, `Input Type`, `Options` (nếu có), trạng thái `Enabled`.
  - Có nút để thêm mới một field với các trường:
    - **Label display** (required, text)
    - **Key** (required, unique key, không cho phép chỉnh sửa key khi update)
    - **Description** (text, default là "")
    - **Input type** (dropdown gồm: `text`, `number`, `select`, `checkbox`, `radio`, `textarea`, `date`, `time`, `datetime`, `duration`)
    - **Options** (chỉ cho phép nhập/quản lý danh sách tag nếu type là `select`, `checkbox` hoặc `radio`)
    - **Enabled** (toggle switch, default là `true`)
  - Cho phép người dùng xóa định nghĩa field (nút Delete).

### AC-3: Dynamic Form Fields in Entry Editor
- **Given**: Người dùng click vào một khung giờ mood để ghi chép
- **When**: Màn hình Mood Entry Editor hiển thị
- **Then**:
  - Chỉ load và render các metadata fields được định nghĩa là `enabled === true`.
  - Triển khai động 3 loại dynamic components chính:
    1. **Text**: Ô input nhập văn bản thông thường.
    2. **Select**: Dropdown chọn một giá trị từ danh sách `options`.
    3. **Checkbox**: Nút toggle / checkbox chọn Đúng/Sai (Boolean).
  - Tự động map giá trị đã lưu trước đó trong `entry.metadata` (nếu có) lên form tương ứng với từng `key`.

### AC-4: Data Persistence & Upsert Logic
- **Given**: Người dùng click Save trong Mood Entry Editor
- **When**: Gửi yêu cầu lưu trữ entry lên backend
- **Then**:
  - Dữ liệu metadata được gom thành một key-value object gửi lên trường `metadata` của `MoodEntry`.
  - Hệ thống thực hiện hành động **Upsert** (ghi đè giá trị mới, không xóa các key dữ liệu cũ không xuất hiện trong form hiện tại - ví dụ các trường đã bị disabled hoặc bị xóa khỏi config nhưng vẫn có dữ liệu lịch sử trong database).
  - Nếu key mới không tồn tại trong form ghi đè, hệ thống vẫn giữ nguyên trường thông tin đó từ database.

---

## Technical Design

### Clean Architecture Layers

| Layer | Thay đổi |
|-------|----------|
| **Core** | - Thêm `MoodMetadataDefinition.cs` model.<br>- Thêm interface `IMoodMetadataRepository.cs`.<br>- Bổ sung property `Metadata` (`Dictionary<string, object>`) vào `MoodEntry.cs`. |
| **Infrastructure** | - Tạo `JsonMoodMetadataRepository.cs` quản lý lưu trữ metadata definitions.<br>- Cập nhật `JsonMoodEntryRepository.cs` để đồng bộ property `Metadata` khi cập nhật entry. |
| **Presentation (Backend)** | - Tạo `MoodMetadataController.cs` cung cấp REST API cho frontend. |
| **Presentation (Frontend)** | - Cập nhật `weekly-tracker.model.ts` với kiểu `MoodMetadataDefinition` và field `metadata` trong `MoodEntry`.<br>- Tạo `MoodMetadataApiService` để gọi API quản lý định nghĩa.<br>- Tạo component `MoodMetadataManagerComponent` (Modal hoặc slide-over panel) quản lý fields.<br>- Cập nhật `MoodEntryEditorComponent` để tự động render dynamic fields theo cấu hình. |

### Files to Create / Modify

#### [NEW] Backend Files
- `src/Lifes.Core/Models/MoodMetadataDefinition.cs`
- `src/Lifes.Core/Interfaces/IMoodMetadataRepository.cs`
- `src/Lifes.Infrastructure/Features/MoodTracker/Repositories/JsonMoodMetadataRepository.cs`
- `src/Lifes.Presentation.WebApi/Controllers/MoodMetadataController.cs`

#### [MODIFY] Backend Files
- `src/Lifes.Core/Models/MoodEntry.cs` ← thêm property `Metadata`
- `src/Lifes.Infrastructure/Features/MoodTracker/Repositories/JsonMoodEntryRepository.cs` ← map metadata when updating
- `src/Lifes.Presentation.WebApi/Program.cs` ← register `IMoodMetadataRepository`

#### [NEW] Frontend Files
- `src/Lifes.Presentation.Electron/src/app/features/weekly-tracker/services/mood-metadata-api.service.ts`
- `src/Lifes.Presentation.Electron/src/app/features/weekly-tracker/mood-metadata-manager/mood-metadata-manager.component.{ts,html,css}`

#### [MODIFY] Frontend Files
- `src/Lifes.Presentation.Electron/src/app/models/weekly-tracker.model.ts` ← add types
- `src/Lifes.Presentation.Electron/src/app/features/weekly-tracker/entry-editor/entry-editor.component.{ts,html}` ← render dynamic components

---

## Tasks Breakdown

### Phase 1: Backend Architecture & APIs
- [x] **Task 1.1**: Tạo model `MoodMetadataDefinition.cs` và interface `IMoodMetadataRepository.cs`.
- [x] **Task 1.2**: Bổ sung property `Metadata` (`Dictionary<string, object>`) vào `MoodEntry.cs`.
- [x] **Task 1.3**: Hiện thực hóa `JsonMoodMetadataRepository.cs` đọc/ghi file `database/mood_metadata_definitions.json`.
- [x] **Task 1.4**: Cập nhật `JsonMoodEntryRepository.cs` map `Metadata` khi upsert entry.
- [x] **Task 1.5**: Tạo controller `MoodMetadataController.cs` và đăng ký DI trong `Program.cs`.

### Phase 2: Frontend Data Layer & Models
- [x] **Task 2.1**: Cập nhật `weekly-tracker.model.ts` với các interfaces mới.
- [x] **Task 2.2**: Tạo `MoodMetadataApiService` giao tiếp với `MoodMetadataController`.

### Phase 3: Metadata Config Manager UI
- [x] **Task 3.1**: Tạo component `MoodMetadataManagerComponent` cho giao diện CRUD cấu hình fields.
- [x] **Task 3.2**: Hiện thực hóa logic thêm, sửa (không đổi key), bật/tắt (enabled), và xóa field.
- [x] **Task 3.3**: Triển khai thiết kế glassmorphism đồng bộ với design system của ứng dụng.

### Phase 4: Dynamic Form Rendering in Editor
- [x] **Task 4.1**: Cập nhật `MoodEntryEditorComponent` để fetch danh sách metadata definitions (chỉ lấy các trường `enabled === true`).
- [x] **Task 4.2**: Triển khai template render động 3 dynamic component: Text, Select (dropdown), Checkbox.
- [x] **Task 4.3**: Bind data đã có của entry lên form khi mở.
- [x] **Task 4.4**: Thực hiện lưu trữ an toàn (Upsert logic - ghi đè key mới, giữ lại key cũ).

---

## Dependencies

- **Depends on**: US-18.2 (Backend API Integration)
- **Blocked by**: Không có

---

## Definition of Done

- [x] API endpoints `/api/MoodMetadata` hoạt động đầy đủ, không phát sinh lỗi.
- [x] Trình quản lý Cấu hình Metadata CRUD thành công các trường thông tin.
- [x] Editor tự động render dynamic fields theo cấu hình và loại component chính xác.
- [x] Dữ liệu lưu trữ đúng đắn dạng key-value và merge an toàn khi upsert.
- [x] Thiết kế đạt chuẩn premium, glassmorphism đồng bộ.
- [x] Chạy lại code và cập nhật tài liệu feature structure thành công.
