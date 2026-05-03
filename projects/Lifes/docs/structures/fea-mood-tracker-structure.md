# Feature Structure: Mood & Activity Tracker (US-18.1, US-18.2)

## Overview

Tính năng Mood Tracker cho phép người dùng ghi chép chi tiết trạng thái cảm xúc (Mood) và các hoạt động thực hiện (Actions) theo từng khung giờ. Dữ liệu được đồng bộ hóa với Backend .NET thông qua REST API, đảm bảo tính bền vững và khả năng truy xuất đa thiết bị.

## User Stories
- ✅ **US-18.1**: Triển khai UI Mood Tracker (Matrix Grid), tích hợp trình chỉnh sửa entry dạng modal glassmorphism.
- ✅ **US-18.2**: Tích hợp Backend API, chuyển đổi từ mock data sang persistence storage (JSON Repository).

## Architecture

### Frontend (Angular)

#### 1. MoodApiService
- **Path**: `src/app/features/weekly-tracker/services/mood-api.service.ts`
- **Role**: Giao tiếp trực tiếp với Backend API.
- **Key Methods**:
    - `getAll()`: Lấy toàn bộ danh sách mood entries.
    - `getByRange(start, end)`: Lấy dữ liệu theo khoảng thời gian.
    - `save(entry)`: Lưu mới hoặc cập nhật entry.
    - `delete(id)`: Xóa entry.

#### 2. MoodTrackerService
- **Path**: `src/app/features/weekly-tracker/weekly-tracker.service.ts`
- **Role**: Quản lý State tập trung bằng Angular Signals.
- **Key Features**:
    - **API Integration**: Tự động load dữ liệu khi khởi tạo và đồng bộ hóa sau mỗi thao tác CRUD.
    - **Reactive Navigation**: Tính toán `rangeInterval` và `dayHeaders` tự động khi `currentDate` hoặc `rangeDays` thay đổi.

#### 3. RangeTrackerPageComponent
- **Path**: `src/app/features/weekly-tracker/range-tracker-page/`
- **Role**: Giao diện ma trận linh hoạt (7, 14, 21 ngày).
- **Styles**: `range-tracker-page.component.css` được thiết kế độc lập, tối ưu cho việc hiển thị lưới mật độ cao.

#### 4. MoodEntryEditorComponent
- **Path**: `src/app/features/weekly-tracker/entry-editor/`
- **Role**: Modal nhập liệu tâm trạng và hoạt động.

### Backend (.NET Core)

#### 1. MoodController
- **Path**: `src/Lifes.Presentation.WebApi/Controllers/MoodController.cs`
- **Role**: Expose REST endpoints cho frontend.

#### 2. IMoodEntryRepository & JsonMoodEntryRepository
- **Path**: `src/Lifes.Infrastructure/Features/MoodTracker/Repositories/JsonMoodEntryRepository.cs`
- **Role**: Xử lý logic lưu trữ dữ liệu vào file `database/mood_entries.json`.

#### 3. MoodEntry Entity
- **Path**: `src/Lifes.Core/Models/MoodEntry.cs`
- **Role**: Domain model dùng chung cho toàn hệ thống.

## Data Models

### MoodEntry
- `id: string` (Guid)
- `date: DateTime` (ISO 8601)
- `moodId: string` (A, B+, B, C, D...)
- `tags: string[]`
- `note: string`
- `reason: string`

## Technical Decisions (ADR)
- **JSON Storage**: Sử dụng file JSON để lưu trữ đơn giản, dễ backup và không cần database server phức tạp.
- **Clean Architecture**: Tách biệt hoàn toàn Core Models và Infrastructure để dễ dàng thay đổi database engine (SQL/NoSQL) trong tương lai.
- **ISO Dates**: Toàn bộ dữ liệu ngày tháng được chuẩn hóa sang ISO string trước khi gửi lên API để tránh lỗi múi giờ.
