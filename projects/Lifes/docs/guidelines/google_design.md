# Google Design System — Principles for AI Agents

> Một bộ nguyên tắc cô đặc trích xuất từ Google/Material Design, định dạng để AI agent có thể tham chiếu khi tạo UI. Mỗi rule có **DO / DON'T** rõ ràng và **lý do** đằng sau.

---

## 1. COLOR — Bộ màu lõi

### 1.1 Bảng màu primary (BẮT BUỘC dùng những hex này)

```
Google Blue    #4285F4   — Primary action, link, focus state, brand
Google Red     #EA4335   — Destructive, error, alert
Google Yellow  #FBBC04   — Warning, highlight, accent
Google Green   #34A853   — Success, confirm, positive state
```

### 1.2 Màu phụ (chỉ dùng khi cần >4 category)

```
Purple   #9334E6
Teal     #00ACC1
Orange   #F9AB00
Pink     #E91E63
```

### 1.3 Neutrals (Google gray scale)

```
gray900  #202124   — Primary text, dark UI
gray700  #3C4043   — Secondary text
gray500  #5F6368   — Tertiary text, icons
gray300  #DADCE0   — Borders, dividers
gray100  #F1F3F4   — Subtle backgrounds, chips inactive
gray50   #F8F9FA   — Page background
```

### 1.4 Rules

- **DO** dùng shade 500 cho mọi "base color" — đủ contrast với white text và white background
- **DO** dùng opacity 0.2-1.0 để tạo intensity scale (ví dụ heatmap) thay vì tạo shade mới
- **DON'T** dùng pure primary (#FF0000, #0000FF) — gây mỏi mắt, chromatic aberration
- **DON'T** trộn >2 màu brand trong cùng 1 element — chỉ multi-color khi thể hiện multi-category
- **DON'T** dùng gradient ngoại trừ cases rất đặc biệt (Gemini branding) — Material ưu tiên flat fills
- **DON'T** tạo màu mới ngoài palette; nếu cần variation, dùng opacity hoặc Material tonal shades (50/100/.../900)

### 1.5 Functional color mapping

| Intent      | Color            | Use cases                           |
|-------------|------------------|-------------------------------------|
| Primary     | Blue #4285F4     | CTA, selected tab, link, progress   |
| Success     | Green #34A853    | Confirmation, sent, completed       |
| Warning     | Yellow #FBBC04   | Draft, pending, caution             |
| Destructive | Red #EA4335      | Delete, error, "today line"         |
| Neutral     | gray500-900      | 90% of text & containers            |

### 1.6 Container tints

- Selected/active state → background `#E8F0FE` (blue-50), text Google Blue
- Hover state → background `gray100` (#F1F3F4)
- Disabled → opacity 0.38 trên text, opacity 0.12 trên fill

---

## 2. TYPOGRAPHY

### 2.1 Font stack (theo ưu tiên)

```
font-family: "Google Sans", "Product Sans", Roboto, Arial, sans-serif;
```

- **Google Sans** — brand, headings, display, large buttons
- **Roboto** — body, UI labels, data-dense views
- **Roboto Mono** — code, numbers cần align

### 2.2 Scale (Material 3 type scale — cô đặc)

| Role          | Size | Weight | Letter-spacing | Dùng cho                |
|---------------|------|--------|----------------|-------------------------|
| Display large | 57   | 400    | -0.25          | Hero screens (hiếm)     |
| Headline      | 32   | 400    | 0              | Page title              |
| Title large   | 22   | 400    | 0              | Card header, dialog     |
| Title medium  | 16   | 500    | 0.15           | List item, section      |
| Body large    | 16   | 400    | 0.5            | Default body            |
| Body medium   | 14   | 400    | 0.25           | Secondary body          |
| Label large   | 14   | 500    | 0.1            | Button, tab             |
| Label small   | 11   | 500    | 0.5            | Overline, eyebrow       |

### 2.3 Rules

- **DO** dùng weight 400 cho headings (không phải bold) — Google Sans đã đủ presence ở 400
- **DO** dùng weight 500 cho buttons và labels — 500 = medium, tạo hierarchy mà không gắt như 700
- **DO** dùng letter-spacing âm (-0.01em to -0.02em) cho headings lớn
- **DO** uppercase + letter-spacing 0.08em cho section eyebrow/overline
- **DON'T** dùng italic (không có trong Material core)
- **DON'T** dùng >3 sizes trên 1 màn hình — nếu cần thêm, dùng weight để tạo hierarchy
- **DON'T** dùng font-size <11px — không accessible
- **DON'T** dùng 700/800/900 — Google hiếm khi push quá weight 500

---

## 3. SHAPE & ELEVATION

### 3.1 Border radius (Material 3)

```
none        0     — chart bars, fullscreen surfaces
xs          4     — small chips, inline tags
sm          8     — buttons, input fields, small cards
md          12    — cards, dialogs (default)
lg          16    — expanded cards, bottom sheets
xl          28    — FAB extended, hero components
full        999   — pills, chips, circular buttons
```

### 3.2 Elevation (shadow)

Material dùng 5 levels. Trong code dùng 2 chính:

```css
/* Level 1 — resting card */
box-shadow: 0 1px 2px rgba(60,64,67,0.1), 0 1px 3px 1px rgba(60,64,67,0.05);

/* Level 2 — raised button, hover card */
box-shadow: 0 1px 2px rgba(60,64,67,0.3), 0 2px 6px 2px rgba(60,64,67,0.15);

/* Level 3 — menu, picker */
box-shadow: 0 4px 8px 3px rgba(60,64,67,0.15), 0 1px 3px rgba(60,64,67,0.3);

/* Level 4 — modal, navigation drawer */
box-shadow: 0 6px 10px 4px rgba(60,64,67,0.15), 0 2px 3px rgba(60,64,67,0.3);
```

### 3.3 Rules

- **DO** dùng shadow có 2 layers (ambient + key light) — Google shadows luôn compound
- **DO** dùng border-radius 12px cho cards (mặc định Material 3)
- **DO** dùng pills (radius 999) cho chips, filter tabs, small buttons
- **DON'T** dùng border + shadow cùng lúc — chọn một (ưu tiên shadow cho cards, border cho inputs)
- **DON'T** dùng rgba đen thuần cho shadow — Google shadow có tint xám (rgba(60,64,67,...))
- **DON'T** dùng radius >16px cho containers có content — chỉ dùng cho decorative elements

---

## 4. SPACING & LAYOUT

### 4.1 Spacing scale (8-point grid với half-steps)

```
0    0px
0.5  4px     — tight, icon-to-text
1    8px     — base unit
1.5  12px    — inside chips
2    16px    — default card padding
3    24px    — section padding
4    32px    — between major sections
6    48px    — page-level margins
8    64px    — hero spacing
```

### 4.2 Component-specific

- Button padding: `8px 20px` (small), `10px 24px` (default)
- Input padding: `12px 16px`
- Card padding: `16px 20px` (compact), `24px` (default)
- List item min-height: `48px` (touch target)
- Tap target minimum: `48×48px`

### 4.3 Density levels (Material 3 compact-to-comfortable)

| Level    | Delta   | Dùng khi                         |
|----------|---------|----------------------------------|
| Default  | 0       | Consumer apps, casual            |
| Compact  | -4px    | Data-dense (Gmail, Sheets)       |
| Dense    | -8px    | Power users, spreadsheets        |

### 4.4 Rules

- **DO** dùng 8px làm đơn vị cơ bản — tất cả spacing là bội của 4px (half-step)
- **DO** giữ horizontal padding lớn hơn vertical (ví dụ button 8×20, không 8×8)
- **DON'T** dùng odd numbers (7, 13, 17) — stick với 4, 8, 12, 16, 24, 32
- **DON'T** center-align text blocks — Material hầu hết left-align

---

## 5. COMPONENTS — Canonical patterns

### 5.1 Button hierarchy

```
Filled        bg: blue-500, text: white, shadow L1       — Primary CTA (1 per screen)
Tonal         bg: #E8F0FE, text: blue-500, no shadow     — Secondary important
Outlined      border: gray-300, text: gray-900           — Secondary
Text          no bg, text: blue-500                      — Tertiary, in-line
Icon          48×48, circular hover bg gray-100          — Toolbar actions
FAB           circular 56px, blue-500, shadow L2         — Floating primary
```

### 5.2 Chip (filter, selection)

- Height 32px, radius 999, padding `6px 14px`
- Inactive: bg white, border gray-300, text gray-700
- Active: bg `#E8F0FE`, border blue-500, text blue-500, weight 500
- Có leading icon: size 18px, gap 8px

### 5.3 Input field

- Outlined variant: radius 4px, border gray-300 → blue-500 on focus
- Label "floats" trên border khi focus/filled
- Helper text: 12px, gray-500, 4px below input
- Error: border + label + helper text đổi sang red-500

### 5.4 Card

- Background white, radius 12, shadow Level 1
- Hover: shadow Level 2 (nếu clickable)
- Padding: `16px 20px` mặc định
- KHÔNG dùng border nếu đã có shadow

### 5.5 Navigation

- **Top app bar**: 64px tall, white bg, thin border-bottom `1px solid gray-300`
- **Tabs**: 48px tall, active underline 2px blue-500, text weight 500
- **Nav rail (pill style)**: radius 999, active bg `#E8F0FE`, active text blue-500
- **Bottom nav**: 80px tall, icon 24px, label 12px, active state = filled icon + indicator pill

### 5.6 Dialog

- Max-width 560px, radius 28 (Material 3), padding 24
- Title: Title Large, no divider below
- Actions: right-aligned, text buttons, gap 8px
- Scrim: rgba(0,0,0,0.32)

---

## 6. MOTION

### 6.1 Duration tokens

```
short1   50ms    — selection state, icon swap
short2   100ms   — hover, small transitions
short3   150ms   — button press
medium1  200ms   — card hover elevation
medium2  300ms   — dialog enter, sheet expand (default)
long1    450ms   — full-screen transitions
long2    500ms   — choreographed sequence
```

### 6.2 Easing

```
Emphasized       cubic-bezier(0.2, 0.0, 0, 1.0)    — default, on-screen motion
Standard         cubic-bezier(0.2, 0.0, 0, 1.0)    — incoming
Standard accel   cubic-bezier(0.3, 0, 1, 1)         — leaving screen
Standard decel   cubic-bezier(0, 0, 0, 1)           — entering screen
```

### 6.3 Rules

- **DO** dùng `medium2` (300ms) làm duration mặc định
- **DO** animate transform và opacity (GPU-accelerated) — không animate width/height/top/left
- **DON'T** dùng ease-in-out blanket — Material dùng asymmetric easing (decel-in, accel-out)
- **DON'T** animate nhiều thứ cùng lúc với cùng timing — stagger bằng delay 30-50ms

---

## 7. ICONOGRAPHY

- Dùng **Material Symbols** (hoặc Material Icons cũ)
- 3 styles: Outlined (default), Rounded, Sharp — chọn 1 và stick với nó
- Sizes: 20 (small/inline), 24 (default), 40 (large touch), 48 (extra large)
- Stroke weight: 400 (default), 300 (light), 500 (medium)
- **DO** match icon color với text color cùng vùng
- **DON'T** mix icon styles trong cùng app
- **DON'T** dùng filled và outlined cho cùng 1 concept ở các vị trí khác nhau

---

## 8. LAYOUT PATTERNS

### 8.1 Page structure

```
┌────────────────────────────────┐
│  Top app bar (64px)            │  ← gray-300 border-bottom
├────────────────────────────────┤
│  Header / toolbar (16-24px pad)│  ← title + actions
├────────────────────────────────┤
│                                │
│  Content (24px padding)        │  ← bg gray-50, cards inside
│    ┌─ Card ──────────────┐    │
│    │ radius 12, shadow L1│    │
│    └──────────────────────┘    │
│                                │
└────────────────────────────────┘
```

### 8.2 Card grid

- Max-width container: 1440px, 1600px, hoặc fullwidth
- Gap giữa cards: 16px (compact), 24px (default)
- Breakpoints: 600 / 905 / 1240 / 1440 / 1600

### 8.3 Data density

- List item: 48px min-height, 1 line + 2 lines text OK
- Table row: 52px default, 40px compact
- Column gutter: 16px minimum

---

## 9. CONTENT & VOICE

- Sentence case cho mọi UI (button, label, title) — KHÔNG Title Case hay ALL CAPS (trừ overline)
- Buttons: verb phrase, ≤3 words ("Save", "Add topic", "Learn more")
- Empty states: 1 sentence mô tả + 1 primary action
- Error messages: nói rõ cái gì sai + cách fix, không blame user
- Tooltip: <10 words, không dấu chấm cuối
- Truncate với ellipsis khi >1 line, không wrap trong buttons/chips

---

## 10. ACCESSIBILITY

- Contrast AA: text ≥4.5:1, large text ≥3:1, UI components ≥3:1
- Focus ring: 2px blue-500 outline, 2px offset
- Touch target: ≥48×48px (kể cả khi visual nhỏ hơn — expand hit area)
- Motion: respect `prefers-reduced-motion`
- Color không phải cách duy nhất truyền thông tin — luôn pair với icon/text/pattern

---

## 11. QUICK HEURISTICS (cho AI agent)

Khi không chắc, check:

1. **Màu này có trong palette không?** Nếu không, quay về 4 primaries + gray scale
2. **Text size có trong scale không?** Nếu không, chọn gần nhất (11/14/16/22/32)
3. **Spacing có phải bội của 4 không?** Nếu không, round lên
4. **Card có shadow Level 1 và radius 12 chưa?**
5. **Button primary có đúng weight 500 và radius 4-8 chưa?**
6. **Có >1 màu brand trên 1 component không?** Không — chỉ 1
7. **Có hơn 1 filled button trên màn hình không?** Không — 1 primary
8. **Có italic hoặc bold 700+ không?** Không — dùng 400/500
9. **Contrast có pass AA không?** Check với text/bg combo
10. **Có dùng gradient không?** Tránh — flat fills

---

## 12. ANTI-PATTERNS (TUYỆT ĐỐI TRÁNH)

| Anti-pattern                              | Thay bằng                                 |
|-------------------------------------------|-------------------------------------------|
| Gradient backgrounds                      | Flat color hoặc subtle gray-50            |
| Pure neon colors (#FF0000)                | Google palette                             |
| Text over busy images                     | Solid overlay hoặc text bên cạnh          |
| >3 font weights                           | Chỉ 400 và 500                             |
| Uppercase paragraph text                  | Sentence case                              |
| Rounded rectangles with 1px left accent   | Filled card hoặc proper icon              |
| Purple-to-pink gradient buttons           | Solid blue-500 filled button               |
| Drop shadow đen thuần                     | rgba(60,64,67,...) shadow                  |
| Hover effect dùng transform scale >1.05   | Elevation change + slight color shift      |
| Emoji trong production UI                 | Material Symbols icons                     |

---

*Phiên bản này là distillation — không thay thế [m3.material.io](https://m3.material.io) nhưng đủ cho AI agent áp dụng mà không lạc hướng.*
