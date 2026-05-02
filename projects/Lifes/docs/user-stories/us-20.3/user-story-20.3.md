# User Story: US-20.3

## Story Information
- **ID**: US-20.3
- **Title**: Laputa Notes Backend Integration (Obsidian & Strategy Query)
- **Priority**: High
- **Estimate**: 12 hours
- **Sprint**: 20

## User Story
- **As a** developer
- **I want to** implement a robust backend API for Laputa Notes
- **So that** notes can be fetched from Obsidian, and queries can be handled flexibly using different strategies based on query types.

## Acceptance Criteria
1. **NoteController**: Implements standard CRUD endpoints:
   - `GET /api/notes`: Fetch paginated notes using a query strategy.
   - `GET /api/notes/{id}`: Fetch a specific note.
   - `POST /api/notes`: Create/Save a note.
   - `POST /api/notes/{id}/duplicate`: Duplicate a note.
   - `DELETE /api/notes/{id}`: Delete a note.
2. **Strategy Query Pattern**:
   - `QueryType` determines which strategy to use: `inbox`, `all`, `category`, `default` (null pattern).
   - Each strategy encapsulates its own logic and parameter validation.
3. **Obsidian Integration**:
   - `ObsidianNoteRepository` implements `INoteRepository`.
   - Notes are read/written to Obsidian markdown files.
4. **Clean Architecture**:
   - Proper separation between Controller (Presentation), Commands/Queries (Application), Interface (Core), and Obsidian Logic (Infrastructure).

## Technical Design

### Presentation Layer (WebApi)
- **NotesController**: Bridge between HTTP requests and Application Layer.

### Application Layer
- **GetNotesQuery**: Uses a `StrategyFactory` to pick the right `INoteQueryStrategy`.
- **SaveNoteCommand**, **DeleteNoteCommand**, **DuplicateNoteCommand**: Orchestrate business logic.
- **Strategies**:
    - `InboxQueryStrategy`: Filters for items in the Inbox.
    - `AllNotesQueryStrategy`: Fetches all notes with standard pagination/sorting.
    - `CategoryQueryStrategy`: Filters by specific category/section.
    - `NullQueryStrategy`: Default behavior when no type is specified.

### Domain Layer
- **Note**: Entity representing a note.
- **NoteQuery**: DTO/Value Object for query parameters.

### Infrastructure Layer
- **ObsidianNoteRepository**: Implementation using FileSystem to interact with Obsidian vault.

### Core Layer
- **INoteRepository**: Contract for data access.
- **INoteQueryStrategy**: Contract for query strategies.

## Tasks Breakdown
- [x] Define `Note` entity and `INoteRepository` in Domain/Core.
- [x] Implement `ObsidianNoteRepository` in Infrastructure.
- [x] Define `INoteQueryStrategy` and its implementations in Application.
- [x] Implement `NoteQueryStrategyFactory`.
- [x] Implement Application Commands/Queries for Notes.
- [x] Create `NotesController` in WebApi.
- [x] Wire up Dependency Injection in `Program.cs`.
- [x] Update `updoc` to reflect backend changes.

## Definition of Done
- [ ] All API endpoints are functional and verified.
- [ ] Strategy pattern correctly dispatches queries based on `QueryType`.
- [ ] Obsidian repository correctly reads/writes to markdown files.
- [ ] Code follows project standards and Clean Architecture.
- [ ] Documentation updated.
