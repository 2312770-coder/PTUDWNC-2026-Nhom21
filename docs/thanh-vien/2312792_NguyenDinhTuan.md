# Hướng Dẫn Chi Tiết & Phân Công Nhiệm Vụ
### Thành viên: Nguyễn Đình Tuấn (MSSV: 2312792)
### Vai trò: Storage, Steps/Ingredients, Profile & Observability

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
| 1 | **FR-FILE-001** | Upload ảnh lên MinIO (S3) | `Infrastructure/Services/MinioStorageService.cs` | Component `ImageUploader.tsx` |
| 2 | **FR-FILE-002** | Xóa ảnh trên MinIO | `Infrastructure/Services/MinioStorageService.cs` | Xử lý nút xóa ảnh preview |
| 3 | **FR-RCP-010** | Quản lý các bước nấu (D9) | `Features/Recipes/Commands/ManageSteps/*` | Component `StepListEditor.tsx` |
| 4 | **FR-RCP-009** | Quản lý nguyên liệu (D10) | `Features/Recipes/Commands/ManageIngredients/*` | Component `IngredientListEditor.tsx` |
| 5 | **FR-AUTH-006** | Xem thông tin hồ sơ cá nhân | `Features/Auth/Queries/GetProfile/*` | Trang cá nhân `/profile` |
| 6 | **FR-AUTH-007** | Cập nhật hồ sơ & đổi Avatar | `Features/Auth/Commands/UpdateProfile/*` | Form chỉnh sửa hồ sơ cá nhân |
| 7 | **FR-JOB-002** | Background job resize ảnh | `Infrastructure/Jobs/ImageResizeJob.cs` | Lưu `thumbnailUrl` cho ảnh công thức |

---

## 3. Lộ Trình Thực Hiện (Tuần 2 → Tuần 7)

```
Tuần 2 (Tuần này): FR-FILE-001 (Upload ảnh MinIO) & FR-FILE-002 (Xóa ảnh MinIO)
Tuần 3: FR-RCP-010 (Quản lý các bước nấu - D9) & FR-RCP-009 (Quản lý nguyên liệu - D10)
Tuần 4: FR-AUTH-006 (Xem Profile) & FR-AUTH-007 (Cập nhật Profile & đổi Avatar)
Tuần 5: FR-JOB-002 (Hangfire background job nén và resize ảnh tự động)
Tuần 6: Tích hợp upload ảnh trong các bước nấu và thư viện ảnh công thức
Tuần 7: Kiểm thử tổng thể MinIO bucket, luồng hồ sơ cá nhân, hoàn thiện báo cáo
```

---

## 4. HƯỚNG DẪN CHI TIẾT TUẦN 2 (TUẦN NÀY BẮT ĐẦU LÀM)

> ⚠️ **Git**: Bạn làm việc trên **nhánh cá nhân** `2312792-NguyenDinhTuan`. **Không tự merge vào `main`**

### Thiết lập nhánh cá nhân (lần đầu)
```powershell
git checkout -b 2312792-NguyenDinhTuan
git push -u origin 2312792-NguyenDinhTuan
```
*Từ những lần sau chỉ cần:* `git checkout 2312792-NguyenDinhTuan`

### Trước khi code mỗi ngày — đồng bộ với `main`
```powershell
git fetch origin
git merge origin/main
```

---

### Chức năng 1: Tải ảnh lên MinIO (FR-FILE-001)

#### Bước 1: Chuyển sang nhánh cá nhân của bạn
```powershell
git checkout 2312792-NguyenDinhTuan
```

#### Bước 2: Hiện thực Backend
1. Mở file `src/Backend/CulinaryBlog.Infrastructure/Services/MinioStorageService.cs`.
2. Hiện thực logic upload bằng AWS S3 SDK hoặc Minio Client:
   - Validate file dung lượng $\le 5$ MB.
   - Kiểm tra Content-Type hợp lệ: `image/jpeg`, `image/png`, `image/webp`.
   - Sinh tên file duy nhất: `Guid.NewGuid() + extension`.
   - Upload vào bucket `culinary-blog` trong MinIO (`localhost:9000`).
   - Trả về URL ảnh công khai.
3. Mở endpoint upload ảnh tại `src/Backend/CulinaryBlog.API/Endpoints/RecipesEndpoints.cs`.

#### Bước 3: Hiện thực Frontend
1. Xây dựng component upload ảnh dùng chung: `src/Frontend/components/ui/ImageUploader.tsx`.
2. Có khung kéo thả ảnh (drag & drop), hiển thị preview ảnh sau khi upload thành công, hiển thị URL ảnh đã upload.

#### Bước 4: Commit và push
```powershell
dotnet build CulinaryBlog.slnx
cd src\Frontend; npm run lint; cd ..\..

git add .
git commit -m "file: hien thuc FR-FILE-001 upload anh len minio"
git push origin 2312792-NguyenDinhTuan
```


---

### Chức năng 2: Xóa ảnh trên MinIO (FR-FILE-002)

#### Bước 1: Tiếp tục trên nhánh cá nhân
```powershell
git checkout 2312792-NguyenDinhTuan
```

#### Bước 2: Hiện thực Backend & Frontend
1. Backend: Bổ sung phương thức `DeleteFileAsync(string fileUrl)` trong `MinioStorageService.cs`.
   - Phân tích tên object từ URL.
   - Gửi yêu cầu xóa tới MinIO qua `RemoveObjectAsync`.
2. Frontend: Nút bấm hình thùng rác hoặc dấu X trên ảnh preview để xóa ảnh đã chọn, có hộp thoại xác nhận trước khi xóa.

#### Bước 3: Commit và push
```powershell
git add .
git commit -m "file: hien thuc FR-FILE-002 xoa anh minio"
git push origin 2312792-NguyenDinhTuan
```


---

## 5. Tiêu Chí Nghiệm Thu (Definition of Done)
- [ ] Upload được ảnh JPG/PNG/WebP lên MinIO; ảnh xuất hiện trong bucket tại MinIO Console `http://localhost:9001` (user: `minioadmin` / `minioadmin`).
- [ ] Truy cập trực tiếp link ảnh trên trình duyệt hiển thị sắc nét.
- [ ] File quá 5MB hoặc sai định dạng bị chặn và trả về lỗi rõ ràng.
- [ ] Nhánh cá nhân `2312792-NguyenDinhTuan` đã được đẩy lên GitHub.
- [ ] Cả backend và frontend đều biên dịch sạch bóng lỗi.

