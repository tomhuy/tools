# User Story: US-13.1 (Draft — Activity Heatmap Migration)

## Story Information
- **ID**: US-13.1
- **Title**: Activity Heatmap Tracker in Electron
- **Priority**: Medium
- **Estimate**: 5 hours
- **Sprint**: Current Sprint

## User Story
- **As a** End User
- **I want to** xem Activity Heatmap Tracker trong giao diện Electron — grid tháng × ngày (1–31) với dot màu đánh dấu ngày có hoạt động theo từng Topic
- **So that** tôi theo dõi được chuỗi liên tục hoặc gián đoạn của các Topic/hành động theo thời gian qua nhiều năm, tương đương `ActivityHeatmapView` bản WPF.

## Scope
- Hiển thị heatmap grid: mỗi **EventGroup** = 1 Topic (parentId == null), mỗi hàng = 1 tháng, mỗi cột = ngày 1–31.
- Dot màu xuất hiện nếu ngày đó nằm trong range của child memento (hoặc chính Topic nếu không có children).
- Màu dot lấy từ field `color` của memento qua mapping danh mục cố định (Work, Personal, Health, v.v.).
- Filter theo Tag (multi-select) + toggle "Bao gồm ghi chú con của Topic được tag".
- Load dữ liệu cho năm 2025 và 2026 (`forkJoin` 2 lần gọi API theo năm).

## Acceptance Criteria

1. **Route / Navigation**: Có thể truy cập qua route `/activity-heatmap`; menu hiển thị mục "Activity Heatmap".
2. **EventGroup header**: Hiển thị tiêu đề `hành động: {topic.title.toLowerCase()}` trên banner xám nhạt.
3. **Grid header**: Hàng đầu hiển thị cột "Month / Date" + các số ngày 1–31.
4. **Month rows**: Mỗi hàng hiển thị nhãn `MM/YYYY` + 31 ô; ô vượt quá số ngày trong tháng ẩn đi (màu `#F9FAFB`).
5. **Activity dot**: Ô ngày có hoạt động hiển thị dot tròn đường kính 10px với màu danh mục; hover tooltip = title Topic.
6. **Tag filter**: Dropdown multi-select; thay đổi selection tự reload dữ liệu qua `effect()`.
7. **Include children toggle**: Checkbox "Bao gồm ghi chú con" ảnh hưởng đến query API; thay đổi tự reload.
8. Tuân thủ `angular_rule.md` (Signals, `inject`, standalone component).

## Technical Design

### Component Split (Smart / Passive)

```
ActivityHeatmapPageComponent   (Smart — biết về service, filter, navigation)
└── HeatmapGridComponent       (Passive — chỉ nhận EventGroup[], render thuần túy)
```

`HeatmapGridComponent` không biết về API hay filter — chỉ nhận `input()` là `EventGroup[]` và render.
Nhờ đó có thể tái sử dụng ở bất kỳ trang nào chỉ bằng cách truyền dữ liệu đã chuẩn hóa.

### Files to Create

| File | Vai trò |
|---|---|
| `features/activity-heatmap/activity-heatmap-page.component.ts/.html/.css` | Smart Page Component |
| `features/activity-heatmap/heatmap-grid/heatmap-grid.component.ts/.html/.css` | Passive Grid Component (reusable) |
| `features/activity-heatmap/activity-heatmap.service.ts` | Service — data loading + build EventGroup[] |
| `features/activity-heatmap/models/event-group.model.ts` | Shared models |

### Files to Modify

| File | Change |
|---|---|
| `app.routes.ts` | Thêm route `/activity-heatmap` |
| `app.component.html` | Thêm nav link "Activity Heatmap" |

### Data Models

```typescript
// models/event-group.model.ts
export interface HeatmapCell {
  isActive: boolean;
  isHidden: boolean;
  bgColor: string;
  tooltip: string;
}

export interface MonthRow {
  monthLabel: string; // "MM/YYYY"
  cells: HeatmapCell[]; // luôn 31 phần tử
}

export interface EventGroup {
  eventTitle: string;
  dayNumbers: number[]; // [1..31]
  months: MonthRow[];
}

export interface SelectableTag {
  id: number;
  name: string;
  color: string;
  isSelected: boolean;
}
```

### Passive Component — `HeatmapGridComponent`

```typescript
@Component({ selector: 'app-heatmap-grid', standalone: true, ... })
export class HeatmapGridComponent {
  readonly groups = input.required<EventGroup[]>();
}
```

- Chỉ render `groups()` — không inject service, không gọi API.
- Template dùng `@for (group of groups(); track group.eventTitle)`.
- CSS chứa toàn bộ style grid: cell 34×34px, dot 10px, month label 110px.
- **Tái sử dụng**: bất kỳ component nào có `EventGroup[]` đều có thể dùng `<app-heatmap-grid [groups]="data" />`.

### Smart Component — `ActivityHeatmapPageComponent`

- inject `ActivityHeatmapService`, đọc signals.
- Quản lý local UI state: `showTagPicker = signal(false)`.
- Template: truyền `[groups]="service.eventGroups()"` vào `<app-heatmap-grid>`.
- Không chứa logic render grid — toàn bộ delegate cho `HeatmapGridComponent`.

### Service Design — `ActivityHeatmapService`

```typescript
@Injectable({ providedIn: 'root' })
export class ActivityHeatmapService {
  private readonly api = inject(CalendarApiService);

  readonly selectedTagIds = signal<number[]>([]);
  readonly includeChildren = signal(true);
  readonly availableTags = signal<SelectableTag[]>([]);
  readonly eventGroups = signal<EventGroup[]>([]);
  readonly isLoading = signal(false);

  // effect() tự reload khi selectedTagIds hoặc includeChildren thay đổi
}
```

- `loadData()`: `forkJoin` 2 lần `getMementos({ year, includeChildren: true, tagIds })` cho year 2025 và 2026.
- Build `EventGroup[]` hoàn toàn trong service (pure TS).
- Color mapping `getColorByCategory(category: string): string` — private helper.

### Color Mapping (tương đương WPF `GetSolidBgColor`)

| Category | Color |
|---|---|
| Work | `#4CAF50` |
| Personal | `#2196F3` |
| Health | `#F44336` |
| Learning | `#9C27B0` |
| Travel | `#FF9800` |
| Event | `#009688` |
| Review | `#FFC107` |
| Planning | `#3F51B5` |
| Conference | `#607D8B` |
| Competition | `#00BCD4` |
| Release | `#E91E63` |
| Psychology | `#4CAF50` |
| _(default)_ | `#9E9E9E` |

## Tasks Breakdown

- [ ] Task 1: Tạo `event-group.model.ts` — 4 interfaces: `HeatmapCell`, `MonthRow`, `EventGroup`, `SelectableTag`.
- [ ] Task 2: Tạo `HeatmapGridComponent` (Passive) — `input.required<EventGroup[]>()`, template `@for`, CSS grid đầy đủ (cell 34×34px, dot 10px, month label 110px, banner).
- [ ] Task 3: Tạo `ActivityHeatmapService` — load 2025+2026 `forkJoin`, build `EventGroup[]`, color mapping, tag filter signals + `effect()`.
- [ ] Task 4: Tạo `ActivityHeatmapPageComponent` (Smart) — inject service, tag dropdown, include children toggle, dùng `<app-heatmap-grid [groups]="service.eventGroups()"/>`.
- [ ] Task 5: Thêm route `/activity-heatmap` + navigation link trong `app.component`.
- [ ] Task 6: Manual test: dot đúng màu, ô ẩn cuối tháng, filter tag tự reload, tooltip.

## Dependencies
- **Depends on**: US-12.1 (`CalendarApiService` đã có `getMementos` với `tagIds`, `includeChildren`).
- **Reuses**: `CalendarApiService`, `TagService`, `Tag` model.

## Out of Scope
- Chọn năm động (hiện tại hardcode 2025-2026).
- Click vào dot mở editor.
- Export heatmap.

## Definition of Done
- [ ] Heatmap grid render đúng theo dữ liệu từ API.
- [ ] Tag filter hoạt động — reload tự động khi thay đổi.
- [ ] Include children toggle ảnh hưởng đúng dữ liệu.
- [ ] Ô vượt ngày trong tháng ẩn đúng.
- [ ] Dot màu đúng theo category.
- [ ] Route và navigation hoạt động.
- [ ] Code tuân thủ `angular_rule.md`.
- [ ] User review & approve.
