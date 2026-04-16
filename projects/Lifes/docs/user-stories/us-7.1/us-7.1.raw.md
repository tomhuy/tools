

tạo một user story, tạo một new form

nhiệm vụ của form đó là manage document:

document sẽ được design như sau:


--- các field trong db thì lưu theo chuẩn này, còn model c# thì sử dung style của c#

- id
- parent_id
- title
- description
- due_date
- status


trong các task sẽ có có các subdocument, subdocument sẽ có các thuộc tính tương tự như document và parent_id sẽ là id của document cha



