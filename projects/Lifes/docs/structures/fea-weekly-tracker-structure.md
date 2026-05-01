# Feature Structure: Weekly Mood & Activity Tracker (US-18.1)

## Overview

Tính năng Weekly Tracker cho phép người dùng ghi chép chi tiết trạng thái cảm xúc (Mood) và các hoạt động thực hiện (Actions) trong từng giờ của mỗi ngày trong tuần. Giao diện ma trận 7x24 cung cấp cái nhìn trực quan về nhịp sinh học và sự biến động tâm lý.

## User Stories
- ✅ **US-18.1**: Triển khai Weekly Tracker (Matrix 7x24), tích hợp trình chỉnh sửa entry dạng modal glassmorphism.

## Components

### 1. RangeTrackerPageComponent (Weekly Tracker)
- **Path**: `src/app/features/weekly-tracker/range-tracker-page/`
- **Role**: Trang chính của Weekly Tracker (thường gọi là Range Tracker).
- **Key Features**:
    - **Minimalist Matrix**: Render lưới 7 cột với phong cách tối giản, tập trung vào việc hiển thị nhanh mã Mood và Activity.
    - **Navigation**: Đồng bộ `currentDate` signal để tính toán khoảng ngày (range) và nạp dữ liệu tuần tương ứng.
    - **Display Filtering**: Cho phép người dùng toggle giữa 3 chế độ: `both` (hiện mood + note), `mood` (chỉ hiện mood code), `action` (hiện note hoạt động).

### 2. WeeklyTrackerService
- **Path**: `src/app/features/weekly-tracker/weekly-tracker.service.ts`
- **Role**: Trung tâm quản lý trạng thái (State) và dữ liệu cho Range Tracker.
- **Key Features**:
    - **Reactive Intervals**: Tự động tính toán `weekInterval` khi `currentDate` thay đổi.
    - **Signal-based CRUD**: Các phương thức `saveEntry` và `deleteEntry` cập nhật trực tiếp signal `entries`.
    - **Day Summarization**: Logic tính toán các chấm màu (summary dots) cho tiêu đề ngày dựa trên phân bổ mood.

### 3. WeeklyEntryEditorComponent
- **Path**: `src/app/features/weekly-tracker/entry-editor/`
- **Role**: Modal overlay phục vụ việc nhập liệu.
- **Key Features**:
    - **Mood Selection**: Grid 8 mức độ cảm xúc với màu sắc phản ánh từ tích cực đến tiêu cực.
    - **Tagging**: Hệ thống tag nhanh cho các hoạt động phổ biến.
    - **Manual Notes**: Ô nhập liệu bổ sung cho lý do thay đổi cảm xúc hoặc mô tả chi tiết hoạt động.

### 4. ContentExplorerPageComponent
- **Path**: `src/app/features/weekly-tracker/content-explorer-page/`
- **Role**: Chế độ hiển thị chuyên sâu cho nội dung văn bản (News/Posts).
- **Key Features**:
    - **Segmented Control**: Bộ lọc danh mục dạng tab phân đoạn với chấm màu nhận diện.
    - **CSS Isolation**: Sử dụng hệ thống style độc lập để đảm bảo căn lề lưới pixel-perfect (34px height) mà không bị ảnh hưởng bởi style toàn cục của Tracker.
    - **Smart Truncation**: Tự động xử lý dấu ba chấm (`...`) cho các tiêu đề dài để giữ lưới luôn ngăn nắp.

### 5. ContentExplorerService
- **Path**: `src/app/features/weekly-tracker/content-explorer-page/content-explorer.service.ts`
- **Role**: Cung cấp dữ liệu tin tức/nội dung phong phú cho chế độ Explorer.
- **Key Features**:
    - **Data Isolation**: Độc lập hoàn toàn với `WeeklyTrackerService`, cho phép hiển thị nội dung "thú vị" (Tech News) mà không phá vỡ layout tối giản của Range Tracker.
    - **Autonomous State**: Tự quản lý `currentDate`, `rangeDays` và `filterCategory`.

## Data Models

### WeeklyEntry
- `id: string`
- `date: Date` (chứa thông tin ngày và giờ cụ thể)
- `moodId: string` (A, B+, B...)
- `tags: string[]` (Mảng ID các hoạt động)
- `note?: string` (Ghi chú tự do)
- `reason?: string` (Lý do thay đổi cảm xúc)

## Aesthetics & UX
- **Glassmorphism**: Modal editor sử dụng hiệu ứng làm mờ nền (backdrop-blur) và bo góc lớn (24px) tạo cảm giác cao cấp.
- **Hourly Context**: Các mốc thời gian đặc biệt (06h, 12h, 18h) được đánh dấu nhãn (SÁNG SỚM, TRƯA, CHIỀU TỐI) để định hướng nhanh.
- **Snappy Interaction**: Sử dụng Angular Signals đảm bảo việc chuyển tuần và lưu dữ liệu diễn ra mượt mà không có độ trễ.
