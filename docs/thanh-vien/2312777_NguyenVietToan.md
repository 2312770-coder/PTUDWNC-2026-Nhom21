# Hướng Dẫn Chi Tiết & Phân Công Nhiệm Vụ
### Thành viên: Nguyễn Viết Toàn (MSSV: 2312777)
### Vai trò: Category Management & Search Engine

---

## 1. Thông Tin Chung
- **Họ và tên**: Nguyễn Viết Toàn
- **MSSV**: 2312777
- **Email**: 2312777@dlu.edu.vn
- **GitHub**: [https://github.com/2312777-rgb](https://github.com/2312777-rgb)
- **Tổng số chức năng phụ trách**: **7 chức năng**

---

## 2. Danh Sách 7 Chức Năng Phụ Trách

| STT | Mã FR | Tên chức năng | File Backend cần làm | File Frontend cần làm |
| :---: | :--- | :--- | :--- | :--- |
| 1 | **FR-CAT-001** | Xem danh sách danh mục | `Features/Categories/Queries/GetCategories/*` | Menu danh mục trên Navbar & Trang chủ |
| 2 | **FR-CAT-003** | Admin tạo danh mục mới | `Features/Categories/Commands/CreateCategory/*` | Form thêm danh mục trong Admin |
| 3 | **FR-CAT-004** | Admin sửa danh mục (D12 giữ slug) | `Features/Categories/Commands/UpdateCategory/*` | Form sửa danh mục trong Admin |
| 4 | **FR-CAT-005** | Admin xóa mềm danh mục (D1) | `Features/Categories/Commands/DeleteCategory/*` | Nút xóa danh mục trong Admin |
| 5 | **FR-SRCH-001** | Tìm kiếm toàn văn (Full-Text) | `Features/Recipes/Queries/SearchRecipes/*` | Trang kết quả tìm kiếm `/recipes?q=...` |
| 6 | **FR-SRCH-002..004** | Lọc đa tiêu chí, sắp xếp D8, phân trang | `Features/Recipes/Queries/GetRecipes/*` | Sidebar lọc thời gian/độ khó/phân trang |
| 7 | **FR-INT-001** | Đánh giá sao công thức (Rating) | `Features/Recipes/Commands/RateRecipe/*` | Component chấm sao 1-5 trên trang chi tiết |

---

## 3. Lộ Trình Thực Hiện (Tuần 2 → Tuần 7)

```
Tuần 2 (Tuần này): FR-CAT-001 (Danh sách danh mục) & FR-CAT-003 (Admin tạo danh mục)
Tuần 3: FR-CAT-004 (Sửa danh mục - giữ slug D12) & FR-CAT-005 (Xóa mềm danh mục D1)
Tuần 4: FR-SRCH-001 (Tìm kiếm toàn văn PostgreSQL unaccent)
Tuần 5: FR-SRCH-002/003/004 (Bộ lọc đa tiêu chí, sắp xếp dual-syntax D8, phân trang)
Tuần 6: FR-INT-001 (Chức năng đánh giá sao Recipe Rating 1-5 sao)
Tuần 7: Kiểm thử UI Admin Category & Search, tối ưu truy vấn, hoàn thiện báo cáo
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (TUẦN NÀY BẮT ĐẦU LÀM)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`, cú pháp: `2312777-NVToan-<Ten-Chuc-Nang>`. **Không tự merge vào `main`** — báo trưởng nhóm Tiến để Tiến review và merge giúp.

---

### Chức năng 1: Xem danh sách danh mục (FR-CAT-001)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312777-NVToan-Danh-Sach-Danh-Muc
```

#### Bước 2: Kiểm tra Backend & Hoàn thiện Frontend
1. Backend: Xem file `src/Backend/CulinaryBlog.Application/Features/Categories/Queries/GetCategories/GetCategoriesQueryHandler.cs` (đã có khung mẫu truy vấn theo `OrderIndex` và đếm số bài viết). Bạn kiểm tra lại logic và bổ sung comment giải thích thuật toán.
2. Frontend: Tạo trang xem toàn bộ danh mục tại `src/Frontend/app/(public)/categories/page.tsx` hiển thị lưới các danh mục dạng card với hình ảnh đại diện, mô tả và số lượng công thức thực tế.

#### Bước 3: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npm run lint; cd ..\..

git add .
git commit -m "category: hien thuc FR-CAT-001 danh sach danh muc"
git push -u origin 2312777-NVToan-Danh-Sach-Danh-Muc
```
Nhắn trưởng nhóm Tiến qua Zalo để Tiến review và merge vào `main`.

---

### Chức năng 2: Admin tạo danh mục mới (FR-CAT-003)

#### Bước 1: Tạo nhánh mới từ `main` (sau khi chức năng 1 đã xong)
```powershell
git checkout main
git pull origin main
git checkout -b 2312777-NVToan-Tao-Danh-Muc
```

#### Bước 2: Hiện thực Backend & Frontend
1. Backend: Mở file `src/Backend/CulinaryBlog.Application/Features/Categories/Commands/CreateCategory/CreateCategoryCommandHandler.cs`.
   - Kiểm tra tên danh mục không trùng lặp: `await _categoryRepository.Query().AnyAsync(c => c.Name == request.Name)`.
   - Tạo danh mục: `var category = Category.Create(request.Name, request.Description, request.ImageUrl, request.OrderIndex)`.
   - Lưu qua `_categoryRepository.AddAsync(category, ct)` và `SaveChangesAsync`.
   - Trả về `CategoryDto`.
2. Frontend: Tạo trang quản trị danh mục `src/Frontend/app/(admin)/categories/page.tsx` có bảng danh mục hiện có và form nhập tên danh mục, mô tả, ảnh đại diện và thứ tự hiển thị `OrderIndex`.

#### Bước 3: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npm run lint; cd ..\..

git add .
git commit -m "category: hien thuc FR-CAT-003 tao danh muc moi"
git push -u origin 2312777-NVToan-Tao-Danh-Muc
```
Nhắn trưởng nhóm Tiến qua Zalo để Tiến review và merge vào `main`.

---

## 5. Tiêu Chí Nghiệm Thu (Definition of Done)
- [ ] Endpoint `GET /api/v1/categories` trả về đúng danh sách và số bài viết.
- [ ] Endpoint `POST /api/v1/categories` tạo được danh mục mới kèm slug tự động chuẩn SEO.
- [ ] Trang `/categories` hiển thị card danh mục trực quan với đúng số lượng công thức.
- [ ] Các nhánh chức năng `2312777-NVToan-Danh-Sach-Danh-Muc` và `2312777-NVToan-Tao-Danh-Muc` đã được đẩy lên GitHub.
- [ ] Build và Lint cả dự án đều đạt 0 lỗi.


