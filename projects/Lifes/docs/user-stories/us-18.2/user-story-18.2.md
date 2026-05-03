# User Story: US-18.2

## Story Information
- **ID**: US-18.2
- **Title**: Mood Tracker API Integration
- **Priority**: High
- **Estimate**: 4 hours
- **Sprint**: 5

## User Story
- **As a** User
- **I want to** lưu trữ dữ liệu tâm trạng và hoạt động của mình lên Backend
- **So that** dữ liệu của tôi được bảo tồn lâu dài và không bị mất khi reload ứng dụng.

## Acceptance Criteria
1. Given a new mood entry
   When the user saves it in the editor
   Then the data should be sent to `POST /api/mood` and saved in `mood_entries.json`.
2. Given the Range Tracker page
   When the component initializes
   Then it should fetch data from `GET /api/mood` and display it on the matrix.
3. Given an existing entry
   When the user deletes it
   Then the data should be removed from the backend storage.
4. All dates must be sent in ISO 8601 format.

## Technical Design

### Clean Architecture Layers
- **Presentation**: `MoodApiService` (Angular)
- **Application**: `MoodController` (WebApi)
- **Domain**: `MoodEntry` (Core Models)
- **Infrastructure**: `JsonMoodEntryRepository` (Infrastructure)
- **Core**: `IMoodEntryRepository`

### Files to Create/Modify
- [x] `src/Lifes.Core/Models/MoodEntry.cs` [NEW]
- [x] `src/Lifes.Core/Interfaces/IMoodEntryRepository.cs` [NEW]
- [x] `src/Lifes.Infrastructure/Features/MoodTracker/Repositories/JsonMoodEntryRepository.cs` [NEW]
- [x] `src/Lifes.Presentation.WebApi/Controllers/MoodController.cs` [NEW]
- [x] `src/app/features/weekly-tracker/services/mood-api.service.ts` [NEW]
- [x] `src/app/features/weekly-tracker/weekly-tracker.service.ts` [MODIFY]

## Definition of Done
- [x] Code implemented following Clean Architecture
- [x] Code reviewed
- [x] Documentation updated (updoc)
- [x] User Story marked as complete
