# User Story: US-18.4

## Story Information

| Field | Value |
|-------|-------|
| **ID** | US-18.4 |
| **Title** | Mood Tracker — Intensity Blocks View & Settings Panel |
| **Priority** | Medium |
| **Estimate** | 5 story points |
| **Sprint** | TBD |

## User Story

- **As a** người dùng Mood Tracker
- **I want to** switch sang chế độ "Intensity Blocks" và tùy chỉnh bảng màu, pattern aids, density
- **So that** tôi có thể phân tích pattern tâm trạng theo chiều trực quan (màu sắc + density) thay vì đọc text

---

## Acceptance Criteria

### AC-1: View Selector Bar (View Mode Switch)
- Given: Người dùng ở trang Range Tracker
- When: Nhìn vào header
- Then:
  - Có view selector để switch giữa **Cards** (view hiện tại) và **Intensity Blocks** (view mới)
  - Switch không reset filter state (displayMode, filterMode, rangeDays)
  - View mode persist qua navigation (service state)

### AC-2: Intensity Blocks — Cell Design
- Given: Người dùng chọn "Intensity Blocks" view
- When: Grid render
- Then:
  - Mỗi cell hiển thị màu theo palette đang chọn, intensity tương ứng mood weight
  - Cell design khác với Cards: font, màu, layout khác biệt rõ ràng
  - Column header được redesign phù hợp với visual language mới
  - Cell không có entry vẫn render (trống, màu nền khác biệt)

### AC-3: Settings Panel
- Given: Người dùng đang ở Intensity Blocks view
- When: Click vào nút settings (hoặc gear icon) ở header
- Then:
  - Một panel trượt ra hoặc dropdown hiển thị với tiêu đề "Range Tracker"
  - Panel có 4 section: VIEW MODE, COLOR PALETTE, PATTERN AIDS, DENSITY

### AC-4: Color Palette Selector
- Given: Người dùng mở Settings Panel
- When: Chọn palette từ dropdown
- Then:
  - Grid cập nhật màu sắc ngay lập tức theo palette mới
  - Palette options tối thiểu: `Default`, `Sky — Ghibli landscape`
  - Palette được persist qua navigation

### AC-5: Pattern Aids Toggles
- Given: Người dùng mở Settings Panel, section PATTERN AIDS
- When: Bật/tắt từng toggle
- Then:
  - **Hourly avg ribbon** (ON): Hiển thị ribbon thể hiện mood trung bình theo giờ trên toàn range
  - **Day mini-summary** (ON): Hiển thị mini summary ở column header của mỗi ngày
  - **Alignment guides on hover** (ON): Khi hover vào một cell, hiển thị guide ngang/dọc để dễ đọc tọa độ
  - **Highlight recurring slump** (ON): Tô đậm các khung giờ mà mood thường xuyên thấp (pattern lặp lại)
  - Tất cả toggles mặc định ON

### AC-6: Density Toggle
- Given: Người dùng mở Settings Panel, section DENSITY
- When: Bật **Compact rows**
- Then:
  - Chiều cao mỗi row giảm xuống (compact hơn), hiển thị nhiều giờ hơn trên màn hình
  - Tắt compact rows → trở về row height mặc định

---

## Technical Design

### Architecture

```
RangeTrackerPageComponent (Container)
│
├── ViewSelectorBarComponent              ← NEW (US-18.4)
│
├── MoodMatrixGridComponent               ← đã có (US-18.3)
│     @if (trackerService.viewMode() === 'cards')
│
├── IntensityBlocksGridComponent          ← NEW (US-18.4)
│     @if (trackerService.viewMode() === 'intensity')
│     Inject MoodTrackerService (read-only)
│     Emit cellClick output
│
└── RangeTrackerSettingsPanelComponent    ← NEW (US-18.4)
      @if (isSettingsOpen())
      Floating panel (absolute positioned)
```

### State thêm vào WeeklyTrackerService

```typescript
viewMode    = signal<'cards' | 'intensity'>('cards');
palette     = signal<string>('default');
patternAids = signal<PatternAidSettings>({
  hourlyAvgRibbon:       true,
  dayMiniSummary:        true,
  alignmentGuidesOnHover: true,
  highlightRecurringSlump: true,
});
compactRows = signal<boolean>(false);
```

**Lý do trong service**: Tất cả là display preferences cần persist qua navigation (exception có chủ ý — ADR 22).

### New Types

```typescript
type ViewMode = 'cards' | 'intensity';

interface PatternAidSettings {
  hourlyAvgRibbon: boolean;
  dayMiniSummary: boolean;
  alignmentGuidesOnHover: boolean;
  highlightRecurringSlump: boolean;
}

interface ColorPalette {
  id: string;
  label: string;
  fg: string[]; // Saturated — left border + mood letter. Index 0 = weight 1 (D), index 7 = weight 8 (A)
  bg: string[]; // Translucent — cell fill background. Same order.
}
```

### Color Palette System

Thay vì dùng màu cố định của MOODS, Intensity Blocks dùng palette system:

```typescript
const PALETTES: ColorPalette[] = [
  { id: 'default', label: 'Default', colors: [...] },
  { id: 'sky-ghibli', label: 'Sky — Ghibli landscape', colors: [...] },
];
```

Màu trong cell được lấy từ: `fg[weight-1]` cho border + mood letter, `bg[weight-1]` cho fill background (weight 1-8 → index 0-7).

### Clean Architecture Layers

| Layer | Thay đổi |
|-------|----------|
| **Presentation** | Tạo `IntensityBlocksGridComponent`, `ViewSelectorBarComponent`, `RangeTrackerSettingsPanelComponent` |
| **Service** | Thêm `viewMode`, `palette`, `patternAids`, `compactRows` signals |
| **Domain / Infrastructure / API** | Không thay đổi |

### Files to Create / Modify

#### New Components
- [ ] `weekly-tracker/view-selector-bar/view-selector-bar.component.{ts,html,css}`
- [ ] `weekly-tracker/intensity-blocks-grid/intensity-blocks-grid.component.{ts,html,css}`
- [ ] `weekly-tracker/range-tracker-settings-panel/range-tracker-settings-panel.component.{ts,html,css}`

#### Modify
- [ ] `weekly-tracker/weekly-tracker.service.ts` ← thêm viewMode, palette, patternAids, compactRows signals
- [ ] `weekly-tracker/range-tracker-page/range-tracker-page.component.html` ← add view selector, conditional grid, settings panel trigger
- [ ] `weekly-tracker/range-tracker-page/range-tracker-page.component.ts` ← settings panel open/close state
- [ ] `models/weekly-tracker.model.ts` ← thêm ViewMode, PatternAidSettings, ColorPalette types

---

## Tasks Breakdown

### Phase 1: Service State + View Selector
- [x] Task 1.1: Thêm types mới vào `weekly-tracker.model.ts` (ViewMode, PatternAidSettings, ColorPalette)
- [x] Task 1.2: Thêm signals vào `WeeklyTrackerService` (viewMode, palette, patternAids, compactRows)
- [x] Task 1.3: Tạo `ViewSelectorBarComponent` — switch Cards / Intensity Blocks
- [x] Task 1.4: Wire vào page — conditional render `MoodMatrixGridComponent` vs `IntensityBlocksGridComponent`

### Phase 2: Intensity Blocks Grid
- [x] Task 2.1: Tạo `IntensityBlocksGridComponent` — cell design mới (color intensity theo mood weight)
- [x] Task 2.2: Implement palette system + color mapping
- [x] Task 2.3: Column header redesign cho Intensity view
- [x] Task 2.4: Compact rows toggle — CSS variable cho row height

### Phase 3: Settings Panel
- [x] Task 3.1: Tạo `RangeTrackerSettingsPanelComponent` — layout 4 sections
- [x] Task 3.2: Implement Color Palette dropdown
- [x] Task 3.3: Implement Pattern Aids toggles (4 toggles)
- [x] Task 3.4: Implement Compact Rows toggle
- [x] Task 3.5: Wire panel open/close vào page header

### Phase 4: Pattern Aids (Visual Enhancements)
- [x] Task 4.1: Hourly avg ribbon — tính avg mood weight theo giờ, render ribbon overlay
- [x] Task 4.2: Day mini-summary — hiển thị summary dots/bar ở column header
- [x] Task 4.3: Alignment guides on hover — CSS/JS guide lines khi hover cell
- [x] Task 4.4: Highlight recurring slump — detect giờ có mood thấp lặp lại ≥ 3 ngày, tô nền

---

## Dependencies

- **Depends on**: US-18.1, US-18.2, US-18.3 (Phase 1 — MoodMatrixGridComponent extracted)
- **Blocked by**: Không có

---

## Definition of Done

- [x] View selector switch đúng giữa Cards và Intensity Blocks
- [x] Intensity Blocks cells render màu theo palette + mood weight
- [x] Settings panel mở/đóng đúng, tất cả controls hoạt động
- [x] Color palette switch → grid update ngay lập tức
- [x] Pattern aids bật/tắt đúng behavior
- [x] Compact rows toggle thay đổi row height
- [x] Tất cả settings persist qua navigation
- [x] Cards view không bị regression
- [x] Tuân thủ `fe_design_rule.md`

---

## Implementation Progress

### Current Status
- **Status**: ✅ Complete
- **Completed**: 100%

### Files Created / Modified
- [x] `weekly-tracker/view-selector-bar/view-selector-bar.component.{ts,html,css}` ← NEW
- [x] `weekly-tracker/intensity-blocks-grid/intensity-blocks-grid.component.{ts,html,css}` ← NEW
- [x] `weekly-tracker/range-tracker-settings-panel/range-tracker-settings-panel.component.{ts,html,css}` ← NEW
- [x] `weekly-tracker/weekly-tracker.service.ts` ← added viewMode, palette, patternAids, compactRows signals
- [x] `weekly-tracker/range-tracker-page/range-tracker-page.component.html` ← view selector, conditional grid, settings panel
- [x] `weekly-tracker/range-tracker-page/range-tracker-page.component.ts` ← toggleSettings(), isSettingsOpen signal
- [x] `models/weekly-tracker.model.ts` ← ViewMode, PatternAidSettings, ColorPalette (fg/bg), PALETTES

## Additional Implementation Notes

> **Cell Design (beyond original spec)**: Cell là flex row: 3px left border (FG saturated) + mood letter (FG) + action text truncated. FG/BG tách riêng trong ColorPalette cho phép border/label dùng màu bão hòa trong khi nền dùng màu mờ, tạo độ tương phản đọc được trên dark background.
>
> **Row Height System**: Default mode = `flex: 1` — 24 rows chia đều toàn bộ chiều cao grid (không scroll, Gestalt whole-field view). Compact = 28px fixed (scroll khi cần). Floor 22px đảm bảo rows không collapse trên màn hình nhỏ.
>
> **Cell Interaction**: `cellClick` output thêm vào `IntensityBlocksGridComponent`, wired to page → mở `MoodEntryEditorComponent`. Future cells không emit. Hover highlight: `brightness(1.2)` + background tint trên non-future cells.
