# User Story: US-20.2

## Story Information
- **ID**: US-20.2
- **Title**: Laputa Notes Core Service & Synchronization Logic
- **Priority**: High
- **Estimate**: 8 hours
- **Sprint**: 20

## User Story
- **As a** developer
- **I want to** implement a robust reactive service for Laputa Notes
- **So that** note data is saved sequentially, synchronized across components, and handled efficiently with pagination.

## Acceptance Criteria
1. **Sequential Saving**: All save requests must be sent to the API and completed in the exact order they were triggered, even if the user switches between different notes.
2. **Auto-save with Debounce**: The editor must wait for 1 second of inactivity after typing before triggering a save.
3. **Reactive State**: Changes made in the Editor must reflect immediately in the Note List and other listening components using Angular Signals.
4. **Pagination with Ordering**: The Get List API must support pagination (inbox, all, starred, section) and ensure data is appended in the correct order even if parallel API calls return out of order.
5. **Search/Sort Reset**: Changing the search query or sort order must reset the pagination state and fetch fresh data.

## Technical Design

### Presentation Layer (Electron + Angular)
- **LaputaEditorComponent**:
    - Refactor to use `ReactiveFormsModule`.
    - Implement RxJS `valueChanges` with `debounceTime(1000)` and `distinctUntilChanged()`.
    - Call service save method.
- **LaputaNoteListComponent**:
    - Update to handle paginated data from the service.

### Application Layer
- **LaputaNotesService**:
    - Manage state using Angular Signals (`notes`, `currentPage`, `queryType`, etc.).
    - Use RxJS `Subject` and `concatMap` to handle sequential save operations.
    - Implement pagination assembly logic to handle out-of-order API responses.
    - Implement `saveById(id, title, contentHtml)` logic.

## Tasks Breakdown
- [x] Refactor `LaputaNotesService` state management for pagination.
- [x] Implement sequential save logic in Service using `concatMap`.
- [x] Refactor `LaputaEditorComponent` to `ReactiveForms`.
- [x] Wire up debounce and auto-save logic in Editor.
- [x] Implement pagination "assembly" logic for out-of-order results.
- [x] Implement search/sort reset logic.
- [x] Verify synchronization between components.

## Dependencies
- Depends on: US-20.1 (UI Framework)

## Definition of Done
- [x] Sequential saving works correctly (verified via logs/network).
- [x] Auto-save triggers 1s after user stops typing.
- [x] Switching notes does not interrupt pending saves for the previous note.
- [x] List updates automatically when a note is saved.
- [x] Pagination works and preserves order.
- [x] Documentation updated (`updoc`).

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-05-02
- **Approved By**: huy
