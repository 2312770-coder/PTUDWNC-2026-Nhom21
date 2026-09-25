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

Tuần 5:
  🔲 FR-RCP-008 — Quản lý gallery ảnh công thức (Upload/Xóa ảnh, chọn ảnh chính IsPrimary)

Tuần 6:
  🔲 FR-RCP-004 — Cập nhật thông tin công thức (UpdateRecipe)
  🔲 FR-RCP-006 — Lưu trữ công thức (ArchiveRecipe)

Tuần 7:
  🔲 FR-RCP-007 — Xóa mềm công thức (Soft Delete theo D1)
  🔲 FR-JOB-003 — Hangfire job tự động sinh sitemap.xml SEO

Tuần 8:
  🔲 Triển khai Production (Docker Compose + Nginx), Kiểm thử tích hợp toàn hệ thống, Hoàn thiện báo cáo đồ án
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (ĐÃ HOÀN THÀNH)

### Chức năng: Tạo công thức mới (FR-RCP-003)
- **Nhánh**: `2312770-LNTien-Tao-Cong-Thuc-Moi` (Đã merge vào `main`)
- **Backend**:
  - `CreateRecipeCommandHandler`: Xử lý kiểm tra tác giả `CurrentUser`, kiểm tra danh mục, sinh slug duy nhất (chống trùng URL), nạp nutrition, nguyên liệu và các bước.
  - Đăng ký route `POST /api/v1/recipes` với quyền `AuthorOrAdmin`.
- **Frontend**:
  - Trang `/dashboard/recipes/new/page.tsx` với đầy đủ thông tin chung, chọn danh mục, thông tin dinh dưỡng, danh sách nguyên liệu và các bước nấu có ảnh.

---

## 5. HƯỚNG DẪN CHI TIẾT TUẦN 3 (ĐÃ HOÀN THÀNH)

### Chức năng: Xem chi tiết danh mục kèm danh sách bài viết (FR-CAT-002)
- **Nhánh**: `2312770-LNTien-Chi-Tiet-Danh-Muc` (Đã merge vào `main`)
- **Backend**:
  - DTO `CategoryDetailDto` kèm danh sách `IReadOnlyList<RecipeListItemDto> Recipes`.
  - `GetCategoryBySlugQueryHandler`: Tìm danh mục theo slug (không phân biệt hoa thường, `!IsDeleted`), nạp các bài viết có `Status == RecipeStatus.Published && !IsDeleted`.
  - Endpoint `GET /api/v1/categories/{slug}`.
- **Frontend**:
  - Trang `app/(public)/categories/[slug]/page.tsx` gồm Breadcrumb, Banner thông tin danh mục, Lưới bài viết `RecipeCard` và Empty state khi chưa có món.

---

## 6. HƯỚNG DẪN CHI TIẾT TUẦN 4 (CHUẨN BỊ LÀM)

> ⚠️ **Quy ước nhánh**: Làm trên nhánh riêng `2312770-LNTien-Chi-Tiet-Cong-Thuc`.

### Chức năng trọng tâm: Xem chi tiết công thức nấu ăn (FR-RCP-002)

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
