# Hướng Dẫn Chi Tiết & Phân Công Nhiệm Vụ
### Thành viên: Lê Nhật Tiến (MSSV: 2312770)
### Vai trò: Trưởng nhóm — Core Recipe & Author Experience

---

## 1. Thông Tin Chung
- **Họ và tên**: Lê Nhật Tiến
- **MSSV**: 2312770
- **Email**: 2312770@dlu.edu.vn
- **GitHub**: [https://github.com/2312770-coder](https://github.com/2312770-coder)
- **Tổng số chức năng phụ trách**: **7 chức năng**

---

## 2. Danh Sách 7 Chức Năng Phụ Trách

| STT | Mã FR | Tên chức năng | File Backend cần làm | File Frontend cần làm |
| :---: | :--- | :--- | :--- | :--- |
| 1 | **FR-RCP-003** | Tạo công thức mới (trạng thái Draft) | `Features/Recipes/Commands/CreateRecipe/*` | `app/dashboard/recipes/new/page.tsx` |
| 2 | **FR-CAT-002** | Xem chi tiết danh mục + bài viết | `Features/Categories/Queries/GetCategoryBySlug/*` | `app/(public)/categories/[slug]/page.tsx` |
| 3 | **FR-RCP-002** | Xem chi tiết công thức nấu ăn | `Features/Recipes/Queries/GetRecipeBySlug/*` | `app/(public)/recipes/[slug]/page.tsx` |
| 4 | **FR-RCP-008** | Quản lý gallery ảnh công thức | `Features/Recipes/Commands/ManageImages/*` | `components/recipes/RecipeGalleryEditor.tsx` |
| 5 | **FR-RCP-004**<br>**FR-RCP-006** | Cập nhật thông tin & Lưu trữ (Archive) | `Features/Recipes/Commands/UpdateRecipe/*`<br>`Features/Recipes/Commands/ArchiveRecipe/*` | `app/dashboard/recipes/[id]/edit/page.tsx` |
| 6 | **FR-RCP-007** | Xóa mềm công thức (D1 Soft Delete) | `Features/Recipes/Commands/DeleteRecipe/*` | Nút xóa trong danh sách bài của tôi |
| 7 | **FR-JOB-003** | Tự động sinh file `sitemap.xml` | `Infrastructure/Jobs/SitemapJob.cs` | Route `/sitemap.xml` SEO |

---

## 3. Lộ Trình Thực Hiện (Tuần 2 → Tuần 8)

```
Tuần 2 (Đã hoàn thành & merge main):
  ✅ Dựng hạ tầng — Docker 5 container, Database Seeder (2 users, 23 danh mục, 100 công thức mẫu)
  ✅ Layout chung — Navbar, Footer, RecipeCard, Layout.tsx, Home Page
  ✅ FR-RCP-003 — Tạo công thức mới trạng thái Draft (Backend & Frontend /dashboard/recipes/new)

Tuần 3 (Đã hoàn thành & merge main):
  ✅ FR-CAT-002 — Xem chi tiết danh mục kèm danh sách bài viết (Backend + Frontend /categories/[slug])

Tuần 4 (Tuần tới):
  🔲 FR-RCP-002 — Xem chi tiết công thức nấu ăn (Backend GetRecipeBySlugQueryHandler & Frontend /recipes/[slug])
  🔲 FR-RCP-008 — Quản lý gallery ảnh công thức (Upload/Xóa ảnh, chọn ảnh chính IsPrimary)

Tuần 5:
  🔲 FR-RCP-004 — Cập nhật thông tin công thức (UpdateRecipe)
  🔲 FR-RCP-006 — Lưu trữ công thức (ArchiveRecipe)

Tuần 6:
  🔲 FR-RCP-007 — Xóa mềm công thức (Soft Delete theo D1)

Tuần 7:
  🔲 FR-JOB-003 — Hangfire job tự động sinh sitemap.xml SEO

Tuần 8:
  🔲 Triển khai Production (Docker Compose + Nginx), Kiểm thử tích hợp toàn hệ thống, Hoàn thiện báo cáo đồ án
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (ĐÃ HOÀN THÀNH)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tách từ `main`, định dạng: `2312770-LNTien-<Ten-Chuc-Nang>`. Sau khi code xong và test không lỗi, bạn tạo Pull Request hoặc merge nhánh đó vào `main`.

---

### Chức năng 1: Tạo công thức mới (FR-RCP-003)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312770-LNTien-Tao-Cong-Thuc-Moi
```

#### Bước 2: Hiện thực Backend
1. Mở file `src/Backend/CulinaryBlog.Application/Features/Recipes/Commands/CreateRecipe/CreateRecipeCommandHandler.cs`.
2. Hiện thực logic tạo Recipe:
   - Lấy `AuthorId` từ `_currentUser.UserId`.
   - Tạo Recipe: `Recipe.Create(...)`.
   - Xử lý trùng lặp slug (thêm hậu tố `-2`, `-3`...).
   - Bổ sung thông tin dinh dưỡng, nguyên liệu và các bước nếu client gửi kèm.
   - Lưu vào database qua `_recipeRepository.AddAsync(recipe, ct)` và `_recipeRepository.SaveChangesAsync(ct)`.
   - Trả về `RecipeDetailDto`.
3. Kiểm tra validation trong `CreateRecipeCommandValidator.cs`.

#### Bước 3: Hiện thực Frontend
1. Tạo giao diện trang tạo bài viết: `src/Frontend/app/dashboard/recipes/new/page.tsx` theo SRS mục 5.1.
2. Form gồm: Tiêu đề, Mô tả, Hướng dẫn chung, Danh mục (dropdown), Thời gian chuẩn bị, Thời gian nấu, Khẩu phần, Độ khó, Dinh dưỡng, Nguyên liệu và Các bước chế biến (kèm ImageUploader MinIO).
3. Nút bấm "Lưu bản nháp" gửi request `POST /api/v1/recipes`.

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npm run lint; cd ..\..

git add .
git commit -m "recipe: hien thuc FR-RCP-003 tao cong thuc moi"
git push -u origin 2312770-LNTien-Tao-Cong-Thuc-Moi
```
Sau đó tạo Pull Request trên GitHub để merge vào `main`.

---

## 5. HƯỚNG DẪN CHI TIẾT TUẦN 3 (ĐÃ HOÀN THÀNH)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tách từ `main`, định dạng: `2312770-LNTien-<Ten-Chuc-Nang>`.

---

### Chức năng: Xem chi tiết danh mục kèm danh sách bài viết (FR-CAT-002)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312770-LNTien-Chi-Tiet-Danh-Muc
```

#### Bước 2: Hiện thực Backend
1. Tạo Query `GetCategoryBySlugQuery(string Slug)` trong `Features/Categories/Queries/GetCategoryBySlug/`.
2. Tạo Handler `GetCategoryBySlugQueryHandler`:
   - Truy vấn CSDL theo `Slug` và `!IsDeleted`.
   - Nạp thông tin Category và danh sách Recipes có `Status == RecipeStatus.Published && !IsDeleted`.
   - Nếu không tìm thấy, ném `NotFoundException("Không tìm thấy danh mục.")`.
   - Trả về `CategoryDetailDto` chứa thông tin danh mục kèm danh sách bài viết tóm tắt.
3. Đăng ký route GET `/api/v1/categories/{slug}` trong `CategoriesEndpoints.cs`.

#### Bước 3: Hiện thực Frontend
1. Thêm hàm `getBySlug(slug: string)` vào `src/Frontend/lib/api/categories.ts`.
2. Tạo trang `src/Frontend/app/(public)/categories/[slug]/page.tsx`:
   - Banner hiển thị tiêu đề danh mục, mô tả, ảnh bìa và số lượng món ăn.
   - Lưới danh sách bài viết (tái sử dụng component `RecipeCard`).
   - Xử lý trạng thái Loading (Skeleton) và Not Found nếu slug không hợp lệ.

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "category: hien thuc FR-CAT-002 xem chi tiet danh muc"
git push -u origin 2312770-LNTien-Chi-Tiet-Danh-Muc
```

---

## 6. HƯỚNG DẪN CHI TIẾT TUẦN 4 (CHUẨN BỊ LÀM)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên một nhánh riêng.

---

### Chức năng 1: Xem chi tiết công thức nấu ăn (FR-RCP-002)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312770-LNTien-Chi-Tiet-Cong-Thuc
```

#### Bước 2: Hiện thực Backend
1. Mở file `src/Backend/CulinaryBlog.Application/Features/Recipes/Queries/GetRecipeBySlug/GetRecipeBySlugQueryHandler.cs`.
2. Thay thế dòng ném `NotImplementedException`:
   - Truy vấn CSDL tìm Recipe theo `Slug` (chưa bị xóa mềm `!IsDeleted`).
   - Nạp thông tin quan hệ (`Include`): `Category`, `Author`, `Steps` (sắp xếp theo `StepNumber`), `Ingredients` (sắp xếp theo `OrderIndex`), `Images` (sắp xếp theo `OrderIndex`), `Nutrition`.
   - Kiểm tra quyền xem: Nếu bài ở trạng thái `Draft` hoặc `Archived`, chỉ tác giả (`AuthorId == currentUser.UserId`) hoặc Admin mới được xem. Nếu không phải, ném `ForbiddenException("Bạn không có quyền xem công thức này.")`.
   - Nếu không tìm thấy công thức, ném `NotFoundException("Không tìm thấy công thức yêu cầu.")`.
   - Ánh xạ sang `RecipeDetailDto`.
3. Đăng ký route `GET /api/v1/recipes/{slug}` trong `RecipesEndpoints.cs`.

#### Bước 3: Hiện thực Frontend
1. Cập nhật hàm `getBySlug(slug: string)` trong `src/Frontend/lib/api/recipes.ts` gọi endpoint `GET /api/v1/recipes/${slug}`.
2. Xây dựng trang `src/Frontend/app/(public)/recipes/[slug]/page.tsx`:
   - Hero Section: Tiêu đề lớn, thông tin tác giả, thời gian nấu, khẩu phần, độ khó, ảnh chính sắc nét.
   - Bảng thông tin dinh dưỡng (Calories, Protein, Carb, Fat).
   - Checklist nguyên liệu có thể tick chọn khi chuẩn bị nấu.
   - Timeline các bước thực hiện chi tiết (kèm hình ảnh minh họa của từng bước).
   - Xử lý Not Found (404) nếu bài viết không tồn tại.

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "recipe: hien thuc FR-RCP-002 xem chi tiet cong thuc"
git push -u origin 2312770-LNTien-Chi-Tiet-Cong-Thuc
```

---

### Chức năng 2: Quản lý gallery ảnh công thức (FR-RCP-008)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312770-LNTien-Gallery-Anh
```

#### Bước 2: Hiện thực Backend
1. Thư mục `Features/Recipes/Commands/ManageImages/`:
   - `AddRecipeImageCommand(Guid RecipeId, string ImageUrl, string? AltText, bool IsPrimary, int OrderIndex)`: Thêm ảnh vào gallery của recipe.
   - `DeleteRecipeImageCommand(Guid RecipeId, Guid ImageId)`: Xóa ảnh khỏi gallery (kiểm tra tác giả hoặc Admin).
   - `SetPrimaryImageCommand(Guid RecipeId, Guid ImageId)`: Đặt 1 ảnh làm ảnh đại diện chính (tự động bỏ cờ IsPrimary của các ảnh khác).
2. Đăng ký các endpoints trong `RecipesEndpoints.cs`:
   - `POST /api/v1/recipes/{id}/images`
   - `DELETE /api/v1/recipes/{id}/images/{imageId}`
   - `PUT /api/v1/recipes/{id}/images/{imageId}/primary`

#### Bước 3: Hiện thực Frontend
1. Xây dựng component `src/Frontend/components/recipes/RecipeGalleryEditor.tsx`:
   - Hiển thị danh sách ảnh hiện có dạng thumbnail grid.
   - Tích hợp `ImageUploader` để upload ảnh mới lên MinIO và thêm vào gallery.
   - Nút gắn sao ⭐ "Đặt làm ảnh chính" và nút thùng rác 🗑️ xóa ảnh khỏi gallery.

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "recipe: hien thuc FR-RCP-008 quan ly gallery anh cong thuc"
git push -u origin 2312770-LNTien-Gallery-Anh
```

---

## 7. Tiêu Chí Nghiệm Thu (Definition of Done)
- [ ] Backend biên dịch không lỗi (`dotnet build CulinaryBlog.slnx`).
- [ ] Frontend không lỗi TypeScript (`npx tsc --noEmit`).
- [ ] Test trực tiếp API trên Scalar: `http://localhost:5000/scalar/v1` hoạt động chính xác.
- [ ] Các nhánh chức năng đã được đẩy lên GitHub và merge vào `main`.
