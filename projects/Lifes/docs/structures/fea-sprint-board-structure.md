# Feature: Sprint Board

## Overview
The Sprint Board feature visualizes features and assignee task allocation in a matrix Kanban board UI. It supports tracking status, assignee colors, and feature dependencies in a highly stylized layout. US-11.4 enhanced this feature by migrating from mock data to a full Backend API and adding advanced management capabilities.

## Architecture

### 1. Backend (Clean Architecture)
- **Core Layer** (Shared):
    - `SprintTask`, `Epic`: Entities representing the board structure, located in `Lifes.Core.Models`.
    - `User`: Global entity for team members, located in `Lifes.Core.Models`.
- **Application Layer**: 
    - Commands and Queries for fetching and saving board/user data.
- **Infrastructure Layer**:
    - `JsonUserRepository`, `JsonSprintBoardRepository`: JSON persistence in the `database/` directory.
- **Presentation Layer**:
    - `UsersController`, `SprintBoardController`: REST endpoints for frontend communication.

### 2. Frontend (Angular 19)
- **Components**:
    - `SprintBoardComponent`: Main matrix UI with Modals for Epic and User management.
- **Services**:
    - `UserService`: Manages global user state using Signals.
    - `SprintBoardService`: Manages Epic/Task state and API synchronization.
- **Models**:
    - `SprintTask`, `SprintFeature`, `User`, `ApiResponse`.

## UI/UX Implementation Details

### Matrix Layout
- **Dynamic Columns**: Columns are generated based on the list of active users.
- **Special Columns**: A dedicated "Làm trước" (Pre) column for unassigned or high-priority tasks.
- **Vertical Accent Borders**: Left-side color coding for Epics.

### Advanced Features
- **Epic Editor**: Full CRUD for Epics and nested subtasks.
- **Top Priority**: Sorting logic that keeps starred tasks at the top.
- **Archive Section**: Collapsible list for completed/archived epics.
- **Summary Bar**: Real-time calculation of workload (Done/Active) per user.

### Drag & Drop
- **Native HTML5 Drag & Drop**: Used for reassigning tasks between users or the "Pre" column. State is instantly updated via Signals and persisted via API.

## Data Flow
1. **Load**: `OnInit` -> `userService.loadUsers()` & `boardService.loadBoard()`.
2. **Interact**: User drags task -> Signal updates locally -> `saveBoard()` API call.
3. **Manage**: Edit Epic -> Modal updates local copy -> Save -> `saveBoard()` API call -> Refresh Signals.

## Key Decisions
- **Separated Users API**: Users are now a standalone entity to support future cross-feature integration.
- **Full State Persistence**: For a local tool, saving the full list of Epics is efficient and ensures data consistency across the matrix.
- **Google Sans Typography**: Used for a premium, modern feel matching the Google ecosystem.
