# User Story: US-17.1

## Story Information
- **ID**: US-17.1
- **Title**: Yearly Stream View (UI Prototype)
- **Priority**: High
- **Estimate**: 10h
- **Sprint**: Phase 15

## User Story
- **As a** User
- **I want to** have a yearly overview of my activities in a matrix format (Months x Days)
- **So that** I can see long-term patterns, moods, and ideas at a glance.

## Acceptance Criteria
1. [x] Display a grid with 12 months as columns and 31 days as rows.
2. [x] Each month header should show the month name and the count of entries for that month.
3. [x] Each cell should display a vertical color bar, a day label (e.g., T5, CN), and status dots if data exists.
4. [x] Include a navigation header to switch years and jump to "Today".
5. [x] Include filters for "All", "Has Mood", and "Has Ideas".
6. [x] Highlight the current day in the grid.
7. [x] Premium dark theme matching the provided prototype image.
8. [x] **Book Reader Popup**: Integrated modal to read books/notes for a specific day.
9. [x] **Mailbox Style Post Reader**: Two-column layout (List/Detail) for reading multiple articles/posts per day.
10. [x] **Future Muting**: Disable colors and mute UI for dates in the future.
11. [x] **4K Optimization**: Responsive layout that fits 31 days on high-resolution screens.

## Technical Design

### Clean Architecture Layers
- **Presentation**: `YearlyStreamPageComponent` (Angular 19).
- **Domain**: `StreamEntry`, `StreamBook`, `StreamPost` models.
- **Infrastructure**: `YearlyStreamService` with reactive state (Signals).

### Files to be Created
- `src/app/models/yearly-stream.model.ts`
- `src/app/features/yearly-stream/yearly-stream.service.ts`
- `src/app/features/yearly-stream/yearly-stream-page/yearly-stream-page.component.ts`
- `src/app/features/yearly-stream/yearly-stream-page/yearly-stream-page.component.html`
- `src/app/features/yearly-stream/yearly-stream-page/yearly-stream-page.component.css`

## Tasks Breakdown
- [x] Setup models and service
- [x] Implement Yearly Stream Page UI (Matrix 12x31)
- [x] Implement Filter and Navigation logic
- [x] Integrate into application shell
- [x] Implement Book Reader modal
- [x] Implement Mailbox Style Post Reader (List-Detail view)
- [x] Implement Future Muting logic (Date comparison)
- [x] Final UI Polish & 4K Optimization

## Definition of Done
- [x] UI matches the prototype image pixel-perfect.
- [x] Navigation and filters work with mock data.
- [x] Code follows project standards (Clean Architecture).
- [x] Documentation updated (updoc).

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-29
- **Approved By**: User
