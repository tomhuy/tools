# User Story: US-5.1

## Story Information

| Field | Value |
|-------|-------|
| **ID** | US-5.1 |
| **Title** | Add Tool Navigation Menu with Hover Dropdown |
| **Priority** | Medium |
| **Estimate** | 3 hours |
| **Sprint** | Sprint 5 |
| **Created Date** | 2026-04-02 |
| **Status** | 📋 Planned |
| **Type** | UI/UX Enhancement |

---

## User Story

- **As a** Developer/User of ETL Tools
- **I want to** have a button that shows a dropdown menu of available tools when hovered
- **So that** I can quickly navigate between different tool forms without going back to the main menu

---

## Business Context

### Problem Statement

Hiện tại khi sử dụng ETL Tools Suite:
- ⚠️ Không có cách nhanh để chuyển đổi giữa các tool forms
- ⚠️ Phải close form hiện tại và mở form mới từ main menu
- ⚠️ Workflow bị gián đoạn khi cần sử dụng nhiều tools
- ⚠️ Thiếu tính liền mạch trong user experience
- ⚠️ Không thể xem danh sách các tools có sẵn khi đang dùng một tool

### Solution

Thêm navigation menu system vào ETL Tool forms:
- ✅ Button với icon menu/navigation ở vị trí dễ truy cập
- ✅ Hover vào button hiển thị dropdown list các tool forms
- ✅ Click vào tool trong list để navigate đến tool đó
- ✅ Highlight tool hiện tại trong dropdown menu
- ✅ Modern UI/UX với animation mượt mà
- ✅ Consistent design với existing dark theme

---

## Acceptance Criteria

### AC-1: Navigation Button Display
**Given** user đang ở trong bất kỳ ETL Tool form nào  
**When** form được load  
**Then**:
- Có một button với icon navigation (hamburger menu hoặc grid icon)
- Button nằm ở vị trí prominent (top-left hoặc top-right của form)
- Button có tooltip "Navigate to Tools" khi hover
- Button style consistent với dark theme của app
- Button có hover effect (subtle highlight/glow)

### AC-2: Hover Dropdown Menu
**Given** navigation button đã được hiển thị  
**When** user hover mouse vào button  
**Then**:
- Dropdown menu xuất hiện với animation smooth (fade in)
- Menu hiển thị danh sách tất cả available tools:
  - Version Increase Tool
  - Build Deploy Tool (nếu có)
  - [Other tools to be added in future]
- Mỗi menu item hiển thị:
  - Tool icon (nếu có)
  - Tool name
  - Optional: short description hoặc tooltip
- Tool hiện tại được highlight/marked (different color hoặc indicator)
- Menu có consistent styling với app theme

### AC-3: Menu Item Click Navigation
**Given** dropdown menu đang được hiển thị  
**When** user click vào một tool trong menu  
**Then**:
- Menu close với animation smooth (fade out)
- Navigate đến tool form tương ứng
- Nếu đang ở tool đó rồi: không làm gì (already highlighted)
- Form mới được load với smooth transition
- Previous form được close hoặc hide properly
- Navigation history được track (if applicable)

### AC-4: Menu Close Behavior
**Given** dropdown menu đang được hiển thị  
**When** user thực hiện các actions sau:
- Mouse leave khỏi button và menu area
- Click outside menu area
- Press Escape key
- Click vào một menu item

**Then**:
- Menu close với animation smooth
- No memory leaks hoặc orphaned UI elements
- Button trở về normal state

### AC-5: Responsive and Performance
**Given** navigation menu system  
**When** user tương tác với menu  
**Then**:
- Hover response time < 100ms
- Animation smooth (60fps)
- No lag khi mở/đóng menu
- Works properly với different screen resolutions
- Menu không bị crop hoặc overflow khỏi window bounds
- Z-index đúng (menu always on top)

---

## Technical Design

### Clean Architecture Layers

#### Presentation Layer (WPF)
**Location**: `src/ETLTools.Presentation.WPF/`

**New Components:**
- `Controls/NavigationMenuButton.xaml` - Custom control for navigation button with dropdown
- `Controls/NavigationMenuButton.xaml.cs` - Code-behind
- `Styles/NavigationMenuStyles.xaml` - Styles for navigation menu
- `Models/ToolMenuItem.cs` - Model for menu items

**Modified Components:**
- `Features/VersionIncrease/VersionIncreaseView.xaml` - Add navigation button
- `Features/VersionIncrease/VersionIncreaseViewModel.cs` - Add navigation command
- `MainWindow.xaml` - Setup navigation service
- `MainWindow.xaml.cs` - Wire up navigation

#### Application Layer
**Location**: `src/ETLTools.Application/`

**New Files:**
- `Services/INavigationService.cs` - Interface for navigation
- `Services/NavigationService.cs` - Navigation implementation
- `Models/ToolInfo.cs` - Tool metadata model

**Purpose:**
- Manage tool registration and discovery
- Handle navigation between tool forms
- Provide tool metadata for menu rendering

#### Core Layer
**Location**: `src/ETLTools.Core/`

**New/Modified:**
- `Interfaces/INavigationService.cs` - Core navigation interface
- `Models/ToolDefinition.cs` - Tool definition model

### Key Classes

#### NavigationMenuButton (UserControl)

**Location**: `src/ETLTools.Presentation.WPF/Controls/NavigationMenuButton.xaml`

**Purpose**: Reusable control hiển thị navigation button và dropdown menu

**Properties:**
```csharp
- ToolItems: ObservableCollection<ToolMenuItem> - Danh sách tools
- CurrentToolId: string - Tool hiện tại (để highlight)
- IsMenuOpen: bool - Menu open state
- NavigateCommand: ICommand - Command khi click menu item
```

**Features:**
- Custom hover trigger
- Popup control cho dropdown
- Animation storyboards
- Event handlers cho click và hover

#### INavigationService Interface

**Location**: `src/ETLTools.Core/Interfaces/INavigationService.cs`

**Purpose**: Service để manage navigation giữa các tools

**Methods:**
```csharp
- void RegisterTool(ToolDefinition tool) - Register tool
- IEnumerable<ToolDefinition> GetAllTools() - Get all registered tools
- void NavigateTo(string toolId) - Navigate to tool by ID
- ToolDefinition GetCurrentTool() - Get current active tool
- event EventHandler<ToolNavigatedEventArgs> ToolNavigated - Navigation event
```

#### ToolMenuItem Model

**Location**: `src/ETLTools.Presentation.WPF/Models/ToolMenuItem.cs`

**Properties:**
```csharp
- string Id - Tool unique identifier
- string Name - Tool display name
- string Description - Tool description (optional)
- string IconPath - Path to tool icon (optional)
- bool IsActive - Is this the current tool?
- ICommand NavigateCommand - Command to navigate
```

### UI/UX Design

#### Button Design
```
┌─────────────────────────────────────┐
│ ≡  [Navigation Button]              │ <- Top-left corner
│                                      │
│    ETL Tool Form Content            │
│                                      │
└─────────────────────────────────────┘
```

#### Dropdown Menu Design
```
┌───────────────────────────┐
│ ≡ Navigation               │
├───────────────────────────┤
│ 🔧 Version Increase Tool ✓│ <- Current tool (highlighted)
│ 📦 Build Deploy Tool      │
│ 🛠️  [Future Tool]         │
└───────────────────────────┘
```

#### Color Scheme (Dark Theme)
- Background: `#2D2D30`
- Highlight: `#007ACC`
- Text: `#FFFFFF`
- Border: `#3F3F46`
- Hover: `#3E3E42`

### Animation Specifications

**Menu Open/Close:**
- Duration: 200ms
- Easing: EaseOutQuart
- Effect: Fade + Scale (0.95 → 1.0)

**Hover Effect:**
- Duration: 150ms
- Easing: EaseInOutQuad
- Effect: Background color transition

### Files to Create/Modify

#### New Files
- [ ] `src/ETLTools.Presentation.WPF/Controls/NavigationMenuButton.xaml`
- [ ] `src/ETLTools.Presentation.WPF/Controls/NavigationMenuButton.xaml.cs`
- [ ] `src/ETLTools.Presentation.WPF/Styles/NavigationMenuStyles.xaml`
- [ ] `src/ETLTools.Presentation.WPF/Models/ToolMenuItem.cs`
- [ ] `src/ETLTools.Core/Interfaces/INavigationService.cs`
- [ ] `src/ETLTools.Core/Models/ToolDefinition.cs`
- [ ] `src/ETLTools.Application/Services/NavigationService.cs`

#### Modified Files
- [ ] `src/ETLTools.Presentation.WPF/Features/VersionIncrease/VersionIncreaseView.xaml`
- [ ] `src/ETLTools.Presentation.WPF/Features/VersionIncrease/VersionIncreaseViewModel.cs`
- [ ] `src/ETLTools.Presentation.WPF/MainWindow.xaml`
- [ ] `src/ETLTools.Presentation.WPF/MainWindow.xaml.cs`
- [ ] `src/ETLTools.Presentation.WPF/App.xaml` (register navigation service in DI)

---

## Tasks Breakdown

### Task 1: Create Core Navigation Models and Interfaces
- [ ] Create `INavigationService` interface in Core layer
- [ ] Create `ToolDefinition` model
- [ ] Create `ToolNavigatedEventArgs` event args
- [ ] Document interfaces with XML comments

### Task 2: Implement Navigation Service
- [ ] Create `NavigationService` class in Application layer
- [ ] Implement tool registration logic
- [ ] Implement navigation methods
- [ ] Implement event raising mechanism
- [ ] Register service in DI container

### Task 3: Create Navigation Menu Button Control
- [ ] Create `NavigationMenuButton` UserControl (XAML)
- [ ] Design button layout with icon
- [ ] Create Popup for dropdown menu
- [ ] Implement hover triggers
- [ ] Create animation storyboards (open/close)
- [ ] Add code-behind logic

### Task 4: Create Navigation Menu Styles
- [ ] Create `NavigationMenuStyles.xaml` resource dictionary
- [ ] Define button styles (normal, hover, pressed)
- [ ] Define menu item styles (normal, hover, active)
- [ ] Define animation resources
- [ ] Add to App.xaml resources

### Task 5: Create ToolMenuItem Model
- [ ] Create `ToolMenuItem` class
- [ ] Implement INotifyPropertyChanged
- [ ] Add properties (Id, Name, Description, IconPath, IsActive)
- [ ] Add NavigateCommand property

### Task 6: Integrate Navigation Button into Tool Forms
- [ ] Modify `VersionIncreaseView.xaml` - add NavigationMenuButton
- [ ] Modify `VersionIncreaseViewModel.cs` - add navigation logic
- [ ] Wire up navigation commands
- [ ] Test navigation from Version Increase Tool

### Task 7: Setup Main Window Navigation
- [ ] Modify `MainWindow.xaml.cs` - initialize NavigationService
- [ ] Register all available tools
- [ ] Setup navigation event handlers
- [ ] Test tool switching

### Task 8: Testing and Polish
- [ ] Test hover behavior (timing, responsiveness)
- [ ] Test navigation between tools
- [ ] Test menu close on various triggers
- [ ] Test with different screen resolutions
- [ ] Verify no memory leaks
- [ ] Polish animations and transitions
- [ ] Add keyboard navigation support (optional)

---

## Dependencies

### Depends On
- Version Increase Tool (US-1.x) - Must be implemented first
- Dark theme styling - Must exist in app resources

### Blocked By
- None

### Blocks
- None (enhancement feature)

---

## Definition of Done

- [ ] Code implemented following Clean Architecture
- [ ] NavigationMenuButton UserControl created and styled
- [ ] NavigationService implemented and registered in DI
- [ ] Navigation button added to all existing tool forms
- [ ] Hover behavior working smoothly with animations
- [ ] Menu shows all registered tools correctly
- [ ] Current tool is highlighted in menu
- [ ] Navigation between tools works correctly
- [ ] Menu closes properly on all triggers
- [ ] No memory leaks or performance issues
- [ ] Consistent with dark theme design
- [ ] Code reviewed and approved
- [ ] Manual testing completed
- [ ] Documentation updated
- [ ] User Story marked as complete

---

## Non-Functional Requirements

### Performance
- Menu open response time: < 100ms
- Animation frame rate: 60fps
- Memory footprint: < 5MB additional
- No UI freezing during navigation

### Usability
- Intuitive hover behavior
- Clear visual feedback
- Consistent with existing UI patterns
- Accessible (keyboard navigation support)

### Maintainability
- Reusable control for all tool forms
- Easy to add new tools to menu
- Centralized navigation logic
- Well-documented code

### Compatibility
- Windows 10/11
- .NET 6.0 runtime
- Screen resolutions: 1920x1080 and above
- High DPI support

---

## Future Enhancements

### Phase 2 (Optional)
- [ ] Add search/filter functionality for tools
- [ ] Add recent tools history
- [ ] Add tool grouping/categories
- [ ] Add keyboard shortcuts display
- [ ] Add tool favorites/pinning
- [ ] Add custom tool ordering
- [ ] Add tool icons from resources
- [ ] Add tool status indicators (e.g., "In Progress", "Completed")

### Phase 3 (Optional)
- [ ] Add context menu on right-click
- [ ] Add tool quick actions in dropdown
- [ ] Add tool settings access from menu
- [ ] Add breadcrumb navigation
- [ ] Add split view (multiple tools side-by-side)

---

## Implementation Notes

### Design Decisions

1. **Hover vs Click Behavior**
   - Decision: Use hover to open menu for quick access
   - Alternative: Click to open (more traditional)
   - Reason: Hover provides faster access and better UX for frequent switching

2. **UserControl vs Custom Control**
   - Decision: Use UserControl for NavigationMenuButton
   - Alternative: Create custom Control class
   - Reason: Simpler implementation, easier to maintain, sufficient for requirements

3. **Navigation Service Location**
   - Decision: Put in Application layer
   - Alternative: Infrastructure layer
   - Reason: Navigation is application-level concern, not external dependency

4. **Menu Positioning**
   - Decision: Top-left corner of tool forms
   - Alternative: Top-right, bottom, or floating
   - Reason: Consistent with common UI patterns, easy to reach

### Technical Considerations

- **Memory Management**: Ensure Popup and menu items are properly disposed
- **Thread Safety**: Navigation service must be thread-safe
- **Z-Index**: Popup must have high z-index to appear on top
- **Event Handling**: Prevent event leaks by properly unsubscribing
- **MVVM Pattern**: Maintain clean separation using commands and bindings

### Risks and Mitigations

| Risk | Impact | Likelihood | Mitigation |
|------|--------|-----------|------------|
| Hover timing too sensitive | Medium | Medium | Add configurable delay (200-300ms) |
| Menu appears off-screen | Low | Low | Implement bounds checking |
| Performance impact on older machines | Low | Low | Profile and optimize animations |
| Complexity for future tools | Medium | Low | Design extensible registration system |

---

## Resources

### Design References
- [Material Design - Navigation Drawer](https://material.io/components/navigation-drawer)
- [Fluent Design System - Navigation](https://docs.microsoft.com/en-us/windows/apps/design/controls/navigationview)
- [WPF Popup Control Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/controls/popup-overview)

### Code Examples
- WPF Popup with Animation
- MVVM Navigation Service Pattern
- UserControl with Dependency Properties

---

## Acceptance Testing Scenarios

### Scenario 1: First Time User
1. User opens Version Increase Tool
2. User sees navigation button clearly visible
3. User hovers over button
4. Dropdown menu appears smoothly
5. User sees list of available tools
6. Current tool is highlighted
7. User clicks another tool
8. App navigates to selected tool

### Scenario 2: Frequent Tool Switching
1. User works in Version Increase Tool
2. User hovers navigation button (no click needed)
3. Menu appears instantly
4. User moves mouse to Build Deploy Tool option
5. User clicks to navigate
6. App switches to Build Deploy Tool
7. Previous tool state is preserved (if applicable)

### Scenario 3: Menu Dismiss
1. User opens navigation menu
2. User moves mouse away from menu
3. Menu closes after brief delay
4. OR user presses Escape key
5. Menu closes immediately
6. OR user clicks outside menu
7. Menu closes immediately

---

## Final Status

- **Status**: 📋 Approved
- **Ready for Implementation**: Yes
- **Approved By**: Pending Review
- **Notes**: Waiting for user approval before implementation

---

**Document Version**: 1.0.0  
**Created**: 2026-04-02  
**Last Modified**: 2026-04-02
