# Feature: Laputa Notes (US-20.1, US-20.2, US-20.3, US-20.4)

## Overview
Laputa Notes là một giao diện Note-taking tích hợp trong hệ thống. Tính năng này cung cấp một trình soạn thảo Markdown, hỗ trợ chia theo các Section/Tag, và cho phép quản lý Note ở nhiều chế độ hiển thị khác nhau. Giao diện đã được tinh chỉnh pixel-perfect và áp dụng hệ thống Design System cao cấp.

## Architecture

### Presentation Layer (Electron + Angular)
- `laputa-notes-page.component.ts|html|css` - Container chính đóng vai trò host toàn bộ giao diện.
- `laputa-sidebar.component.ts|html|css` - Quản lý Navigation (Sections/Tags), Auto-hide sidebar và Theme.
- `laputa-note-list.component.ts|html|css` - Danh sách ghi chú hỗ trợ **Infinite Scrolling**, **Context Menu** và 4 chế độ view tùy chỉnh theo Design System.
- `laputa-editor.component.ts|html|css` - Trình soạn thảo sử dụng **Reactive Forms**, hỗ trợ **Auto-save (1s debounce)** và cơ chế giữ scroll ổn định.
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
- **Sidebar Navigation (Hover-to-show)**: Triển khai vùng cảm ứng (trigger zone) sát lề trái để hiện icon mở sidebar khi đang ẩn, giúp mở rộng không gian soạn thảo tối đa.
- **Scroll Position Persistence**: Sử dụng cơ chế resize textarea kết hợp deep comparison trong Angular `effect` (thay vì AfterViewChecked) để giữ vị trí scroll ổn định 100% trong quá trình soạn thảo.
- **Design System Tokens (US-20.4)**: Áp dụng hệ thống biến CSS `--sp-1` đến `--sp-12` và các token màu sắc/typography từ `laputa-design-system.md` để đảm bảo tính nhất quán và cảm giác "Premium".

