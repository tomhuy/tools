# Feature: Laputa Notes (US-20.1, US-20.2, US-20.3)

## Overview
Laputa Notes là một giao diện Note-taking tích hợp trong hệ thống. Tính năng này cung cấp một trình soạn thảo Markdown, hỗ trợ chia theo các Section/Tag, và cho phép quản lý Note ở nhiều chế độ hiển thị khác nhau. Giao diện đã được tinh chỉnh pixel-perfect đạt độ hoàn thiện cao so với nguyên mẫu.

## Architecture

### Presentation Layer (Electron + Angular)
- `laputa-notes-page.component.ts|html|css` - Container chính đóng vai trò host toàn bộ giao diện.
- `laputa-sidebar.component.ts|html|css` - Quản lý Navigation (Sections/Tags) và Theme.
- `laputa-note-list.component.ts|html|css` - Danh sách ghi chú hỗ trợ **Infinite Scrolling** và **Context Menu**.
- `laputa-editor.component.ts|html|css` - Trình soạn thảo sử dụng **Reactive Forms**, hỗ trợ **Auto-save (1s debounce)**.
- `laputa-notes.service.ts` - Quản lý Reactive State thông qua Angular Signals.
- `laputa-api.service.ts` - Tầng giao tiếp HTTP với API Backend.

### Presentation Layer (WebApi)
- `NotesController.cs` - REST API endpoints cho Note CRUD, nhân bản và xử lý query.

### Application Layer (C#)
- **Commands & Queries**: `GetNotesQuery`, `SaveNoteCommand`, `DeleteNoteCommand`, `DuplicateNoteCommand`.
- **Query Strategies**: `InboxQueryStrategy`, `AllNotesQueryStrategy`, `CategoryQueryStrategy` - Đóng gói logic lọc ghi chú theo từng loại yêu cầu.
- **Factory**: `NoteQueryStrategyFactory` - Quyết định strategy dựa trên `QueryType`.

### Application Layer (Frontend Service)
- `laputa-notes.service.ts` - Quản lý Reactive State và Side-effects:
    - **Sequential Save/Delete**: Sử dụng `concatMap` để xếp hàng các yêu cầu.
    - **Reactive Fetch**: Sử dụng `switchMap` để hủy request cũ.
    - **Optimistic UI**: Cập nhật giao diện ngay lập tức.
    - **String-based ID**: Đồng bộ ID kiểu `string` xuyên suốt hệ thống.

### Infrastructure Layer
- `ObsidianNoteRepository.cs` - Triển khai `INoteRepository`, tương tác trực tiếp với các file Markdown trong Obsidian Vault thông qua FileSystem.

### Domain Layer
- `Note.cs` (C#) - Entity đại diện cho ghi chú trong backend.
- `note.model.ts` (TS) - Model định nghĩa cấu trúc ghi chú ở frontend.
- `INoteRepository.cs` - Interface định nghĩa các thao tác dữ liệu.

## Key Decisions
- **Angular Signals & RxJS Integration**: Kết hợp Signals cho State và RxJS (`Subject`, `concatMap`, `switchMap`) cho Side-effects (API calls) tạo ra một hệ thống cực kỳ ổn định và phản hồi nhanh.
- **Debounced Auto-save**: Sử dụng `valueChanges` của Reactive Forms kết hợp `debounceTime(1000)` giúp tối ưu hóa băng thông API.
- **Server-side Duplication**: Chuyển logic nhân bản ghi chú về phía Server để đảm bảo clone chính xác các thuộc tính ẩn và quan hệ dữ liệu.
- **Infinite Scrolling logic**: Tự động gọi trang tiếp theo khi người dùng cuộn đến gần cuối danh sách (`onScroll` event).
- **Pixel-perfect UI Refinement**: Tinh chỉnh từng pixel cho Grid View và Card View, bao gồm cả khoảng cách (gap), độ bo góc (border-radius) và typography màu vàng (accent) cho tiêu đề ở chế độ Grid.
