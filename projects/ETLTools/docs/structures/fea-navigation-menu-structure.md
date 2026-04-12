# Feature: Navigation Menu (US-5.1)

## Overview

Navigation Menu là một **shared UI/UX enhancement** cho toàn bộ ETL Tools Suite. Feature này bổ sung một hamburger button ở góc trên-trái của mỗi tool form, cho phép user hover để hiển thị dropdown menu chứa danh sách tất cả tool forms đang có. Mục tiêu là giảm friction khi chuyển đổi giữa các tools mà không cần quay về màn hình chính.

**User Story**: US-5.1  
**Sprint**: Sprint 5  
**Status**: ✅ Completed — 2026-04-07  
**Type**: UI/UX Enhancement (cross-cutting concern)

---

## Architecture

Feature này được thiết kế theo **Clean Architecture** với navigation service ở Application layer và control ở Presentation layer.

### Core Layer — `src/Lifes.Core/`

| File | Purpose |
|------|---------|
| `Interfaces/INavigationService.cs` | Contract cho toàn bộ navigation logic |
| `Models/ToolDefinition.cs` | Metadata của một tool (Id, Name, Description, IconPath) |
| `Models/ToolNavigatedEventArgs.cs` | EventArgs cho event `ToolNavigated` |

### Application Layer — `src/Lifes.Application/Services/`

| File | Purpose |
|------|---------|
| `NavigationService.cs` | Singleton implementation của `INavigationService`; thread-safe với `lock` |

### Presentation Layer — `src/Lifes.Presentation.WPF/`

| File | Purpose |
|------|---------|
| `Controls/NavigationMenuButton.xaml` | UserControl: hamburger button + animated Popup dropdown |
| `Controls/NavigationMenuButton.xaml.cs` | Code-behind: `ToolItems` DependencyProperty, 300ms close timer, fade animation |
| `Models/ToolMenuItem.cs` | ObservableObject cho từng item trong dropdown (Id, Name, IsActive, NavigateCommand) |
| `Styles/NavigationMenuStyles.xaml` | `NavButtonStyle`, `NavMenuItemStyle` — dark theme colors |
| `Constants/ToolIds.cs` | Static constants cho tool identifiers (`"version-increase"`) |

### Modified Files

| File | Change |
|------|--------|
| `App.xaml` | Merge `NavigationMenuStyles.xaml` vào global resources |
| `App.xaml.cs` | Register `INavigationService` (Singleton), gọi `RegisterNavigationTools()` trước khi tạo MainWindow |
| `MainWindow.xaml.cs` | Inject `INavigationService`, subscribe `ToolNavigated`, swap `ContentControl.Content` |
| `Features/VersionIncrease/VersionIncreaseView.xaml` | Thêm `<controls:NavigationMenuButton>` vào header |
| `Features/VersionIncrease/VersionIncreaseViewModel.cs` | Inject `INavigationService`, build `NavigationMenuItems` từ registered tools |

---

## Key Classes

### INavigationService
**Location**: `src/Lifes.Core/Interfaces/INavigationService.cs`  
**Purpose**: Contract cho navigation service  
**Methods**:
- `RegisterTool(ToolDefinition)` — đăng ký tool khi startup
- `GetAllTools()` — lấy danh sách để build menu
- `NavigateTo(string toolId)` — trigger navigation (raises `ToolNavigated` event)
- `GetCurrentTool()` — trả về tool đang active

### NavigationService
**Location**: `src/Lifes.Application/Services/NavigationService.cs`  
**Purpose**: Thread-safe singleton implementation  
**Dependencies**: None (pure application logic)  
**Used by**: `App.xaml.cs` (registration), `VersionIncreaseViewModel` (query tools), `MainWindow` (subscribe event)

### NavigationMenuButton
**Location**: `src/Lifes.Presentation.WPF/Controls/NavigationMenuButton.xaml`  
**Purpose**: Reusable UserControl hiển thị navigation button và dropdown  
**DependencyProperty**: `ToolItems` (ObservableCollection\<ToolMenuItem\>)  
**Behavior**:
- MouseEnter button → open popup (fade-in 180ms)
- MouseLeave button/popup → 300ms timer → close (fade-out 160ms)
- Click menu item → execute `ToolMenuItem.NavigateCommand`, close popup
- Escape key → close popup immediately

### ToolMenuItem
**Location**: `src/Lifes.Presentation.WPF/Models/ToolMenuItem.cs`  
**Purpose**: ViewModel cho một row trong dropdown  
**Properties**: `Id`, `Name`, `Description`, `IsActive` (bool), `NavigateCommand`  
**Active state**: `IsActive = true` khi tool này đang được hiển thị; hiển thị màu `#3ECF8E` (green) và dấu ✓

---

## Data Flow

```
App.xaml.cs (startup)
    │
    ├─ NavigationService.RegisterTool("version-increase")
    │
    └─ Resolve MainWindow
           │
           ├─ Subscribe: NavigationService.ToolNavigated → OnToolNavigated()
           │
           └─ Resolve VersionIncreaseViewModel
                  │
                  └─ NavigationService.GetAllTools()
                         │
                         └─ Build NavigationMenuItems (ObservableCollection<ToolMenuItem>)
                                │
                                └─ Bind to NavigationMenuButton.ToolItems (XAML)

User hovers "≡" button
    │
    └─ NavigationMenuButton popup opens
           │
           └─ User clicks tool item
                  │
                  ├─ ToolMenuItem.NavigateCommand.Execute()
                  │      └─ NavigationService.NavigateTo(toolId)
                  │             └─ Raise ToolNavigated event
                  │
                  └─ MainWindow.OnToolNavigated()
                         └─ Swap MainContentControl.Content = new [Tool]View
```

---

## Design Decisions

| Decision | Chosen | Rationale |
|----------|--------|-----------|
| Hover vs Click | **Hover** open | Faster UX, no extra click required |
| Popup close | **Timer 300ms** + animation | Prevents accidental close when moving mouse |
| UserControl vs Custom Control | **UserControl** | Simpler, easier to maintain |
| Navigation event in Application layer | **Event-based** | Decoupled; MainWindow handles view swap |
| Tool registration | **App.xaml.cs startup** | Ensures tools registered before first ViewModel is resolved |
| Active color | **#3ECF8E** (green) | Consistent với app accent color |

---

## Extensibility — Adding a New Tool

Để thêm tool mới vào navigation menu:

**1. Thêm constant** vào `Constants/ToolIds.cs`:
```csharp
public const string BuildDeploy = "build-deploy";
```

**2. Register tool** trong `App.xaml.cs → RegisterNavigationTools()`:
```csharp
nav.RegisterTool(new ToolDefinition
{
    Id = ToolIds.BuildDeploy,
    Name = "Build Deploy Tool",
    Description = "Build and deploy ETL projects"
});
```

**3. Handle navigation** trong `MainWindow.cs → OnToolNavigated()`:
```csharp
case ToolIds.BuildDeploy:
    MainContentControl.Content = new BuildDeployView { DataContext = ... };
    break;
```

**4. Inject INavigationService** vào ViewModel của tool mới và build `NavigationMenuItems` tương tự `VersionIncreaseViewModel`.

---

## Acceptance Criteria Status

| AC | Description | Status |
|----|-------------|--------|
| AC-1 | Navigation button visible trên mọi tool form | ✅ Done |
| AC-2 | Hover dropdown với danh sách tools, current tool highlighted | ✅ Done |
| AC-3 | Click tool → navigate, menu close | ✅ Done |
| AC-4 | Mouse leave / Escape / click outside → menu close | ✅ Done (mouse leave + Escape) |
| AC-5 | Responsive, animation smooth, z-index đúng | ✅ Done |

---

**Document Version**: 1.0.0  
**Created**: 2026-04-07  
**Last Modified**: 2026-04-07
