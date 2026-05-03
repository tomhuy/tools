# Laputa — Design System Reference

> Tài liệu này mô tả toàn bộ design tokens, typography, spacing, và nguyên tắc
> để AI hoặc developer có thể tái tạo lại style nhất quán.

---

## 1. Fonts

| Role | Family | Weights | Usage |
|------|--------|---------|-------|
| UI / Body | `Geist` (sans-serif) | 300, 400, 500, 600 | Tất cả text giao diện, labels, body |
| Editorial / Display | `Instrument Serif` (serif) | 400, 400 italic | Title note (h1), heading trong preview, app name |
| Monospace | `Geist Mono` | 400, 500 | Tags, timestamps, metadata, code, page numbers |

```html
<link href="https://fonts.googleapis.com/css2?family=Instrument+Serif:ital@0;1&family=Geist:wght@300;400;500;600&family=Geist+Mono:wght@400;500&display=swap" rel="stylesheet">
```

**Nguyên tắc font:**
- Serif chỉ dùng cho tiêu đề chính (note title, app name) — tạo cảm giác editorial
- Mono chỉ dùng cho data nhỏ (tags, count, time) — không bao giờ dùng cho body text
- Sans-serif là default cho mọi thứ còn lại

---

## 2. Type Scale

| Token | Size | Weight | Line-height | Usage |
|-------|------|--------|-------------|-------|
| `--t-app-name` | 16px | 400 | — | App title (Instrument Serif) |
| `--t-note-title` | 32px | 400 | 1.2 | Note editor title (Instrument Serif) |
| `--t-note-title-popup` | 22px | 400 | 1.2 | Title trong compact popup |
| `--t-section-title` | 14px | 500 | — | Panel header, list title |
| `--t-body` | 14px | 400 | 1.75 | Content editor, markdown body |
| `--t-card-title` | 13px | 500–600 | 1.3 | Card title trong list/card view |
| `--t-card-title-grid` | 11px | 600 | 1.35 | Card title trong grid (5-col) |
| `--t-card-preview` | 11–12px | 400 | 1.5 | Preview text trong card |
| `--t-label` | 12px | 400–500 | — | Navigation labels, breadcrumb |
| `--t-meta` | 11px | 400 | — | Metadata, section labels |
| `--t-tag` | 10–11px | 400 | — | Tags (Geist Mono) |
| `--t-micro` | 9–10px | 400 | — | Timestamps, counts (Geist Mono) |
| `--t-section-header` | 10px | 500 | — | UPPERCASE section labels |
| `--t-status` | 11px | 400 | — | Status bar (Geist Mono) |
| `--t-tooltip` | 10px | 400 | — | Tooltip text |
| `--t-ctx-menu` | 12px | 400 | — | Context menu items |

**Letter spacing:**
- Section labels (UPPERCASE): `0.08em`
- Section sub-labels: `0.04em`
- App name: `0.01em`

---

## 3. Color Tokens

### Dark Theme (default)

```css
:root {
  /* Backgrounds — 5 levels từ tối đến sáng hơn */
  --bg:       #141414;   /* App background, editor */
  --bg2:      #1A1A1A;   /* Note list panel */
  --bg3:      #202020;   /* Cards, active items, search bg */
  --bg4:      #272727;   /* Hover states, deeper surfaces */
  --bg-hover: #2A2A2A;   /* Hover highlight */

  /* Borders */
  --border:   rgba(255,255,255,0.06);   /* Subtle dividers */
  --border2:  rgba(255,255,255,0.10);   /* Active/focus borders */

  /* Text — 3 levels */
  --text:     #E8E4DD;   /* Primary — warm white, not pure */
  --text2:    #8A8580;   /* Secondary — muted labels */
  --text3:    #504E4A;   /* Tertiary — disabled, timestamps */

  /* Accents */
  --accent:   #C9B06A;   /* Primary accent — warm gold */
  --accent2:  #7F77DD;   /* Secondary — purple/indigo */
  --red:      #D4537E;   /* Danger, delete, psychology */
  --green:    #1D9E75;   /* Success, resources */
  --blue:     #378ADD;   /* Info, tasks, work */
}
```

### Sepia Theme

```css
html.sepia {
  --bg:       #F0EBE1;
  --bg2:      #EAE3D7;
  --bg3:      #E2D9CB;
  --bg4:      #D8CEBD;
  --bg-hover: #DDD4C4;
  --border:   rgba(100,75,40,0.10);
  --border2:  rgba(100,75,40,0.18);
  --text:     #2C2015;   /* Warm dark brown */
  --text2:    #6B5A44;
  --text3:    #9C876A;
  --accent:   #8B6914;   /* Darker gold for sepia */
  --accent2:  #5B52AA;
  --red:      #AA3056;
  --green:    #1A7A55;
}
```

**Nguyên tắc màu:**
- Không dùng màu thuần (#000 hay #fff) — luôn dùng warm tones
- Background có 5 tầng để tạo depth mà không cần shadow
- Text có 3 tầng: primary (đọc được), secondary (label), tertiary (disabled/hint)
- Accent chỉ dùng cho: active state, CTA button, caret, progress bar
- Màu ngữ cảnh (red/green/blue/purple) encode ý nghĩa category, không dùng trang trí

---

## 4. Section Colors (Category Encoding)

| Section | Color Token | Hex |
|---------|-------------|-----|
| Psychology / Cảm xúc | `--red` | #D4537E |
| Ideas / Ý tưởng | `--accent2` | #7F77DD |
| Resources / Tài nguyên | `--green` | #1D9E75 |
| Journal / Nhật ký | `--accent` | #C9B06A |
| Tasks / Nhiệm vụ | `--blue` | #378ADD |
| Work | `--blue` | #378ADD |
| Learning | `--accent2` | #7F77DD |
| Personal | `--accent` | #C9B06A |
| Default / Unknown | `--text3` | #504E4A |

---

## 5. Spacing System

Laputa dùng **4px base unit**:

| Token | Value | Usage |
|-------|-------|-------|
| `--sp-1` | 4px | Gap nhỏ nhất, icon spacing |
| `--sp-2` | 8px | Gap between elements |
| `--sp-3` | 12px | Padding card nhỏ |
| `--sp-4` | 16px | Padding standard (sidebar, list) |
| `--sp-5` | 20px | Padding editor topbar |
| `--sp-6` | 24px | Section separator |
| `--sp-8` | 32px | Editor scroll top padding |
| `--sp-12` | 48px | Editor horizontal padding (desktop) |

### Panel heights

| Element | Height |
|---------|--------|
| Topbar / Toolbar | 40px |
| Markdown toolbar | 34px |
| Status bar | 24px |
| Nav item | ~32px (6px top+bottom padding) |
| Compact note card | 7px+7px padding |
| Standard note card | 12px+12px padding |
| Grid note card | 160px fixed |

### Panel widths

| Panel | Default | Min | Notes |
|-------|---------|-----|-------|
| Sidebar | 210px | — | Fixed |
| Note list (default) | 290px | 220px | Resizable |
| Note list (card view) | 320px | 220px | Resizable |
| Detail popup | 340px | — | Slides in from right |
| Editor | flex: 1 | — | Takes remaining space |

---

## 6. Border Radius

| Usage | Radius |
|-------|--------|
| Cards (note card, modal) | 10px |
| Buttons, inputs | 8px |
| Small buttons, tags | 6px |
| Inline tags / chips | 20px (pill) |
| Tiny badges | 4px |
| Accent bars | 3px |
| Progress, status dot | 50% (circle) |

---

## 7. Border System

```css
/* Luôn dùng rgba thay vì màu đặc — subtle không bị nặng */
border: 1px solid var(--border);    /* Default divider */
border: 1px solid var(--border2);   /* Focus / hover state */
border: 1px solid var(--accent);    /* Active / selected */
```

**Quy tắc:**
- Không bao giờ dùng `2px` border trừ active indicator (left strip)
- Left strip active: `width: 2px, height: 14px` (nav item) hoặc `left: 0, top: 0, bottom: 0, width: 2px` (note card)
- Card accent bar trên cùng: `height: 2–3px`, border-radius `8px 8px 0 0`

---

## 8. Shadow & Elevation

```css
/* Context menu, popups */
box-shadow: 0 8px 32px rgba(0,0,0,.4);

/* Card hover lift */
transform: translateY(-1px);  /* Không dùng shadow cho card hover */

/* Detail popup */
/* Không cần shadow — dùng border + background difference */
```

**Nguyên tắc:** Laputa dùng **background layers thay vì shadow** để tạo depth. Shadow chỉ dùng cho floating elements (context menu, overlay popup).

---

## 9. Layout Structure

```
┌─────────────────────────────────────────────────────┐
│ Sidebar (210px)  │ Note List (290px) │ Editor (flex)│
│                  │                   │              │
│ .sidebar         │ .note-list        │ .editor-wrap │
│                  │                   │              │
│ ├ App name       │ ├ List header     │ ├ Topbar 40h │
│ ├ Search         │ │  (title + btns) │ ├ MD toolbar │
│ ├ Nav items      │ ├ Note cards      │ ├ Editor body│
│ └ Theme dots     │ └ (scrollable)    │ └ Status 24h │
└─────────────────────────────────────────────────────┘
```

**3-panel rule:**
- Panel trái (sidebar): navigation only, không có content
- Panel giữa (list): scan & select, không edit
- Panel phải (editor): full focus, tối giản chrome

---

## 10. Interactive States

| State | Visual |
|-------|--------|
| Default | `--bg2` / `--bg3` background |
| Hover | `--bg3` / `--bg4` + `transition: background 0.1–0.15s` |
| Active/Selected | `--bg3`/`--bg4` + accent left strip (2px) hoặc accent border |
| Focus (input) | `border-color: --border2` |
| Disabled | `opacity: 0.3–0.4` |
| Drag hover | `border-color: --border2`, accent handle visible |

**Transition speeds:**
```css
transition: background .1s;          /* Hover states nhanh */
transition: all .15s;                /* Buttons, interactive */
transition: transform .12s;          /* Card hover lift */
transition: width .3s ease;          /* Panel slide */
transition: transform .2s ease;      /* Popup slide-in */
```

---

## 11. Component Patterns

### Navigation Item
```
Height: ~32px
Padding: 6px 14px
Icon: 14×14px, color: --text3 (default), --accent (active)
Label: 12px Geist 400, --text2 (default), --text (active)
Count: 11px Geist Mono, --text3
Active indicator: left strip 2px × 14px, --accent, border-radius 0 2px 2px 0
Active bg: --bg3
```

### Note Card (List mode)
```
Padding: 12px 16px
Border-bottom: 1px solid --border
Title: 13px Geist 500, --text
Preview: 11px Geist 400, --text3, line-clamp: 2
Tags: 10px Geist Mono, background --bg4, color --text3, padding 1px 7px, radius 20px
Time: 10px Geist Mono, --text3
Active: background --bg3, left strip 2px solid --accent
```

### Note Card (Card mode)
```
Padding: 14px
Border: 1px solid --border, radius 10px
Background: --bg3
Accent bar top: 2px height, radius 8px 8px 0 0, color = section color
Title: 13px Geist 600
Preview: 12px (replaced by bullet list)
Bullet: dot 4×4px --text3 + 11px --text2
Hover: border --border2, bg --bg4, translateY(-1px)
Active: border --accent
```

### Note Card (Grid mode — 5 columns)
```
Height: 160px fixed (grid-auto-rows)
Padding: 12px 12px 10px
Border: 1px solid --border, radius 10px
Background: --bg3
Accent bar: 3px height, radius 3px
Title: 11px Geist 600, line-clamp: 2
Preview: 10px Geist 400, --text2, line-clamp: 4
Tags: 9px, padding 1px 5px
Time: 9px Geist Mono
Grid gap: 8px
```

### Button sizes
```
Icon button square: 26–28px × 26–28px, radius 6–8px
Text button: padding 4–7px 10–14px, radius 6–8px
Primary (accent): background --accent, color #141414
Danger: color --red, hover background rgba(red, 0.1)
Pill button: radius 20px
```

### Input / Textarea
```
Padding: 6–8px 10–12px
Background: --bg3 (search) / --bg2 (modal)
Border: 1px solid --border
Border-radius: 8px
Font: Geist 12–13px, --text
Placeholder: --text3
Focus: border-color --border2
Outline: none (custom focus style)
```

---

## 12. Editor Content Rules

```
Title: 32px Instrument Serif 400, --text, line-height 1.2
Body: 14px Geist 400, --text2, line-height 1.75
H1 (preview): 26px Instrument Serif, --text
H2 (preview): 20px Instrument Serif, --text
H3 (preview): 16px Instrument Serif, --text
Heading margin: 1.2em top, 0.5em bottom
Paragraph spacing: 0.9em bottom
Code inline: 12px Geist Mono, --accent, bg --bg3, padding 1px 5px, radius 4px
Blockquote: border-left 2px --accent, padding-left 14px, color --text3, italic
Hr: border-top 1px --border, margin 1.5em 0
Link: color --accent, no underline, underline on hover
List indent: padding-left 20px
Editor scroll padding: 48px top, 20% horizontal (centers content), 80px bottom
Caret color: --accent
```

---

## 13. Markdown Toolbar Pattern

```
Height: 34px
Background: --bg2
Border-bottom: 1px solid --border
Padding: 0 20px, gap 2px
Button: height 24px, padding 0 7px, radius 5px
Button text/icon: 12px Geist, color --text3
Button hover: bg --bg4, color --text2
Button active: bg --bg4, color --text
Separator: 1px × 14px, --border2, margin 0 3px
```

---

## 14. Do's & Don'ts

**✓ Do:**
- Dùng warm tones thay vì pure black/white
- Layer backgrounds để tạo depth (bg > bg2 > bg3 > bg4)
- Encode category bằng màu accent bar, không phải text
- Dùng Instrument Serif cho display text, Geist Mono cho data
- Transition nhanh (0.1–0.15s) cho hover, chậm hơn (0.2–0.3s) cho panel animation
- Dùng `rgba` cho border thay vì màu đặc

**✗ Don't:**
- Không dùng `#000000` hay `#FFFFFF` thuần
- Không thêm shadow trừ floating elements
- Không dùng font weight > 600
- Không dùng font-size > 32px trong UI
- Không mix serif và sans trong cùng một paragraph
- Không dùng `border-radius > 10px` cho card (trừ pill = 20px)
- Không dùng màu sắc trang trí — màu phải encode ý nghĩa

---

## 15. Quick Reference Card

```
FONTS:    Geist (UI) | Instrument Serif (title) | Geist Mono (data)
BASE:     13px body | 4px spacing unit | 10px min text
PANELS:   Sidebar 210px | List 290px | Editor flex:1
HEIGHTS:  Topbar 40px | MD-toolbar 34px | Status 24px | Grid card 160px
RADIUS:   Card 10px | Button 8px | Chip 6px | Tag 20px | Bar 3px
DARK:     bg #141414 | text #E8E4DD | accent #C9B06A
SEPIA:    bg #F0EBE1 | text #2C2015 | accent #8B6914
MOTION:   Hover 0.1–0.15s | Panel 0.2–0.3s ease
BORDERS:  Default rgba(255,255,255,0.06) | Active rgba(255,255,255,0.10)
ACCENT:   Active strip 2px | Accent bar 2–3px top | Caret --accent
```
