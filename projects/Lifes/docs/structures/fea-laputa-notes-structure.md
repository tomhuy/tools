# Feature: Laputa Notes (US-20.1, US-20.2)

## Overview
Laputa Notes là một giao diện Note-taking tích hợp trong hệ thống. Tính năng này cung cấp một trình soạn thảo Markdown, hỗ trợ chia theo các Section/Tag, và cho phép quản lý Note ở nhiều chế độ hiển thị khác nhau. Đã được nâng cấp hệ thống đồng bộ hóa dữ liệu real-time với API Backend.

## Architecture

### Presentation Layer (Electron + Angular)
- `laputa-notes-page.component.ts|html|css` - Container chính đóng vai trò host toàn bộ giao diện.
- `laputa-sidebar.component.ts|html|css` - Quản lý Navigation (Sections/Tags) và Theme.
- `laputa-note-list.component.ts|html|css` - Danh sách ghi chú hỗ trợ **Infinite Scrolling** và **Context Menu**.
- `laputa-editor.component.ts|html|css` - Trình soạn thảo sử dụng **Reactive Forms**, hỗ trợ **Auto-save (1s debounce)**.

### Application Layer
- `laputa-notes.service.ts` - Quản lý Reactive State thông qua Angular Signals. Triển khai logic đồng bộ nâng cao:
    - **Sequential Save/Delete**: Sử dụng `concatMap` để xếp hàng các yêu cầu ghi/xóa, đảm bảo toàn vẹn dữ liệu.
    - **Reactive Fetch**: Sử dụng `switchMap` để tự động hủy các request cũ khi đổi bộ lọc hoặc tìm kiếm.
    - **Optimistic UI**: Cập nhật giao diện ngay lập tức khi xóa ghi chú.
- `laputa-api.service.ts` - Tầng giao tiếp HTTP với API Backend `${API_BASE_URL}/notes`.

### Domain Layer (Models)
- `note.model.ts` - Định nghĩa cấu trúc `Note` và `NoteQuery` (Search, Page, Sort).

## Key Decisions
- **Angular Signals & RxJS Integration**: Kết hợp Signals cho State và RxJS (`Subject`, `concatMap`, `switchMap`) cho Side-effects (API calls) tạo ra một hệ thống cực kỳ ổn định và phản hồi nhanh.
- **Debounced Auto-save**: Sử dụng `valueChanges` của Reactive Forms kết hợp `debounceTime(1000)` giúp tối ưu hóa băng thông API.
- **Server-side Duplication**: Chuyển logic nhân bản ghi chú về phía Server để đảm bảo clone chính xác các thuộc tính ẩn và quan hệ dữ liệu.
- **Infinite Scrolling logic**: Tự động gọi trang tiếp theo khi người dùng cuộn đến gần cuối danh sách (`onScroll` event).
