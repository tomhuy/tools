# Coding Rules & Standards
## Everyday Web - Windows Forms Application

Tài liệu này định nghĩa các quy tắc và tiêu chuẩn code cho dự án Everyday Web.

---

## 📋 Mục lục
1. [Kiến trúc và Layered Architecture](#kiến-trúc-và-layered-architecture)
2. [SOLID Principles](#solid-principles)
3. [Clean Code Standards](#clean-code-standards)
4. [Naming Conventions](#naming-conventions)
5. [File Organization](#file-organization)
6. [Dependency Rules](#dependency-rules)
7. [Code Structure Rules](#code-structure-rules)
8. [Async/Await Patterns](#asyncawait-patterns)
9. [Error Handling](#error-handling)
10. [UI Development Rules](#ui-development-rules)

---

## 🏗️ Kiến trúc và Layered Architecture

### ✅ Layered Architecture Structure

Dự án tuân thủ **Layered Architecture** với 5 layers:

```
EverydayWeb (UI Layer)
    ↓ depends on
EverydayWeb.Application (Business Logic)
    ↓ depends on
EverydayWeb.Domain (Contracts/Interfaces)
    ↓ depends on
EverydayWeb.Infrastructure (Implementations)
EverydayWeb.DTOs (Data Transfer Objects)
```

### ✅ Layer Responsibilities

#### 1. **Domain Layer** (`EverydayWeb.Domain`)
**Mục đích**: Core business entities và contracts

**Nên chứa**:
- ✅ Domain entities (nếu có)
- ✅ Repository interfaces (`IRepository<T>`, `IPostRepository`, `ISyncDataRepository`)
- ✅ Domain interfaces (`IGraphQLClient` - abstraction cho external service)
- ✅ Domain value objects

**Không nên chứa**:
- ❌ Implementations
- ❌ Business logic
- ❌ Dependencies vào Infrastructure hoặc Application

#### 2. **Application Layer** (`EverydayWeb.Application`)
**Mục đích**: Business logic, use cases, application services

**Nên chứa**:
- ✅ Application services (`SyncService`, `GraphQLScraperService`)
- ✅ Business logic và use cases
- ✅ Application-specific configurations
- ✅ Abstractions cho business logic (`IGraphQLQueryConfig`, `ISyncService`)
- ✅ Business rules và mappings
- ✅ DTOs transformations

**Không nên chứa**:
- ❌ Technical implementations (HTTP, database, file system)
- ❌ UI components
- ❌ Infrastructure-specific code

#### 3. **Infrastructure Layer** (`EverydayWeb.Infrastructure`)
**Mục đích**: Technical implementations, external dependencies

**Nên chứa**:
- ✅ Implementations của Domain interfaces (`GraphQLClient`, `PostRepository`)
- ✅ External service integrations (HTTP clients, APIs)
- ✅ Database access implementations
- ✅ File system operations
- ✅ Third-party library wrappers
- ✅ Technical utilities

**Không nên chứa**:
- ❌ Business logic
- ❌ Business abstractions
- ❌ Application services
- ❌ Domain entities

#### 4. **DTOs Layer** (`EverydayWeb.DTOs`)
**Mục đích**: Data transfer objects

**Nên chứa**:
- ✅ DTOs cho data transfer giữa các layers
- ✅ Simple data structures

**Không nên chứa**:
- ❌ Business logic
- ❌ Dependencies vào các layers khác

#### 5. **UI Layer** (`EverydayWeb`)
**Mục đích**: Windows Forms UI

**Nên chứa**:
- ✅ Forms (`MainForm`, `SettingsForm`, `PostsViewForm`)
- ✅ Custom Controls (`PostCardControl`)
- ✅ UI-specific logic
- ✅ Logging sinks (nếu cần Windows Forms references)

**Không nên chứa**:
- ❌ Business logic
- ❌ Data access code
- ❌ External service implementations

---

## 🔗 Dependency Rules

### ✅ Allowed Dependencies

```
UI → Application ✅
UI → Domain ✅
UI → DTOs ✅
Application → Domain ✅
Application → Infrastructure ✅ (để sử dụng implementations)
Infrastructure → Domain ✅ (để implement interfaces)
```

### ❌ Forbidden Dependencies

```
Domain → Application ❌
Domain → Infrastructure ❌
Infrastructure → Application ❌
Application → UI ❌
```

### 📐 Dependency Flow Diagram

```
┌─────────────────────────────────────────┐
│         EverydayWeb (UI)                │
│  - Forms, Controls, UI Logic           │
└──────────────┬──────────────────────────┘
               │ depends on
               ▼
┌─────────────────────────────────────────┐
│      EverydayWeb.Application            │
│  - Business Logic                       │
│  - Application Services                 │
│  - IGraphQLQueryConfig                  │
└──────┬──────────────────────┬──────────┘
       │ depends on            │ depends on
       ▼                      ▼
┌──────────────┐    ┌──────────────────────┐
│   Domain     │    │   Infrastructure     │
│  - Entities  │    │  - Implementations   │
│  - Interfaces│    │  - GraphQLClient     │
└──────────────┘    │  - Repositories      │
                    └──────────────────────┘
```

---

## 🎯 SOLID Principles

### 1. **Single Responsibility Principle (SRP)**
- Mỗi class chỉ có một lý do để thay đổi
- Mỗi method chỉ làm một việc
- Functions ngắn (10-20 dòng), single responsibility

**Ví dụ**:
- `SyncService`: Chỉ chịu trách nhiệm sync data
- `PostRepository`: Chỉ chịu trách nhiệm data access cho posts
- `GraphQLClient`: Chỉ chịu trách nhiệm gọi GraphQL API

### 2. **Open/Closed Principle (OCP)**
- Open for extension, closed for modification
- Sử dụng interfaces và base classes để mở rộng

**Ví dụ**:
- `IGraphQLQueryConfig`: Có thể thêm query config mới mà không sửa code cũ
- `IPostQueryProvider`: Có thể thêm provider mới mà không sửa registry

### 3. **Liskov Substitution Principle (LSP)**
- Derived classes phải có thể thay thế base classes
- Implementations phải tuân thủ contract của interface

**Ví dụ**:
- Tất cả `IGraphQLQueryConfig` implementations có thể thay thế cho nhau
- Tất cả `IPostQueryProvider` implementations có thể thay thế cho nhau

### 4. **Interface Segregation Principle (ISP)**
- Clients không nên phụ thuộc vào interfaces mà họ không sử dụng
- Tách interfaces thành các interfaces nhỏ hơn, cụ thể hơn

**Ví dụ**:
- `IRepository<T>`: Generic interface cho basic CRUD
- `IPostRepository`: Specific interface cho posts với methods riêng
- `ISyncDataRepository`: Specific interface cho sync data

### 5. **Dependency Inversion Principle (DIP)**
- High-level modules không nên phụ thuộc vào low-level modules
- Cả hai nên phụ thuộc vào abstractions
- Sử dụng Dependency Injection

**Ví dụ**:
- `SyncService` phụ thuộc vào `IPostRepository` (interface), không phụ thuộc vào `PostRepository` (implementation)
- Tất cả dependencies được inject qua constructor

---

## 📝 Clean Code Standards

### ✅ Function/Method Rules

1. **Function Length**: 
   - Functions nên ngắn (10-20 dòng)
   - Nếu function dài hơn 30 dòng, nên refactor thành các functions nhỏ hơn

2. **Single Responsibility**:
   - Mỗi function chỉ làm một việc
   - Function name phải mô tả chính xác những gì function làm

3. **Parameters**:
   - Tối đa 3-4 parameters
   - Nếu nhiều hơn, nên sử dụng DTO hoặc configuration object

4. **Return Types**:
   - Luôn return meaningful values
   - Sử dụng `Task<T>` cho async methods
   - Tránh return `void` nếu có thể return meaningful value

### ✅ Code Organization

1. **Using Statements**:
   - Group using statements theo namespace
   - Remove unused using statements
   - Không sử dụng redundant prefixes (ví dụ: `GraphQL.DailyDevFeedQueryConfig` khi đã có `using EverydayWeb.Application.Services.GraphQL;`)

2. **Fields và Properties**:
   - Private fields: `_camelCase` (ví dụ: `_syncService`, `_postRepository`)
   - Public properties: `PascalCase` (ví dụ: `IsEnabled`, `Count`)
   - Read-only fields: `readonly` keyword

3. **Constants**:
   - Sử dụng `const` cho compile-time constants
   - Sử dụng `static readonly` cho runtime constants
   - Naming: `PascalCase` (ví dụ: `MaxRetryCount`, `DefaultTimeout`)

4. **Magic Strings**:
   - Hạn chế magic strings trong trường hợp nhiều nơi sử dụng cùng một giá trị
   - Nếu giá trị được sử dụng ở nhiều nơi (3+ nơi), nên tạo constant để dễ quản lý và tránh typo
   - Nếu giá trị chỉ xuất hiện ít (1-2 nơi) và giữ được tính dễ đọc, dễ hiểu thì không cần tạo constant
   - Tổ chức constants vào các class static trong folder `Constants/` (ví dụ: `QuerySourceTypes`, `PostQueryProviderNames`)
   - **Constants hiện có**:
     - `QuerySourceTypes`: Constants cho query source types (DailyDevFeed, PinterestBoard, Obsidian, etc.)
     - `PostQueryProviderNames`: Constants cho post query provider names (AllPosts, RecentPosts, FavoritePosts, BlockedPosts, etc.)
   - Ví dụ:
     ```csharp
     // ❌ Bad - Magic strings được sử dụng ở nhiều nơi
     if (queryType == "PinterestBoard") { }
     post.QuerySourceType = "PinterestBoard";
     registry.Register(new BySourceTypeQueryProvider(postRepository, "PinterestBoard"));

     // ✅ Good - Sử dụng constants
     if (queryType == QuerySourceTypes.PinterestBoard) { }
     post.QuerySourceType = QuerySourceTypes.PinterestBoard;
     registry.Register(new BySourceTypeQueryProvider(postRepository, QuerySourceTypes.PinterestBoard));

     // ❌ Bad - Magic strings cho provider names
     if (_currentProvider?.Name == "FavoritePosts") { }
     public override string Name => "AllPosts";

     // ✅ Good - Sử dụng PostQueryProviderNames constants
     if (_currentProvider?.Name == PostQueryProviderNames.FavoritePosts) { }
     public override string Name => PostQueryProviderNames.AllPosts;
     ```

---

## 🏷️ Naming Conventions

### ✅ Classes và Interfaces

- **Classes**: `PascalCase` (ví dụ: `SyncService`, `PostRepository`, `GraphQLClient`)
- **Interfaces**: `IPascalCase` với prefix `I` (ví dụ: `ISyncService`, `IPostRepository`, `IGraphQLClient`)
- **Abstract Classes**: `PascalCase` với suffix `Base` (ví dụ: `GraphQLQueryConfigBase`, `PostQueryProviderBase`)
- **DTOs**: `PascalCase` với suffix `Dto` (ví dụ: `PostDto`, `SyncResultDto`, `SettingsDto`)

### ✅ Methods và Functions

- **Public Methods**: `PascalCase` (ví dụ: `SyncAsync`, `GetAllPostsAsync`, `SaveSettings`)
- **Private Methods**: `PascalCase` (ví dụ: `LoadProviders`, `UpdateProgressBarPosition`)
- **Async Methods**: Suffix `Async` (ví dụ: `SyncAsync`, `LoadImageAsync`)

### ✅ Variables và Fields

- **Private Fields**: `_camelCase` với prefix `_` (ví dụ: `_syncService`, `_postRepository`, `_cancellationTokenSource`)
- **Local Variables**: `camelCase` (ví dụ: `postDto`, `queryConfig`, `cancellationToken`)
- **Parameters**: `camelCase` (ví dụ: `cancellationToken`, `queryConfig`, `postDto`)
- **Constants**: `PascalCase` (ví dụ: `MaxRetryCount`, `DefaultTimeout`)

### ✅ Namespaces

- **Pattern**: `ProjectName.LayerName.[SubFolder]`
- **Ví dụ**:
  - `EverydayWeb.Application.Services`
  - `EverydayWeb.Application.Services.GraphQL`
  - `EverydayWeb.Application.Services.PostQueryProviders`
  - `EverydayWeb.Infrastructure.Repositories`
  - `EverydayWeb.Domain.Repositories`

### ✅ Files và Folders

- **Files**: Match class/interface name (ví dụ: `SyncService.cs` chứa class `SyncService`)
- **Folders**: `PascalCase` (ví dụ: `Services/`, `GraphQL/`, `PostQueryProviders/`)
- **One class per file**: Mỗi file chỉ chứa một class/interface chính

---

## 📁 File Organization

### ✅ Directory Structure Rules

1. **Related Files Grouping**:
   - Files liên quan nên được tổ chức vào cùng một thư mục
   - Sử dụng subdirectories để nhóm các files theo chức năng

2. **Service Organization**:
   ```
   EverydayWeb.Application/Services/
   ├── GraphQL/
   │   ├── IGraphQLQueryConfig.cs
   │   ├── GraphQLQueryConfigBase.cs
   │   ├── DailyDevFeedQueryConfig.cs
   │   └── ...
   ├── PostQueryProviders/
   │   ├── IPostQueryProvider.cs
   │   ├── PostQueryProviderBase.cs
   │   ├── AllPostsQueryProvider.cs
   │   └── ...
   └── SyncService.cs
   ```

3. **UI Organization**:
   ```
   EverydayWeb/
   ├── Controls/
   │   └── PostCardControl.cs
   ├── Views/
   │   └── PostsViewForm.cs
   ├── Logging/
   │   └── ListViewSink.cs
   └── MainForm.cs
   ```

4. **Repository Organization**:
   ```
   EverydayWeb.Infrastructure/
   ├── Repositories/
   │   ├── PostRepository.cs
   │   └── SyncDataRepository.cs
   └── GraphQL/
       └── GraphQLClient.cs
   ```

### ✅ File Naming Rules

- **One class per file**: Mỗi file chỉ chứa một class/interface chính
- **File name = Class name**: File name phải match với class/interface name
- **Namespace = Folder structure**: Namespace phải phản ánh folder structure

### ✅ Database Migration Files

**Naming Convention**: `YYYYMMDD_HHMMSS_Migration_Description.sql`

**Mục đích**:
- Đảm bảo migrations luôn được thực thi theo đúng thứ tự thời gian
- Khi sort theo filename, người chạy migration biết file nào nên chạy trước
- Tránh conflict khi nhiều người tạo migration cùng lúc

**Format**:
- `YYYYMMDD`: Ngày tạo migration (năm 4 chữ số, tháng 2 chữ số, ngày 2 chữ số)
- `HHMMSS`: Thời gian tạo migration (giờ, phút, giây - 24h format)
- `Migration_`: Prefix cố định để dễ nhận biết
- `Description`: Mô tả ngắn gọn migration làm gì (PascalCase)

**Ví dụ**:
```
20251115_110100_Migration_AddFavoritePostFeature.sql
20251115_143000_Migration_AddBlockedPostFeature.sql
20251116_090000_Migration_UpdateTitleSize.sql
20251116_153000_Migration_AddQuerySourceType.sql
```

**Quy tắc bổ sung**:
- Mỗi migration file nên sử dụng `GO` statements để tách các batches
- Migration nên bao gồm các comments mô tả từng batch
- Test migration trước khi commit vào source control
- Không sửa migration đã chạy trên production, tạo migration mới để fix

**Template**:
```sql
-- Migration: [Description]
-- Date: [YYYY-MM-DD]
-- Description: [Chi tiết migration làm gì]

-- Batch 1: [Description]
[SQL statements]
GO

-- Batch 2: [Description]
[SQL statements]
GO
```

---

## 🔄 Async/Await Patterns

### ✅ Async Method Rules

1. **Async Suffix**:
   - Tất cả async methods phải có suffix `Async`
   - Ví dụ: `SyncAsync`, `GetAllPostsAsync`, `LoadImageAsync`

2. **Return Types**:
   - Async methods return `Task` hoặc `Task<T>`
   - Không return `void` cho async methods

3. **Cancellation Tokens**:
   - Tất cả async methods nên accept `CancellationToken` parameter
   - Pass cancellation token xuống các async calls
   - Ví dụ: `public async Task<SyncResultDto> SyncAsync(CancellationToken cancellationToken = default)`

4. **ConfigureAwait**:
   - Sử dụng `ConfigureAwait(false)` trong library code (Application, Infrastructure)
   - Không sử dụng `ConfigureAwait(false)` trong UI code (cần UI thread)

**Ví dụ**:
```csharp
// ✅ Good - Application layer
public async Task<List<PostDto>> GetAllPostsAsync(CancellationToken cancellationToken = default)
{
    var posts = await _repository.GetAllAsync(cancellationToken).ConfigureAwait(false);
    return posts;
}

// ✅ Good - UI layer
private async void OnSyncButtonClick(object sender, EventArgs e)
{
    var result = await _syncService.SyncAsync(_cancellationTokenSource.Token);
    // Update UI on UI thread
}
```

---

## ⚠️ Error Handling

### ✅ Exception Handling Rules

1. **Catch Specific Exceptions**:
   - Catch specific exception types, không catch `Exception` chung chung
   - Ví dụ: `catch (HttpRequestException ex)` thay vì `catch (Exception ex)`

2. **Logging**:
   - Log tất cả exceptions với context
   - Sử dụng Serilog với structured logging
   - Ví dụ: `Log.Error(ex, "Failed to sync data. QueryType: {QueryType}", queryType)`

3. **Error Results**:
   - Return error results thay vì throw exceptions khi có thể
   - Ví dụ: `SyncResultDto` với `IsSuccess`` và `ErrorMessage`

4. **Validation**:
   - Validate inputs trước khi xử lý
   - Throw `ArgumentNullException` hoặc `ArgumentException` cho invalid inputs

**Ví dụ**:
```csharp
// ✅ Good
public async Task<SyncResultDto> SyncAsync(CancellationToken cancellationToken = default)
{
    try
    {
        // Sync logic
        return new SyncResultDto { IsSuccess = true };
    }
    catch (HttpRequestException ex)
    {
        Log.Error(ex, "HTTP error during sync");
        return new SyncResultDto { IsSuccess = false, ErrorMessage = ex.Message };
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Unexpected error during sync");
        return new SyncResultDto { IsSuccess = false, ErrorMessage = "Unexpected error occurred" };
    }
}
```

---

## 🖥️ UI Development Rules

### ✅ Windows Forms Rules

1. **Form Initialization**:
   - Tất cả UI initialization trong `InitializeComponent()` hoặc constructor
   - Sử dụng `SetupUI()` method cho complex UI setup

2. **Event Handlers**:
   - Event handlers nên là private methods với naming: `On[EventName]`
   - Ví dụ: `OnSyncButtonClick`, `OnProviderChanged`, `OnRefreshButtonClick`

3. **UI Thread Updates**:
   - Tất cả UI updates phải trên UI thread
   - Sử dụng `Invoke` hoặc `BeginInvoke` khi update UI từ background thread
   - Không sử dụng `ConfigureAwait(false)` trong UI code

4. **Control Naming**:
   - Private controls: `_camelCase` với prefix `_`
   - Ví dụ: `_syncButton`, `_progressBar`, `_statusLabel`

5. **Disposal**:
   - Implement `IDisposable` cho forms và controls có unmanaged resources
   - Dispose controls trong `Dispose()` method

**Ví dụ**:
```csharp
// ✅ Good
private async void OnSyncButtonClick(object sender, EventArgs e)
{
    _syncButton.Enabled = false;
    try
    {
        var result = await _syncService.SyncAsync(_cancellationTokenSource.Token);
        UpdateUI(result);
    }
    finally
    {
        _syncButton.Enabled = true;
    }
}

private void UpdateUI(SyncResultDto result)
{
    if (InvokeRequired)
    {
        Invoke(new Action<SyncResultDto>(UpdateUI), result);
        return;
    }
    
    _statusLabel.Text = result.IsSuccess ? "Sync completed" : "Sync failed";
}
```

---

## 🎨 Code Style Rules

### ✅ Code Formatting

1. **Indentation**:
   - Sử dụng 4 spaces (không dùng tabs)
   - Consistent indentation trong toàn bộ project

2. **Braces**:
   - Opening brace trên cùng dòng với statement
   - Closing brace trên dòng riêng
   - Ví dụ:
   ```csharp
   if (condition)
   {
       // code
   }
   ```

3. **Spacing**:
   - Space sau keywords (`if`, `for`, `while`, etc.)
   - Space sau commas
   - No space sau opening parenthesis, before closing parenthesis
   - Ví dụ: `if (condition) { }` không phải `if(condition){ }`

4. **Line Length**:
   - Giữ line length dưới 120 characters
   - Break long lines với proper indentation

### ✅ Comments

1. **XML Documentation**:
   - Public APIs nên có XML documentation comments
   - Sử dụng `///` cho XML comments

2. **Inline Comments**:
   - Comments nên giải thích "why", không phải "what"
   - Code nên tự giải thích (self-documenting)

3. **TODO Comments**:
   - Sử dụng `// TODO: description` cho future work
   - Remove TODO comments khi đã implement

**Ví dụ**:
```csharp
/// <summary>
/// Synchronizes posts from GraphQL API to database.
/// </summary>
/// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
/// <returns>Sync result with success status and statistics.</returns>
public async Task<SyncResultDto> SyncAsync(CancellationToken cancellationToken = default)
{
    // Implementation
}
```

---

## 🔍 Code Review Checklist

Khi review code, kiểm tra:

- [ ] Tuân thủ SOLID principles
- [ ] Functions ngắn và có single responsibility
- [ ] Proper error handling và logging
- [ ] Async/await được sử dụng đúng cách
- [ ] Cancellation tokens được pass đúng cách
- [ ] Naming conventions được tuân thủ
- [ ] Dependencies đúng theo dependency rules
- [ ] Code được tổ chức đúng folder structure
- [ ] No redundant code hoặc unused code
- [ ] Proper disposal của resources
- [ ] UI updates trên UI thread
- [ ] Comments giải thích "why" không phải "what"

---

## 📚 References

- **SOLID Principles**: https://en.wikipedia.org/wiki/SOLID
- **Clean Code**: Robert C. Martin
- **.NET Coding Conventions**: https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
- **Async/Await Best Practices**: https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming

---

**Last Updated**: [Current Date]
**Version**: 1.0

