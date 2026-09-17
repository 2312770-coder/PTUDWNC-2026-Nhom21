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
| 1 | **FR-RCP-003** | Tạo công thức mới (Draft) | `Features/Recipes/Commands/CreateRecipe/*` | `app/(author)/recipes/create/page.tsx` |
| 2 | **FR-CAT-002** | Xem chi tiết danh mục + bài viết | `Features/Categories/Queries/GetCategoryBySlug/*` | `app/(public)/categories/[slug]/page.tsx` |
| 3 | **FR-RCP-004** | Cập nhật thông tin công thức | `Features/Recipes/Commands/UpdateRecipe/*` | `app/(author)/recipes/[id]/edit/page.tsx` |
| 4 | **FR-RCP-008** | Quản lý gallery ảnh công thức | `Features/Recipes/Commands/ManageImages/*` | `components/recipes/RecipeGalleryEditor.tsx` |
| 5 | **FR-RCP-006** | Lưu trữ công thức (Archive) | `Features/Recipes/Commands/ArchiveRecipe/*` | `components/recipes/RecipeStatusBadge.tsx` |
| 6 | **FR-RCP-007** | Xóa mềm công thức (D1) | `Features/Recipes/Commands/DeleteRecipe/*` | Nút xóa trong danh sách bài của tôi |
| 7 | **FR-JOB-003** | Tự động sinh `sitemap.xml` | `Infrastructure/Jobs/SitemapJob.cs` | Route `/sitemap.xml` |

---

## 3. Lộ Trình Thực Hiện (Tuần 2 → Tuần 7)

```
Tuần 2 (Tuần này):
  ✅ Dựng hạ tầng — Database Seeder (2 users, 6 danh mục, 6 công thức mẫu), Query Handlers
  ✅ Layout chung — Navbar, Footer, RecipeCard, Layout.tsx
  ✅ Trang chủ — Hero section, Category filter chips, Recipe grid, Features section, CTA banner
  🔲 FR-RCP-003 — Tạo công thức mới trạng thái Draft
  🔲 FR-CAT-002 — Xem chi tiết danh mục kèm danh sách bài viết

Tuần 3: FR-RCP-004 (Sửa công thức) & FR-RCP-008 (Gallery ảnh công thức)
Tuần 4: FR-RCP-006 (Lưu trữ công thức Archive)
Tuần 5: FR-RCP-007 (Xóa mềm công thức - Soft Delete theo D1)
Tuần 6: FR-JOB-003 (Hangfire background job sinh sitemap.xml tự động)
Tuần 7: Kiểm thử tổng thể, review toàn bộ PRs của nhóm, hoàn thiện tài liệu báo cáo
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (TUẦN NÀY BẮT ĐẦU LÀM)

> ⚠️ **Git**: Bạn làm việc trên **nhánh cá nhân** `2312770-LeNhatTien`. Sau khi push xong, bạn tự review rồi merge vào `main` với tư cách trưởng nhóm.

### Thiết lập nhánh cá nhân (lần đầu)
```powershell
git checkout -b 2312770-LeNhatTien
git push -u origin 2312770-LeNhatTien
```
*Từ những lần sau chỉ cần:* `git checkout 2312770-LeNhatTien`

### Trước khi code mỗi ngày — đồng bộ với `main`
```powershell
git fetch origin
git merge origin/main
```

---

### Chức năng 1: Tạo công thức mới (FR-RCP-003)

#### Bước 1: Chuyển sang nhánh cá nhân của bạn
```powershell
git checkout 2312770-LeNhatTien
```

#### Bước 2: Hiện thực Backend
1. Mở file `src/Backend/CulinaryBlog.Application/Features/Recipes/Commands/CreateRecipe/CreateRecipeCommandHandler.cs`.
2. Thay thế `throw new NotImplementedException` bằng logic:
   - Lấy `AuthorId` từ `_currentUser.UserId`.
   - Tạo Recipe: `Recipe.Create(request.Title, request.Description, request.Instructions, request.CategoryId, authorId, request.PrepTime, request.CookTime, request.Servings, request.Difficulty)`.
   - Lưu vào database qua `_recipeRepository.AddAsync(recipe, ct)` và `_recipeRepository.SaveChangesAsync(ct)`.
   - Trả về DTO kết quả.
3. Kiểm tra validation trong `CreateRecipeCommandValidator.cs`.

#### Bước 3: Hiện thực Frontend
1. Tạo giao diện trang tạo bài viết: `src/Frontend/app/(public)/recipes/create/page.tsx`.
2. Form gồm: Tiêu đề, Mô tả, Hướng dẫn chung, Danh mục (dropdown lấy từ `categoriesApi.getAll()`), Thời gian chuẩn bị, Thời gian nấu, Khẩu phần, Độ khó.
3. Nút bấm "Lưu bản nháp" gửi request `POST /api/v1/recipes`.

#### Bước 4: Commit và push
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npm run lint; cd ..\..

git add .
git commit -m "recipe: hien thuc FR-RCP-003 tao cong thuc moi"
git push origin 2312770-LeNhatTien
```

---

### Chức năng 2: Xem chi tiết danh mục kèm công thức (FR-CAT-002)

#### Bước 1: Tiếp tục trên nhánh cá nhân
```powershell
git checkout 2312770-LeNhatTien
```

#### Bước 2: Hiện thực Backend & Frontend
1. Backend: Mở `GetCategoryBySlugQueryHandler.cs` trong `Features/Categories/Queries/GetCategoryBySlug/`.
   - Lấy category theo `slug`.
   - Lấy danh sách recipes thuộc category đó (`Status == Published`).
2. Frontend: Tạo trang `src/Frontend/app/(public)/categories/[slug]/page.tsx` hiển thị banner danh mục và danh sách `RecipeCard` tương ứng.

#### Bước 3: Commit và push
```powershell
git add .
git commit -m "category: hien thuc FR-CAT-002 xem chi tiet danh muc"
git push origin 2312770-LeNhatTien
```

---

## 5. Tiêu Chí Nghiệm Thu (Definition of Done)
- [ ] Backend biên dịch không lỗi (`dotnet build CulinaryBlog.slnx`).
- [ ] Frontend không lỗi lint và TypeScript (`npm run lint` & `npx tsc --noEmit`).
- [ ] Test trực tiếp API trên Scalar: `http://localhost:5000/scalar/v1` hoạt động chính xác.
- [ ] Nhánh cá nhân `2312770-LeNhatTien` đã được đẩy lên GitHub.

