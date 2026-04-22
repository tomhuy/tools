# Feature: Sprint Board

## Overview
The Sprint Board feature visualizes features and assignee task allocation in a matrix Kanban board UI. It supports tracking status, assignee colors, and feature dependencies in a highly stylized layout. This feature is implemented as a cross-platform component available in both WPF and Electron (Angular).

## Architecture

### 1. WPF Presentation Layer (Legacy)
- `SprintBoardView.xaml` - Main grid interface defining the top header, an avatar column mapping, and the core dynamic matrix for Kanban tasks.
- `SprintBoardViewModel.cs` - Mock data state providing bindings for Assignees and FeatureRows.
- `Models/SprintBoardModels.cs` - Contains `BoardAssigneeModel`, `BoardTaskModel`, `BoardFeatureModel`, and `FeatureRowViewModel` defining the mock database states.

### 2. Electron/Angular Presentation Layer (Modern)
- `SprintBoardComponent` (`.ts`, `.html`, `.css`) - Angular component managing the board matrix.
- `SprintBoardService` - Reactive service using **Angular Signals** for state management and local data mock.
- `SprintBoardData`, `SprintFeature`, `SprintTask` - TypeScript interfaces defining the board data structure.

## UI/UX Implementation Details

### Vertical Accent Borders
- The board uses custom accent borders (3px solid) on the left of each feature block to indicate the current category.
- A controlled gap is maintained between feature blocks by applying the accent border only to the feature cells (`.feat-td`) and omitting it from the "Add task" spacer row (`.feat-td-spacer`).

### Drag & Drop
- **WPF**: Utilizes native WPF `DoDragDrop` commands with code-behind handlers.
- **Electron**: Utilizes native **HTML5 Drag & Drop API**. Tasks include `draggable="true"` and the matrix cells handle `dragover` and `drop` events to reassign tasks via the `SprintBoardService`.

### Layout & Scrolling
- To prevent redundant scrollbars in Electron, the main shell's overflow is disabled, and scrolling is delegated to the `SprintBoardComponent`'s host element.
- The Matrix uses `position: sticky` for both the header and the Feature column to maintain context during large sprint reviews.

## Data Flow
- **WPF**: Static Initializer -> `SprintBoardViewModel` -> Two-way bounded to XAML.
- **Electron**: `SprintBoardService` (Signals) -> `SprintBoardComponent` template. State changes (like Drag & Drop or Toggling Done) update the Signals, triggering automated UI refresh.

## Key Decisions
- **Native HTML5 Drag & Drop over CDK**: For US-11.2, native HTML5 D&D was chosen to maintain maximum control over the table-row/cell drag behavior without introducing additional heavy dependencies, ensuring high-fidelity reproduction of the raw prototype's feel.
- **Strict Content Security Policy (CSP)**: Implemented in `index.html` to allow loading Google Fonts (Google Sans) while satisfying Electron's security requirements.
