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

