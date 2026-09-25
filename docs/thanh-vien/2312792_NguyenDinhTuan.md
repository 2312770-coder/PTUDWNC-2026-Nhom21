# Hướng Dẫn Chi Tiết & Phân Công Nhiệm Vụ
### Thành viên: Nguyễn Đình Tuấn (MSSV: 2312792)
### Vai trò: Storage, Recipe Details & Profile

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
| 4 | **FR-RCP-009** | Quản lý nguyên liệu công thức (D10) | `Features/Recipes/Commands/ManageIngredients/*` | Component `IngredientListEditor.tsx` |
| 5 | **FR-AUTH-006**<br>**FR-AUTH-007** | Xem & Cập nhật Hồ sơ cá nhân | `Features/Auth/Queries/GetProfile/*`<br>`Features/Auth/Commands/UpdateProfile/*` | Trang cá nhân `/profile` & Form sửa hồ sơ |
| 6 | **FR-JOB-002** | Background job nén & resize ảnh | `Infrastructure/Jobs/ImageResizeJob.cs` | Lưu `thumbnailUrl` cho ảnh công thức |
| 7 | Tối ưu Media | Tích hợp hoàn thiện Media & Profile | `Infrastructure/Services/*` | Kiểm thử tương thích toàn bộ ảnh S3 |

---

## 3. Lộ Trình Thực Hiện (Tuần 2 → Tuần 8)

```
Tuần 2 (Đã hoàn thành & merge main):
  ✅ FR-FILE-001 — Upload ảnh lên MinIO S3 (Validate 5MB, JPG/PNG/WebP, Magic Bytes)
  ✅ FR-FILE-002 — Xóa ảnh trên MinIO (DeleteAsync & UI Uploader)

Tuần 3 (Đã hoàn thành & merge main):
  ✅ FR-RCP-010 — Quản lý các bước nấu (Tuân thủ quyết định D9: server tự sinh stepNumber liên tục)

Tuần 4 (Tuần tới):
  🔲 FR-RCP-009 — Quản lý nguyên liệu công thức (Tuân thủ quyết định D10: cho phép null unit nêm gia vị)

Tuần 5:
  🔲 FR-AUTH-006 & FR-AUTH-007 — Xem và Cập nhật hồ sơ cá nhân (Đổi avatar MinIO, Bio, DisplayName)

Tuần 6:
  🔲 FR-JOB-002 — Hangfire background job tự động nén & tạo thumbnail cho ảnh MinIO

Tuần 7:
  🔲 Kiểm thử toàn diện Media MinIO, xử lý ảnh lỗi, tối ưu tốc độ tải ảnh S3

Tuần 8:
  🔲 Triển khai Production, Tối ưu hóa hiệu năng lưu trữ ảnh MinIO, Hoàn thiện báo cáo đồ án
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (ĐÃ HOÀN THÀNH)

### Chức năng 1: Tải ảnh lên MinIO (FR-FILE-001)
- **Nhánh**: `2312792-ndtuan-upload-minio` (Đã merge vào `main`)
- **Backend**:
  - `MinioFileStorageService.UploadAsync`: Validate file dung lượng $\le 5$ MB, định dạng JPG/PNG/WebP/AVIF, kiểm tra Magic Bytes, sinh tên file UUID và upload vào bucket `culinary-blog`.
- **Frontend**:
  - Component `ImageUploader.tsx`: Kéo thả hoặc click chọn ảnh, preview ảnh sau upload, hiển thị link URL ảnh.

### Chức năng 2: Xóa ảnh trên MinIO (FR-FILE-002)
- **Nhánh**: `2312792-ndtuan-delete-minio` (Đã merge vào `main`)
- **Backend**:
  - `MinioFileStorageService.DeleteAsync`: Xóa object trên bucket MinIO.
- **Frontend**:
  - Nút xóa trên ảnh preview, hộp thoại xác nhận trước khi xóa.

---

## 5. HƯỚNG DẪN CHI TIẾT TUẦN 3 (ĐÃ HOÀN THÀNH)

### Chức năng: Quản lý các bước nấu (FR-RCP-010)
- **Nhánh**: `2312792-NDTuan-Cac-Buoc-Nau` (Đã merge vào `main`)
- **Backend**:
  - Các Command thêm/sửa/xóa bước nấu trong `Features/Recipes/Commands/ManageSteps/*`.
  - **Quyết định D9**: `Title` bắt buộc, `StepNumber` tùy chọn (`int?`) — nếu client không truyền thì server tự động lấy số bước lớn nhất + 1. Tự động renumber khi xóa bước.
  - Endpoints: `POST /api/v1/recipes/{id}/steps`, `PUT /api/v1/recipes/{id}/steps/{stepId}`, `DELETE /api/v1/recipes/{id}/steps/{stepId}`.
- **Frontend**:
  - Component `StepListEditor.tsx` hiển thị danh sách các bước có hẹn giờ, hướng dẫn và tích hợp upload ảnh MinIO.

---

## 6. HƯỚNG DẪN CHI TIẾT TUẦN 4 (CHUẨN BỊ LÀM)

> ⚠️ **Quy ước nhánh**: Làm trên nhánh riêng `2312792-NDTuan-Nguyen-Lieu`.

### Chức năng trọng tâm: Quản lý nguyên liệu công thức (FR-RCP-009)

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
   - Kiểm tra quyền: Chỉ tác giả của bài viết (`AuthorId == currentUser.UserId`) hoặc Admin mới được thêm/sửa/xóa nguyên liệu.
3. Đăng ký endpoints trong `RecipesEndpoints.cs`:
   - `POST /api/v1/recipes/{id}/ingredients`
   - `PUT /api/v1/recipes/{id}/ingredients/{ingredientId}`
   - `DELETE /api/v1/recipes/{id}/ingredients/{ingredientId}`

#### Bước 3: Hiện thực Frontend
1. Xây dựng component `src/Frontend/components/recipes/IngredientListEditor.tsx`:
   - Danh sách nguyên liệu hiển thị theo bảng (Tên, Định lượng, Đơn vị, Ghi chú).
   - Nút "Thêm nguyên liệu", cho phép nhập nhanh nhiều dòng liên tiếp.
   - Nút sửa và xóa từng nguyên liệu.
