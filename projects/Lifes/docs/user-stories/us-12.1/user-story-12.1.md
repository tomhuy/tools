# User Story: US-12.1

## Story Information
- **ID**: US-12.1
- **Title**: Migrate Monthly Calendar Core View (Memento Rendering) to Electron
- **Priority**: High
- **Estimate**: 12 hours
- **Sprint**: Next Sprint

## User Story
- **As a** End User / Developer
- **I want to** xem Monthly Calendar trong Electron với việc hiển thị Topic mementos và child mementos dạng Gantt/Dot theo tháng được chọn
- **So that** em có view đọc (read-only) hoàn chỉnh của calendar trên nền Angular, làm nền cho các tính năng chỉnh sửa ở các US tiếp theo (Tag, Topic, Memento Management).

## Scope (Core Only)
US này **chỉ làm phần core rendering**:
- Hiển thị danh sách tháng chọn được.
- Render Topic mementos (rows) + child mementos / phases (bars) lên Gantt.
- 3 display modes (Gantt / Dot / Pure Dot).
- Today indicator.
- State management reactive bằng Signals, chuẩn bị sẵn shape để các US sau inject hành động (Add/Edit/Delete) mà KHÔNG phải refactor service.

**KHÔNG nằm trong US này** (chuyển sang US-12.2/12.3/12.4):
- Tag Management UI (CRUD tags, color palette) → **US-12.3**.
- Topic Editor (Add/Update/Delete Topic) → **US-12.2**.
- Memento Management (central ordering + filter) → **US-12.4**.

## Acceptance Criteria

1. **Route & Navigation**: Menu trong Electron shell dẫn tới route `/monthly-calendar`, render `MonthlyCalendarPageComponent` (standalone). Page này mount `MonthlyGridComponent` bên trong.

2. **Month Selection**: Component hiển thị danh sách 12 tháng. User có thể chọn/bỏ chọn nhiều tháng. Các tháng được chọn hiển thị đồng thời, row đồng bộ giữa các tháng.

3. **Topic Row Rendering**: Với mỗi tháng đã chọn, render mỗi Topic memento (`parentId == null`) thành một row. Row sắp xếp theo field `Order` (ưu tiên) rồi đến `StartDate`.

4. **Child/Phase Bar Rendering**: Mỗi child memento (`parentId != null`) render thành phase bar hoặc dot trong row của topic cha, đúng vị trí cột theo ngày (CSS Grid 31 cột).

5. **Display Modes**: User có thể switch giữa 3 chế độ:
   - **Gantt**: thanh đặc (solid bar).
   - **Dot**: chấm có viền mờ.
   - **Pure Dot**: chỉ có chấm.
   Switch chỉ đổi class binding, KHÔNG fetch lại data.

6. **Today Indicator**: Cột ngày hiện tại có vạch dọc màu `#FF4D4D` xuyên suốt từ header đến các row (tương đương US-9.9 bản WPF).

7. **Reactive Re-render (Design Contract)**: State được quản lý bằng Signals với immutable updates + `@for ... track id`:
   - Khi một child memento được thêm/xóa/update thông qua service API, **chỉ row của topic cha re-render** (không re-render toàn bộ Gantt).
   - Khi topic memento đổi `title` hoặc `color`, **chỉ binding của đúng row đó update**; DOM của các row khác giữ nguyên.
   - US-12.1 phải tạo sẵn các service method stubs (`addChild`, `updateMemento`, `deleteMemento`) để US sau gọi — không cần UI trigger trong US này, chỉ cần verify behavior bằng test hoặc dev console.

8. **Data Source**: Fetch từ WebApi nếu có endpoint sẵn; nếu chưa, dùng fake data trong service (pattern US-11.2) và ghi TODO rõ ràng. **KHÔNG block UI vì backend.**

9. **Angular Rules Compliance** ([docs/guidelines/angular_rule.md](docs/guidelines/angular_rule.md)):
   - Standalone components, `inject()`, Signals.
   - kebab-case file naming.
   - One-attribute-per-line trong template.
   - Không dùng `any`.

## Technical Design

### Component Architecture (Container / Presentational Split)

```
MonthlyCalendarPageComponent   ← smart, route entry, orchestrator
├── (toolbar: month selector + display mode toggle — inline or sub-component)
└── MonthlyGridComponent       ← pure presentational, rendering only
```

#### `MonthlyCalendarPageComponent` — "Page" (smart container)
**Location**: `features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component.ts`

**Responsibilities:**
- Entry point của route `/monthly-calendar`.
- Inject `MonthlyCalendarService`, đọc signals (`mementos`, `tags`, `displayMode`, `selectedMonths`).
- Render toolbar: chọn tháng, switch display mode, nút "Manage Tags" / "Add Topic" (trong các US sau).
- **Truyền data xuống** `MonthlyGridComponent` qua signal inputs.
- **Lắng nghe outputs** từ grid (row click, phase click) để điều hướng hoặc mở modal (trong US-12.2/12.3).
- KHÔNG chứa logic render Gantt — mọi render uỷ quyền cho grid.

#### `MonthlyGridComponent` — "Grid" (pure presentational)
**Location**: `features/monthly-calendar/monthly-grid/monthly-grid.component.ts`

**Responsibilities:**
- Chỉ render Gantt: topic rows + child/phase bars + today indicator + display mode styles.
- Nhận data **qua signal inputs** — KHÔNG inject service (giữ pure).
- Phát sự kiện user tương tác qua `output()` — không tự xử lý state.

**Input contract:**
```ts
readonly topics           = input.required<Memento[]>();
readonly childrenByParent = input.required<Map<number, Memento[]>>();
readonly selectedMonths   = input.required<number[]>();
readonly displayMode      = input.required<DisplayMode>();
readonly today            = input.required<Date>();
```

**Output contract** (chuẩn bị cho US-12.2):
```ts
readonly topicClick = output<Memento>();
readonly phaseClick = output<Memento>();
readonly cellClick  = output<{ topic: Memento; date: Date }>();
```

**Vì sao split thế này:**
- **Testable**: Grid nhận pure inputs → dễ viết test/Storybook không cần mock service.
- **Reusable**: Nếu sau này cần embed Monthly Grid vào dashboard/report khác, chỉ cần truyền data.
- **Re-render scope rõ ràng**: Page chỉ dirty khi signal service đổi; Grid chỉ dirty khi input signal đổi — cùng hệ Angular Signals nên tự động fine-grained (xem contract bên dưới).
- **Single source of truth**: State vẫn chỉ ở service. Grid không tự cache.

### State Management Contract (Core Decision)

Service giữ state dạng **flat signal** với **immutable updates** — đây là fondation cho 12.2/12.3/12.4:

```ts
// monthly-calendar.service.ts
readonly mementos = signal<Memento[]>([]);
readonly tags     = signal<Tag[]>([]);
readonly displayMode = signal<DisplayMode>('gantt');
readonly selectedMonths = signal<number[]>([...]);

readonly topicRows = computed(() =>
  this.mementos().filter(m => m.parentId === null).sort(byOrder)
);

readonly childrenByParent = computed(() => {
  const map = new Map<number, Memento[]>();
  for (const m of this.mementos()) {
    if (m.parentId !== null) {
      map.set(m.parentId, [...(map.get(m.parentId) ?? []), m]);
    }
  }
  return map;
});

// Mutation methods — immutable, tạo object/array mới
addChild(child: Memento)    { this.mementos.update(l => [...l, child]); }
updateMemento(m: Memento)   { this.mementos.update(l => l.map(x => x.id === m.id ? m : x)); }
deleteMemento(id: number)   { this.mementos.update(l => l.filter(x => x.id !== id)); }
```

### Re-render Behavior (answers to design question)

Data flow: `Service signal` → `Page reads signal` → `Page truyền vào Grid input()` → `Grid template bind`. Angular Signals lan từ service đến input của grid tự động; `@for track id` trong grid cắt scope xuống từng row.

| Action | Component re-render scope |
|--------|---------------------------|
| Insert/update/delete **child memento** | `childrenByParent` computed recomputes → Page đọc signal → truyền vào `MonthlyGrid.childrenByParent` input → grid template `@for (child of childrenByParent().get(topic.id); track child.id)` → Angular chỉ patch DOM của đúng row topic cha. Các row topic khác không dirty. **Page component KHÔNG re-render toolbar** vì toolbar không đọc signal đó. |
| Update **topic title** | `{{ topic.title }}` binding trong đúng row grid re-evaluate. Row khác không đụng. Page không bị ảnh hưởng. |
| Update **topic color** | `[style.background]="topic.color"` binding của đúng row update. |
| Add **topic mới** | `topicRows()` trả list mới; grid `@for (topic of topics(); track topic.id)` → Angular insert duy nhất 1 `<div>` mới, các row cũ giữ DOM. |
| Switch **display mode** | `displayMode` signal đổi → Page truyền input mới xuống grid → grid `[class.mode-gantt]` / `[class.mode-dot]` đổi. KHÔNG re-render row nào, chỉ đổi class trên container. |
| Toggle **month selection** | `selectedMonths` input đổi → grid chỉ re-evaluate phần render columns; row structure giữ nguyên. |

**Điều kiện**: tất cả mutation trong service PHẢI tạo object/array mới (`{...topic, title: newTitle}`). Mutation in-place sẽ phá `track id` và phá input signal equality check của grid.

### Files to Create

**Page component (smart / container):**
- [x] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component.html`
- [x] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component.css`

**Grid component (pure presentational):**
- [x] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-grid/monthly-grid.component.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-grid/monthly-grid.component.html`
- [x] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-grid/monthly-grid.component.css`

**Shared (service + constants + models):**
- [x] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-calendar.service.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-calendar.constants.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/models/memento.model.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/models/tag.model.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/models/display-mode.model.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/models/selectable-month.model.ts`

### Files to Modify

- [x] `src/Lifes.Presentation.Electron/src/app/app.routes.ts` — thêm route `/monthly-calendar`.
- [x] `src/Lifes.Presentation.Electron/src/app/app.component.html` — thêm menu item.

## Tasks Breakdown

- [x] Task 1: Định nghĩa TypeScript interfaces: `Memento`, `Tag`, `DisplayMode`, `SelectableMonth`.
- [x] Task 2: Tạo `MonthlyCalendarService` với signal state + computed derivations (`topicRows`, `childrenByParent`) + mutation stubs (immutable).
- [x] Task 3: Seed fake data trong service khớp cấu trúc `MementoModel` bản WPF (để parity test).
- [x] Task 4: Tạo `MonthlyGridComponent` (pure presentational) với `input.required<>()` + `output<>()` signature.
- [x] Task 5: Implement render Gantt trong grid: topic rows + child phases với `@for track id`, CSS Grid 31 cột.
- [x] Task 6: Implement 3 display modes trong grid bằng class binding trên container (không đụng row).
- [x] Task 7: Implement Today Indicator trong grid (CSS `::before` + `[class.is-today]`).
- [x] Task 8: Tạo `MonthlyCalendarPageComponent` (smart) — inject service, render toolbar (month selector + display mode toggle), truyền data vào grid.
- [x] Task 9: Wire outputs của grid lên page (tạm chỉ log hoặc stub — hành vi thực ở US-12.2).
- [x] Task 10: Routing `/monthly-calendar` → page + menu navigation.
- [x] Task 11: Dev-console verify: gọi `service.addChild/updateMemento/deleteMemento` và quan sát chỉ row liên quan re-render (qua Angular DevTools / inspection).
- [x] Task 12: Build Electron thành công, không lỗi TS.

## Dependencies
- **Depends on**: US-11.1, US-11.2.
- **Blocks**: US-12.2 (Topic Editor), US-12.3 (Tag Management), US-12.4 (Memento Management) — các US này gọi vào service đã dựng ở US-12.1.

## Out of Scope
- Tag Management UI → US-12.3.
- Topic Editor UI → US-12.2.
- Memento Management UI → US-12.4.
- Annual Calendar migration → US riêng.
- Activity Heatmap migration → US riêng.
- Xoá WPF implementation cũ → US cleanup sau khi toàn bộ 12.x duyệt.

## Definition of Done
- [x] Monthly Calendar accessible qua menu Electron.
- [x] Render đúng topic rows + child phases cho các tháng được chọn.
- [x] 3 display modes + Today indicator hoạt động.
- [x] Service stubs (`addChild/updateMemento/deleteMemento`) đã có và verify được re-render scope đúng contract.
- [x] Code tuân thủ `angular_rule.md`.
- [x] Build Electron không lỗi.
- [x] User review & approve.

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-24
- **Approved By**: bmhuy
