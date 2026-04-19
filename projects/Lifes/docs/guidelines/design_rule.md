

các rule design:
Đối với presentation layer:

Rule 1:
- khi các nhiệm vụ là CRUD, Add, Update, thì hãy xem xét lại phương án đó là trong form đó sẽ thực hiện hành động, và chỉ bắn event để thông báo cho parent xử lý ví dụ như có nên load lại data không, chúng ta không để mainview sẽ handle cách component xử lý business logic, mainview chỉ có trách nhiệm là như truyền vào các function để bên trong component đó thực hiện hành động (ví dụ như bản thân componenet không tương tác với api, lúc save chỉ là có model, nhưng bên ngoài truyền vào function save được call api chẳn hạn, component không biết về việc đó chỉ biết excute là ok), hoặc bản thân component có thể biết về service call backend luôn và chỉ bắn event khi có lỗi hoặc thành công để bên ngoài xử lý nên làm gì, ví dụ như refresh data

When: khi design các loại hành động như thế này, hãy tạo ra open question lại cho người dùng, anh muốn chọn phương án nào


