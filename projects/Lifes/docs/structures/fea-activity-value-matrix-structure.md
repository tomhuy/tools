# Feature: Activity Value Matrix (US-21.1)

## Overview

Ma trận 2×2 tương tác phân tích 4 hoạt động cốt lõi theo 2 chiều:
- **Trục Y (Giá trị dài hạn)**: Cao / Thấp
- **Trục X (Thời gian đầu tư)**: Ít / Nhiều

4 hoạt động được biểu diễn bằng bubble tương tác. Kích thước bubble phản ánh compound effect theo mốc thời gian (1 / 3 / 5 / 10 năm). Click bubble hiển thị detail card với metrics và insight.

**Pure frontend prototype** — không cần backend API. Data tĩnh được định nghĩa trong service.

---

## Architecture

### Presentation Layer (Electron/Angular)

```
src/Lifes.Presentation.Electron/src/app/features/activity-value-matrix/
├── models/
│   └── activity-value-matrix.model.ts       — Interfaces: ActivityItem, ActivityMetrics, ActivityBar, Quadrant, YearKey
├── bubble-area/
│   ├── bubble-area.component.ts             — Presenter: render bubbles trong 1 quadrant
│   ├── bubble-area.component.html
│   └── bubble-area.component.css
├── activity-value-matrix-page/
│   ├── activity-value-matrix-page.component.ts   — Smart page: orchestrate toàn bộ UI
│   ├── activity-value-matrix-page.component.html
│   └── activity-value-matrix-page.component.css
└── activity-value-matrix.service.ts         — Reactive state (Signals): currentYear, selectedId, activities data
```

### Route

| Path | Component |
|------|-----------|
| `/activity-value-matrix` | `ActivityValueMatrixPageComponent` |

Đăng ký trong `src/app/app.routes.ts`.

---

## Key Classes

### ActivityValueMatrixService
**Location**: `features/activity-value-matrix/activity-value-matrix.service.ts`  
**Purpose**: Quản lý toàn bộ reactive state bằng Angular Signals. Chứa static data `ACTIVITY_DATA` cho 4 hoạt động. Export `YEAR_INDEX` map.  
**Signals**:
- `activities` — danh sách 4 `ActivityItem`
- `currentYear` — mốc năm đang chọn (`1 | 3 | 5 | 10`), default `3`
- `selectedId` — id của activity đang được click, default `null`
- `selectedActivity` (computed) — derive từ `selectedId`

**Methods**: `setYear(year)`, `selectActivity(id)`, `getBubbleSize(activity)`

---

### ActivityValueMatrixPageComponent
**Location**: `features/activity-value-matrix/activity-value-matrix-page/`  
**Purpose**: Smart container — inject service, tính `activitiesByQuad` (computed), `detailMetrics` (computed), truyền data xuống `BubbleAreaComponent`.  
**Dependencies**: `ActivityValueMatrixService`, `BubbleAreaComponent`  
**Template structure**:
1. Year selector buttons (1 / 3 / 5 / 10 năm)
2. Quad grid 2×2 — mỗi ô chứa `<app-bubble-area>`
3. Axis labels (Y: Giá trị dài hạn, X: Thời gian đầu tư)
4. Detail card (hiển thị khi `selectedActivity` khác null)
5. Hint text (hiển thị khi chưa chọn)

---

### BubbleAreaComponent
**Location**: `features/activity-value-matrix/bubble-area/`  
**Purpose**: Passive presenter — render bubbles từ `activities` input. Bubble size tính qua service `getBubbleSize()`. Bubble absolute-positioned trong container.  
**Inputs**: `activities: ActivityItem[]`, `selectedId: string | null`  
**Outputs**: `bubbleClick: string` (emit activity id)

---

### ActivityItem (interface)
**Location**: `features/activity-value-matrix/models/activity-value-matrix.model.ts`

| Field | Type | Mô tả |
|-------|------|-------|
| `id` | `string` | Unique key |
| `name` | `string` | Tên hiển thị |
| `icon` | `string` | Emoji icon |
| `color` | `string` | Hex màu chính |
| `colorLight` | `string` | rgba nền bubble |
| `quad` | `Quadrant` | Vị trí trong ma trận (`tl/tr/bl/br`) |
| `pos` | `{x, y}` | Tọa độ % trong quadrant |
| `metrics` | `ActivityMetrics` | compound/clarity/network × 4 năm |
| `bars` | `ActivityBar[]` | 3 progress bar labels + colors |
| `insights` | `Record<YearKey, string>` | Đoạn text insight theo từng năm |

---

## Data Flow

```
ActivityValueMatrixService (Signals)
  ├── currentYear (signal)  ←── setYear() ←── year buttons
  ├── selectedId  (signal)  ←── selectActivity() ←── bubbleClick output
  │
  └── selectedActivity (computed) ──► detail card
      activitiesByQuad (computed in page) ──► BubbleAreaComponent × 4
      getBubbleSize() ──► bubble width/height
```

---

## Bubble Size Formula

```
size = base + compound[yearIndex] × 10
base = 54  (nếu id === 'build')
base = 52  (các activity còn lại)
yearIndex: { 1→0, 3→1, 5→2, 10→3 }
```

---

## Activities Data

| ID | Tên | Quadrant | Compound (1/3/5/10y) |
|----|-----|----------|----------------------|
| `note` | Ghi chép 📝 | `tl` — Cao·Ít | 1.4 / 2.8 / 5.2 / 18 |
| `learn` | Học kiến thức mới 📚 | `tr` — Cao·Nhiều | 1.3 / 2.2 / 4.1 / 12 |
| `apply` | Áp dụng kiến thức ⚡ | `tr` — Cao·Nhiều | 1.8 / 3.5 / 7.2 / 25 |
| `build` | Build app 🛠 | `br` — Thấp·Nhiều | 1.1 / 1.4 / 1.8 / 2.5 |

---

## Key Decisions

1. **Pure frontend, no API**: Data tĩnh trong service — không cần backend. Pattern tương tự `DailyTimeline` và `YearlyStream`.
2. **Angular Signals toàn bộ**: Không dùng RxJS Subject hay `ngModel` cho state — tuân thủ `angular_rule.md`.
3. **Container/Presenter split**: `PageComponent` (smart) + `BubbleAreaComponent` (dumb) — tuân thủ `design_rule.md`.
4. **CSS native, không chart lib**: Grid 2×2 + absolute positioning — đủ cho visualization này, không cần D3 hay Chart.js.
