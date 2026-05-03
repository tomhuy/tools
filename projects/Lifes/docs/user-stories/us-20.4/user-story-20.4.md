# User Story: US-20.4

## Story Information
- **ID**: US-20.4
- **Title**: Laputa Notes — Grid Mode Redesign (Colors & Aesthetics)
- **Priority**: High
- **Estimate**: 4 hours
- **Sprint**: TBD

## User Story
- **As a** user
- **I want to** have a more premium and consistent grid view in Laputa Notes
- **So that** it follows the established Laputa Design System and feels high-end.

## Acceptance Criteria
1. **Color Palette**: All colors in Grid mode must use tokens from `laputa-design-system.md` (bg, bg2, bg3, bg4, text, text2, text3, accent).
2. **Typography**: Titles in Grid mode use `--t-card-title-grid` (11px Geist 600), preview text uses `--t-card-preview` (10px Geist 400).
3. **Card Styling**:
    - Border radius: 10px.
    - Border: 1px solid `var(--border)`.
    - Background: `var(--bg3)`.
    - Accent Bar: 3px height at the top, radius 3px, color matching the section.
4. **Interactive States**:
    - Hover: background `var(--bg4)`, border `var(--border2)`, `translateY(-1px)`, transition 0.12s.
    - Active: border `var(--accent)`.
5. **Spacing**: Grid gap 8px, card padding 12px 12px 10px.

## Technical Design

### Presentation Layer
- **Component**: `LaputaNoteListComponent`
- **File**: `laputa-note-list.component.css`

## Tasks Breakdown
- [x] Task 1: Update Grid card background and borders according to Design System.
- [x] Task 2: Implement Top Accent Bar with section-specific colors.
- [x] Task 3: Refine typography for title, preview text, tags, and timestamp in Grid mode.
- [x] Task 4: Enhance hover and active states (translateY, transition, border focus).
- [x] Task 5: Verify Sepia theme consistency for Grid mode.

## Definition of Done
- [x] UI matches the specification in `laputa-design-system.md`.
- [x] Code follows Clean Architecture (CSS scoped correctly).
- [x] Hover and Active states are smooth and professional.
- [x] Sepia theme works correctly for all grid elements.
- [x] Documentation updated.

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-05-03
- **Approved By**: User
