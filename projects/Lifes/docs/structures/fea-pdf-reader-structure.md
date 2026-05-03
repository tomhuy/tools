# Feature: PDF Reader (US-19.1, US-19.2)

## Overview
PDF Reader là một giao diện tích hợp chuyên dụng dành cho việc đọc, quản lý tài liệu PDF và ghi chú. Giao diện được thiết kế linh hoạt với khả năng chuyển đổi giữa nhiều bố cục (Layouts) khác nhau để phục vụ các mục đích như đọc tập trung, nghiên cứu kết hợp ghi chú, và tra cứu nhanh.

## Architecture

### Presentation Layer (Electron + Angular)
- `pdf-reader-page.component.ts|html|css` - Container chính quản lý bố cục và phân phối không gian cho các thành phần con. Đảm nhận logic chuyển đổi giữa 3 Layout chính (Classic, Focus, Contextual).
- **Library/Book List Panel** - Hiển thị danh sách các tài liệu PDF đang có trong hệ thống.
- **LaputaPdfViewerComponent (US-19.2)** - Dumb Component thuần túy bọc lấy thư viện `ngx-extended-pdf-viewer`. Xử lý các sự kiện bôi đen, tiêm annotations và đồng bộ hóa CSS với Design System.
- **Floating Toolbar & Note Popup** - Các thành phần UI tương tác xuất hiện khi bôi đen văn bản hoặc tạo mới ghi chú. Tọa độ được truyền từ PDF Viewer Component ra Container chính.
- **Read Mode (Layout 4)** - Bố cục sử dụng PDF Viewer thật thay vì các bản fake page, cho phép đọc tài liệu thực tế.

### Application Layer
- `pdf-reader.service.ts` (Dự kiến) - Quản lý trạng thái (Signals) bao gồm Layout hiện tại, Theme (Dark/Light Mode), Mức độ Zoom, và danh sách Ghi chú/Sách.

## Data Flow
1. **Layout State:** Khi người dùng chọn Layout (1, 2, 3), trạng thái được cập nhật qua Signal. `PdfReaderPageComponent` phản hồi bằng cách thay đổi CSS Grid/Flexbox để bố trí lại các Panel.
2. **Text Selection:** Khi người dùng bôi đen văn bản trên PDF Viewport, Floating Toolbar sẽ được kích hoạt tại vị trí con trỏ chuột.
3. **Note Creation:** Người dùng nhập ghi chú qua Note Popup, dữ liệu được truyền vào Service và cập nhật lên Notes Panel.

## Key Decisions
- **Dynamic Layout Management:** Sử dụng CSS Grid/Flexbox kết hợp với Angular Host Binding để hoán đổi nhanh giữa các Layout (Classic, Focus, Contextual) mà không cần phải reload hay khởi tạo lại các component con bên trong.
- **Theme Support:** Hỗ trợ Dark Mode cho không chỉ UI xung quanh mà còn xử lý màu sắc bên trong khu vực PDF (nếu có thể) để tối ưu hóa trải nghiệm đọc vào ban đêm.
- **Floating Toolbar:** Sử dụng tọa độ DOM Selection được tính toán từ Selection API của thư viện để định vị Floating Toolbar một cách chính xác.
- **Dumb Component Pattern (ADR 17):** Tách PDF Viewer thành component thuần túy để đảm bảo khả năng tái sử dụng cao nhất (Memento, Task viewer, etc.).
- **CSP Adjustment:** Cấu hình Content Security Policy của Electron để cho phép tải PDF từ external domains và chạy Worker script từ Blob/Data URI.
