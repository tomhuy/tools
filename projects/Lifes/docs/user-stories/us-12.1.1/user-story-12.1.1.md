# User Story: US-12.1.1

## Story Information
- **ID**: US-12.1.1
- **Title**: Integrate Monthly Calendar (Electron) with Backend via WebApi
- **Priority**: High
- **Estimate**: 9 hours
- **Sprint**: Next Sprint

## User Story
- **As a** End User / Developer
- **I want to** Monthly Calendar trong Electron fetch dữ liệu Memento/Tag thật từ backend (WebApi) thay vì fake data
- **So that** thao tác CRUD ở US-12.2/12.3/12.4 persist được xuống JSON repository hiện có, và dữ liệu đồng nhất giữa bản WPF và Electron.

## Description
US-12.1 đã hoàn tất render Monthly Calendar trên Electron với fake data trong `MonthlyCalendarService` (xem [be-all-structure.md](docs/structures/be-all-structure.md) mục Feature 12). Backend .NET đã có sẵn:

- `ICalendarService` ([src/Lifes.Core/Interfaces/ICalendarService.cs](src/Lifes.Core/Interfaces/ICalendarService.cs)) — đủ CRUD cho Memento & Tag.
- `CalendarService` + `JsonMementoRepository` + `JsonTagRepository` ở Infrastructure — persistence qua JSON files tại `[AppPath]/database/`.

US-12.1.1 **chỉ bridge 2 lớp**: expose `ICalendarService` qua WebApi REST controller + tạo Angular API service consume endpoints + wire vào `MonthlyCalendarService` (giữ signal contract từ US-12.1).

## Scope
- **Backend**: Thêm `CalendarController` vào `Lifes.Presentation.WebApi` exposing toàn bộ `ICalendarService` methods dưới dạng REST.
- **Frontend**: Thêm `CalendarApiService` (HttpClient wrapper) trong `Lifes.Presentation.Electron`.
- **Frontend**: Refactor `MonthlyCalendarService` — thay seed fake data bằng load từ API khi init; các mutation stubs (`addChild/updateMemento/deleteMemento` + `addTopic/updateTopic/deleteTopic` + tag CRUD) gọi API rồi update signal immutably.
- **Giữ nguyên** re-render contract từ US-12.1: mutation tạo object mới → signal trigger → `@for track id` chỉ patch DOM thay đổi.

**KHÔNG trong scope:**
- UI cho CRUD (Topic Editor, Tag Management, Memento Management) → vẫn là US-12.2/12.3/12.4.
- Authentication / authorization.
- Pagination / caching layer.
- Realtime sync giữa WPF và Electron đang chạy song song.

## Acceptance Criteria

1. **Backend — CalendarController**: `Lifes.Presentation.WebApi` có controller `CalendarController` với các endpoint:
   - `GET  /api/calendar/mementos?year={y}&month={m?}&tagIds={csv?}&parentOnly={bool?}&includeChildren={bool?}` → list Mementos.
   - `GET  /api/calendar/tags` → list Tags.
   - `POST /api/calendar/mementos` → save (create/update theo id) Memento.
   - `DELETE /api/calendar/mementos/{id}` → delete Memento.
   - `POST /api/calendar/tags` → save Tag.
   - `DELETE /api/calendar/tags/{id}` → delete Tag.

2. **Backend — ApiResponse Envelope (Result Pattern + HTTP status)**: MỌI response (success lẫn error) được wrap trong envelope `ApiResponse<T>`:
   ```json
   { "success": true,  "data": <T>, "error": null }
   { "success": false, "data": null, "error": "message" }
   ```
   **Đồng thời** vẫn set HTTP status code đúng ngữ nghĩa:
   - `200 OK` + `{success:true, data:...}` cho GET/POST success.
   - `204 NoContent` + `{success:true, data:null}` cho DELETE success.
   - `400 BadRequest` + `{success:false, error:...}` cho validation fail.
   - `404 NotFound` + `{success:false, error:...}` cho resource không tồn tại.
   - `500 InternalServerError` + `{success:false, error:...}` cho exception server-side.

   Lý do giữ cả 2: HTTP status cho tooling/monitoring/interceptor; envelope cho app-level error message đồng nhất, không phụ thuộc shape error của framework.

3. **Backend — DI Registration**: `CalendarController` được DI resolve `ICalendarService` qua DI container đã cấu hình (không thêm registration trừ khi chưa có).

4. **Backend — CORS**: Endpoints truy cập được từ Electron (`http://localhost:4200` dev và `file://` production) — kiểm tra CORS config hiện tại, mở rộng nếu cần.

5. **Frontend — CalendarApiService**: File `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/calendar-api.service.ts` wrap HttpClient, expose methods khớp 1-1 với controller: `getMementos(query)`, `getTags()`, `saveMemento(m)`, `deleteMemento(id)`, `saveTag(t)`, `deleteTag(id)`. Return Observable.

6. **Frontend — MonthlyCalendarService refactor**:
   - Load initial data qua API khi service khởi tạo (lần đầu component mount).
   - Mỗi mutation (`addTopic`, `updateMemento`, `deleteMemento`, `addChild`, tag CRUD) gọi API trước, khi server trả 200 mới update signal immutably.
   - Có 2 signal phụ: `isLoading = signal(false)`, `lastError = signal<string | null>(null)` để UI (trong US sau) hiển thị state.
   - Re-render contract US-12.1 vẫn giữ: mutation tạo object/array mới, `@for track id` chỉ patch row thay đổi.

7. **Error handling**:
   - API failure → set `lastError` signal + log qua `console.error`. KHÔNG crash component.
   - Load failure ban đầu → `mementos` signal vẫn là `[]`, UI hiển thị empty state.
   - Mutation failure → KHÔNG update signal (để signal đúng với server state); caller (UI ở US sau) đọc `lastError` để show toast.

8. **Data Model Parity**: TypeScript `Memento` / `Tag` interfaces (từ US-12.1) khớp JSON payload mà controller serialize từ `MementoModel` / `TagModel` (camelCase bởi default ASP.NET Core JSON serializer). Nếu lệch, chỉnh interface TS — KHÔNG đổi C# model.

9. **Manual Test E2E**:
   - Chạy WebApi + Electron → mở Monthly Calendar → verify data render từ JSON repo (đang chứa seed data của WPF).
   - Dev-console gọi `service.addChild(...)` → verify API được hit (Network tab) → reload trang → data vẫn còn.
   - Tắt backend → mở Monthly Calendar → verify `lastError` signal set, UI không crash.

10. **Angular Rules Compliance**: `inject()`, Signals, no `any`, kebab-case (xem [angular_rule.md](docs/guidelines/angular_rule.md)).

## Technical Design

### Clean Architecture Layers

- **Presentation (WebApi)** — `src/Lifes.Presentation.WebApi/Controllers/`:
  - `CalendarController.cs` (NEW) — inject `ICalendarService`, map REST to service methods.
- **Presentation (Electron/Angular)** — `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/`:
  - `calendar-api.service.ts` (NEW) — HttpClient wrapper.
  - `monthly-calendar.service.ts` (MODIFY) — thay fake data bằng API calls.
- **Application / Infrastructure / Domain / Core**: KHÔNG đổi. Dùng sẵn `ICalendarService` + `CalendarService` + repositories.

### ApiResponse Envelope (WebApi-layer DTO)

```csharp
// Lifes.Presentation.WebApi/Models/ApiResponse.cs
public sealed class ApiResponse<T>
{
    public bool    Success { get; init; }
    public T?      Data    { get; init; }
    public string? Error   { get; init; }

    public static ApiResponse<T> Ok(T data)           => new() { Success = true,  Data = data };
    public static ApiResponse<T> Fail(string message) => new() { Success = false, Error = message };
}
```

**Vị trí**: Đặt ở `Lifes.Presentation.WebApi/Models/` — KHÔNG để ở Core vì:
- Core đã có `Result<T>` cho internal service results (domain-level Result pattern). `ApiResponse<T>` là transport DTO wrap nó — khác ngữ cảnh.
- Envelope gắn với HTTP semantics (success ↔ status code mapping) → thuộc tầng Presentation/transport.
- WPF không consume envelope này (WPF dùng `Result<T>` trực tiếp qua Command pattern).
- Nếu sau này có transport khác (gRPC/IPC/CLI) cần envelope tương tự, lúc đó mới extract — KISS/YAGNI.

### Endpoint Design (Backend)

```csharp
[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _svc;
    public CalendarController(ICalendarService svc) => _svc = svc;

    [HttpGet("mementos")]
    public async Task<ActionResult<ApiResponse<IEnumerable<MementoModel>>>> GetMementos(
        [FromQuery] int year,
        [FromQuery] int? month,
        [FromQuery] string? tagIds,       // csv
        [FromQuery] bool parentOnly = false,
        [FromQuery] bool includeChildren = false)
    {
        try
        {
            var query = new MementoQueryModel {
                Year = year,
                Month = month,
                TagIds = tagIds?.Split(',').Select(int.Parse).ToList(),
                ParentOnly = parentOnly
            };
            var data = await _svc.GetMementosAsync(query, includeChildren);
            return Ok(ApiResponse<IEnumerable<MementoModel>>.Ok(data));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<IEnumerable<MementoModel>>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<MementoModel>>.Fail(ex.Message));
        }
    }

    [HttpGet("tags")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TagModel>>>> GetTags()
    {
        try
        {
            var data = await _svc.GetTagsAsync();
            return Ok(ApiResponse<IEnumerable<TagModel>>.Ok(data));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<TagModel>>.Fail(ex.Message));
        }
    }

    [HttpPost("mementos")]
    public async Task<ActionResult<ApiResponse<MementoModel>>> SaveMemento([FromBody] MementoModel m)
    {
        try
        {
            await _svc.SaveMementoAsync(m);
            return Ok(ApiResponse<MementoModel>.Ok(m));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<MementoModel>.Fail(ex.Message));
        }
    }

    [HttpDelete("mementos/{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMemento(int id)
    {
        try
        {
            await _svc.DeleteMementoAsync(id);
            return StatusCode(204, ApiResponse<object>.Ok(null!));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Fail(ex.Message));
        }
    }

    // POST /tags, DELETE /tags/{id} tương tự
}
```

**Note**: Có thể refactor try/catch boilerplate thành `ExceptionFilter` hoặc extension method `ToApiResponse<T>()` sau — scope US này giữ đơn giản với try/catch inline.

### Frontend Wiring (Angular Service)

TS cũng định nghĩa `ApiResponse<T>` khớp envelope backend, và `CalendarApiService` unwrap ở operator `map`:

```ts
// api-response.model.ts
export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  error: string | null;
}

// calendar-api.service.ts
@Injectable({ providedIn: 'root' })
export class CalendarApiService {
  private readonly http = inject(HttpClient);
  private readonly base = `${API_BASE_URL}/calendar`;

  getMementos(q: MementoQuery): Observable<Memento[]> {
    return this.http.get<ApiResponse<Memento[]>>(`${this.base}/mementos`, { params: toParams(q) })
      .pipe(map(r => this.unwrap(r)));
  }

  getTags(): Observable<Tag[]> {
    return this.http.get<ApiResponse<Tag[]>>(`${this.base}/tags`)
      .pipe(map(r => this.unwrap(r)));
  }

  saveMemento(m: Memento): Observable<Memento> {
    return this.http.post<ApiResponse<Memento>>(`${this.base}/mementos`, m)
      .pipe(map(r => this.unwrap(r)));
  }

  deleteMemento(id: number): Observable<void> {
    return this.http.delete<ApiResponse<null>>(`${this.base}/mementos/${id}`)
      .pipe(map(r => { this.unwrap(r); }));
  }

  private unwrap<T>(r: ApiResponse<T>): T {
    if (!r.success) throw new Error(r.error ?? 'Unknown API error');
    return r.data as T;
  }
}
```

Alternative: dùng `HttpInterceptor` để unwrap global — cho phép tất cả service chỉ khai `Observable<T>` thay vì `Observable<ApiResponse<T>>`. Quyết định lúc implement; nếu chỉ có 1 controller dùng envelope thì inline `unwrap` như trên là đủ.

// monthly-calendar.service.ts (refactor)
@Injectable({ providedIn: 'root' })
export class MonthlyCalendarService {
  private readonly api = inject(CalendarApiService);

  readonly mementos = signal<Memento[]>([]);
  readonly tags     = signal<Tag[]>([]);
  readonly isLoading = signal(false);
  readonly lastError = signal<string | null>(null);

  loadInitial(year: number) {
    this.isLoading.set(true);
    forkJoin({
      mementos: this.api.getMementos({ year, includeChildren: true }),
      tags: this.api.getTags()
    }).subscribe({
      next: ({ mementos, tags }) => {
        this.mementos.set(mementos);
        this.tags.set(tags);
        this.isLoading.set(false);
      },
      error: err => {
        this.lastError.set(String(err?.message ?? err));
        this.isLoading.set(false);
      }
    });
  }

  updateMemento(m: Memento) {
    this.api.saveMemento(m).subscribe({
      next: saved => this.mementos.update(l => l.map(x => x.id === saved.id ? saved : x)),
      error: err => this.lastError.set(String(err?.message ?? err))
    });
  }
  // addChild, deleteMemento, tag CRUD tương tự
}
```

### Data Model Mapping

| C# `MementoModel` | TS `Memento` | Notes |
|-------------------|--------------|-------|
| `int Id` | `id: number` | |
| `int? ParentId` | `parentId: number \| null` | |
| `string Title` | `title: string` | |
| `DateTime StartDate` | `startDate: string` | ISO 8601 string sau JSON serialize |
| `DateTime EndDate` | `endDate: string` | Parse tại Angular khi cần Date |
| `List<int> TagIds` | `tagIds: number[]` | |
| `string Color` | `color: string` | |
| `int Order` | `order: number` | |

Nếu `CalendarEventModel` vẫn được dùng (có `Phases`), map tương tự. Xác nhận lại shape khi implement (đọc `MementoModel.cs`).

### Files to Create
- [ ] `src/Lifes.Presentation.WebApi/Models/ApiResponse.cs` — envelope DTO (WebApi-layer).
- [ ] `src/Lifes.Presentation.WebApi/Controllers/CalendarController.cs`
- [ ] `src/Lifes.Presentation.Electron/src/app/models/api-response.model.ts` — TS interface khớp envelope.
- [ ] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/calendar-api.service.ts`

### Files to Modify
- [ ] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-calendar.service.ts` — refactor load + mutations qua API.
- [ ] `src/Lifes.Presentation.WebApi/Program.cs` — verify CORS + DI cho `ICalendarService` (thêm nếu chưa registered).
- [ ] `src/Lifes.Presentation.Electron/src/app/features/monthly-calendar/monthly-calendar-page/monthly-calendar-page.component.ts` — gọi `service.loadInitial(currentYear)` trong `ngOnInit` hoặc effect.

## Tasks Breakdown

- [ ] Task 0: Tạo `ApiResponse<T>` ở `Lifes.Presentation.WebApi/Models/ApiResponse.cs` + TS mirror `api-response.model.ts`.
- [ ] Task 1: Verify `ICalendarService` đã được DI register trong `Lifes.Presentation.WebApi/Program.cs`; thêm nếu thiếu.
- [ ] Task 2: Tạo `CalendarController` với 6 endpoints (GET mementos, GET tags, POST memento, DELETE memento, POST tag, DELETE tag), mỗi endpoint wrap `ApiResponse<T>` + set HTTP status đúng.
- [ ] Task 3: Test endpoints bằng Postman/curl — verify: (a) JSON shape đúng envelope `{success, data, error}`; (b) HTTP status đúng ngữ nghĩa.
- [ ] Task 4: Verify CORS cho phép origin Electron.
- [ ] Task 5: Tạo `CalendarApiService` (Angular HttpClient wrapper) với logic `unwrap<T>()` kiểm tra `success` + throw error nếu fail.
- [ ] Task 6: Refactor `MonthlyCalendarService`: thêm `isLoading`, `lastError` signals; thay fake seed bằng `loadInitial()` gọi API.
- [ ] Task 7: Refactor mutation methods (`addChild`, `updateMemento`, `deleteMemento`, `addTopic`, `updateTopic`, `deleteTopic`, `addTag`, `updateTag`, `deleteTag`) — gọi API rồi update signal immutably khi thành công.
- [ ] Task 8: Wire `loadInitial(currentYear)` trong `MonthlyCalendarPageComponent` init.
- [ ] Task 9: Manual E2E test: start WebApi + Electron → verify render từ backend + CRUD persist sau reload.
- [ ] Task 10: Test error path: tắt backend → UI không crash, `lastError` set.
- [ ] Task 11: TS interface parity check — verify JSON shape khớp `Memento` / `Tag` interfaces (sau khi unwrap khỏi envelope).
- [ ] Task 12: Test error path đầy đủ: (a) server throw → frontend nhận `success:false` + set `lastError`; (b) server 404 → frontend vẫn unwrap envelope đúng, không bị interceptor eat mất body.

## Dependencies
- **Depends on**: US-12.1 (service shape + signals + immutable contract), US-11.1 (WebApi project), US-11.3 (optional — nếu đã có build script auto-run backend thì test E2E dễ hơn).
- **Blocks**: US-12.2, US-12.3, US-12.4 — các US này dùng service methods đã wire API ở đây, KHÔNG phải tự add endpoint.

## Risks & Notes
- **JSON case sensitivity**: ASP.NET Core mặc định camelCase; verify trong Program.cs không set `PropertyNamingPolicy = null`. Nếu có, Angular interfaces phải dùng PascalCase — cần align.
- **DateTime serialization**: Backend serialize `DateTime` thành ISO string. Angular dùng string, parse `new Date(startDate)` khi cần so sánh ngày.
- **Concurrent edit WPF + Electron**: Nếu cả 2 app chạy cùng lúc ghi cùng file JSON → race condition. Document as known limitation; handle ở US sau nếu cần.
- **Port conflict**: Xác nhận port WebApi (hiện `5110` theo [api.service.ts](src/Lifes.Presentation.Electron/src/app/api.service.ts)). Giữ nguyên cho consistency.

## Definition of Done
- [ ] `ApiResponse<T>` envelope được implement + dùng cho TẤT CẢ response (success + error).
- [ ] HTTP status code đúng ngữ nghĩa (200/204/400/404/500) tương ứng với `success` flag.
- [ ] `CalendarController` hoàn chỉnh với 6 endpoints.
- [ ] CORS pass cho Electron origin.
- [ ] `CalendarApiService` wrap đủ endpoints.
- [ ] `MonthlyCalendarService` load từ API, mutations persist qua API.
- [ ] `isLoading` + `lastError` signals hoạt động.
- [ ] Re-render contract US-12.1 vẫn giữ (verify: mutation vẫn tạo object mới, signal trigger chỉ patch row thay đổi).
- [ ] Manual E2E: reload giữ được CRUD changes.
- [ ] Error path: tắt backend không crash UI.
- [ ] Code tuân thủ `angular_rule.md`.
- [ ] User review & approve.
## Final Status
- **Status**: ✅ Completed
- **Completed Date**: 2026-04-25
- **Approved By**: bmhuy
