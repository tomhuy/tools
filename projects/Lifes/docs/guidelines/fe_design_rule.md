# Front-end Design Rules (Pixel-Perfect Style)

This document defines the unified design language for the Lifes application's Electron frontend, based on the **Topic Editor** and **Tag Management** components.

## 1. Core Dimensions & Geometry

| Property | Value | Use case |
|----------|-------|----------|
| Modal Max Width | 380px | Compact dialogs, forms |
| Corner Radius (L) | 10px | Modal containers, large cards |
| Corner Radius (M) | 6px | Buttons, input fields |
| Corner Radius (S) | 4px | Tag badges, chips |
| Elevation | `0 8px 32px rgba(0, 0, 0, 0.12)` | Active modals |

## 2. Typography

**Font Stack**: `-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif`

| Role | Size | Weight | Color | Use case |
|------|------|--------|-------|----------|
| Heading 2 | 1.15rem | 700 | `#2c3e50` | Modal headers |
| Heading 3 | 0.9rem | 700 | `#2c3e50` | Section titles |
| Label | 0.7rem | 600 | `#95a5a6` | Input labels, eyebrows |
| Input Text | 0.8rem | 400 | `#343a40` | Typed content |
| Button Text | 0.75rem | 600 | Variable | Action labels |
| Badge Text | 0.7rem | 500 | `#495057` | Tag names, chips |

## 3. Colors

### 3.1 Primary Palette
- **Primary Text**: `#2c3e50` (Slate Navy)
- **Secondary Text**: `#95a5a6` (Cool Gray)
- **Accent Blue**: `#3498db` (Sky Blue)
- **Border/Divider**: `#e9ecef` (Soft Gray)
- **Background (Page)**: `#f8f9fa`

### 3.2 Action Colors
- **Success**: `#2ecc71`
- **Danger**: `#e74c3c`
- **Warning**: `#f1c40f`

## 4. Components

### 4.1 Input Fields
- **Height**: 28px
- **Padding**: `5px 10px`
- **Border**: `1px solid #e9ecef`
- **Focus**: `border-color: #3498db; box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.05)`

### 4.2 Buttons
- **Height**: 28px
- **Min-width**: 80px
- **Primary (Filled)**:
    - Default: Background `#f8f9fa`, Text `#495057`
    - Hover/Active: Background `#3498db`, Text `#ffffff`
- **Secondary (Outlined)**:
    - Default: Border `1px solid #dee2e6`, Text `#6c757d`

### 4.3 Tag Badges / Chips
- **Padding**: `3px 10px`
- **Gap**: 6px
- **Indicator Dot**: 6px diameter, circle
- **Hover**: Background `#e7f1ff`, Border `#3498db`

### 4.4 Color Palette Grid
- **Swatch Size**: 18px diameter circle
- **Grid Layout**: 9 columns
- **Gap**: 8px
- **Active State**: 2px solid border (usually blue)

## 5. Spacing
- **Base Grid**: 4px
- **Modal Header Padding**: `16px 20px 8px`
- **Modal Body Padding**: `0 20px 16px`
- **Section Gap**: 12px to 16px
