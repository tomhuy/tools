# User Story: US-21.1

## Story Information

| Field | Value |
|-------|-------|
| **ID** | US-21.1 |
| **Title** | Activity Value Matrix — Màn hình phân tích giá trị dài hạn |
| **Priority** | Medium |
| **Estimate** | 3 hours |
| **Sprint** | — |

---

## User Story

- **As a** người dùng muốn định hướng thời gian đầu tư cá nhân
- **I want to** xem ma trận 2×2 phân tích 4 hoạt động (Ghi chép, Học kiến thức, Áp dụng, Build app) theo trục Giá trị dài hạn × Thời gian đầu tư, với bubble tương tác và detail card
- **So that** tôi hiểu rõ hơn compound effect của từng loại hoạt động theo các mốc thời gian (1 / 3 / 5 / 10 năm) để ra quyết định phân bổ thời gian tốt hơn

---

## Acceptance Criteria

1. **Given** người dùng mở màn hình Activity Value Matrix
   **When** trang load
   **Then** hiển thị lưới 2×2 với 4 quadrant được phân biệt bằng màu nền nhẹ và label mô tả vị trí (Cao giá trị · Ít thời gian, v.v.)

2. **Given** trang đã load
   **When** render lần đầu
   **Then** 4 bubble (📝 Ghi chép, 📚 Học kiến thức mới, ⚡ Áp dụng kiến thức, 🛠 Build app) xuất hiện đúng quadrant, kích thước phản ánh compound metric ở mốc 3 năm (mặc định)

3. **Given** người dùng click một trong các nút năm (1 / 3 / 5 / 10)
   **When** năm thay đổi
   **Then** kích thước bubble cập nhật smooth để phản ánh compound metric tương ứng, nút đang chọn được highlight

4. **Given** người dùng click vào một bubble
   **When** bubble được chọn
   **Then** detail card hiển thị bên dưới ma trận với: icon + tên + mô tả, 3 metric number (Compound ×, Clarity %, Kết nối ý tưởng %), 3 progress bar, và đoạn insight tương ứng với năm đang chọn

5. **Given** một bubble đã được chọn
   **When** người dùng đổi mốc năm
   **Then** detail card cập nhật ngay với số liệu và insight của năm mới, không cần click lại bubble

6. **Given** màn hình
   **When** chưa có bubble nào được click
   **Then** hiển thị hint text "Click vào bong bóng để xem phân tích chi tiết"

7. **Given** màn hình
   **When** render
   **Then** trục Y có label "Giá trị dài hạn" (rotate −90°), trục X có label "Thời gian đầu tư" — sử dụng CSS thuần, không cần thư viện chart

8. **Given** route `/activity-value-matrix` được truy cập
   **When** Angular router resolve
   **Then** component render đúng, không lỗi console

---

## Technical Design

### Architecture Decision

Màn hình này là **pure-frontend UI prototype** — không cần backend API, không cần Angular service inject HTTP. Toàn bộ data được định nghĩa tĩnh trong service (signal-based state). Pattern tương tự `DailyTimelinePageComponent` và `YearlyStreamPageComponent`.

### Electron/Angular Layers

| Layer | Scope |
|-------|-------|
| **Presentation (Angular)** | `ActivityValueMatrixPageComponent` (smart page), `BubbleAreaComponent` (passive presenter cho từng quadrant) |
| **Service** | `ActivityValueMatrixService` (Angular Signals: selectedId, currentYear, activities data) |
| **Model** | `activity-value-matrix.model.ts` (ActivityItem, ActivityMetrics interfaces) |
| **Route** | `activity-value-matrix` → lazy load hoặc eager (match pattern hiện tại) |
| **Backend** | Không cần |

### Clean Architecture Mapping

```
Lifes.Presentation.Electron/src/app/features/activity-value-matrix/
├── activity-value-matrix-page/
│   ├── activity-value-matrix-page.component.ts
│   ├── activity-value-matrix-page.component.html
│   └── activity-value-matrix-page.component.css
├── bubble-area/
│   ├── bubble-area.component.ts
│   ├── bubble-area.component.html
│   └── bubble-area.component.css
├── models/
│   └── activity-value-matrix.model.ts
└── activity-value-matrix.service.ts
```

### Angular Signals Pattern (bắt buộc theo `angular_rule.md`)

```typescript
// activity-value-matrix.service.ts
export class ActivityValueMatrixService {
  readonly currentYear = signal<1 | 3 | 5 | 10>(3)
  readonly selectedId = signal<string | null>(null)
  readonly activities = signal<ActivityItem[]>(ACTIVITY_DATA)

  readonly selectedActivity = computed(() =>
    this.activities().find(a => a.id === this.selectedId()) ?? null
  )

  setYear(y: 1 | 3 | 5 | 10) { this.currentYear.set(y) }
  selectActivity(id: string) { this.selectedId.set(id) }
}
```

### Data Model

```typescript
// activity-value-matrix.model.ts
export interface ActivityMetrics {
  compound: [number, number, number, number]  // 1y, 3y, 5y, 10y
  clarity:  [number, number, number, number]
  network:  [number, number, number, number]
}

export type Quadrant = 'tl' | 'tr' | 'bl' | 'br'

export interface ActivityItem {
  id:          string
  name:        string
  icon:        string
  color:       string
  colorLight:  string
  timeInvest:  number
  desc:        string
  quad:        Quadrant
  pos:         { x: number; y: number }
  metrics:     ActivityMetrics
  bars:        Array<{ label: string; color: string }>
  insights:    Record<1 | 3 | 5 | 10, string>
}
```

### CSS Strategy

- Dùng CSS Grid cho quad layout (2×2), giống pattern trong `activity_value_matrix.html`
- Bubble dùng `position: absolute` trong từng quadrant container
- Không dùng thư viện chart hay D3 — pure CSS/HTML
- Dùng CSS custom properties của Design System hiện tại (`--color-text-primary`, `--color-background-secondary`, v.v.)
- Transition trên bubble size: `transition: width .3s ease, height .3s ease`

### Routing

Thêm vào `app.routes.ts`:
```typescript
{ path: 'activity-value-matrix', component: ActivityValueMatrixPageComponent }
```

---

## Files to Create/Modify

### Tạo mới

- [x] `src/Lifes.Presentation.Electron/src/app/features/activity-value-matrix/models/activity-value-matrix.model.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/activity-value-matrix/activity-value-matrix.service.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/activity-value-matrix/activity-value-matrix-page/activity-value-matrix-page.component.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/activity-value-matrix/activity-value-matrix-page/activity-value-matrix-page.component.html`
- [x] `src/Lifes.Presentation.Electron/src/app/features/activity-value-matrix/activity-value-matrix-page/activity-value-matrix-page.component.css`
- [x] `src/Lifes.Presentation.Electron/src/app/features/activity-value-matrix/bubble-area/bubble-area.component.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/activity-value-matrix/bubble-area/bubble-area.component.html`
- [x] `src/Lifes.Presentation.Electron/src/app/features/activity-value-matrix/bubble-area/bubble-area.component.css`

### Chỉnh sửa

- [x] `src/Lifes.Presentation.Electron/src/app/app.routes.ts` — thêm route `activity-value-matrix`
- [x] `src/Lifes.Presentation.Electron/src/app/app.component.html` — thêm nav button 🎯 Activity Matrix

---

## Tasks Breakdown

- [x] Task 1: Tạo model `activity-value-matrix.model.ts` — định nghĩa interfaces `ActivityItem`, `ActivityMetrics`, `Quadrant`
- [x] Task 2: Tạo `activity-value-matrix.service.ts` — Signals: `currentYear`, `selectedId`, `selectedActivity` (computed), `setYear()`, `selectActivity()`; inline static data `ACTIVITY_DATA` (4 activities từ HTML gốc)
- [x] Task 3: Tạo `BubbleAreaComponent` — `activities` input, `selectedId` input, `bubbleClick` output; tính bubble size từ service; render bubbles bằng `@for`
- [x] Task 4: Tạo `ActivityValueMatrixPageComponent` — inject service, render 4 quadrant qua `<app-bubble-area>`, year selector buttons, detail card section, hint text
- [x] Task 5: Style CSS — quad grid 2×2 (420px height), axis labels, bubble absolute positioning, detail card, progress bars; dùng CSS vars Design System
- [x] Task 6: Đăng ký route `activity-value-matrix` trong `app.routes.ts`
- [x] Task 7: Thêm navigation button 🎯 Activity Matrix vào `app.component.html`

---

## Dependencies

- Depends on: Không có dependency với US khác
- Blocked by: Không

---

## Definition of Done

- [x] Component render đúng tại route `/activity-value-matrix`
- [x] 4 bubble hiển thị đúng quadrant, kích thước phản ánh năm đang chọn
- [x] Click bubble → detail card hiển thị với số liệu và insight đúng
- [x] Đổi năm → bubble size và detail card cập nhật ngay
- [x] Dùng Angular Signals (không dùng `ngModel` hay RxJS Subject cho state)
- [x] Dùng `inject()` thay vì constructor injection
- [x] Không có `any` type rõ ràng, không có lỗi TypeScript
- [x] CSS dùng custom properties Design System, visual match với `activity_value_matrix.html` gốc
- [x] Không có console error khi load trang

## Final Status

- **Status**: ✅ Completed
- **Completed Date**: 2026-05-13
- **Approved By**: huy

---

## Reference

**HTML Prototype**: `docs/user-stories/us-21.1/activity_value_matrix.html`

Data gốc (4 activities):

| ID | Name | Quad | timeInvest |
|----|------|------|-----------|
| `note` | Ghi chép 📝 | tl (Cao giá trị · Ít thời gian) | 20% |
| `learn` | Học kiến thức mới 📚 | tr (Cao giá trị · Nhiều thời gian) | 35% |
| `apply` | Áp dụng kiến thức ⚡ | tr (Cao giá trị · Nhiều thời gian) | 40% |
| `build` | Build app 🛠 | br (Thấp giá trị · Nhiều thời gian) | 65% |

Bubble size formula: `base + compound[yearIdx] * 10` (base = 54 cho `build`, 52 cho còn lại)
