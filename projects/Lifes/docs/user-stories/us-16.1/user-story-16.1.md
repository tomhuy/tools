# User Story: US-16.1

## Story Information
- **ID**: US-16.1
- **Title**: Daily Timeline Feature (UI Prototype)
- **Priority**: High
- **Estimate**: 8h
- **Sprint**: Phase 14

## User Story
- **As a** User
- **I want to** have a daily timeline view to track my activities and energy levels hour by hour
- **So that** I can analyze my daily productivity and well-being patterns.

## Acceptance Criteria
1. [x] Given the application is running, when I navigate to "Daily Timeline", then I should see a 24-hour grid.
2. [x] Given the daily view, when I look at the summary cards, then I should see "Đã ghi", "Cao nhất", and "Thấp nhất" metrics.
3. [x] Given the timeline grid, when I click on a slot, then a modal editor should open.
4. [x] Given the editor modal, I can select an energy grade (A to D).
5. [x] Given the editor modal, I can select activity tags from a predefined list.
6. [x] Given the editor modal, I can enter notes.
7. [x] Given the UI, there should be smooth animations (fade-in and slide-up).

## Technical Design

### Clean Architecture Layers
- **Presentation**: `DailyTimelinePageComponent`, `EntryEditorComponent` (Angular 19 + CSS Animations).
- **Domain**: `DailyEntry` model with `EnergyGrade` and `ActivityTag`.
- **Infrastructure**: `DailyTimelineService` managing in-memory state using Signals.

### Files Created
- [x] `src/Lifes.Presentation.Electron/src/app/models/daily-timeline.model.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/daily-timeline/daily-timeline.service.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/daily-timeline/daily-timeline-page/daily-timeline-page.component.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/daily-timeline/daily-timeline-page/daily-timeline-page.component.html`
- [x] `src/Lifes.Presentation.Electron/src/app/features/daily-timeline/daily-timeline-page/daily-timeline-page.component.css`
- [x] `src/Lifes.Presentation.Electron/src/app/features/daily-timeline/entry-editor/entry-editor.component.ts`
- [x] `src/Lifes.Presentation.Electron/src/app/features/daily-timeline/entry-editor/entry-editor.component.html`
- [x] `src/Lifes.Presentation.Electron/src/app/features/daily-timeline/entry-editor/entry-editor.component.css`

## Tasks Breakdown
- [x] Setup models and service
- [x] Implement Daily Timeline Page
- [x] Implement Entry Editor Modal
- [x] Integrate into App Routing and Navigation
- [x] Polish UI with animations and dark theme styles

## Definition of Done
- [x] Code implemented as UI Prototype
- [x] Mock data working for all components
- [x] UI matches the provided prototype images
- [x] Documentation updated (updoc)
- [x] User Story marked as complete

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-28
- **Approved By**: User
