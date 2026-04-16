# User Story: US-8.5

## Story Information
- **ID**: US-8.5
- **Title**: Hamburger Navigation Menu
- **Priority**: High
- **Estimate**: 6 hours
- **Sprint**: Phase 5

## User Story
- **As a** user
- **I want to** navigate between different application tools using a clean hamburger menu
- **So that** the interface remains uncluttered and more space is available for the main content.

## Acceptance Criteria
1. **Hamburger Button**:
   - A modern ☰ button is located in the top-left header.
   - The button triggers a dropdown/popup menu showing all available tools.
2. **Tool Selection**:
   - Clicking a tool in the menu initiates navigation.
   - The active tool is highlighted in the menu.
3. **Responsive Design**:
   - The menu closes automatically after navigation.
   - The layout adjusts to accommodate the menu button without overlapping title text.

## Technical Design

### Clean Architecture Layers
- **Core**: Uses `INavigationService` to list registered tools.
- **Presentation**: 
    - Implemented `HamburgerButtonStyle` and `ToolMenuButtonStyle` in XAML.
    - Used a `Popup` control for the menu container.
    - ViewModels inject `INavigationService` and expose `NavigationMenuItems`.

## Implementation Progress

### Files Modified
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/AnnualCalendarView.xaml`
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/AnnualCalendarViewModel.cs`
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarView.xaml`
- [x] `Lifes.Presentation.WPF/Features/AnnualCalendar/MonthlyCalendarViewModel.cs`

## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-16
- **Approved By**: User
