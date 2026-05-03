# User Story: US-19.2

## Story Information
- **ID**: US-19.2
- **Title**: Shared PDF Viewer Component
- **Priority**: High
- **Estimate**: 3 Story Points

## User Story
- **As a** developer/user
- **I want to** have a generic, reusable PDF Viewer Component
- **So that** I can easily view PDFs, highlight text, and read annotations across different features (PDF Reader, Memento) without duplicating configuration and UI logic.

## Acceptance Criteria
1. Given a valid PDF URL, When passed to the `pdfSrc` Input, Then the PDF should load and render correctly.
2. Given the component is loaded, When the `theme` Input is set to `dark`, Then the UI and PDF background should adapt to dark mode, matching the Laputa Design System.
3. Given the `showToolbar` Input is false, When the PDF loads, Then the default toolbar of the library should be completely hidden.
4. Given the user selects text on the PDF, When the mouse is released, Then the component must emit an `onTextSelected` event containing the selected text and its DOMRect coordinates.
5. Given a valid JSON string of annotations is passed to `annotationsData`, When the PDF is fully loaded, Then the highlights and notes should be rendered correctly at their original positions.

## Technical Design

### Clean Architecture Layers
- **Presentation**: `LaputaPdfViewerComponent` (Dumb/Pure Component)
  - Sử dụng thư viện: `ngx-extended-pdf-viewer`
  - Nằm ở thư mục Shared để tái sử dụng tối đa.

### Files to Create/Modify
- [x] `package.json` (add ngx-extended-pdf-viewer)
- [x] `angular.json` (add assets mapping)
- [x] `src/app/shared/components/laputa-pdf-viewer/laputa-pdf-viewer.component.ts|html|css`

## Tasks Breakdown
- [x] Install library and configure assets in `angular.json`
- [x] Create basic Component structure with Inputs/Outputs
- [x] Implement text selection capturing logic via DOM events
- [x] Apply Laputa Design System CSS variables to override library defaults
- [x] Implement Editor API calls to inject saved annotations (Partial - mocked for now)

## Definition of Done
- [x] Code implemented as a pure component
- [x] CSS complies with `laputa-design-system.md`
- [x] Text selection correctly emits coordinates
- [x] Documentation updated
- [x] Component successfully compiles without errors

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-05-03
- **Approved By**: Huy
