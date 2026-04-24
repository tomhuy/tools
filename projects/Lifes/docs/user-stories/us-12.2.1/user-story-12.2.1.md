# User Story: US-12.2.1

## Story Information
- **ID**: US-12.2.1
- **Title**: API Integration for Topic Editor (Returning Saved Memento)
- **Priority**: High
- **Estimate**: 3 hours
- **Sprint**: Next Sprint

## User Story
- **As a** Developer
- **I want to** backend trả về object memento sau khi save (bao gồm ID mới)
- **So that** frontend có thể cập nhật state chính xác mà không cần load lại toàn bộ dữ liệu.

## Acceptance Criteria
1. `ICalendarService.SaveMementoAsync` trả về `Task<MementoModel>`.
2. `CalendarController.SaveMemento` trả về memento đã save từ service.
3. Frontend nhận được ID mới và cập nhật signal `mementos`.

## Technical Design
- Modify `ICalendarService` interface.
- Update `CalendarService` (Infrastructure) and any other implementations.
- Update `CalendarController`.

## Tasks Breakdown
- [x] Task 1: Update `ICalendarService` interface.
- [x] Task 2: Update `CalendarService` implementation.
- [x] Task 3: Update `CalendarController` actions.
- [x] Task 4: Verify integration with Electron UI.

## Definition of Done
- [x] Backend returns saved memento with ID.
- [x] Frontend handles the returned object correctly.
- [x] No regressions in existing calendar features.

## Implementation Progress

### Files Modified
- [x] `src/Lifes.Core/Interfaces/ICalendarService.cs`
- [x] `src/Lifes.Core/Interfaces/IMementoRepository.cs`
- [x] `src/Lifes.Core/Interfaces/ITagRepository.cs`
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Repositories/JsonMementoRepository.cs`
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Repositories/JsonTagRepository.cs`
- [x] `src/Lifes.Infrastructure/Features/AnnualCalendar/Services/CalendarService.cs`
- [x] `src/Lifes.Presentation.WebApi/Controllers/CalendarController.cs`

### Current Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-25
- **Notes**: API Integration hoàn tất. Backend hiện đã trả về đối tượng vừa lưu kèm ID mới, giúp Frontend cập nhật state đồng bộ mà không cần reload.
