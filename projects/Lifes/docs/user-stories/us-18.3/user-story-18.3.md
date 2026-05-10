# User Story: US-18.3

## Story Information

| Field | Value |
|-------|-------|
| **ID** | US-18.3 |
| **Title** | Mood Tracker — Pluggable View System & Content Filter |
| **Priority** | Medium |
| **Estimate** | 5 story points |
| **Sprint** | TBD |

## User Story

- **As a** người dùng Mood Tracker
- **I want to** chọn loại nội dung muốn hiển thị trong lưới (mood / hoạt động / lý do / cả hai) và thấy text không bị tràn ô
- **So that** tôi có thể tập trung vào chiều dữ liệu mình cần phân tích mà không bị nhiễu thông tin

---

## Acceptance Criteria

### AC-1: Extract Grid thành Component độc lập
- Given: Logic render matrix hiện đang nằm trực tiếp trong `RangeTrackerPageComponent`
- When: Refactor hoàn thành
- Then:
  - Toàn bộ markup và logic render lưới được tách ra thành `MoodMatrixGridComponent`
  - `RangeTrackerPageComponent` chỉ còn là container, dùng `<mood-matrix-grid>` như một black box
  - Behavior hiện tại không thay đổi: click cell mở editor, màu mood đúng, text entry đúng

### AC-2: Content Filter Bar tại Page Level
- Given: Người dùng đang ở trang Range Tracker
- When: Nhìn vào control bar
- Then:
  - Có segmented control với 4 option: **Cả hai** | **Mood** | **Hoạt động** | **Lý do**
  - Lựa chọn được giữ nguyên khi navigate đi rồi quay lại (in-memory service state)
  - Content filter được truyền xuống grid component qua `@Input()`, grid không tự quản lý filter state

### AC-3: Grid Render theo Content Filter
- Given: Người dùng chọn một content filter
- When: Grid render các cell
- Then:
  - **Cả hai**: Hiện mood label + text hoạt động
  - **Mood**: Chỉ hiện mood label (A, B+, B...), ẩn text
  - **Hoạt động**: Chỉ hiện text hoạt động, ẩn mood label
  - **Lý do**: Hiện trường `reason` thay vì `note`
  - Cell không có entry vẫn render bình thường (trống)

### AC-4: Text Truncation
- Given: Một entry có text dài hơn chiều cao/rộng của ô
- When: Grid render cell đó
- Then:
  - Text bị cắt với ellipsis (`...`) và không vượt ra ngoài boundary của ô
  - Không có scrollbar xuất hiện trong cell
  - Khi hover vào cell, có thể thấy đầy đủ nội dung qua tooltip hoặc mở editor

---

## Technical Design

### Architecture Decision: Content Filter tại Page Level

Content filter là **display preference** dùng chung cho mọi view hiện tại và tương lai. Grid component nhận filter qua `@Input()` và render theo — nó không sở hữu state này.

```
RangeTrackerPageComponent          ← owns: viewMode, contentFilter, currentDate, rangeDays
│
├── MoodControlBarComponent         ← NEW: date nav + content filter selector (dumb, emits events)
│     @Output() contentFilterChange
│     @Output() dateChange / rangeChange
│
└── MoodMatrixGridComponent         ← EXTRACTED + enhanced (dumb, renders only)
      @Input() entries: MoodEntry[]
      @Input() contentFilter: ContentFilter
      @Input() dayHeaders: DayHeader[]
```

### Types mới

```typescript
type ContentFilter = 'both' | 'mood' | 'activity' | 'reason';
```

### Angular Signals thêm vào WeeklyTrackerService

```typescript
contentFilter = signal<ContentFilter>('both');
```

### CSS cho Text Truncation (trong Grid Cell)

```css
.cell-text {
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;       /* số dòng tối đa */
  -webkit-box-orient: vertical;
  text-overflow: ellipsis;
}
```

### Clean Architecture Layers

| Layer | Thay đổi |
|-------|----------|
| **Presentation** | Extract `MoodMatrixGridComponent`, tạo `MoodControlBarComponent` |
| **Service (Weekly Tracker)** | Thêm `contentFilter` signal |
| **Domain / Infrastructure / API** | Không thay đổi |

### Files to Create / Modify

#### Extract & Refactor
- [ ] `weekly-tracker/mood-matrix-grid/mood-matrix-grid.component.ts`
- [ ] `weekly-tracker/mood-matrix-grid/mood-matrix-grid.component.html`
- [ ] `weekly-tracker/mood-matrix-grid/mood-matrix-grid.component.css`

#### New Components
- [ ] `weekly-tracker/mood-control-bar/mood-control-bar.component.ts`
- [ ] `weekly-tracker/mood-control-bar/mood-control-bar.component.html`
- [ ] `weekly-tracker/mood-control-bar/mood-control-bar.component.css`

#### Modify
- [ ] `weekly-tracker/weekly-tracker.service.ts` ← thêm `contentFilter` signal
- [ ] `weekly-tracker/range-tracker-page/range-tracker-page.component.ts` ← wire up
- [ ] `weekly-tracker/range-tracker-page/range-tracker-page.component.html` ← dùng component mới
- [ ] `weekly-tracker/range-tracker-page/range-tracker-page.component.css` ← cleanup

---

## Tasks Breakdown

### Phase 1: Extract Grid Component (Refactor, không thay đổi behavior)
- [x] Task 1.1: Tạo `MoodMatrixGridComponent` — inject service read-only, emit `cellClick` output
- [x] Task 1.2: Move markup và render logic từ `RangeTrackerPageComponent` sang
- [x] Task 1.3: Page lắng nghe `(cellClick)="onGridCellClick($event)"`, mở editor

### Phase 2: Content Filter
- [ ] Task 2.1: Thêm `ContentFilter` type và `contentFilter` signal vào service
- [ ] Task 2.2: Tạo `MoodControlBarComponent` — segmented control (Cả hai / Mood / Hoạt động / Lý do)
- [ ] Task 2.3: Wire `contentFilter` từ page → grid qua `@Input()`
- [ ] Task 2.4: Grid render đúng theo từng filter value

### Phase 3: Text Truncation
- [ ] Task 3.1: Apply CSS truncation vào cell text trong `MoodMatrixGridComponent`
- [ ] Task 3.2: Đảm bảo không vỡ layout ở các độ rộng cột khác nhau (7 / 14 / 21 ngày)
- [ ] Task 3.3: (Optional) Tooltip on hover để xem full text

---

## Dependencies

- **Depends on**: US-18.1 (Matrix Grid UI), US-18.2 (Backend API)
- **Blocked by**: Không có

---

## ADR Notes

**Quyết định kiến trúc quan trọng #1**: Content filter bar nằm ở **Range Tracker Page**, không nằm trong Grid Component.

**Lý do**: Content filter là display preference có ý nghĩa với mọi view (hiện tại và tương lai). Đặt tại page level giúp:
1. Tuân thủ Container/Presenter pattern (ADR 1)
2. Tránh duplicate state khi thêm view mới
3. Grid component luôn "dumb" — chỉ render theo Input

**Quyết định kiến trúc quan trọng #2**: `contentFilter` signal đặt trong **WeeklyTrackerService**, không phải local state của page.

**Lý do**: `contentFilter` về bản chất là UI state (display preference, không ảnh hưởng API call), lẽ ra nên là local component state. Tuy nhiên, yêu cầu **persist qua navigation** (navigate đi rồi quay lại vẫn giữ filter) buộc phải đưa lên service để tồn tại ngoài lifecycle của component.

**Đây là exception có chủ ý** — không phải mọi UI state đều nên vào service. Chỉ khi cần persistence mà không muốn dùng localStorage.

---

## Definition of Done

- [ ] `MoodMatrixGridComponent` hoạt động đúng như cũ sau extract
- [ ] Content filter bar hiển thị, switch đúng giữa 4 mode
- [ ] Grid render đúng nội dung theo từng filter
- [ ] Text không tràn ô, có ellipsis
- [ ] `contentFilter` state được giữ khi navigate
- [ ] Tuân thủ `fe_design_rule.md` cho segmented control UI
- [ ] `all-ards.md` cập nhật quyết định về vị trí content filter bar

---

## Implementation Progress

### Current Status
- **Status**: 🔄 In Progress
- **Completed**: Phase 1 done (33%)

### Files Created / Modified
- [x] `weekly-tracker/mood-matrix-grid/mood-matrix-grid.component.ts` ← NEW
- [x] `weekly-tracker/mood-matrix-grid/mood-matrix-grid.component.html` ← NEW
- [x] `weekly-tracker/mood-matrix-grid/mood-matrix-grid.component.css` ← NEW
- [x] `weekly-tracker/range-tracker-page/range-tracker-page.component.ts` ← simplified
- [x] `weekly-tracker/range-tracker-page/range-tracker-page.component.html` ← grid replaced by `<app-mood-matrix-grid>`
- [x] `weekly-tracker/range-tracker-page/range-tracker-page.component.css` ← grid CSS removed
- [x] `weekly-tracker/services/mood-api.service.ts` ← date bug fix (`toEntry()`)
