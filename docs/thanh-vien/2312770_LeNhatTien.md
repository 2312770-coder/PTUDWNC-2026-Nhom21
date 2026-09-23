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
| 1 | **FR-RCP-003** | Tạo công thức mới (Draft) | `Features/Recipes/Commands/CreateRecipe/*` | `app/dashboard/recipes/new/page.tsx` |
| 2 | **FR-CAT-002** | Xem chi tiết danh mục + bài viết | `Features/Categories/Queries/GetCategoryBySlug/*` | `app/(public)/categories/[slug]/page.tsx` |
| 3 | **FR-RCP-008** | Quản lý gallery ảnh công thức | `Features/Recipes/Commands/ManageImages/*` | `components/recipes/RecipeGalleryEditor.tsx` |
| 4 | **FR-RCP-004** | Cập nhật thông tin công thức | `Features/Recipes/Commands/UpdateRecipe/*` | `app/dashboard/recipes/[id]/edit/page.tsx` |
| 5 | **FR-RCP-006** | Lưu trữ công thức (Archive) | `Features/Recipes/Commands/ArchiveRecipe/*` | `components/recipes/RecipeStatusBadge.tsx` |
| 6 | **FR-RCP-007** | Xóa mềm công thức (D1) | `Features/Recipes/Commands/DeleteRecipe/*` | Nút xóa trong danh sách bài của tôi |
| 7 | **FR-JOB-003** | Tự động sinh `sitemap.xml` | `Infrastructure/Jobs/SitemapJob.cs` | Route `/sitemap.xml` |

---

## 3. Lộ Trình 6 Tuần (Tuần 2 → Tuần 7)

```
Tuần 2 (Đã hoàn thành):
  ✅ Dựng hạ tầng — Docker 5 dịch vụ, Database Seeder (2 users, 23 danh mục, 100 công thức mẫu)
  ✅ Layout chung — Navbar, Footer, RecipeCard, Layout.tsx, Home Page
  ✅ FR-RCP-003 — Tạo công thức mới trạng thái Draft (Backend & Frontend /dashboard/recipes/new)

Tuần 3 (Tuần này):
  🔲 FR-CAT-002: Xem chi tiết danh mục kèm danh sách bài viết (Backend + Frontend)
  🔲 FR-RCP-008: Quản lý gallery ảnh công thức (Upload/Update/Delete nhiều ảnh, chọn ảnh chính IsPrimary)

Tuần 4:
  🔲 FR-RCP-004: Cập nhật thông tin công thức (UpdateRecipe)
  🔲 FR-RCP-006: Lưu trữ công thức (ArchiveRecipe)

Tuần 5:
  🔲 FR-RCP-007: Xóa mềm công thức (Soft Delete theo D1)
  🔲 FR-JOB-003: Hangfire job tự động sinh sitemap.xml SEO

Tuần 6:
  🔲 Tổng kết toàn bộ hệ thống, Review code & Merge PRs lần cuối, Hoàn thiện báo cáo đồ án
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 3 (TUẦN NÀY LÀM)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tách từ `main`, định dạng: `2312770-LNTien-<Ten-Chuc-Nang>`.

---

### Chức năng 1: Xem chi tiết danh mục kèm danh sách bài viết (FR-CAT-002)

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
   - `DeleteRecipeImageCommand(Guid RecipeId, Guid ImageId)`: Xóa ảnh khỏi gallery (lưu ý: kiểm tra người thực hiện là tác giả bài viết).
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
