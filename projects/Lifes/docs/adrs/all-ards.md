# Architectural Decision Records (ADR)

## Cấu trúc của 1 ADR
- Ngày ra quyết định: 
- Người viết:
- Vấn đề/Concern/Feature đang cần ra quyết định: 
- Các phương án đã được gợi ý: 
- Lựa chọn và lý do lựa chọn:
    - 
    - 
    - 




## ADR 1: Component Design Pattern for Memento Management (US-12.4)
**Ngày ra quyết định: 2026-04-26**
**Người viết: AI, huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Thiết kế kiến trúc cho `MementoManagementComponent` trong Electron/Angular. Cần xác định cách Component tương tác với `MonthlyCalendarService` và cách phân chia trách nhiệm giữa Page Component và UI Component để đảm bảo tuân thủ Clean Architecture và `design_rule.md`.

**2. Các phương án đã được gợi ý:**
- **Phương án 1 (Integrated/Active Component)**: `MementoManagementComponent` trực tiếp inject `MonthlyCalendarService`, xử lý cả UI và Business Logic (gọi API).
- **Phương án 2 (Decoupled/Passive Component)**: Sử dụng mô hình Container (Smart) và Presenter (Dumb). Page component quản lý service, UI component (Table) chỉ nhận data qua `Input` và bắn sự kiện qua `Output`.

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 2 (Decoupled/Passive Component)**.
- **Lý do**: 
    Huy, 
    lý do là không muốn phụ thuộc vào MonthlyCalendarService, vì nó hiện tại chỉ đang xử lý cho một 1 loại momento và business trong nó cũng khá là nhiều, trong tình huồng sau này, muốn mở rộng nguồn data, không chỉ là thể loại momento (track), mà có thể là momento (task), thì chúng ta không bị limit về design, có thể tái sử dụng lại UI, khác biết là các loại momento nào được display lên thôi

    AI:
    - Tuân thủ chặt chẽ `design_rule.md` (Rule 1).
    - Tách biệt hoàn toàn logic nghiệp vụ và logic hiển thị.
    - Giúp UI component (Table) có thể tái sử dụng và dễ dàng viết Unit Test độc lập.
    - Duy trì sự nhất quán với định hướng "Clean Architecture" của toàn dự án.

## ADR 2: Dedicated Service for Memento Management (Refined Decision)
**Ngày ra quyết định: 2026-04-26**
**Người viết: AI, huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Có nên sử dụng `MonthlyCalendarService` cho `MementoManagementComponent` hay không?

**2. Các phương án đã được gợi ý:**
- **Phương án 1**: Dùng chung `MonthlyCalendarService`.
- **Phương án 2**: Tạo `MementoManagementService` riêng biệt.

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 2 (Dedicated Service)**.
- **Lý do**: 
    - `MonthlyCalendarService` được thiết kế để quản lý dữ liệu theo năm/tháng (Year data) hiện tại đang load trên lịch. Việc dùng chung sẽ gây side-effect khi Management view cần load dữ liệu từ quá khứ hoặc tương lai (vượt ngoài phạm vi năm đang xem trên lịch).
    - Tách biệt service giúp Management view có thể fetch dữ liệu độc lập qua API với các bộ lọc mở rộng (keyword, start date, end date) mà không làm thay đổi trạng thái của Monthly Calendar UI.
    - Tránh phụ thuộc vào logic phức tạp của `MonthlyCalendarService` vốn đang xử lý rất nhiều business liên quan đến hiển thị Grid/Gantt.
    - Cho phép mở rộng nguồn data linh hoạt hơn trong tương lai (ví dụ: hiển thị cả task memento).


## ADR 3: Calendar controller & api get memento
**Ngày ra quyết định: 2026-04-26**
**Người viết: Huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Có nên tách ra 1 api riêng để cho manage momento sử dụng không hay là sử dụng trên api hiện tại và thêm 2 field mới là startdate và enddate

**2. Các phương án đã được gợi ý:**
- **Phương án 1**: tách ra 1 api khác
- **Phương án 2**: sử dụng api hiện tại và thêm 2 field mới là startdate và enddate và logic như sau:
    - nếu có startdate và enddate thì ưu tiên
    - sau đó mới tính tới month và year



tại sao lại cần concern chuyên này
- vì nguyên tắc trong đầu đang suy nghĩ là: "các module phải sâu"
- nhưng nếu chính sửa lên api hiện tại, có phải đang khiến nó trở nên quá phức tạp hay không, nếu nó qua dynamic  thì làm sao sau này quản lý được sự complex của nó và khi có refactor thì chúng ta vẫn biết chắc rằng nó work
- hiên tại chỉ có 2 page được gợi đến nó sau này sẽ nhiều nữa.
- vẫn concern nhất là về mặc giảm thiếu sự phức tạp thì sự phức tạp khi sử dụng api nên được đóng gói ở BE hay FE

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 2 (Dedicated Service)**.
- **Lý do**: 
    - Các module phải sâu
    - logic đang vẫn ở cấp độ đơn giản và dễ hiểu
    - FE sẽ có nhiều lựa chọn hơn
 
## ADR 4: Ghost Memento Visualization for Tag Filtering (US-12.5)
**Ngày ra quyết định: 2026-04-26**
**Người viết: AI, huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Làm thế nào để người dùng vẫn thấy được "tầm vóc thời gian" của một Topic khi đang lọc theo Tag mà không gây nhầm lẫn với các phase con (Ghost Memento). Ban đầu tính năng này phụ thuộc vào việc ẩn phase con (`!includeChildren`), nhưng cần một cách tiếp cận linh hoạt hơn.

**2. Các phương án đã được gợi ý:**
- **Phương án 1 (Implicit)**: Tự động hiển thị Ghost Bar khi người dùng tắt "Include children".
- **Phương án 2 (Explicit/Decoupled)**: Tách biệt hoàn toàn tính năng này thành một checkbox riêng biệt tên là **"Show timeline"**. Ghost Bar sẽ chỉ hiển thị khi người dùng chủ động chọn, không phụ thuộc vào trạng thái của "Include children".

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 2 (Explicit/Decoupled - Show timeline)**.
- **Lý do**: 
    - **Linh hoạt**: Cho phép người dùng kết hợp linh hoạt (ví dụ: vừa hiện timeline tổng quát làm nền, vừa hiện phase con chi tiết).
    - **Dễ hiểu**: Tên gọi "Show timeline" phản ánh đúng bản chất thị giác hơn là việc suy luận từ trạng thái ẩn phase con.
    - **UX**: Người dùng có quyền kiểm soát tuyệt đối giao diện lưới (grid) mà không bị ràng buộc bởi logic tải dữ liệu (recursive loading).
## ADR 5: Visualization Engine & Multi-row Layout for Data Analysis (US-14.2)
**Ngày ra quyết định: 2026-04-26**
**Người viết: AI, huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Lựa chọn công nghệ vẽ biểu đồ (Visualization Engine) và thiết kế Layout cho tính năng phân tích dữ liệu chuyên sâu. Cần đảm bảo tính thẩm mỹ "Premium", hiệu suất cao và khả năng mở rộng trong tương lai.

**2. Các phương án đã được gợi ý:**
- **Phương án 1 (Native SVG + Angular Templates)**: Sử dụng trực tiếp thẻ `<svg>` trong Angular template, binding dữ liệu thông qua logic tính toán tọa độ đơn giản.
- **Phương án 2 (D3.js Library)**: Sử dụng thư viện D3.js chuyên dụng để quản lý DOM SVG, toán học (Scales) và transitions.
- **Phương án 3 (Hybrid Approach)**: Triển khai engine chính bằng Native SVG để tối ưu hiệu suất và dễ styling bằng CSS, đồng thời tích hợp D3.js như một engine bổ trợ cho các phân tích toán học phức tạp.

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 3 (Hybrid Approach - Native SVG with D3 Sample)**.
- **Lý do**: 
    - **Thẩm mỹ Premium**: Native SVG cho phép tận dụng tối đa CSS và Angular template binding để đạt được giao diện pixel-perfect y hệt bản thiết kế mẫu (sample_chart.svg).
    - **Hiệu suất**: Tránh việc D3.js can thiệp trực tiếp vào DOM của Angular (giảm xung đột Change Detection).
    - **Tính linh hoạt (Multi-row Layout)**: Quyết định sử dụng **Stacked Multi-row Layout** (Events -> Emotions -> Sleep) giúp quan sát được mối tương quan giữa các chiều dữ liệu khác nhau trên cùng một trục thời gian.
    - **Scale & Math**: D3.js vẫn được giữ lại trong codebase như một "Alternative Engine" để xử lý các biểu đồ yêu cầu toán học cao hơn trong tương lai (ví dụ: Zooming, Complex interpolations).
## ADR 6: Naming Convention for Achievement Filtering (US-15.1)
**Ngày ra quyết định: 2026-04-26**
**Người viết: AI, huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Lựa chọn tên gọi cho tham số query lọc các Topic đã hoàn thành. Ban đầu field dữ liệu là `IsAchieved`, nhưng khi đưa vào Query Model để làm filter thì cần một tên gọi phản ánh đúng hành vi người dùng trên UI.

**2. Các phương án đã được gợi ý:**
- **Phương án 1 (`isAchieved`)**: Giữ nguyên tên field từ database. Giá trị `false` nghĩa là ẩn, `true` nghĩa là hiện, hoặc lọc chính xác.
- **Phương án 2 (`showAchieved`)**: Sử dụng tiền tố `show` để biểu thị đây là một tùy chọn hiển thị.

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 2 (`showAchieved`)**.
- **Lý do**: 
    - **Tính trực quan**: Trên UI, người dùng đang tương tác với một toggle "Show completed". Việc ánh xạ trực tiếp sang `showAchieved` giúp logic ở cả Frontend và Backend trở nên cực kỳ dễ đọc.
    - **Logic mặc định**: Hệ thống yêu cầu "mặc định ẩn các topic đã hoàn thành". Với `showAchieved`, chúng ta chỉ cần kiểm tra `showAchieved == true` để quyết định có bao gồm dữ liệu cũ hay không. Nếu không được truyền (null/false), hệ thống sẽ tự động lọc bỏ (`!m.IsAchieved`).
    - **Ngữ nghĩa**: `isAchieved` ám chỉ một trạng thái cố định của object, trong khi `showAchieved` ám chỉ một hành động lọc/hiển thị của hệ thống.

## ADR 7: Local State Management vs. Event-driven Reload (US-15.1)
**Ngày ra quyết định: 2026-04-26**
**Người viết: AI, huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Khi một Topic được cập nhật trạng thái `isAchieved`, làm thế nào để đồng bộ giao diện (ẩn/hiện) một cách tối ưu nhất. Có nên sử dụng cơ chế "Bắn Event -> Reload toàn bộ từ Server" hay "Tự quản lý và lọc dữ liệu tại Local State"?

**2. Các phương án đã được gợi ý:**
- **Phương án 1 (Event-driven Reload)**: Sử dụng `Subject` (ví dụ `mementoChanged$`) để báo hiệu cho Component biết cần gọi lại API `loadMementos()`.
- **Phương án 2 (Local State Management - Option B)**: Service tự chịu trách nhiệm cập nhật hoặc loại bỏ item khỏi Signal `mementos` dựa trên giá trị mới và bộ lọc hiện tại.

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 2 (Local State Management)**.
- **Lý do**: 
    - **Trách nhiệm của Service**: Các Service nên làm tốt công việc của mình. Khi có sự kiện thay đổi dữ liệu xảy ra, Service nên tự quản lý tốt state mà nó đang nắm giữ vì nó đang cung cấp dữ liệu đó cho nhiều bên ngoài sử dụng. Việc Service tự "làm sạch" dữ liệu của mình giúp các component sử dụng nó luôn nhận được data chuẩn mà không cần thêm logic phụ trợ.
    - **Tối ưu hóa hiệu năng**: Việc sử dụng Subject để emit ra bên ngoài và kích hoạt reload toàn bộ danh sách là quá tốn kém về performance (HTTP request, re-calculate computed signals, re-render DOM) và không cần thiết khi chúng ta đã có sẵn dữ liệu mới trong tay sau khi Save thành công.
    - **UX**: Mang lại trải nghiệm mượt mà (snappy), dữ liệu thay đổi tức thì mà không có độ trễ của mạng.

## ADR 8: Implementation Approach for Daily Timeline (US-16.1)
**Ngày ra quyết định: 2026-04-28**
**Người viết: AI, huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Cần triển khai tính năng Daily Timeline để theo dõi năng lượng và hoạt động theo giờ. Câu hỏi là nên triển khai tích hợp đầy đủ Backend ngay lập tức hay làm bản UI Prototype trước?

**2. Các phương án đã được gợi ý:**
- **Phương án 1**: Triển khai đầy đủ từ Database, WebAPI đến Frontend. Đảm bảo dữ liệu được lưu trữ thật ngay từ đầu.
- **Phương án 2**: Triển khai UI Prototype với dữ liệu Mock. Tập trung vào việc clone chính xác giao diện người dùng (Pixel-perfect) và trải nghiệm người dùng (UX/Animations).

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 2 (UI Prototype with Mock Data)**.
- **Lý do**: 
    - **Yêu cầu người dùng**: Người dùng yêu cầu "clone cái giao diện này về, chỉ là prototype UI, sẽ quyết định backend sau".
    - **Tính linh hoạt**: Giúp người dùng có cái nhìn trực quan về giao diện và tương tác trước khi cam kết cấu trúc dữ liệu backend.
    - **Tốc độ & Thẩm mỹ**: Tốc độ triển khai nhanh, tập trung tối đa vào phần "Premium Aesthetics" (màu sắc, animations, logic summary).
    - **Dữ liệu mock**: Được quản lý qua Angular Signals giúp dễ dàng chuyển đổi sang API Call sau này mà không làm thay đổi logic hiển thị.

## ADR 9: Yearly Matrix Grid Rendering & Navigation Strategy (US-17.1)
**Ngày ra quyết định: 2026-04-29**
**Người viết: AI, huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Làm thế nào để render một lưới 12 tháng x 31 ngày hiệu quả mà vẫn đảm bảo tính thẩm mỹ cao (Premium UI) và tối ưu hóa cho màn hình 4K. Ngoài ra, cần xác định cơ chế hiển thị nội dung chi tiết (Books/Posts).

**2. Các phương án đã được gợi ý:**
- **Phương án 1 (Calendar Library)**: Sử dụng các thư viện lịch có sẵn (FullCalendar, DayJS) để render.
- **Phương án 2 (Custom Matrix with Flexbox)**: Tự xây dựng ma trận dữ liệu 2 chiều (31 ngày x 12 tháng) và render bằng Flexbox thuần. Sử dụng Angular Computed Signals để tính toán dữ liệu lưới từ danh sách phẳng.

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 2 (Custom Matrix with Flexbox)**.
- **Lý do**: 
    - **Kiểm soát tuyệt đối**: Thư viện lịch thường khó tùy chỉnh để đạt được giao diện ma trận đặc thù (31 hàng dọc) và các indicator tùy biến cao (chấm mood, hình chữ nhật post).
    - **Tối ưu 4K**: Flexbox cho phép kiểm soát chính xác việc co giãn các cell để lấp đầy không gian màn hình lớn mà không làm vỡ bố cục.
    - **Mailbox Style Reader**: Quyết định sử dụng bố cục 2 cột (List - Detail) cho bài viết giúp xử lý tình huống một ngày có nhiều nội dung mà không gây rối giao diện.
    - **Future Muting logic**: Việc tự render giúp dễ dàng áp dụng các logic điều kiện (như ẩn màu sắc cho ngày tương lai) một cách linh hoạt nhất.
## ADR 10: Service & CSS Isolation for Content Explorer Refinement
**Ngày ra quyết định: 2026-04-30**
**Người viết: AI, huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Làm thế nào để triển khai tính năng Content Explorer (với dữ liệu tin tức công nghệ phong phú) mà không làm "gãy" giao diện hoặc làm ô nhiễm dữ liệu của trang Range Tracker vốn đang cần sự tối giản và ổn định tuyệt đối.

**2. Các phương án đã được gợi ý:**
- **Phương án 1 (Unified Service)**: Sử dụng chung `WeeklyTrackerService` cho cả hai trang, sử dụng các cờ (flags) để switch loại dữ liệu mock.
- **Phương án 2 (Isolated Services & Styles)**: Tách biệt hoàn toàn `ContentExplorerService` và viết lại CSS độc lập cho trang Explorer.

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 2 (Isolated Services & Styles)**.
- **Lý do**: 
    - **Tính ổn định (Robustness)**: Dữ liệu tin tức dài (Tech News) gây vỡ layout trên các lưới mật độ cao của Range Tracker. Việc tách Service giúp Range Tracker luôn giữ được dữ liệu "sạch" và an toàn.
    - **Tự do thiết kế (Design Freedom)**: Trang Explorer yêu cầu các quy tắc CSS riêng (căn lề 34px, xử lý ellipsis, segmented control). CSS Isolation giúp tinh chỉnh Explorer đạt mức pixel-perfect mà không sợ làm ảnh hưởng đến các trang Tracker hiện có.
    - **Maintainability**: Tuân thủ nguyên tắc "Module sâu" và "Tách biệt mối quan tâm" (Separation of Concerns). Mỗi page quản lý state và style riêng giúp việc debug và mở rộng sau này dễ dàng hơn nhiều.

## ADR 11: Layout Strategy for Grid and Editor Interaction (Laputa Notes - US-20.1)
**Ngày ra quyết định: 2026-05-02**
**Người viết: AI, huy**

**1. Vấn đề/Concern/Feature đang cần ra quyết định:**
Làm thế nào để xử lý layout cho Editor khi người dùng ở chế độ xem Grid (5 cột). Khi click vào một thẻ Note, Detail Panel phải xuất hiện bên phải mà không làm vỡ cấu trúc Grid, đồng thời danh sách lưới phải tự động co lại để nhường không gian cho Panel, và panel này phải dùng chung logic với Editor ở các view mode khác.

**2. Các phương án đã được gợi ý:**
- **Phương án 1 (Absolute Position Popup)**: Sử dụng CSS `position: absolute` hoặc `fixed` để đè một panel hiển thị chi tiết lên trên lưới. Lưới phía dưới không đổi kích thước.
- **Phương án 2 (Flexbox + Grid Shrink)**: Kết hợp CSS Flexbox và CSS Grid. Khi Editor được mở ở chế độ Grid, sử dụng class `has-note` để giới hạn chiều rộng của Note List panel (bằng hàm `calc` và `flex: 1`), từ đó đẩy Grid co lại, đồng thời Editor panel sẽ slide-in. Editor dùng chung một component nhưng điều chỉnh chiều rộng tối đa thông qua Host Binding (`:host`).

**3. Lựa chọn và lý do lựa chọn:**
- **Lựa chọn**: **Phương án 2 (Flexbox + Grid Shrink)**.
- **Lý do**: 
    - **UX/Thẩm mỹ**: Đảm bảo layout linh hoạt, phần list tự động co lại và giữ nguyên bố cục chữ nhật của các thẻ, tạo cảm giác mượt mà và trực quan thay vì bị che khuất bởi panel absolute.
    - **Reusability**: Sử dụng lại toàn bộ component `laputa-editor` mà không cần viết một component riêng biệt cho Detail Panel. Chỉ cần thay đổi thuộc tính CSS dựa trên Angular Host Binding (`[class.view-grid]`, `[class.has-note]`).
    - **Clean Code**: Quản lý state thông qua Signals và class-binding giúp việc tính toán layout được giao hoàn toàn cho CSS engine (sử dụng hàm `max()`, `calc()`), giảm thiểu việc phải dùng JavaScript để tính chiều rộng, tuân thủ nguyên tắc responsive layout.
