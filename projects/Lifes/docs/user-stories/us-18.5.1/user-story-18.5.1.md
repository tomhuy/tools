# User Story: US-18.5.1

## Story Information

| Field | Value |
|-------|-------|
| **ID** | US-18.5.1 |
| **Title** | Mood Tracker — Extended Edit Modal & Multi-Column Dynamic Fields Layout |
| **Priority** | High |
| **Estimate** | 3 story points |
| **Sprint** | TBD |

## User Story

- **As a** người dùng Mood Tracker
- **I want to** trải nghiệm giao diện Modal Edit rộng rãi hơn với các trường metadata tùy biến được sắp xếp gọn gàng theo hàng 3 cột
- **So that** tôi có thể dễ dàng điền thông tin chi tiết một cách trực quan, khoa học, không bị kéo dài danh sách theo chiều dọc khi cấu hình nhiều trường bổ sung.

---

## Acceptance Criteria

### AC-1: Modal Edit Width Extension
- **Given**: Người dùng click vào một khung giờ mood để ghi chép
- **When**: Màn hình Mood Entry Editor hiển thị
- **Then**:
  - Chiều rộng của modal (`.editor-card`) được tăng lên gần 3 lần chiều rộng cũ (từ 380px lên khoảng 1100px).
  - Giữ nguyên thiết kế cao cấp, glassmorphism, các bo góc và hiệu ứng chuyển động mượt mà.
  - Modal tự căn giữa màn hình một cách hoàn hảo.

### AC-2: Multi-Column Dynamic Fields Layout
- **Given**: Có các trường metadata được kích hoạt (`enabled === true`)
- **When**: Modal Entry Editor hiển thị khu vực "Thông tin bổ sung"
- **Then**:
  - Các trường metadata được hiển thị chính xác theo dạng lưới gồm **3 cột trên mỗi dòng (3 fields per row)**.
  - Sử dụng CSS Grid hoặc Flexbox linh hoạt để đảm bảo các input (Text, Number, Date, Select) và checkbox chiếm trọn bề ngang cột của chúng và tự động ngắt dòng sau mỗi 3 trường.
  - Khoảng cách và lề giữa các trường được căn chỉnh gọn gàng, cân đối.

### AC-3: Preserving Fixed Fields Layout
- **Given**: Có các trường cố định (Mood Selector, Activity Tags, Note, Reason)
- **When**: Modal được mở rộng
- **Then**:
  - Các trường cố định vẫn được giữ nguyên cấu trúc hiển thị hiện tại (chiếm trọn chiều rộng của modal hoặc kéo giãn đều).
  - Tránh tình trạng vỡ layout hoặc hiển thị quá trống trải tại các khu vực cố định này.

---

## Technical Design

### Clean Architecture Layers

| Layer | Thay đổi |
|-------|----------|
| **Presentation (Frontend)** | - Điều chỉnh `entry-editor.component.css` để mở rộng bề ngang của `.editor-card` lên 1100px.<br>- Cập nhật `.metadata-form-grid` thành CSS Grid với `grid-template-columns: repeat(3, minmax(0, 1fr))` để hiển thị 3 cột trên cùng 1 dòng.<br>- Tinh chỉnh CSS để layout của các inputs/checkboxes hiển thị premium và cân đối tại kích thước lớn. |

### Files to Create / Modify

#### [MODIFY] Frontend Files
- `src/Lifes.Presentation.Electron/src/app/features/weekly-tracker/entry-editor/entry-editor.component.css`
- `src/Lifes.Presentation.Electron/src/app/features/weekly-tracker/entry-editor/entry-editor.component.html`

---

## Tasks Breakdown

### Phase 1: Planning & Approval
- [x] **Task 1.1**: Tạo User Story `user-story-18.5.1.md` và trình kế hoạch triển khai.

### Phase 2: UI Sizing & Multi-Column Layout Implementation
- [x] **Task 2.1**: Mở rộng bề ngang `.editor-card` lên 1100px trong `entry-editor.component.css`.
- [x] **Task 2.2**: Thiết kế grid 3 cột cho `.metadata-form-grid` để hiển thị 3 metadata fields trên một hàng.
- [x] **Task 2.3**: Điều chỉnh style cho các phần tử con (Text inputs, Select box, Checkbox row) để chúng cân đối và hiển thị đẹp mắt trong ô grid.

### Phase 3: Fine-Tuning & Documentation
- [x] **Task 3.1**: Tinh chỉnh khoảng cách lề, scrollbar, và độ giãn của các vùng cố định (Mood selection, tags, note, reason).
- [x] **Task 3.2**: Chạy kiểm tra build và chạy updoc đồng bộ tài liệu hệ thống.

---

## Dependencies

- **Depends on**: US-18.5 (Mood Tracker — Metadata Customization & Dynamic Form Fields)

---

## Definition of Done

- [x] Chiều rộng modal edit được kéo rộng thành 1100px (gần 3 lần 380px).
- [x] Các trường metadata được hiển thị đều đặn theo lưới 3 cột trên một dòng.
- [x] Các trường cố định giữ nguyên hành vi và cấu trúc, hiển thị thẩm mỹ.
- [x] Ứng dụng biên dịch thành công không có cảnh báo hoặc lỗi.
- [x] Quy trình `updoc` được chạy hoàn chỉnh đồng bộ tài liệu.

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-05-26
- **Approved By**: huy
