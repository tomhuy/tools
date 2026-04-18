chúng ta sẽ có một user story 9.1
chúng ta refactor lại event model, nó trở thành một cái khái niệm đơn giản hơn "memento"

memento:
- title
- startdate
- enddate
- parentid
	- các memento sẽ thuộc về nhau
	- eventpharsemodel thực ra là memento nhưng có parentid
- order: dùng để sau này sắp sếp thứ tự của các ghi chú chủ đề
- createddate
- color
- description



về triết lý thiết kế:
một memento như một ghi chú chủ đề (ParentId là null)
còn các thứ khác giống như các ghi chú khái niệm bổ xung và hình thành nên chủ đề đó


ở các view khác nhau sau này có thể lấy được những ghi chủ chủ đề khác nhau