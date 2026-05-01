# User Story: US-18.1

## Story Information
- **ID**: US-18.1
- **Title**: Weekly Mood & Activity Tracker (Hourly Matrix)
- **Priority**: High
- **Estimate**: 12h
- **Sprint**: Phase 16

## User Story
- **As a** User
- **I want to** ghi nhận tâm trạng và hoạt động theo từng khung giờ trong tuần (7 ngày x 24 giờ)
- **So that** tôi có thể phân tích sự biến động của cảm xúc dựa trên các hoạt động cụ thể và thói quen sinh hoạt.

## Acceptance Criteria
1. [x] **Weekly Grid Layout**: Hiển thị lưới 7 cột (Thứ 2 - Chủ Nhật) và 24 hàng (00h - 23h).
2. [x] **Navigation Header**:
    - Hiển thị khoảng ngày của tuần (vd: 27/4 - 3/5/2026).
    - Nút điều hướng tuần trước/tuần sau và nút "Tuần này".
3. [x] **Advanced Filtering**:
    - Toggle/Radio chọn chế độ hiển thị: "Cả hai", "Mood", "Hành động".
    - Dropdown lọc theo "Ngưỡng cảm xúc".
4. [x] **Grid Cell Content**:
    - Mỗi ô hiển thị chữ cái Mood (A, B+, B...) kèm thanh màu dọc tương ứng.
    - Hiển thị tên hoạt động (nếu có) bên cạnh Mood.
5. [x] **Day Header**: Hiển thị Thứ, Ngày và các chấm tròn (dots) tóm tắt mood tổng quát của ngày đó.
6. [x] **Entry Editor Popup**:
    - Tiêu đề: Thứ, Ngày và Giờ đang chọn.
    - **Mood Selector**: Chọn 1 trong 8 mức độ (A, B+, B, B-, C+, C, C-, D) với màu sắc đặc trưng.
    - **Activity Tags**: Danh sách tag nhanh (Họp, Đọc sách, Coding, v.v.). Cho phép chọn/bỏ chọn.
    - **Free Text**: Ô nhập "Hoặc ghi tự do" và "Tại sao cảm xúc thay đổi?".
    - **Actions**: Nút Xoá (nếu đã có dữ liệu), Huỷ, Lưu.
7. [x] **Premium UI/UX**: Dark theme, hiệu ứng hover, glassmorphism modal, mượt mà và hiện đại.

## Technical Design

### Clean Architecture Layers
- **Presentation**: 
    - `WeeklyTrackerPageComponent`: Quản lý lưới và navigation.
    - `WeeklyEntryEditorComponent`: Modal chỉnh sửa entry.
- **Application**: 
    - `WeeklyTrackerService`: Quản lý state (Signals), tính năng chuyển tuần và lọc dữ liệu.
- **Domain**:
    - `WeeklyEntry` model: `{ time: Date, mood: MoodType, tags: string[], note: string, reason: string }`.
    - `MoodType`: Enum/Type định nghĩa 8 mức độ cảm xúc.

### Files to be Created/Modified
- [x] `src/app/models/weekly-tracker.model.ts`
- [x] `src/app/features/weekly-tracker/weekly-tracker.service.ts`
- [x] `src/app/features/weekly-tracker/weekly-tracker-page/weekly-tracker-page.component.[ts|html|css]`
- [x] `src/app/features/weekly-tracker/entry-editor/entry-editor.component.[ts|html|css]`

## Tasks Breakdown
- [x] Tạo User Story (Planning)
- [x] Thiết kế Models và Service
- [x] Implement Weekly Grid UI (Matrix 7x24)
- [x] Implement Navigation & Filter logic
- [x] Implement Entry Editor Modal (Mood selector, Tags, Text fields)
- [x] Mock data để test hiển thị
- [x] Final UI Polish & Animations

## Definition of Done
- [x] Giao diện clone chính xác theo ảnh mẫu pixel-perfect.
- [x] Các tính năng điều hướng tuần và lọc hoạt động chính xác.
- [x] Modal editor cho phép thêm/sửa/xoá dữ liệu mượt mà.
- [x] Code tuân thủ Clean Architecture và Angular Rules (Signals).
- [x] Tài liệu project được cập nhật (updoc).

## Implementation Progress
- [x] Planning & US Creation
- [x] Implementation & Refinement (Content Explorer Isolated)
- [x] Final Documentation (updoc)

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-05-01
- **Approved By**: User
- **Notes**: Đã tách biệt ContentExplorerService và tối ưu hóa CSS cho chế độ xem nội dung văn bản.
