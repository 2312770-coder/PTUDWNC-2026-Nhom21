# Hướng Dẫn Chi Tiết & Phân Công Nhiệm Vụ
### Thành viên: Nguyễn Đình Tuấn (MSSV: 2312792)
### Vai trò: Storage, Steps/Ingredients & Profile

---

## 1. Thông Tin Chung
- **Họ và tên**: Nguyễn Đình Tuấn
- **MSSV**: 2312792
- **Email**: 2312792@dlu.edu.vn
- **GitHub**: [https://github.com/2312792-debug](https://github.com/2312792-debug)
- **Tổng số chức năng phụ trách**: **7 chức năng**

---

## 2. Danh Sách 7 Chức Năng Phụ Trách

| STT | Mã FR | Tên chức năng | File Backend cần làm | File Frontend cần làm |
| :---: | :--- | :--- | :--- | :--- |
| 1 | **FR-FILE-001** | Upload ảnh lên MinIO (S3) | `Infrastructure/Services/MinioFileStorageService.cs` | Component `ImageUploader.tsx` |
| 2 | **FR-FILE-002** | Xóa ảnh trên MinIO | `Infrastructure/Services/MinioFileStorageService.cs` | Xử lý nút xóa ảnh preview |
| 3 | **FR-RCP-010** | Quản lý các bước nấu (D9) | `Features/Recipes/Commands/ManageSteps/*` | Component `StepListEditor.tsx` |
| 4 | **FR-RCP-009** | Quản lý nguyên liệu (D10) | `Features/Recipes/Commands/ManageIngredients/*` | Component `IngredientListEditor.tsx` |
| 5 | **FR-AUTH-006** | Xem thông tin hồ sơ cá nhân | `Features/Auth/Queries/GetProfile/*` | Trang cá nhân `/profile` |
| 6 | **FR-AUTH-007** | Cập nhật hồ sơ & đổi Avatar | `Features/Auth/Commands/UpdateProfile/*` | Form chỉnh sửa hồ sơ cá nhân |
| 7 | **FR-JOB-002** | Background job resize ảnh | `Infrastructure/Jobs/ImageResizeJob.cs` | Lưu `thumbnailUrl` cho ảnh công thức |

---

## 3. Lộ Trình 6 Tuần (Tuần 2 → Tuần 7)

```
Tuần 2 (Đã hoàn thành):
  ✅ FR-FILE-001 — Upload ảnh lên MinIO S3 (Validate 5MB, JPG/PNG/WebP, Magic Bytes)
  ✅ FR-FILE-002 — Xóa ảnh trên MinIO (DeleteAsync & UI Uploader)

Tuần 3 (Tuần này):
  🔲 FR-RCP-010: Quản lý các bước nấu ăn (Tuân thủ quyết định D9: Tự sinh stepNumber)
  🔲 FR-RCP-009: Quản lý nguyên liệu nấu ăn (Tuân thủ quyết định D10: Cho phép null đơn vị)

Tuần 4:
  🔲 FR-AUTH-006: Xem thông tin hồ sơ cá nhân (Profile Query)
  🔲 FR-AUTH-007: Cập nhật hồ sơ cá nhân & đổi Avatar (Tích hợp MinIO Avatar)

Tuần 5:
  🔲 FR-JOB-002: Hangfire background job tự động nén & tạo thumbnail cho ảnh MinIO
  🔲 Tích hợp thành phần upload ảnh & quản lý bước nấu vào trang chi tiết công thức

Tuần 6:
  🔲 Kiểm thử tương thích MinIO Bucket, Tối ưu hóa tải ảnh, Hoàn thiện báo cáo đồ án
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 3 (TUẦN NÀY LÀM)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`, cú pháp: `2312792-NDTuan-<Ten-Chuc-Nang>`. Sau khi code xong và test build không lỗi, gửi PR để trưởng nhóm Tiến review và merge.

---

### Chức năng 1: Quản lý các bước nấu (FR-RCP-010)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312792-NDTuan-Cac-Buoc-Nau
```

#### Bước 2: Hiện thực Backend
1. Tạo thư mục `src/Backend/CulinaryBlog.Application/Features/Recipes/Commands/ManageSteps/`:
   - `AddRecipeStepCommand(Guid RecipeId, string Title, string? Description, int? StepNumber, int? TimerMinutes, string? ImageUrl)`: `IRequest<RecipeStepDto>`
   - `UpdateRecipeStepCommand(Guid RecipeId, Guid StepId, string Title, string? Description, int? TimerMinutes, string? ImageUrl)`: `IRequest<RecipeStepDto>`
   - `DeleteRecipeStepCommand(Guid RecipeId, Guid StepId)`: `IRequest<bool>`
2. **Tuân thủ Quyết định Kiến trúc D9**:
   - `Title` là bắt buộc.
   - `StepNumber` là tùy chọn (`int?`): nếu client không truyền, server tự động lấy số bước hiện tại lớn nhất + 1.
   - Kiểm tra người sửa/xóa bước nấu phải là tác giả của công thức đó hoặc Admin.
3. Đăng ký endpoints trong `RecipesEndpoints.cs`:
   - `POST /api/v1/recipes/{id}/steps`
   - `PUT /api/v1/recipes/{id}/steps/{stepId}`
   - `DELETE /api/v1/recipes/{id}/steps/{stepId}`

#### Bước 3: Hiện thực Frontend
1. Xây dựng component `src/Frontend/components/recipes/StepListEditor.tsx`:
   - Danh sách các bước nấu có đánh số thứ tự 1, 2, 3...
   - Nút "Thêm bước", Form nhập tiêu đề, hướng dẫn chi tiết, thời gian đếm ngược (phút) và nút upload ảnh minh họa cho bước đó (sử dụng component `ImageUploader` đã làm ở Tuần 2).
   - Nút sửa và xóa từng bước với hộp thoại xác nhận.

---

### Chức năng 2: Quản lý nguyên liệu (FR-RCP-009)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312792-NDTuan-Nguyen-Lieu
```

#### Bước 2: Hiện thực Backend
1. Tạo thư mục `src/Backend/CulinaryBlog.Application/Features/Recipes/Commands/ManageIngredients/`:
   - `AddRecipeIngredientCommand(Guid RecipeId, string Name, decimal? Quantity, string? Unit, string? Notes, int OrderIndex)`: `IRequest<RecipeIngredientDto>`
   - `UpdateRecipeIngredientCommand(Guid RecipeId, Guid IngredientId, string Name, decimal? Quantity, string? Unit, string? Notes, int OrderIndex)`: `IRequest<RecipeIngredientDto>`
   - `DeleteRecipeIngredientCommand(Guid RecipeId, Guid IngredientId)`: `IRequest<bool>`
2. **Tuân thủ Quyết định Kiến trúc D10**:
   - `Name` là bắt buộc.
   - `Quantity` và `Unit` có thể `null` (để hỗ trợ nguyên liệu nêm nếm gia vị: "vừa ăn", "một chút").
   - Kiểm tra quyền tác giả hoặc Admin.
3. Đăng ký endpoints trong `RecipesEndpoints.cs`:
   - `POST /api/v1/recipes/{id}/ingredients`
   - `PUT /api/v1/recipes/{id}/ingredients/{ingredientId}`
   - `DELETE /api/v1/recipes/{id}/ingredients/{ingredientId}`

#### Bước 3: Hiện thực Frontend
1. Xây dựng component `src/Frontend/components/recipes/IngredientListEditor.tsx`:
   - Danh sách nguyên liệu hiển thị theo bảng hoặc dòng (Tên, Số lượng, Đơn vị, Ghi chú).
   - Cho phép thêm nhanh nhiều nguyên liệu liên tiếp.
   - Nút xóa nguyên liệu trực tiếp.
