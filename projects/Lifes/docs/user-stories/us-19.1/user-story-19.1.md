# User Story: US-19.1

## Story Information
- **ID**: US-19.1
- **Title**: PDF Reader UI Clone
- **Priority**: High
- **Estimate**: 8 hours
- **Sprint**: TBD

## User Story
- **As a** user
- **I want to** have a PDF Reader interface with different reading layouts (Classic, Focus, Contextual)
- **So that** I can customize my reading experience, view my library, and take notes effectively.

## Acceptance Criteria
1. **Layout Switching**: Users can switch between Classic, Focus, and Contextual layouts. The layouts must match the design in `samle.read.html`.
2. **Dark Mode**: Users can toggle between light and dark modes, with colors adapting accordingly.
3. **Zooming**: Users can zoom in and out of the PDF page content.
4. **Library & Notes Panel**: Depending on the layout, users can see the book list and notes list.
5. **Text Selection & Highlighting**: When text is selected, a floating toolbar appears to allow highlighting or adding notes.
6. **Note Popup**: A popup appears when adding a note to a highlight.

## Technical Design

### Clean Architecture Layers
- **Presentation**: `PdfReaderPageComponent` (Angular Component)
- **Application**: Layout state management, Theme state management
- **Domain**: LayoutEnum, ThemeEnum, Note models
- **Infrastructure**: LocalStorage for saving theme/layout preferences (if applicable)

### Files to Create/Modify
- [ ] `src/Lifes.Presentation.Electron/src/app/features/pdf-reader/pdf-reader-page/pdf-reader-page.component.ts`
- [ ] `src/Lifes.Presentation.Electron/src/app/features/pdf-reader/pdf-reader-page/pdf-reader-page.component.html`
- [ ] `src/Lifes.Presentation.Electron/src/app/features/pdf-reader/pdf-reader-page/pdf-reader-page.component.css`

## Tasks Breakdown
- [ ] Task 1: Setup basic layout and routing for PdfReaderPageComponent
- [ ] Task 2: Migrate CSS variables and styles from `samle.read.html` to `pdf-reader-page.component.css`
- [ ] Task 3: Migrate HTML structure for Layout 1, Layout 2, and Layout 3
- [ ] Task 4: Implement layout switching logic in Angular (Signals or properties)
- [ ] Task 5: Implement dark mode toggle
- [ ] Task 6: Implement zoom logic and progress bar updates
- [ ] Task 7: Implement selection floating toolbar and note popup

## Dependencies
- Depends on: None
- Blocked by: None

## Definition of Done
- [ ] Code implemented following Clean Architecture
- [ ] UI perfectly matches `samle.read.html`
- [ ] Code reviewed
- [ ] Documentation updated
- [ ] User Story marked as complete
