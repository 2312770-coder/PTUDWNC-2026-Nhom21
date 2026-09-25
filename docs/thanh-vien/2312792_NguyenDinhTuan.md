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
| 5 | **FR-AUTH-006** | Xem thông tin hồ sơ cá nhân | `Features/Auth/Queries/GetProfile/*` | Trang cá nhân `/profile` |
| 6 | **FR-AUTH-007** | Cập nhật hồ sơ & đổi Avatar MinIO | `Features/Auth/Commands/UpdateProfile/*` | Form chỉnh sửa hồ sơ & upload avatar |
| 7 | **FR-JOB-002** | Background job nén & resize ảnh | `Infrastructure/Jobs/ImageResizeJob.cs` | Lưu `thumbnailUrl` cho ảnh công thức |

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
  🔲 FR-AUTH-006 — Xem thông tin hồ sơ cá nhân (Query Profile /auth/me)

Tuần 5:
  🔲 FR-AUTH-007 — Cập nhật hồ sơ & đổi Avatar (Tích hợp MinIO Avatar và cập nhật DisplayName, Bio)

Tuần 6:
  🔲 FR-JOB-002 — Hangfire background job tự động nén & tạo thumbnail cho ảnh MinIO

Tuần 7:
  🔲 Tối ưu hóa toàn diện Media MinIO, xử lý ảnh lỗi, tối ưu tốc độ tải ảnh S3

Tuần 8:
  🔲 Triển khai Production, Tối ưu hóa hiệu năng lưu trữ ảnh MinIO, Hoàn thiện báo cáo đồ án
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (ĐÃ HOÀN THÀNH)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`, cú pháp: `2312792-NDTuan-<Ten-Chuc-Nang>`. **Không tự merge vào `main`** — báo trưởng nhóm Tiến để Tiến review và merge giúp.

---

### Chức năng 1: Tải ảnh lên MinIO (FR-FILE-001)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312792-NDTuan-Upload-Minio
```

#### Bước 2: Hiện thực Backend
1. Mở file `src/Backend/CulinaryBlog.Infrastructure/Services/MinioStorageService.cs`.
2. Hiện thực logic upload bằng AWS S3 SDK hoặc Minio Client:
   - Validate file dung lượng <= 5 MB.
   - Kiểm tra Content-Type hợp lệ: `image/jpeg`, `image/png`, `image/webp`.
   - Sinh tên file duy nhất: `Guid.NewGuid() + extension`.
   - Upload vào bucket `culinary-blog` trong MinIO (`localhost:9000`).
   - Trả về URL ảnh công khai.
3. Mở endpoint upload ảnh tại `src/Backend/CulinaryBlog.API/Endpoints/RecipesEndpoints.cs`.

#### Bước 3: Hiện thực Frontend
1. Xây dựng component upload ảnh dùng chung: `src/Frontend/components/ui/ImageUploader.tsx`.
2. Có khung kéo thả ảnh (drag & drop), hiển thị preview ảnh sau khi upload thành công, hiển thị URL ảnh đã upload.

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npm run lint; cd ..\..

git add .
git commit -m "file: hien thuc FR-FILE-001 upload anh len minio"
git push -u origin 2312792-NDTuan-Upload-Minio
```

---

### Chức năng 2: Xóa ảnh trên MinIO (FR-FILE-002)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312792-NDTuan-Xoa-Anh-Minio
```

#### Bước 2: Hiện thực Backend
1. Mở file `src/Backend/CulinaryBlog.Infrastructure/Services/MinioFileStorageService.cs`.
2. Hiện thực hàm `DeleteAsync(string fileUrl, CancellationToken ct)`:
   - Phân tích tên object key từ đường dẫn URL.
   - Gọi phương thức xóa đối tượng `RemoveObjectAsync` của MinIO Client.

#### Bước 3: Hiện thực Frontend
1. Cập nhật component `ImageUploader.tsx`:
   - Thêm nút xóa (icon thùng rác hoặc nút hủy) trên ảnh đã tải lên.
   - Khi bấm, gọi API DELETE `/api/v1/files` và xóa preview trên giao diện.

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npm run lint; cd ..\..

git add .
git commit -m "file: hien thuc FR-FILE-002 xoa anh tren minio"
git push -u origin 2312792-NDTuan-Xoa-Anh-Minio
```

---

## 5. HƯỚNG DẪN CHI TIẾT TUẦN 3 (ĐÃ HOÀN THÀNH)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`, cú pháp: `2312792-NDTuan-<Ten-Chuc-Nang>`. Sau khi code xong và test build không lỗi, gửi PR để trưởng nhóm Tiến review và merge.

---

### Chức năng: Quản lý các bước nấu (FR-RCP-010)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312792-NDTuan-Cac-Buoc-Nau
```

#### Bước 2: Hiện thực Backend
1. Tạo thư mục `src/Backend/CulinaryBlog.Application/Features/Recipes/Commands/ManageSteps/`:
   - `AddRecipeStepCommand(Guid RecipeId, string Title, string? Description, int? StepNumber, int? TimerMinutes, string? ImageUrl)`: `IRequest<RecipeStepDto>`
   - `UpdateRecipeStepCommand(Guid RecipeId, Guid StepId, string Title, string? Description, int? StepNumber, int? TimerMinutes, string? ImageUrl)`: `IRequest<RecipeStepDto>`
   - `DeleteRecipeStepCommand(Guid RecipeId, Guid StepId)`: `IRequest<bool>`
2. **Tuân thủ Quyết định Kiến trúc D9**:
   - `Title` là bắt buộc.
   - `StepNumber` là tùy chọn (`int?`): nếu client không truyền, server tự động lấy số bước hiện tại lớn nhất + 1.
   - Tự động renumber thứ tự khi xóa bước nấu.
   - Kiểm tra người sửa/xóa bước nấu phải là tác giả của công thức đó hoặc Admin.
3. Đăng ký endpoints trong `RecipesEndpoints.cs`:
   - `GET /api/v1/recipes/{id}/steps`
   - `POST /api/v1/recipes/{id}/steps`
   - `PUT /api/v1/recipes/{id}/steps/{stepId}`
   - `DELETE /api/v1/recipes/{id}/steps/{stepId}`

#### Bước 3: Hiện thực Frontend
1. Xây dựng component `src/Frontend/components/recipes/StepListEditor.tsx`:
   - Danh sách các bước nấu có đánh số thứ tự 1, 2, 3...
   - Nút "Thêm bước", Form nhập tiêu đề, hướng dẫn chi tiết, thời gian đếm ngược (phút) và nút upload ảnh minh họa cho bước đó (sử dụng component `ImageUploader` đã làm ở Tuần 2).
   - Nút sửa và xóa từng bước với hộp thoại xác nhận.

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "recipe: hien thuc FR-RCP-010 quan ly cac buoc nau"
git push -u origin 2312792-NDTuan-Cac-Buoc-Nau
```

---

## 6. HƯỚNG DẪN CHI TIẾT TUẦN 4 (CHUẨN BỊ LÀM)

> ⚠️ **Quy ước nhánh**: Mỗi chức năng làm trên **một nhánh riêng** tự tạo từ `main`.

---

### Chức năng 1: Quản lý nguyên liệu (FR-RCP-009)

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

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "recipe: hien thuc FR-RCP-009 quan ly nguyen lieu cong thuc"
git push -u origin 2312792-NDTuan-Nguyen-Lieu
```

---

### Chức năng 2: Xem thông tin hồ sơ cá nhân (FR-AUTH-006)

#### Bước 1: Tạo nhánh mới từ `main`
```powershell
git checkout main
git pull origin main
git checkout -b 2312792-NDTuan-Xem-Ho-So
```

#### Bước 2: Hiện thực Backend
1. Thư mục `Features/Auth/Queries/GetProfile/`:
   - `GetProfileQuery()`: `IRequest<UserDto>`
   - `GetProfileQueryHandler`: Lấy `UserId` từ `ICurrentUser`, truy vấn User từ `UserManager`, trả về `{ id, email, displayName, bio, avatarUrl, roles }`. Không bao giờ trả về password hash.
2. Đăng ký endpoint `GET /api/v1/auth/me` trong `AuthEndpoints.cs` (`RequireAuthorization`).

#### Bước 3: Hiện thực Frontend
1. Tạo trang `src/Frontend/app/(public)/profile/page.tsx`:
   - Hiển thị thông tin người dùng: Avatar lớn, DisplayName, Email, Bio giới thiệu bản thân và các vai trò (Roles).

#### Bước 4: Kiểm tra và đẩy nhánh lên GitHub
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npx tsc --noEmit; cd ..\..

git add .
git commit -m "auth: hien thuc FR-AUTH-006 xem ho so ca nhan"
git push -u origin 2312792-NDTuan-Xem-Ho-So
```

---

## 7. Tiêu Chí Nghiệm Thu (Definition of Done)
- [ ] Upload & Xóa ảnh MinIO hoạt động trơn tru.
- [ ] Quản lý các bước nấu và nguyên liệu hoạt động đúng theo D9 và D10.
- [ ] Xem hồ sơ cá nhân `/profile` trả về đúng thông tin user hiện tại.
- [ ] Các nhánh chức năng đã được đẩy lên GitHub và merge vào `main`.
