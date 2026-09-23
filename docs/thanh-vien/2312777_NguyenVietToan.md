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
| 1 | **FR-CAT-001** | Xem danh sách danh mục (kèm Cache) | `Features/Categories/Queries/GetCategories/*` | Menu danh mục trên Navbar & Trang chủ |
| 2 | **FR-CAT-003** | Admin tạo danh mục mới | `Features/Categories/Commands/CreateCategory/*` | Form thêm danh mục trong Admin |
| 3 | **FR-CAT-004** | Admin sửa danh mục (D12 giữ slug) | `Features/Categories/Commands/UpdateCategory/*` | Form sửa danh mục trong Admin |
| 4 | **FR-CAT-005** | Admin xóa mềm danh mục (D1) | `Features/Categories/Commands/DeleteCategory/*` | Nút xóa danh mục trong Admin |
| 5 | **FR-SRCH-001** | Tìm kiếm toàn văn (Full-Text) | `Features/Recipes/Queries/SearchRecipes/*` | Trang kết quả tìm kiếm `/recipes?q=...` |
| 6 | **FR-SRCH-002..004** | Lọc đa tiêu chí, sắp xếp D8, phân trang | `Features/Recipes/Queries/GetRecipes/*` | Sidebar lọc thời gian/độ khó/phân trang |
| 7 | **FR-INT-001** | Đánh giá sao công thức (Rating) | `Features/Recipes/Commands/RateRecipe/*` | Component chấm sao 1-5 trên trang chi tiết |

---

## 3. Lộ Trình 6 Tuần (Tuần 2 → Tuần 7)

```
Tuần 2 (Đã hoàn thành):
  ✅ FR-CAT-001 — Xem danh sách danh mục (Truy vấn EF Core & Frontend API categories.ts)

Tuần 3 (Tuần này):
  🔲 FR-CAT-003: Admin tạo danh mục mới (Kiểm tra trùng tên, sinh slug, Require Role Admin)
  🔲 Tích hợp Redis Caching cho FR-CAT-001: Tự inject ICacheService để cache danh sách danh mục

Tuần 4:
  🔲 FR-CAT-004: Admin sửa danh mục (Tuân thủ D12: giữ nguyên slug, xóa cache danh mục)
  🔲 FR-CAT-005: Admin xóa mềm danh mục (Tuân thủ D1: Soft delete IsDeleted = true)

Tuần 5:
  🔲 FR-SRCH-001: Tìm kiếm toàn văn Full-Text Search PostgreSQL (tsvector + unaccent)
  🔲 FR-SRCH-002/003/004: Lọc đa tiêu chí (độ khó, thời gian), sắp xếp dual-syntax D8, phân trang

Tuần 6:
  🔲 FR-INT-001: Chức năng đánh giá sao Recipe Rating 1-5 sao, Hoàn thiện báo cáo đồ án
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 3 (TUẦN NÀY LÀM)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`, cú pháp: `2312777-NVToan-<Ten-Chuc-Nang>`. Sau khi code xong và test build không lỗi, gửi PR để trưởng nhóm Tiến review và merge.

---

### Chức năng 1: Admin tạo danh mục mới (FR-CAT-003)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312777-NVToan-Tao-Danh-Muc
```

#### Bước 2: Hiện thực Backend
1. Tạo thư mục `src/Backend/CulinaryBlog.Application/Features/Categories/Commands/CreateCategory/`:
   - `CreateCategoryCommand(string Name, string? Description, string? ImageUrl, int OrderIndex)`: `IRequest<CategoryDto>`
   - `CreateCategoryCommandValidator`: Tên không rỗng, tối đa 100 ký tự.
   - `CreateCategoryCommandHandler`:
     - Kiểm tra tên danh mục đã tồn tại trong CSDL chưa: `await _categoryRepository.NameExistsAsync(request.Name)`. Nếu có ném `ConflictException("Tên danh mục đã tồn tại.")`.
     - Tự động sinh `Slug` chuẩn SEO từ tên tiếng Việt (bỏ dấu tiếng Việt, nối bằng dấu `-`).
     - Khởi tạo entity `Category.Create(request.Name, slug, request.Description, request.ImageUrl, request.OrderIndex)`.
     - Lưu CSDL: `await _categoryRepository.AddAsync(category, ct)` và `SaveChangesAsync(ct)`.
     - **Tự tay xóa cache danh mục**: Inject `ICacheService` và gọi `await _cacheService.RemoveAsync("categories:all", ct)`.
     - Trả về `CategoryDto`.
2. Đăng ký endpoint `POST /api/v1/categories` trong `CategoriesEndpoints.cs`:
   - Yêu cầu quyền Admin: `.RequireAuthorization("AdminOnly")`.

#### Bước 3: Hiện thực Frontend
1. Tạo trang quản trị danh mục `src/Frontend/app/(admin)/admin/categories/page.tsx` hoặc Form thêm danh mục:
   - Form nhập: Tên danh mục, Mô tả, Thứ tự hiển thị (`OrderIndex`), Upload ảnh đại diện danh mục (tái sử dụng component `ImageUploader`).
   - Gọi API POST `/api/v1/categories`.
   - Hiển thị danh sách danh mục hiện có bên cạnh để Admin dễ theo dõi.

---

### Kỹ thuật 2: Tích hợp Redis Caching cho FR-CAT-001 (Danh sách danh mục)

#### Bước 1: Mở file `GetCategoriesQueryHandler.cs`
1. Inject `ICacheService` vào constructor của `GetCategoriesQueryHandler`.
2. Khi người dùng gọi lấy danh mục:
   - Kiểm tra Redis: `var cached = await _cacheService.GetAsync<IReadOnlyList<CategoryDto>>("categories:all", ct);`
   - Nếu `cached != null` -> Ghi log Cache HIT và trả về ngay lập tức (không cần truy vấn Postgres).
   - Nếu không có -> Truy vấn EF Core từ CSDL Postgres, sau đó lưu vào Redis: `await _cacheService.SetAsync("categories:all", categories, TimeSpan.FromMinutes(30), ct);`
   - Trả về dữ liệu.
3. Test trên trình duyệt hoặc Scalar và kiểm tra thời gian phản hồi nhanh vượt trội khi có Redis Cache.
