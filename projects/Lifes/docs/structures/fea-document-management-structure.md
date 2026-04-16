# Feature: Document Management Visual Tracker

## Overview
Document Management provides a pixel-perfect visual status tracking grid that plots document and task occurrences across the current month, functioning similar to a detailed Gantt chart but laid out on a grid calendar template with continuous day columns and exact 25x25 day-cell aspect ratios.

## Architecture

### Presentation Layer
- `Features/DocumentManagement/DocumentManagementView.xaml` - WPF Data Grid Alternative built on deep nested `ItemsControl`, applying synchronized `SharedSizeGroup` splitters parsing dynamic grid sizes.
- `Features/DocumentManagement/DocumentManagementViewModel.cs` - Translates document entities to visual hierarchy collections (`DayHeaderViewModel`, `DocumentRowViewModel`, `DayCellViewModel`).
- `DocumentManagementView.xaml.cs` - Code-behind to handle smart window-size initialization via measuring `ScrollViewer.ViewportWidth`.

### Infrastructure Layer
- `Features/DocumentManagement/Services/MockDocumentService.cs` - Instantiates mock structural data to test visual tracking (Phase 1).

### Core Layer
- `Interfaces/IDocumentService.cs` - Contract fetching user document statuses.
- `Models/DocumentModel.cs` - Contains `StartDate`, `EndDate`, and task hierarchy flags.

## Key Classes

### DocumentManagementView
**Location**: `src/Lifes.Presentation.WPF/Features/DocumentManagement/DocumentManagementView.xaml`
**Purpose**: Primary UI element utilizing intricate `ItemsControl` nesting rather than Standard DataGrids for pixel-perfect manipulation.
**Dependencies**: `DocumentManagementViewModel`

## Data Flow
`DocumentManagementViewModel` fetches from `IDocumentService` -> Filters current month timeline -> Splits into arrays of exactly corresponding Days -> UI `ItemsControl` plots these days across the column definitions natively synchronized by structural matching.

## Key Decisions
- **Abandoning DataGrid**: A Custom `ItemsControl` approach was chosen over `DataGrid` because standard table elements lack the rendering flexibility required to exactly layout the day-month cell sizes and the particular nested border styles provided in the clone requirements.
- **Dynamic Stretching UI (`SharedSizeGroup`)**: Configured code-behind tracking of window drag sizes allowing the primary title column to stretch dynamically whilst holding day cells to exact fixed pixel widths.
