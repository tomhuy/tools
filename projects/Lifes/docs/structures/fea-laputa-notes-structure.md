# Feature: Laputa Notes (US-20.1)

## Overview
Laputa Notes là một giao diện Note-taking tích hợp trong hệ thống, được clone từ nguyên mẫu `sample.note.app.html`. Tính năng này cung cấp một trình soạn thảo Markdown, hỗ trợ chia theo các Section/Tag, và cho phép quản lý Note ở nhiều chế độ hiển thị khác nhau (List, Card, Compact, Grid).

## Architecture

### Presentation Layer (Electron + Angular)
- `laputa-notes-page.component.ts|html|css` - Container chính đóng vai trò host toàn bộ giao diện và truyền trạng thái cho Sidebar, Note List, và Editor.
- `laputa-sidebar.component.ts|html|css` - Hiển thị danh sách các phân mục (All, Starred, Inbox) và hỗ trợ toggle Theme (Dark/Sepia).
- `laputa-note-list.component.ts|html|css` - Hiển thị danh sách các ghi chú, hỗ trợ chức năng Context Menu cục bộ, chuyển đổi giữa 4 chế độ View Mode, và Resize Panel Width.
- `laputa-editor.component.ts|html|css` - Khu vực Editor chính bao gồm Input cho Title, Content, Toolbar cho Markdown, hỗ trợ Detail Popup ở chế độ Grid, và Auto-Resize textarea.

### Application Layer
- `laputa-notes.service.ts` - Quản lý Reactive State thông qua Angular Signals (viewMode, theme, isSidebarOpen, searchQuery, currentSection, currentNoteId, etc.), đồng thời cung cấp các mock dữ liệu về Notes ban đầu. Các thao tác nghiệp vụ như CRUD Note, Toggle Star, Duplicate đều được quản lý tại đây.

## Key Decisions
- **Angular Signals:** Sử dụng Angular Signals cho toàn bộ State Management giúp các Component giao tiếp mượt mà mà không phụ thuộc vào `@Input` hay `@Output` phức tạp.
- **Flexbox & Grid Layout:** Xây dựng Layout Responsive dựa trên Flexbox và CSS Grid.
- **View Modes Handling:** Khi ở chế độ Grid, Editor chính sẽ ẩn đi và hiển thị nội dung thông qua một Popup Detail Panel xuất hiện từ bên phải, trong khi danh sách sẽ mở rộng lấp đầy không gian.
- **Markdown Handling:** Xây dựng logic Editor hoàn toàn dựa trên Javascript Native kết hợp Regex, xử lý các Markdown command trực tiếp vào vùng con trỏ của TextArea, thay vì dùng thư viện bên ngoài nặng nề.
