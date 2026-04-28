# Feature: Daily Timeline (UI Prototype)

## Overview
Tính năng Daily Timeline cung cấp giao diện theo dõi và ghi nhận hoạt động hàng ngày chi tiết theo từng giờ. Đây là bản UI Prototype tập trung vào trải nghiệm người dùng cao cấp (Premium UX), sử dụng dữ liệu giả lập (mock data) để demo luồng hoạt động trước khi tích hợp backend.

## Architecture

### Presentation Layer (Electron/Angular)
- `daily-timeline-page/`
    - `daily-timeline-page.component.ts`: Logic điều khiển màn hình chính, tính toán các chỉ số tóm tắt (Highest/Lowest Energy) và quản lý trạng thái modal.
    - `daily-timeline-page.component.html`: Cấu trúc layout gồm Header (điều hướng ngày), Summary Cards, và Timeline Grid (24 giờ).
    - `daily-timeline-page.component.css`: Styling cho giao diện dark theme, glassmorphism và hiệu ứng fade-in.
- `entry-editor/`
    - `entry-editor.component.ts`: Logic cho modal chỉnh sửa khung giờ (Energy grade, tags, notes).
    - `entry-editor.component.html`: Giao diện modal với danh sách Energy Grades và Tag Chips.
    - `entry-editor.component.css`: Styling cho modal overlay, backdrop blur và slide-up animation.

### Models & Services
- `daily-timeline.model.ts`: Định nghĩa các interface `DailyEntry`, `EnergyGrade`, `EnergyLevelInfo`.
- `daily-timeline.service.ts`: Service quản lý trạng thái dữ liệu (sử dụng Angular Signals) và cung cấp dữ liệu mock.

## Key Classes & Components

### DailyTimelinePageComponent
**Location**: `src/app/features/daily-timeline/daily-timeline-page/`
**Purpose**: Component chính quản lý hiển thị 24 khung giờ và các chỉ số tóm tắt của ngày.
**Key Features**:
- Tính toán `highestEnergy` và `lowestEnergy` dựa trên dữ liệu hiện tại.
- Điều hướng ngày (Back/Next/Today).
- Mở modal `EntryEditor` khi người dùng click vào một khung giờ.

### EntryEditorComponent
**Location**: `src/app/features/daily-timeline/entry-editor/`
**Purpose**: Modal overlay cho phép người dùng nhập liệu chi tiết cho một khung giờ.
**Key Features**:
- Chọn mức năng lượng (A, B+, B, B-, C+, C, C-, D).
- Chọn nhiều Tag hoạt động từ danh sách có sẵn.
- Nhập ghi chú tự do.

### DailyTimelineService
**Location**: `src/app/features/daily-timeline/`
**Purpose**: Cung cấp dữ liệu cho toàn bộ feature thông qua Signals.
**Methods**:
- `updateEntry(hour, entry)`: Cập nhật thông tin khung giờ.
- `deleteEntry(hour)`: Xóa thông tin khung giờ.

## Key Decisions
- **UI Prototype first**: Tập trung hoàn thiện giao diện pixel-perfect theo prototype trước khi làm backend.
- **Mock Data Persistence**: Dữ liệu chỉ tồn tại trong phiên làm việc hiện tại (In-memory Signals).
- **Premium Aesthetics**: Sử dụng CSS animations (Fade-in cho trang, Slide-up cho modal) để tăng tính chuyên nghiệp.
