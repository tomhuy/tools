# Feature Structure: Mood & Activity Tracker (US-18.1, US-18.2, US-18.3)

## Overview

Tính năng Mood Tracker cho phép người dùng ghi chép chi tiết trạng thái cảm xúc (Mood) và các hoạt động thực hiện (Actions) theo từng khung giờ. Dữ liệu được đồng bộ hóa với Backend .NET thông qua REST API, đảm bảo tính bền vững và khả năng truy xuất đa thiết bị.

## User Stories
- ✅ **US-18.1**: Triển khai UI Mood Tracker (Matrix Grid), tích hợp trình chỉnh sửa entry dạng modal glassmorphism.
- ✅ **US-18.2**: Tích hợp Backend API, chuyển đổi từ mock data sang persistence storage (JSON Repository).
- ✅ **US-18.3**: Pluggable View System & Content Filter — extract grid thành component, content filter bar, text truncation.
- ✅ **US-18.4**: Intensity Blocks View & Settings Panel — new heatmap view, palette system, pattern aids, settings panel.

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

#### 8. MoodEntryEditorComponent
- **Path**: `src/app/features/weekly-tracker/entry-editor/`
- **Role**: Modal nhập liệu tâm trạng và hoạt động.

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

## Data Models

### MoodEntry
- `id: string` (Guid)
- `date: DateTime` (ISO 8601)
- `moodId: string` (A, B+, B, C, D...)
- `tags: string[]`
- `note: string`
- `reason: string`

### ColorPalette
- `id: string`
- `label: string`
- `fg: string[]` — 8 màu saturated (index 0 = weight 1 = D/worst, index 7 = weight 8 = A/best). Dùng cho border trái và mood letter.
- `bg: string[]` — 8 màu translucent tương ứng. Dùng cho fill background của cell.
- **Palettes có sẵn**: `default` (green→red), `sky-ghibli` (Ghibli-inspired blue→red với oklch BG colors).

## Technical Decisions (ADR)
- **JSON Storage**: Sử dụng file JSON để lưu trữ đơn giản, dễ backup và không cần database server phức tạp.
- **Clean Architecture**: Tách biệt hoàn toàn Core Models và Infrastructure để dễ dàng thay đổi database engine (SQL/NoSQL) trong tương lai.
- **ISO Dates**: Toàn bộ dữ liệu ngày tháng được chuẩn hóa sang ISO string trước khi gửi lên API để tránh lỗi múi giờ.
