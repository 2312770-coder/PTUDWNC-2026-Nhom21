# Hướng Dẫn Chi Tiết & Phân Công Nhiệm Vụ
### Thành viên: Nguyễn Viết Toàn (MSSV: 2312777)
### Vai trò: Category, Cache & Search Engine

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
| 1 | **FR-CAT-001** | Xem danh sách danh mục ẩm thực | `Features/Categories/Queries/GetCategories/*` | Trang `/categories` & Menu Navbar |
| 2 | **FR-CAT-003** | Admin tạo danh mục mới | `Features/Categories/Commands/CreateCategory/*` | Trang Admin `/admin/categories` |
| 3 | **Kỹ thuật Cache** | Tích hợp Redis Caching cho Danh mục | `Features/Categories/Queries/GetCategories/*` | Đo lường thời gian phản hồi API |
| 4 | **FR-CAT-004**<br>**FR-CAT-005** | Cập nhật (D12) & Xóa mềm danh mục (D1) | `Features/Categories/Commands/UpdateCategory/*`<br>`Features/Categories/Commands/DeleteCategory/*` | Form sửa danh mục & nút xóa trong Admin |
| 5 | **FR-SRCH-001** | Tìm kiếm toàn văn (PostgreSQL unaccent) | `Features/Recipes/Queries/SearchRecipes/*` | Trang kết quả tìm kiếm `/recipes?q=...` |
| 6 | **FR-SRCH-002..004** | Lọc đa tiêu chí, sắp xếp D8, phân trang | `Features/Recipes/Queries/GetRecipes/*` | Sidebar bộ lọc độ khó/thời gian trên `/recipes` |
| 7 | **FR-INT-001** | Đánh giá sao công thức (Rating 1-5) | `Features/Recipes/Commands/RateRecipe/*` | Component chấm sao trên trang chi tiết món ăn |

---

## 3. Lộ Trình Thực Hiện (Tuần 2 → Tuần 8)

```
Tuần 2 (Đã hoàn thành & merge main):
  ✅ FR-CAT-001 — Xem danh sách danh mục (Truy vấn EF Core theo OrderIndex & đếm recipe)

Tuần 3 (Đã hoàn thành & merge main):
  ✅ FR-CAT-003 — Admin tạo danh mục mới (Sinh slug chuẩn SEO, RequireAuthorization AdminOnly & UI Admin)

Tuần 4 (Tuần tới):
  🔲 Tích hợp Redis Caching cho Danh mục (Tự inject ICacheService vào GetCategoriesQueryHandler, cache HIT/MISS, TTL 30 phút)

Tuần 5:
  🔲 FR-CAT-004 — Admin cập nhật danh mục (Tuân thủ D12: giữ nguyên slug SEO, xóa cache Redis)
  🔲 FR-CAT-005 — Admin xóa mềm danh mục (Tuân thủ D1: kiểm tra không có recipes mới cho xóa)

Tuần 6:
  🔲 FR-SRCH-001 — Tìm kiếm toàn văn Full-Text Search PostgreSQL (tsvector + unaccent)
  🔲 FR-SRCH-002..004 — Lọc đa tiêu chí, sắp xếp dual-syntax D8, phân trang

Tuần 7:
  🔲 FR-INT-001 — Đánh giá sao công thức (Rating 1-5 sao, tính điểm trung bình và số lượt đánh giá)

Tuần 8:
  🔲 Triển khai Production, Tối ưu hóa truy vấn CSDL PostgreSQL & Redis Cache, Hoàn thiện báo cáo
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (ĐÃ HOÀN THÀNH)

### Chức năng: Xem danh sách danh mục (FR-CAT-001)
- **Nhánh**: `2312777-NVToan-Danh-Sach-Danh-Muc` (Đã merge vào `main`)
- **Backend**:
  - `GetCategoriesQueryHandler`: Truy vấn danh mục theo `OrderIndex`, đếm số lượng bài viết trạng thái `Published` và chưa xóa mềm `!IsDeleted`.
  - Đăng ký endpoint `GET /api/v1/categories`.
- **Frontend**:
  - Module `src/Frontend/lib/api/categories.ts` với hàm `getAll()`.
  - Hiển thị danh mục tại trang chủ và trang danh mục `/categories`.

---

## 5. HƯỚNG DẪN CHI TIẾT TUẦN 3 (ĐÃ HOÀN THÀNH)

### Chức năng: Admin tạo danh mục mới (FR-CAT-003)
- **Nhánh**: `2312777-NVToan-Tao-Danh-Muc` (Đã merge vào `main`)
- **Backend**:
  - `CreateCategoryCommandHandler`: Kiểm tra tên danh mục trùng lặp, tự sinh slug chuẩn SEO (bỏ dấu tiếng Việt, nối bằng gạch ngang), lưu CSDL.
  - Phân quyền endpoint `POST /api/v1/categories` với chính sách `AdminOnly`.
- **Frontend**:
  - Trang quản trị `src/Frontend/app/(admin)/admin/categories/page.tsx`: Form thêm danh mục, thứ tự hiển thị và upload ảnh.

---

## 6. HƯỚNG DẪN CHI TIẾT TUẦN 4 (CHUẨN BỊ LÀM)

> ⚠️ **Quy ước nhánh**: Làm trên nhánh riêng `2312777-NVToan-Cache-Danh-Muc`.

### Chức năng trọng tâm: Tích hợp Redis Caching cho Danh mục

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312777-NVToan-Cache-Danh-Muc
```

#### Bước 2: Hiện thực Backend
1. Mở file `src/Backend/CulinaryBlog.Application/Features/Categories/Queries/GetCategories/GetCategoriesQueryHandler.cs`.
2. Inject `ICacheService` vào constructor của `GetCategoriesQueryHandler`.
3. Xử lý luồng Caching trong hàm `Handle`:
   - Bước 1 (Kiểm tra Cache): `var cached = await _cacheService.GetAsync<IReadOnlyList<CategoryDto>>("categories:all", ct);`
   - Bước 2 (Cache HIT): Nếu `cached != null`, ghi log Cache HIT và trả về kết quả ngay lập tức (không cần query PostgreSQL).
   - Bước 3 (Cache MISS): Nếu chưa có trong cache, thực hiện query EF Core từ CSDL PostgreSQL.
   - Bước 4 (Lưu Cache): Lưu kết quả vào Redis với thời hạn TTL 30 phút:
     `await _cacheService.SetAsync("categories:all", categories, TimeSpan.FromMinutes(30), ct);`
4. Cập nhật `CreateCategoryCommandHandler.cs`: Sau khi Admin tạo danh mục mới thành công, gọi `await _cacheService.RemoveAsync("categories:all", ct)` để xóa cache cũ.

#### Bước 3: Kiểm thử
- Chạy Backend, gọi API `GET /api/v1/categories` lần 1 (Cache MISS, xem log Seq).
- Gọi lại lần 2 (Cache HIT, tốc độ phản hồi < 5ms).
