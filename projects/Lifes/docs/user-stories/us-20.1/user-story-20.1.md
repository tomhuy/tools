# User Story: US-20.1

## Story Information
- **ID**: US-20.1
- **Title**: Laputa Notes UI Clone
- **Priority**: High
- **Estimate**: 8 hours
- **Sprint**: TBD

## User Story
- **As a** user
- **I want to** have a fully functional note-taking interface (Laputa Notes) integrated into the application
- **So that** I can manage my notes, use markdown, organize them by tags/sections, and switch viewing modes effectively.

## Acceptance Criteria
1. **Layout & Sidebar**:
   - Sidebar displays pinned sections, dynamically loaded sections, and a theme switcher (Dark/Sepia).
   - Sidebar can be toggled via an icon button.
   - Search input is available in the sidebar.
2. **Note List Panel**:
   - Displays a list of notes with different view modes: List, Card, Compact, Grid.
   - Panel is resizable using a drag handle.
   - Notes can be sorted and filtered based on the selected section or search query.
   - Clicking a note opens it in the editor.
3. **Editor Area**:
   - Includes a top bar with breadcrumb, preview toggle, export, delete, and close actions.
   - Provides a markdown formatting toolbar.
   - Supports editing note title and content with auto-resizing textareas.
   - Can toggle a markdown preview mode.
   - Includes a detail popup for Grid view mode.
4. **Modals & Context Menus**:
   - Context menu is available for note items (Open, Rename, Duplicate, Delete).
   - New Note modal allows entering a title and selecting tags before creating.
5. **State Management & Mock Data**:
   - Use Angular Signals for managing state (selected note, view mode, theme, list of notes).
   - Create a service to handle mock data mimicking the `sample.note.app.html` data structure.
   - Ensure the UI components are modularized.

## Technical Design

### Clean Architecture Layers
- **Presentation**: `LaputaNotesPageComponent`, `NoteSidebarComponent`, `NoteListComponent`, `NoteEditorComponent`, `NoteService`
- **Application**: Data flow management via Angular Signals in `NoteService`
- **Infrastructure**: Local storage or mock data initial state.

### Files to Create/Modify
- [x] `src/app/features/laputa-notes/laputa-notes-page/laputa-notes-page.component.ts|html|css`
- [x] `src/app/features/laputa-notes/services/laputa-notes.service.ts`
- [x] `src/app/app.routes.ts` (Add route for Laputa Notes)
- [x] `src/app/app.component.html` (Add navigation link)

## Tasks Breakdown
- [x] Task 1: Setup feature folder and service with mock data.
- [x] Task 2: Implement the layout (Sidebar, Note List, Editor).
- [x] Task 3: Implement Theme switcher and Sidebar toggle logic.
- [x] Task 4: Implement Note List with view modes (List, Card, Compact, Grid) and resize handle.
- [x] Task 5: Implement Editor area with Markdown formatting toolbar and preview.
- [x] Task 6: Implement Context Menu and New Note Modal.
- [x] Task 7: Integrate routing and navigation.

## Dependencies
- Depends on: None
- Blocked by: None

## Definition of Done
- [x] Layout matches `sample.note.app.html` perfectly (Pixel-perfect clone).
- [x] CSS is scoped locally to avoid affecting other features.
- [x] Logic relies on Angular Signals instead of raw DOM manipulation.
- [x] Code reviewed and approved.
- [x] Documentation updated.
