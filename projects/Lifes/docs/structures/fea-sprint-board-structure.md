# Feature: Sprint Board

## Overview
The Sprint Board feature visualizes features and assignee task allocation in a matrix Kanban board UI. It supports tracking status, assignee colors, and feature dependencies in a highly stylized layout.

## Architecture

### Presentation Layer
- `SprintBoardView.xaml` - Main grid interface defining the top header, an avatar column mapping, and the core dynamic matrix for Kanban tasks.
- `SprintBoardView.xaml.cs` - Code-behind containing basic UX initializations.
- `SprintBoardViewModel.cs` - Mock data state providing bindings for Assignees and FeatureRows.
- `Models/SprintBoardModels.cs` - Contains `BoardAssigneeModel`, `BoardTaskModel`, `BoardFeatureModel`, and `FeatureRowViewModel` defining the mock database states.

### Application Layer
- Registered within `NavigationService` via `ToolIds.SprintBoard`.

## Data Flow
Static Initializer -> `SprintBoardViewModel` -> Two-way bounded to `SprintBoardView.xaml` via `ItemsControl` and `UniformGrid`.
User drag interaction -> `DataObject` injected to `MoveTask` Command -> Source bounds reset, Target Bounds trigger rerender.

## Key Decisions
- **UniformGrid over custom mapping Grids**: Employing `UniformGrid` for rendering dynamic Assignee columns allows the cells in the rows below it to automatically align and scale proportionally with the header. This negates the need for complex `SharedSizeGroup` overhead for the column allocations and works great right out of the box.
- **POCO models**: Aggregated into a single `SprintBoardModels.cs` file due to the data structures strictly being representations of XAML layout groups rather than domain abstractions (during this static Mock phase). They are derived from `ObservableObject` matching MVVM Toolkits to support real-time UI state refreshing.
- **Native WPF Drag & Drop**: Native structural `DoDragDrop` commands bind low-level Windows interactions enabling moving tasks directly to alternative target rows using standard Code-Behind handlers passed up to ViewModel Context logic.
