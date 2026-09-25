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
| 4 | **FR-CAT-004** | Admin cập nhật danh mục (D12) | `Features/Categories/Commands/UpdateCategory/*` | Form sửa danh mục trong Admin |
| 5 | **FR-CAT-005** | Admin xóa mềm danh mục (D1) | `Features/Categories/Commands/DeleteCategory/*` | Nút xóa có modal cảnh báo trong Admin |
| 6 | **FR-SRCH-001** | Tìm kiếm toàn văn (PostgreSQL unaccent) | `Features/Recipes/Queries/SearchRecipes/*` | Trang kết quả tìm kiếm `/recipes?q=...` |
| 7 | **FR-SRCH-002..004**<br>**FR-INT-001** | Lọc đa tiêu chí & Đánh giá sao | `Features/Recipes/Queries/GetRecipes/*`<br>`Features/Recipes/Commands/RateRecipe/*` | Sidebar bộ lọc `/recipes` & Component Rating |

---

## 3. Lộ Trình Thực Hiện (Tuần 2 → Tuần 8)

```
Tuần 2 (Đã hoàn thành & merge main):
  ✅ FR-CAT-001 — Xem danh sách danh mục (Truy vấn EF Core theo OrderIndex & đếm recipe)

Tuần 3 (Đã hoàn thành & merge main):
  ✅ FR-CAT-003 — Admin tạo danh mục mới (Sinh slug chuẩn SEO, RequireAuthorization AdminOnly & UI Admin)

Tuần 4 (Tuần tới):
  🔲 Tích hợp Redis Caching cho Danh mục (Tự inject ICacheService vào GetCategoriesQueryHandler, cache HIT/MISS, TTL 30 phút)
  🔲 FR-CAT-004 — Admin cập nhật danh mục (Tuân thủ D12: giữ nguyên slug SEO, xóa cache Redis)

Tuần 5:
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

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`, cú pháp: `2312777-NVToan-<Ten-Chuc-Nang>`. **Không tự merge vào `main`** — báo trưởng nhóm Tiến để Tiến review và merge giúp.

---

### Chức năng: Xem danh sách danh mục (FR-CAT-001)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312777-NVToan-Danh-Sach-Danh-Muc
```

#### Bước 2: Hiện thực Backend & Hoàn thiện Frontend
1. Backend: Xem file `src/Backend/CulinaryBlog.Application/Features/Categories/Queries/GetCategories/GetCategoriesQueryHandler.cs` (hiện tại là khung mẫu ném `NotImplementedException`). Bạn hiện thực truy vấn EF Core lấy danh sách danh mục theo `OrderIndex`, đếm số lượng công thức Published và ánh xạ sang `CategoryDto`. Đăng ký endpoint GET `/api/v1/categories` trong `CategoriesEndpoints.cs`.
2. Frontend: Hiện thực hàm `categoriesApi.getAll()` trong `src/Frontend/lib/api/categories.ts` để gọi API Backend. Tạo trang xem toàn bộ danh mục tại `src/Frontend/app/(public)/categories/page.tsx` hiển thị lưới các danh mục dạng card với hình ảnh đại diện, mô tả và số lượng công thức thực tế.

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

## 5. HƯỚNG DẪN CHI TIẾT TUẦN 3 (ĐÃ HOÀN THÀNH)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`, cú pháp: `2312777-NVToan-<Ten-Chuc-Nang>`. Sau khi code xong và test build không lỗi, gửi PR để trưởng nhóm Tiến review và merge.

---

### Chức năng: Admin tạo danh mục mới (FR-CAT-003)

#### Bước 1: Tạo nhánh mới từ `main`
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
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "category: hien thuc FR-CAT-003 tao danh muc moi"
git push -u origin 2312777-NVToan-Tao-Danh-Muc
```
Nhắn trưởng nhóm Tiến qua Zalo để Tiến review và merge vào `main`.

---

## 6. HƯỚNG DẪN CHI TIẾT TUẦN 4 (CHUẨN BỊ LÀM)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên một nhánh riêng.

---

### Chức năng 1: Tích hợp Redis Caching cho Danh mục

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

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "cache: tich hop redis caching cho danh muc TTL 30 phut"
git push -u origin 2312777-NVToan-Cache-Danh-Muc
```

---

### Chức năng 2: Admin cập nhật danh mục (FR-CAT-004)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312777-NVToan-Sua-Danh-Muc
```

#### Bước 2: Hiện thực Backend
1. Thư mục `Features/Categories/Commands/UpdateCategory/`:
   - `UpdateCategoryCommand(Guid Id, string Name, string? Description, string? ImageUrl, int? OrderIndex)`: `IRequest<CategoryDto>`
   - `UpdateCategoryCommandHandler`:
     - Tìm category theo `Id` (chưa bị xóa mềm `!IsDeleted`). Nếu không thấy ném `NotFoundException`.
     - Tuân thủ **Quyết định D12**: Cập nhật Name, Description, ImageUrl, OrderIndex nhưng **giữ nguyên Slug** để bảo toàn SEO URL.
     - Lưu CSDL và xóa cache Redis `await _cacheService.RemoveAsync("categories:all", ct)`.
2. Đăng ký endpoint `PUT /api/v1/categories/{id}` trong `CategoriesEndpoints.cs` (`RequireAuthorization("AdminOnly")`).

#### Bước 3: Hiện thực Frontend
1. Mở trang quản trị `/admin/categories`:
   - Thêm nút "Sửa" trên từng dòng danh mục.
   - Bấm vào hiển thị Modal sửa thông tin (Tên, Mô tả, OrderIndex, ImageUrl).
   - Gọi API `PUT /api/v1/categories/{id}` và tải lại bảng danh mục.

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "category: hien thuc FR-CAT-004 admin cap nhat danh muc"
git push -u origin 2312777-NVToan-Sua-Danh-Muc
```

---

## 7. Tiêu Chí Nghiệm Thu (Definition of Done)
- [ ] Endpoint `GET /api/v1/categories` trả về đúng danh sách và cache Redis hoạt động mượt mà.
- [ ] Endpoint `POST /api/v1/categories` tạo được danh mục mới kèm slug tự động chuẩn SEO.
- [ ] Endpoint `PUT /api/v1/categories/{id}` cập nhật thông tin thành công và bảo toàn slug D12.
- [ ] Trang `/categories` và `/admin/categories` hiển thị trực quan và hoạt động chính xác.
- [ ] Các nhánh chức năng đã được đẩy lên GitHub và merge vào `main`.
