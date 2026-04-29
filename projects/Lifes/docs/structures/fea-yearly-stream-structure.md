# Feature Structure: Yearly Stream View (US-17.1)

## Overview

Yearly Stream View cung cấp một ma trận 12 tháng x 31 ngày giúp người dùng theo dõi thói quen, tâm trạng và các hoạt động đọc sách/viết lách trong suốt một năm. Tính năng này được tối ưu hóa cho màn hình độ phân giải cao (4K) và sử dụng hệ thống lọc dữ liệu mạnh mẽ dựa trên Angular Signals.

## User Stories
- ✅ **US-17.1**: Triển khai Yearly Stream (Matrix 12x31), tích hợp hệ thống đọc sách (Book Reader) và bài viết (Post Reader) dạng Mailbox.

## Components

### 1. YearlyStreamPageComponent
- **Path**: `src/app/features/yearly-stream/yearly-stream-page/`
- **Role**: Màn hình chính điều khiển việc hiển thị lưới ma trận.
- **Key Features**:
    - **Matrix Calculation**: Sử dụng `computed` signal để biến đổi danh sách phẳng `StreamEntry[]` thành ma trận 2 chiều `31 x 12` phục vụ cho việc render theo hàng (ngày) và cột (tháng).
    - **Responsive Grid**: Sử dụng Flexbox layout để đảm bảo lưới tự động giãn cách và lấp đầy chiều cao cửa sổ, hỗ trợ hiển thị trọn vẹn 31 ngày trên màn hình 4K.
    - **Interactive Indicators**: Hiển thị biểu tượng 📖 cho sách và các hình chữ nhật nhỏ (`post-rect`) cho các bài viết.
    - **Future Muting**: Sử dụng logic `isCellFuture` để vô hiệu hóa màu sắc cho các ô thuộc về tương lai.

### 2. YearlyStreamService
- **Path**: `src/app/features/yearly-stream/yearly-stream.service.ts`
- **Role**: Quản lý trạng thái và cung cấp dữ liệu cho component.
- **Key Features**:
    - **Reactive State**: Sử dụng `signal` để quản lý năm hiện tại và bộ lọc (`all`, `mood`, `idea`).
    - **Mock Data Generator**: Cung cấp dữ liệu mẫu phong phú với hàng chục đầu sách và bài viết được phân bổ đều trong năm.

### 3. Mailbox Style Reader (UI Concept)
- **Path**: Tích hợp trong `YearlyStreamPageComponent` template và CSS.
- **Layout**: 
    - **Sidebar (300px)**: Danh sách các item (posts/books) có thể cuộn độc lập.
    - **Main Content**: Nội dung chi tiết của item đang được chọn.
- **State**: Quản lý qua `selectedPostIndex` signal để chuyển đổi nội dung mượt mà.

## Data Models

### StreamEntry
- `date: Date`
- `label: string` (T2, T3...)
- `color: string` (Màu sắc thanh chỉ báo)
- `dots: string[]` (Mảng màu tâm trạng)
- `books?: StreamBook[]`
- `posts?: StreamPost[]`

### StreamPost
- `title, excerpt, content, author, date`

## Aesthetics & UX
- **Dark Theme**: Sử dụng nền `#1a1a1a` với các panel `glassmorphism` (backdrop-blur).
- **Animations**: Hiệu ứng `slideUp` cho các modal popup và `fadeIn` cho việc nạp trang.
- **Colors**: Hệ thống màu sắc tương phản cao (Pink, Green, Orange, Purple) giúp nhận diện nhanh các loại hoạt động.

## Integration
- Hiện tại đang sử dụng Mock Data.
- Sẵn sàng tích hợp API từ `.NET WebApi` thông qua `YearlyStreamService`.
