# Feature Structure: Mood & Activity Tracker (US-18.1, US-18.2, US-18.3, US-18.4, US-18.5)

## Overview

Tính năng Mood Tracker cho phép người dùng ghi chép chi tiết trạng thái cảm xúc (Mood) và các hoạt động thực hiện (Actions) theo từng khung giờ. Dữ liệu được đồng bộ hóa với Backend .NET thông qua REST API, đảm bảo tính bền vững và khả năng truy xuất đa thiết bị.

## User Stories
- ✅ **US-18.1**: Triển khai UI Mood Tracker (Matrix Grid), tích hợp trình chỉnh sửa entry dạng modal glassmorphism.
- ✅ **US-18.2**: Tích hợp Backend API, chuyển đổi từ mock data sang persistence storage (JSON Repository).
- ✅ **US-18.3**: Pluggable View System & Content Filter — extract grid thành component, content filter bar, text truncation.
- ✅ **US-18.4**: Intensity Blocks View & Settings Panel — new heatmap view, palette system, pattern aids, settings panel.
- ✅ **US-18.5**: Mood Tracker — Metadata Customization & Dynamic Form Fields.

## Architecture

### Frontend (Angular)

#### 1. MoodApiService
- **Path**: `src/app/features/weekly-tracker/services/mood-api.service.ts`
- **Role**: Giao tiếp trực tiếp với Backend API.
- **Key Methods**:
    - `getAll()`: Lấy toàn bộ danh sách mood entries.
    - `getByRange(start, end)`: Lấy dữ liệu theo khoảng thời gian.
    - `save(entry)`: Lưu mới hoặc cập nhật entry.
    - `delete(id)`: Xóa entry.
- **Note**: `toEntry()` private method normalize `date: string → Date` tại deserialization boundary — UTC-as-source-of-truth pattern.

#### 2. MoodTrackerService
- **Path**: `src/app/features/weekly-tracker/weekly-tracker.service.ts`
- **Role**: Quản lý State tập trung bằng Angular Signals.
- **Key Signals**: `entries`, `currentDate`, `rangeDays`, `displayMode`, `filterMode`, `filterMoodId`, `viewMode`, `palette`, `patternAids`, `compactRows`.
- **Key Features**:
    - **API Integration**: Tự động load dữ liệu khi khởi tạo và đồng bộ hóa sau mỗi thao tác CRUD.
    - **Reactive Navigation**: Tính toán `rangeInterval` và `dayHeaders` tự động khi `currentDate` hoặc `rangeDays` thay đổi.
    - **displayMode**: Signal persist display preference qua navigation (vì là UI state cần persist, đặt trong service có chủ ý — xem ADR 22).
    - **viewMode**, **palette**, **patternAids**, **compactRows**: Signals persist display preferences qua navigation (exception có chủ ý — ADR 22).

#### 3. RangeTrackerPageComponent
- **Path**: `src/app/features/weekly-tracker/range-tracker-page/`
- **Role**: **Container** — quản lý navigation state, filter dropdowns, editor modal. Không chứa grid markup.
- **Hosts**: `MoodMatrixGridComponent`, `MoodEntryEditorComponent`.
- **Pattern**: Container/Presenter (ADR 1). Page lắng nghe `cellClick` từ grid, mở editor.

#### 4. MoodMatrixGridComponent *(US-18.3)*
- **Path**: `src/app/features/weekly-tracker/mood-matrix-grid/`
- **Role**: **Presenter** — render lưới 24h × N ngày. Inject `MoodTrackerService` chỉ để đọc signals.
- **Output**: `cellClick: { day: Date, hour: number }` — page xử lý việc mở editor.
- **Display modes**: `both` (mood label + note), `mood` (label only), `action` (note only), `reason` (reason only).
- **Text truncation**: `overflow: hidden; text-overflow: ellipsis; white-space: nowrap` trên tất cả cell text.

#### 5. IntensityBlocksGridComponent *(US-18.4)*
- **Path**: `src/app/features/weekly-tracker/intensity-blocks-grid/`
- **Role**: **Presenter** — render heatmap grid 24h × N ngày theo intensity (màu sắc). Inject `MoodTrackerService` chỉ để đọc signals.
- **Output**: `cellClick: { day: Date, hour: number }` — page xử lý mở editor.
- **Cell Design**: Flex row — 3px left border (palette FG) + mood letter (FG) + action text truncated (ellipsis). Background = palette BG (translucent).
- **Row Height**: Default = `flex: 1` (24 rows fill available height). Compact = 28px fixed.
- **Pattern Aids**: hourlyAvgRibbon (computed avg per hour), dayMiniSummary (dots in header), alignmentGuidesOnHover (hover signals), highlightRecurringSlump (≥3 days weight≤3).
- **Content Filter**: Reads `displayMode` from service. `getBlockText(entry)` returns '' (mood), note (action), reason (reason), or note·reason joined (both).

#### 6. ViewSelectorBarComponent *(US-18.4)*
- **Path**: `src/app/features/weekly-tracker/view-selector-bar/`
- **Role**: Segmented control để switch `viewMode` (cards / intensity) trong service.

#### 7. RangeTrackerSettingsPanelComponent *(US-18.4)*
- **Path**: `src/app/features/weekly-tracker/range-tracker-settings-panel/`
- **Role**: Settings panel (absolute positioned) với 4 sections: VIEW MODE, COLOR PALETTE, PATTERN AIDS, DENSITY.
- **Controls**: View mode selector, palette picker (fg color swatches), 4 pattern aid toggles, compact rows toggle.

#### 8. MoodEntryEditorComponent *(US-18.5 updated)*
- **Path**: `src/app/features/weekly-tracker/entry-editor/`
- **Role**: Modal nhập liệu tâm trạng và hoạt động.
- **Dynamic fields**: Tự động load danh sách metadata active và render động 3 loại component đầu vào chính: **Text**, **Select (Dropdown)** và **Checkbox**.
- **Defensive Field Sorting**: Áp dụng sắp xếp động phòng thủ các trường metadata theo thứ tự `order` tăng dần trước khi render trên giao diện nhập liệu.
- **Memory safety**: Tích hợp toán tử dọn dẹp `takeUntilDestroyed` từ Angular Core để tự động hủy luồng khi modal đóng.

#### 9. MoodMetadataApiService *(US-18.5)*
- **Path**: `src/app/features/weekly-tracker/services/mood-metadata-api.service.ts`
- **Role**: API Client giao tiếp trực tiếp với Backend REST API để lấy và lưu cấu hình các trường metadata phụ.

#### 10. MoodMetadataManagerComponent *(US-18.5)*
- **Path**: `src/app/features/weekly-tracker/mood-metadata-manager/`
- **Role**: Modal quản lý (CRUD) danh sách cấu hình các trường thông tin bổ sung. Giao diện kính mờ (glassmorphism) sang trọng.
- **Auto key generation**: Cơ chế tự động sinh key duy nhất (dạng chữ thường không dấu, phân cách bằng dấu gạch dưới, ví dụ: `luong_nuoc_uong`).
- **Flexible Options**: Cho phép nhập danh sách tùy chọn dưới 2 định dạng: chuỗi text phân cách bằng dấu phẩy (`Tên hiển thị:giá trị lưu`) hoặc mảng JSON objects.
- **Optimistic Reordering & Sequential Queue**: Bổ sung hai nút Up/Down sắp xếp. Cập nhật state UI tức thì (Optimistic UI) và xếp các luồng lưu vào hàng đợi `reorderQueue` (Subject + concatMap + concat) để thực thi tuần tự hoàn hảo trên DB, chống race condition.

### Backend (.NET Core)

#### 1. MoodController
- **Path**: `src/Lifes.Presentation.WebApi/Controllers/MoodController.cs`
- **Role**: Expose REST endpoints cho frontend.

#### 2. IMoodEntryRepository & JsonMoodEntryRepository
- **Path**: `src/Lifes.Infrastructure/Features/MoodTracker/Repositories/JsonMoodEntryRepository.cs`
- **Role**: Xử lý logic lưu trữ dữ liệu vào file `database/mood_entries.json`.

#### 3. MoodEntry Entity
- **Path**: `src/Lifes.Core/Models/MoodEntry.cs`
- **Role**: Domain model dùng chung cho toàn hệ thống.

#### 4. MoodMetadataController *(US-18.5)*
- **Path**: `src/Lifes.Presentation.WebApi/Controllers/MoodMetadataController.cs`
- **Role**: REST controller expose các endpoints cho frontend CRUD định nghĩa metadata (`GET`, `POST`, `DELETE`).

#### 5. IMoodMetadataRepository & JsonMoodMetadataRepository *(US-18.5)*
- **Path**: `src/Lifes.Infrastructure/Features/MoodTracker/Repositories/JsonMoodMetadataRepository.cs`
- **Role**: Repository lưu trữ danh sách định nghĩa cấu hình metadata an toàn, thread-safe vào file `database/mood_metadata_definitions.json`, hỗ trợ tự động gán order mới và lấy danh sách sắp xếp sẵn theo `Order` tăng dần.

## Data Models

### MoodEntry
- `id: string` (Guid)
- `date: DateTime` (ISO 8601)
- `moodId: string` (A, B+, B, C, D...)
- `tags: string[]`
- `note: string`
- `reason: string`
- `metadata: Dictionary<string, object>` *(US-18.5)* — lưu trữ động các giá trị của các trường thông tin phụ được cấu hình thêm.

### MoodMetadataDefinition *(US-18.5)*
- `key: string` — Key định danh duy nhất (dạng slug, ví dụ: `luong_nuoc_uong`)
- `labelDisplay: string` — Nhãn hiển thị trên giao diện
- `description: string` — Mô tả chi tiết / placeholder
- `inputType: string` — Loại trường hiển thị (text, select, checkbox, textarea...)
- `enabled: boolean` — Trạng thái kích hoạt
- `options: string[]` — Mảng chứa các tùy chọn (cho select/radio) dưới dạng text thô hoặc JSON string
- `order: number` — Chỉ số thứ tự hiển thị sắp xếp (tăng dần)

### ColorPalette
- `id: string`
- `label: string`
- `fg: string[]` — 8 màu saturated.
- `bg: string[]` — 8 màu translucent tương ứng.

## Technical Decisions (ADR)
- **JSON Storage**: Sử dụng file JSON để lưu trữ đơn giản, dễ backup và không cần database server phức tạp.
- **Clean Architecture**: Tách biệt hoàn toàn Core Models và Infrastructure để dễ dàng thay đổi database engine (SQL/NoSQL) trong tương lai.
- **ISO Dates**: Toàn bộ dữ liệu ngày tháng được chuẩn hóa sang ISO string trước khi gửi lên API để tránh lỗi múi giờ.
- **Option B (CRUD API Flow)**: Component quản lý metadata tự inject `MoodMetadataApiService` và trực tiếp lưu/xóa qua API. Nó emit các event `close` và `changed` để Parent page component biết khi nào cần đóng hoặc xử lý cập nhật.
- **Option A (Access Entrypoint)**: Tích hợp button/tab cấu hình ngay bên cạnh View Selector Bar của Mood Tracker Page. Khi click, hiển thị modal kính mờ `app-mood-metadata-manager`.
- **Key Slugification Rules**: Hệ thống tự động chuyển đổi chữ hiển thị sang dạng thường, bỏ dấu tiếng Việt, thay khoảng trắng/ký tự đặc biệt bằng dấu gạch dưới `_` để tạo key duy nhất (ví dụ: `luong_nuoc_uong`).
- **Flexible options parsing**: Cho phép nhập danh sách select options linh hoạt dưới dạng chuỗi `Tên:giá trị` hoặc dạng Mảng JSON Objects, hỗ trợ tự động hiển thị dạng indented JSON định dạng hoàn chỉnh.
- **takeUntilDestroyed**: Quản lý vòng đời dọn dẹp subscription bằng toán tử RxJS hiện đại từ Angular Core để giải phóng bộ nhớ triệt để khi destroy component.
- **Optimistic UI & Sequential Save Queue (concatMap)**: Để triệt tiêu race condition (xung đột tài nguyên file JSON ở backend) khi người dùng click sắp xếp quá nhanh mà vẫn giữ giao diện mượt mà tức thời, ứng dụng áp dụng optimistic UI update local signal, đồng thời đẩy các hành động hoán đổi vào một Subject hàng đợi `reorderQueue` xử lý lưu tuần tự tuyệt đối bằng `concatMap` và `concat`.
- **Extended Modal & 3-Column Grid Layout (US-18.5.1)**: Để tối ưu hóa không gian nhập liệu và tránh kéo dài danh sách theo chiều dọc, chiều rộng của modal nhập liệu `MoodEntryEditorComponent` được mở rộng gấp gần 3 lần (1100px) kết hợp `max-width: 95vw` giúp tự thích ứng màn hình. Các trường thông tin động được phân bổ theo lưới CSS Grid 3 cột (`grid-template-columns: repeat(3, minmax(0, 1fr))`) giúp hiển thị đều đặn 3 ô nhập liệu trên mỗi dòng, đồng thời đồng bộ hóa các chiều cao (.form-input-single, .form-select-single, và .metadata-cb-row đều có chiều cao cố định 38px) tạo nên giao diện thống nhất, cân xứng cực kỳ cao cấp.

