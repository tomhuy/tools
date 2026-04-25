# User Story: US-14.1

## Story Information
- **ID**: US-14.1
- **Title**: View Chart Page - Topic Selection & Layout
- **Priority**: High
- **Estimate**: 6 hours
- **Sprint**: Current Sprint

## User Story
- **As a** End User
- **I want to** có một trang View Chart với bố cục chia đôi: bên trái là danh sách Topic có thể tìm kiếm, bên phải là khu vực hiển thị biểu đồ.
- **So that** tôi có thể dễ dàng chọn một hoặc nhiều Topic để so sánh và phân tích dữ liệu trên biểu đồ (sẽ được implement ở các US tiếp theo).

## Acceptance Criteria

1. **Route / Navigation**:
   - Truy cập được qua route `/view-chart`.
   - Sidebar/Menu chính hiển thị mục "View Chart".
2. **Layout Structure (Email-box Style)**:
   - **Bên trái (300px - 350px)**: Danh sách Topic (Card List) với thanh Search ở trên cùng.
   - **Bên phải (Flexible)**: Khu vực nội dung chính, có Header hiển thị các Topic đã chọn.
3. **Topic List Component (Left)**:
   - Hiển thị danh sách các Topic (parent mementos).
   - Có thanh tìm kiếm (Search) lọc danh sách theo tên.
   - Mỗi item hiển thị tiêu đề và chỉ báo màu sắc của Topic.
   - Click vào một Topic sẽ "chọn" Topic đó.
4. **Chart Box Header (Right)**:
   - Hiển thị danh sách các Topic đã được chọn từ bên trái dưới dạng các "Chip" hoặc "Tab" ở thanh header.
   - Người dùng có thể bỏ chọn Topic từ header này.
5. **Selection Logic**:
   - Cho phép chọn nhiều Topic đồng thời để chuẩn bị cho việc so sánh biểu đồ đa trục.
6. **Technical Standard**:
   - Tuân thủ `angular_rule.md` (Signals, standalone components).
   - Tách biệt rõ ràng Smart (Page) và Passive (List, Box) components.

## Technical Design

### Component Split
- **ViewChartPageComponent** (Smart): Quản lý state danh sách Topic đã chọn, gọi API lấy danh sách Topic.
- **TopicSearchListComponent** (Passive): Nhận danh sách Topic, render card list, emit sự kiện khi chọn/bỏ chọn.
- **ChartContainerComponent** (Passive): Nhận danh sách Topic đã chọn, render Header và placeholder cho biểu đồ.

### Files to Create
- `src/app/features/view-chart/view-chart-page.component.ts/.html/.css`
- `src/app/features/view-chart/topic-search-list/topic-search-list.component.ts/.html/.css`
- `src/app/features/view-chart/chart-container/chart-container.component.ts/.html/.css`

## Tasks Breakdown
- [x] Task 1: Thiết lập Routing cho `/view-chart` và thêm menu navigation.
- [x] Task 2: Tạo `ViewChartPageComponent` với layout 2 cột (Flexbox/Grid).
- [x] Task 3: Phát triển `TopicSearchListComponent` (Bên trái) với tính năng tìm kiếm.
- [x] Task 4: Phát triển `ChartContainerComponent` (Bên phải) với Header hiển thị các Topic đã chọn.
- [x] Task 5: Implement logic chọn nhiều Topic (multi-selection) và đồng bộ giữa 2 bên.

## Definition of Done
- [x] Giao diện layout hiển thị đúng như thiết kế sketch.
- [x] Tìm kiếm Topic hoạt động mượt mà.
- [x] Click chọn Topic bên trái hiển thị ngay lập tức lên Header bên phải.
- [x] Có thể xóa Topic đã chọn từ Header.
- [x] Code đạt chuẩn Clean Architecture.

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-26
- **Approved By**: huy
