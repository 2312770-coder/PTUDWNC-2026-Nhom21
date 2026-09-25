# Culinary Blog

Đồ án môn Phát triển Ứng dụng Web Nâng cao — Nhóm 21

Web chia sẻ công thức nấu ăn chuẩn vị Việt Nam. Backend .NET 10 (Clean Architecture) và Frontend Next.js 15 (App Router) là hai ứng dụng tách rời, giao tiếp qua REST API. Toàn bộ yêu cầu và quyết định kỹ thuật lấy từ [SRS v1.0.0](./docs/SRS_Culinary_Blog_v1.0.0.md) và [DECISIONS.md](./docs/DECISIONS.md).

---

## 📌 TIẾN ĐỘ THỰC HIỆN CHI TIẾT THEO TUẦN

### ✅ TUẦN 2: Khởi tạo Hạ tầng, Dữ liệu mẫu & Tính năng Nền tảng (ĐÃ HOÀN THÀNH & MERGE `main`)

| Thành viên | Mã FR | Tên chức năng | Tên nhánh | Nội dung đã hoàn thành | Trạng thái |
| :--- | :---: | :--- | :--- | :--- | :---: |
| **Lê Nhật Tiến** | — | Hạ tầng Docker & Database Seeder | `2312770-LNTien-Database` | Cấu hình 5 container Docker, thiết kế Entity, áp dụng Migration PostgreSQL, viết Seeder nạp 2 role, 2 user, 23 categories, 100 recipes chuẩn dữ liệu Việt. Viết tài liệu `docs/DATABASE.md`. | ✅ Đã merge |
| **Lê Nhật Tiến** | `FR-RCP-003` | Tạo công thức mới (trạng thái Draft) | `2312770-LNTien-Tao-Cong-Thuc-Moi` | Backend xử lý slug duy nhất, lưu nguyên liệu, các bước nấu, dinh dưỡng. Frontend form `/dashboard/recipes/new` hoàn chỉnh (816 dòng code). | ✅ Đã merge |
| **Lâm Văn Đức** | `FR-AUTH-001` | Đăng ký tài khoản mới | `2314299-LVDuc-Dang-Ky` | Backend Identity PBKDF2 hash, gán role Author, cấp JWT & Refresh Token. Frontend trang `/register`. | ✅ Đã merge |
| **Nguyễn Viết Toàn** | `FR-CAT-001` | Xem danh sách danh mục ẩm thực | `2312777-NVToan-Danh-Sach-Danh-Muc` | Backend EF Core query theo `OrderIndex`, đếm số recipe Published. Frontend module `categories.ts` & trang `/categories`. | ✅ Đã merge |
| **Nguyễn Đình Tuấn** | `FR-FILE-001`<br>`FR-FILE-002` | Upload & Xóa ảnh trên MinIO (S3) | `2312792-ndtuan-upload-minio`<br>`2312792-ndtuan-delete-minio` | `MinioFileStorageService` kiểm tra Magic Bytes, giới hạn 5MB, JPG/PNG/WebP/AVIF. Component UI `ImageUploader.tsx` kéo thả ảnh và xóa ảnh. | ✅ Đã merge |

---

### 🚀 TUẦN 3: Xác thực, Danh mục & Chi tiết Công thức (ĐÃ HOÀN THÀNH & MERGE `main`)

> 💡 **Quy tắc**: Mỗi thành viên phụ trách **đúng 1 chức năng trọng tâm** trên nhánh độc lập, tự kiểm tra 0 lỗi trước khi gửi PR cho trưởng nhóm Tiến review và merge.

| # | Thành viên | Mã FR | Tên chức năng | Tên nhánh | Kết quả đã thực hiện & merge vào `main` |
| :---: | :--- | :---: | :--- | :--- | :--- |
| 1 | **Lê Nhật Tiến** | `FR-CAT-002` | Xem chi tiết danh mục + bài viết | `2312770-LNTien-Chi-Tiet-Danh-Muc` | Query `GetCategoryBySlug`, nạp công thức Published (D1 Soft Delete). Giao diện `/categories/[slug]` kèm Breadcrumb, Banner danh mục, Lưới RecipeCard và Empty state. |
| 2 | **Lâm Văn Đức** | `FR-AUTH-002` | Đăng nhập Email + Rate Limiting | `2314299-LVDuc-Dang-Nhap` | Xác thực mật khẩu, xử lý lockout sau 5 lần sai, cấp JWT Access + Refresh token. Cấu hình ASP.NET Core Rate Limiting 5 req/phút chống dò pass. Giao diện `/login`. |
| 3 | **Nguyễn Viết Toàn** | `FR-CAT-003` | Admin tạo danh mục mới | `2312777-NVToan-Tao-Danh-Muc` | Command `CreateCategory`, kiểm tra trùng tên, sinh slug chuẩn SEO, phân quyền `AdminOnly`. Giao diện trang quản trị thêm danh mục `/admin/categories`. |
| 4 | **Nguyễn Đình Tuấn** | `FR-RCP-010` | Quản lý các bước nấu (D9) | `2312792-NDTuan-Cac-Buoc-Nau` | Thêm, sửa, xóa bước nấu (D9: server tự sinh `stepNumber` liên tục nếu client không truyền). Component `StepListEditor.tsx` hẹn giờ và upload ảnh MinIO. |

---

### 📅 KẾ HOẠCH TUẦN 4: Chi tiết Món ăn, Quản lý Nguyên liệu, Đăng xuất & Cache

| Thành viên | Mã FR | Tên chức năng | Tên nhánh | Backend cần làm | Frontend cần làm |
| :--- | :---: | :--- | :--- | :--- | :--- |
| **Lê Nhật Tiến** | **`FR-RCP-002`** | **Xem chi tiết công thức nấu ăn** | `2312770-LNTien-Chi-Tiet-Cong-Thuc` | `GetRecipeBySlugQueryHandler` (truy vấn đầy đủ nguyên liệu, các bước, ảnh, dinh dưỡng, tác giả) | Trang `/recipes/[slug]`: Hero ảnh lớn, bảng dinh dưỡng, checklist nguyên liệu, timeline các bước nấu kèm hẹn giờ |
| **Lâm Văn Đức** | `FR-AUTH-005` | Đăng xuất & Thu hồi phiên làm việc | `2314299-LVDuc-Dang-Xuat` | `LogoutCommandHandler` (thu hồi refresh token trong CSDL, vô hiệu hóa phiên) | Nút "Đăng xuất" trên dropdown Navbar, xóa token và điều hướng về trang đăng nhập |
| **Nguyễn Viết Toàn** | Kỹ thuật Cache | Tích hợp Redis Caching cho Danh mục | `2312777-NVToan-Cache-Danh-Muc` | Inject `ICacheService` vào `GetCategoriesQueryHandler`, kiểm tra cache HIT/MISS (TTL 30 phút). Gọi xóa cache khi tạo danh mục mới | Đo lường và kiểm tra tốc độ phản hồi API danh mục khi có cache Redis |
| **Nguyễn Đình Tuấn** | `FR-RCP-009` | Quản lý nguyên liệu công thức (D10) | `2312792-NDTuan-Nguyen-Lieu` | Các Command thêm/sửa/xóa nguyên liệu trong `ManageIngredients/*` (D10: cho phép null unit nêm gia vị) | Component `IngredientListEditor.tsx` cho phép thêm, sửa, xóa nguyên liệu và định lượng |

---

### 📅 KẾ HOẠCH TUẦN 5: Thư viện Ảnh, Hồ sơ Cá nhân, Cập nhật & Xóa Danh mục

| Thành viên | Mã FR | Tên chức năng | Tên nhánh | Backend cần làm | Frontend cần làm |
| :--- | :---: | :--- | :--- | :--- | :--- |
| **Lê Nhật Tiến** | `FR-RCP-008` | Quản lý Gallery ảnh công thức | `2312770-LNTien-Gallery-Anh` | Thêm, xóa ảnh bài viết, đổi ảnh đại diện chính `IsPrimary` trong `ManageImages/*` | Component `RecipeGalleryEditor.tsx` xem lưới ảnh, chọn ảnh chính ⭐, xóa ảnh |
| **Lâm Văn Đức** | `FR-AUTH-004` | Refresh Token Rotation | `2314299-LVDuc-Refresh-Token` | Cấp mới access token từ refresh token còn hạn, phát hiện tấn công tái sử dụng (Reuse Detection) | Axios interceptor trong `client.ts` tự bắt lỗi 401 để refresh token ngầm |
| **Nguyễn Viết Toàn** | `FR-CAT-004`<br>`FR-CAT-005` | Cập nhật (D12) & Xóa mềm danh mục (D1) | `2312777-NVToan-Quan-Ly-Danh-Muc` | Sửa danh mục (D12: giữ nguyên slug SEO), Xóa mềm danh mục (D1: kiểm tra không có recipe mới cho xóa) | Form sửa danh mục, nút xóa có modal xác nhận trong trang Admin `/admin/categories` |
| **Nguyễn Đình Tuấn** | `FR-AUTH-006`<br>`FR-AUTH-007` | Xem & Cập nhật Hồ sơ cá nhân | `2312792-NDTuan-Ho-So-Ca-Nhan` | Query xem profile `/auth/me` và Command sửa profile (đổi DisplayName, Bio, AvatarUrl MinIO) | Trang `/profile` xem và chỉnh sửa thông tin tài khoản, upload avatar cá nhân |

---

### 📅 KẾ HOẠCH TUẦN 6: Tìm kiếm Thông minh, Sửa/Lưu trữ/Xuất bản Công thức & Google OAuth

| Thành viên | Mã FR | Tên chức năng | Tên nhánh | Backend cần làm | Frontend cần làm |
| :--- | :---: | :--- | :--- | :--- | :--- |
| **Lê Nhật Tiến** | `FR-RCP-004`<br>`FR-RCP-006` | Cập nhật thông tin & Lưu trữ (Archive) | `2312770-LNTien-Sua-Luu-Tru-Cong-Thuc` | Sửa thông tin chung công thức và lệnh chuyển đổi trạng thái `ArchiveRecipe` (chỉ tác giả hoặc Admin) | Giao diện chỉnh sửa công thức `/dashboard/recipes/[id]/edit` và nút lưu trữ bài viết |
| **Lâm Văn Đức** | `FR-AUTH-003`<br>`FR-RCP-005` | Đăng nhập Google & Xuất bản (D11) | `2314299-LVDuc-Google-Publish` | Xác thực Google OAuth 2.0 (GoogleLogin) + Kiểm tra điều kiện xuất bản D11 (ít nhất 1 bước và 1 nguyên liệu) | Nút "Đăng nhập với Google" tại `/login` và nút Xuất bản/Hủy xuất bản trong Dashboard tác giả |
| **Nguyễn Viết Toàn** | `FR-SRCH-001`<br>`FR-SRCH-002..004` | Full-Text Search unaccent & Bộ lọc đa tiêu chí | `2312777-NVToan-Tim-Kiem-Loc` | PostgreSQL Full-Text Search không dấu (`tsvector`/`unaccent`), lọc độ khó, thời gian nấu, sắp xếp dual-syntax D8 | Giao diện thanh tìm kiếm và sidebar bộ lọc đa tiêu chí tại trang `/recipes` |
| **Nguyễn Đình Tuấn** | `FR-JOB-002` | Background Job nén & resize ảnh | `2312792-NDTuan-Resize-Anh-Job` | Hangfire Job tự động resize ảnh công thức thành thumbnail 400x300 và lưu link `thumbnailUrl` | Hiển thị ảnh thumbnail tối ưu tốc độ tải trên lưới `RecipeCard` |

---

### 📅 KẾ HOẠCH TUẦN 7: Đánh giá Sao, Xóa mềm Công thức, Background Job Email & Sitemap SEO

| Thành viên | Mã FR | Tên chức năng | Tên nhánh | Backend cần làm | Frontend cần làm |
| :--- | :---: | :--- | :--- | :--- | :--- |
| **Lê Nhật Tiến** | `FR-RCP-007`<br>`FR-JOB-003` | Xóa mềm công thức (D1) & Sinh Sitemap XML | `2312770-LNTien-Xoa-Mem-Sitemap` | Soft Delete Recipe (`IsDeleted = true`) + Hangfire Job định kỳ sinh file `sitemap.xml` chuẩn SEO | Nút xóa công thức trong danh sách bài và route `/sitemap.xml` phục vụ công cụ tìm kiếm |
| **Lâm Văn Đức** | `FR-JOB-001` | Background Job gửi Email chào mừng | `2314299-LVDuc-Email-Job` | Hangfire Job tích hợp MailKit gửi email chào mừng thành viên mới đăng ký qua hòm thư ảo MailHog (:8025) | Kiểm tra nhận email kích hoạt/chào mừng trên giao diện MailHog |
| **Nguyễn Viết Toàn** | `FR-INT-001` | Đánh giá sao công thức (Rating) | `2312777-NVToan-Danh-Gia-Sao` | Command chấm điểm rating 1-5 sao, tính điểm trung bình và số lượt đánh giá của món ăn | Component chấm sao tương tác trực tiếp trên trang chi tiết công thức |
| **Nguyễn Đình Tuấn** | Tối ưu hóa & Kiểm thử | Tích hợp hoàn thiện Media & Profile | `2312792-NDTuan-Kiem-Thu-Media` | Kiểm thử toàn diện bucket MinIO, xử lý ảnh lỗi, tối ưu tốc độ phản hồi ảnh S3 | Kiểm tra hiển thị hình ảnh đại diện, avatar trên toàn bộ các trang giao diện |

---

### 🏁 TUẦN 8: Triển khai Hệ thống (Deployment), Tối ưu NFR & Nghiệm thu Báo cáo
- **Hạ tầng Production**: Cấu hình Docker Compose môi trường Production, Nginx Reverse Proxy, SSL HTTPS, kiểm tra phân quyền và CORS.
- **Kiểm thử tích hợp (End-to-End)**: Chạy toàn bộ kịch bản người dùng từ Đăng ký ➜ Tạo món ➜ Thêm bước/nguyên liệu/ảnh ➜ Xuất bản ➜ Tìm kiếm ➜ Đánh giá sao.
- **Hoàn thiện tài liệu**: Tổng kết tài liệu báo cáo đồ án môn học, kiểm tra đối chiếu đặc tả SRS v1.0.0 và chuẩn bị slide thuyết trình vấn đáp.

---

## 👥 Phân công tổng thể (7 chức năng / thành viên)

| MSSV | Họ và tên | Chuyên môn phụ trách | Danh sách 7 chức năng | Hướng dẫn riêng |
| :---: | :--- | :--- | :--- | :---: |
| **2312770** | **Lê Nhật Tiến** *(Trưởng nhóm)* | **Core Recipe & Author Experience** | `FR-RCP-003`, `FR-CAT-002`, `FR-RCP-002`, `FR-RCP-008`, `FR-RCP-004`, `FR-RCP-006`, `FR-RCP-007` & `FR-JOB-003` | [Xem file](./docs/thanh-vien/2312770_LeNhatTien.md) |
| **2314299** | **Lâm Văn Đức** | **Authentication & Security** | `FR-AUTH-001`, `FR-AUTH-002`, `FR-AUTH-005`, `FR-AUTH-004`, `FR-AUTH-003`, `FR-RCP-005`, `FR-JOB-001` | [Xem file](./docs/thanh-vien/2314299_LamVanDuc.md) |
| **2312777** | **Nguyễn Viết Toàn** | **Category, Cache & Search Engine** | `FR-CAT-001`, `FR-CAT-003`, Caching Danh mục, `FR-CAT-004`, `FR-CAT-005`, `FR-SRCH-001..004`, `FR-INT-001` | [Xem file](./docs/thanh-vien/2312777_NguyenVietToan.md) |
| **2312792** | **Nguyễn Đình Tuấn** | **Storage, Recipe Details & Profile** | `FR-FILE-001`, `FR-FILE-002`, `FR-RCP-010`, `FR-RCP-009`, `FR-AUTH-006`, `FR-AUTH-007`, `FR-JOB-002` | [Xem file](./docs/thanh-vien/2312792_NguyenDinhTuan.md) |

---

## 🛠️ Quy tắc nhánh Git & Quy trình làm việc

> ⚠️ **Quan trọng**: Thành viên **KHÔNG tự merge vào `main`**. Chỉ có trưởng nhóm **Lê Nhật Tiến (2312770)** mới được merge sau khi review code và xác nhận không xung đột.

```powershell
# 1. Luôn cập nhật code mới nhất từ main trước khi tạo nhánh mới
git checkout main
git pull origin main

# 2. Tạo nhánh làm việc theo chuẩn: <MSSV>-<VietTatHoDemTen>-<Ten-Chuc-Nang>
git checkout -b 2312770-LNTien-Chi-Tiet-Cong-Thuc

# 3. Code chức năng và kiểm tra đạt 0 lỗi trước khi đẩy:
dotnet build CulinaryBlog.slnx     # 0 lỗi
cd src/Frontend; npx tsc --noEmit   # 0 lỗi; cd ../..

# 4. Commit và đẩy lên GitHub:
git add .
git commit -m "<module>: mo ta ngan chuc nang <Ma-FR>"
git push -u origin <Ten-Nhanh-Cua-Ban>

# 5. Báo trưởng nhóm Tiến để review và merge vào main!
```

---

## 💻 Hướng dẫn chạy dự án nhanh

```powershell
# 1. Khởi động 5 dịch vụ hạ tầng Docker:
docker compose up -d

# 2. Chạy Backend .NET 10 API:
dotnet run --project src\Backend\CulinaryBlog.API
# -> API Docs (Scalar): http://localhost:5000/scalar/v1

# 3. Chạy Frontend Next.js 15:
cd src\Frontend
npm install
npm run dev
# -> Website: http://localhost:3000
```

Tài khoản thử nghiệm có sẵn trong CSDL:
- **Admin**: `admin@culinary.local` / Mật khẩu: `Admin@123`
- **Author**: `bep_truong_an@culinary.local` / Mật khẩu: `Author@123`
