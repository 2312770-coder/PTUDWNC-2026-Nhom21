# Culinary Blog

Đồ án môn Phát triển Ứng dụng Web Nâng cao — Nhóm 21

Web chia sẻ công thức nấu ăn chuẩn vị Việt Nam. Backend .NET 10 (Clean Architecture) và Frontend Next.js 15 (App Router) là hai ứng dụng tách rời, giao tiếp qua REST API. Toàn bộ yêu cầu và quyết định kỹ thuật lấy từ [SRS v1.0.0](./docs/SRS_Culinary_Blog_v1.0.0.md) và [DECISIONS.md](./docs/DECISIONS.md).

---

## 📌 LỘ TRÌNH TỔNG THỂ 8 TUẦN (CÂN BẰNG KHỐI LƯỢNG — 7 CHỨC NĂNG / THÀNH VIÊN)

| Tuần | Trọng tâm công việc | Lê Nhật Tiến (2312770) | Lâm Văn Đức (2314299) | Nguyễn Viết Toàn (2312777) | Nguyễn Đình Tuấn (2312792) |
| :---: | :--- | :--- | :--- | :--- | :--- |
| **Tuần 2** | Khởi tạo hạ tầng, Base & Core | Hạ tầng Docker & Seeder, `FR-RCP-003` (Tạo món Draft) | `FR-AUTH-001` (Đăng ký tài khoản) | `FR-CAT-001` (Danh sách danh mục) | `FR-FILE-001`, `FR-FILE-002` (Upload & Xóa ảnh MinIO) |
| **Tuần 3** | Xác thực, Chi tiết Danh mục & Steps | `FR-CAT-002` (Chi tiết danh mục + recipes) | `FR-AUTH-002` (Đăng nhập Email + Rate Limiter) | `FR-CAT-003` (Admin tạo danh mục mới) | `FR-RCP-010` (Quản lý các bước nấu D9) |
| **Tuần 4** | Core Recipe Detail, Media, Auth & Ingredients | `FR-RCP-002` (Chi tiết công thức), `FR-RCP-008` (Gallery ảnh) | `FR-AUTH-005` (Đăng xuất), `FR-AUTH-004` (Refresh token rotation) | Redis Caching Danh mục, `FR-CAT-004` (Sửa danh mục D12) | `FR-RCP-009` (Quản lý nguyên liệu D10), `FR-AUTH-006` (Xem Profile) |
| **Tuần 5** | Cập nhật, Lưu trữ món, Google OAuth & Avatar | `FR-RCP-004` (Sửa công thức), `FR-RCP-006` (Lưu trữ Archive) | `FR-AUTH-003` (Đăng nhập Google OAuth 2.0) | `FR-CAT-005` (Xóa mềm danh mục D1) | `FR-AUTH-007` (Sửa hồ sơ & đổi Avatar MinIO) |
| **Tuần 6** | Tìm kiếm unaccent, Bộ lọc D8 & Thumbnail Job | `FR-RCP-007` (Xóa mềm công thức D1) | `FR-RCP-005` (Xuất bản công thức - điều kiện D11) | `FR-SRCH-001` (Full-Text Search unaccent), `FR-SRCH-002..004` (Lọc & Sắp xếp D8) | `FR-JOB-002` (Hangfire job resize ảnh thumbnail) |
| **Tuần 7** | Đánh giá sao, Background Jobs (Email & Sitemap) | `FR-JOB-003` (Hangfire sinh Sitemap XML SEO) | `FR-JOB-001` (Hangfire gửi Email chào mừng) | `FR-INT-001` (Đánh giá sao công thức 1-5 sao) | Tối ưu hóa toàn diện Media MinIO & Kiểm thử tải |
| **Tuần 8** | Triển khai Production & Nghiệm thu | Cấu hình Docker Production, Nginx SSL HTTPS, Kiểm thử tích hợp E2E, Tổng kết báo cáo & Slide vấn đáp |

---

## 📖 HƯỚNG DẪN CHI TIẾT THEO TỪNG TUẦN

---

### 1. HƯỚNG DẪN CHI TIẾT TUẦN 2 (ĐÃ HOÀN THÀNH & MERGE MAIN)

#### 1.1. Lê Nhật Tiến (MSSV: 2312770)
- **Chức năng 1: Dựng hạ tầng & Database Seeder** (Nhánh: `2312770-LNTien-Database` - Đã merge)
  - Cấu hình 5 container Docker Compose: PostgreSQL 16 (5432), Redis 7 (6379), MinIO (9000/9001), Seq (5341), MailHog (8025).
  - Thiết kế Entity Framework Core 10, cấu hình Migration và DatabaseSeeder sinh 2 role (Admin, Author), 2 tài khoản mẫu, 23 categories và 100 recipes chuẩn văn hóa ẩm thực Việt.
  - Viết tài liệu hướng dẫn CSDL chi tiết tại `docs/DATABASE.md`.
- **Chức năng 2: Tạo công thức mới trạng thái Draft (FR-RCP-003)** (Nhánh: `2312770-LNTien-Tao-Cong-Thuc-Moi` - Đã merge)
  - Backend: `CreateRecipeCommandHandler` xác thực `CurrentUser`, kiểm tra danh mục, tự sinh slug duy nhất (chống trùng lặp URL), nạp nutrition, nguyên liệu và các bước. Đăng ký `POST /api/v1/recipes` với quyền `AuthorOrAdmin`.
  - Frontend: Trang `/dashboard/recipes/new/page.tsx` (816 dòng code) với Form thông tin chung, danh mục, bảng dinh dưỡng, danh sách nguyên liệu và các bước thực hiện.

#### 1.2. Lâm Văn Đức (MSSV: 2314299)
- **Chức năng: Đăng ký tài khoản mới (FR-AUTH-001)** (Nhánh: `2314299-LVDuc-Dang-Ky` - Đã merge)
  - Backend: `RegisterCommandHandler` kiểm tra email trùng, tạo user qua ASP.NET Core Identity (hash PBKDF2), gán role "Author", sinh JWT Access Token và Refresh Token lưu database. `RegisterCommandValidator` kiểm tra email và mật khẩu $\ge 8$ ký tự.
  - Frontend: Trang `/register` (`app/(auth)/register/page.tsx`): Form Email, DisplayName, Password, Confirm Password, validate trực quan và liên kết chuyển trang đăng nhập.

#### 1.3. Nguyễn Viết Toàn (MSSV: 2312777)
- **Chức năng: Xem danh sách danh mục (FR-CAT-001)** (Nhánh: `2312777-NVToan-Danh-Sach-Danh-Muc` - Đã merge)
  - Backend: `GetCategoriesQueryHandler` truy vấn danh mục theo `OrderIndex`, đếm số lượng công thức Published (`Status == RecipeStatus.Published && !IsDeleted`). Đăng ký `GET /api/v1/categories`.
  - Frontend: Module `src/Frontend/lib/api/categories.ts` hàm `getAll()` và hiển thị danh mục tại trang chủ, trang `/categories`.

#### 1.4. Nguyễn Đình Tuấn (MSSV: 2312792)
- **Chức năng: Upload & Xóa ảnh trên MinIO (FR-FILE-001 & FR-FILE-002)** (Nhánh: `2312792-ndtuan-upload-minio` & `2312792-ndtuan-delete-minio` - Đã merge)
  - Backend: `MinioFileStorageService` kiểm tra Magic Bytes file, giới hạn $\le 5$MB, định dạng JPG/PNG/WebP/AVIF, upload vào bucket `culinary-blog` và hàm xóa file `DeleteAsync`. Endpoints `POST /api/v1/files/upload` và `DELETE /api/v1/files`.
  - Frontend: Component dùng chung `src/Frontend/components/ui/ImageUploader.tsx`: kéo thả ảnh, preview ảnh, hiển thị link sau upload và nút xóa ảnh có xác nhận.

---

### 2. HƯỚNG DẪN CHI TIẾT TUẦN 3 (ĐÃ HOÀN THÀNH & MERGE MAIN)

#### 2.1. Lê Nhật Tiến (MSSV: 2312770)
- **Chức năng: Xem chi tiết danh mục kèm bài viết (FR-CAT-002)** (Nhánh: `2312770-LNTien-Chi-Tiet-Danh-Muc` - Đã merge)
  - Backend: Tạo DTO `CategoryDetailDto` kèm danh sách `IReadOnlyList<RecipeListItemDto> Recipes`. Query `GetCategoryBySlugQuery` và Handler nạp các bài viết có `Status == RecipeStatus.Published && !IsDeleted`, sắp xếp giảm dần theo ngày xuất bản. Endpoint `GET /api/v1/categories/{slug}`.
  - Frontend: Cập nhật `categoriesApi.getBySlug(slug)`. Tạo trang `app/(public)/categories/[slug]/page.tsx` gồm Breadcrumb, Banner danh mục, Lưới `RecipeCard`, xử lý Empty state và Not Found 404.

#### 2.2. Lâm Văn Đức (MSSV: 2314299)
- **Chức năng: Đăng nhập Email/Mật khẩu + Rate Limiting (FR-AUTH-002)** (Nhánh: `2314299-LVDuc-Dang-Nhap` - Đã merge)
  - Backend: `LoginCommandHandler` kiểm tra tài khoản khóa (`IsLockedOutAsync`), kiểm tra mật khẩu, đếm số lần sai và tự động khóa 15 phút sau 5 lần thất bại. Sinh cặp token mới. Cấu hình ASP.NET Core `AddRateLimiter` 5 req/phút chống Brute-Force tại `API/DependencyInjection.cs`.
  - Frontend: Trang `app/(auth)/login/page.tsx` form đăng nhập, lưu token, cập nhật trạng thái User trên Navbar.

#### 2.3. Nguyễn Viết Toàn (MSSV: 2312777)
- **Chức năng: Admin tạo danh mục mới (FR-CAT-003)** (Nhánh: `2312777-NVToan-Tao-Danh-Muc` - Đã merge)
  - Backend: `CreateCategoryCommandHandler` kiểm tra tên trùng lặp, tự sinh slug chuẩn SEO (bỏ dấu tiếng Việt, nối bằng gạch ngang), lưu CSDL. Endpoint `POST /api/v1/categories` phân quyền `AdminOnly`.
  - Frontend: Trang quản trị `app/(admin)/admin/categories/page.tsx` gồm bảng danh mục, Form nhập tên, mô tả, thứ tự hiển thị và upload ảnh.

#### 2.4. Nguyễn Đình Tuấn (MSSV: 2312792)
- **Chức năng: Quản lý các bước nấu ăn (FR-RCP-010)** (Nhánh: `2312792-NDTuan-Cac-Buoc-Nau` - Đã merge)
  - Backend: Thêm, sửa, xóa bước nấu trong `Features/Recipes/Commands/ManageSteps/*`. Tuân thủ **Quyết định D9**: `Title` bắt buộc, `StepNumber` tùy chọn (`int?`) — nếu client không truyền thì server tự động lấy số lớn nhất + 1. Tự động renumber thứ tự khi xóa bước. Endpoints `POST/PUT/DELETE /api/v1/recipes/{id}/steps`.
  - Frontend: Component `StepListEditor.tsx` hiển thị danh sách các bước có hẹn giờ, hướng dẫn và tích hợp upload ảnh MinIO.

---

### 3. HƯỚNG DẪN CHI TIẾT TUẦN 4 (CÂN BẰNG KHỐI LƯỢNG — CHUẨN BỊ LÀM)

> 💡 **Quy tắc**: Mỗi thành viên tự tạo nhánh mới từ `main` mới nhất, code và kiểm tra 0 lỗi trước khi gửi PR cho trưởng nhóm Tiến.

#### 3.1. Lê Nhật Tiến (MSSV: 2312770)
- **Chức năng 1: Xem chi tiết công thức nấu ăn (FR-RCP-002)** (Nhánh: `2312770-LNTien-Chi-Tiet-Cong-Thuc`)
  - Backend: Viết `GetRecipeBySlugQueryHandler`: nạp quan hệ Category, Author, Steps, Ingredients, Images, Nutrition. Phân quyền xem Draft/Archived. Endpoint `GET /api/v1/recipes/{slug}`.
  - Frontend: Hàm `recipesApi.getBySlug(slug)` và trang `src/Frontend/app/(public)/recipes/[slug]/page.tsx` (Hero banner, dinh dưỡng, checklist nguyên liệu, timeline các bước nấu).
- **Chức năng 2: Quản lý gallery ảnh công thức (FR-RCP-008)** (Nhánh: `2312770-LNTien-Gallery-Anh`)
  - Backend: Commands `AddRecipeImageCommand`, `DeleteRecipeImageCommand`, `SetPrimaryImageCommand` trong `ManageImages/*`. Endpoints `POST/DELETE/PUT /api/v1/recipes/{id}/images`.
  - Frontend: Component `RecipeGalleryEditor.tsx`: lưới ảnh thumbnail, gắn sao ảnh chính ⭐, xóa ảnh 🗑️ và tích hợp MinIO upload.

#### 3.2. Lâm Văn Đức (MSSV: 2314299)
- **Chức năng 1: Đăng xuất & Thu hồi phiên làm việc (FR-AUTH-005)** (Nhánh: `2314299-LVDuc-Dang-Xuat`)
  - Backend: `LogoutCommandHandler` tìm và thu hồi/xóa refresh token trong CSDL. Endpoint `POST /api/v1/auth/logout`.
  - Frontend: Nút "Đăng xuất" trên Navbar, xóa token và cập nhật trạng thái giao diện.
- **Chức năng 2: Refresh Token Rotation (FR-AUTH-004)** (Nhánh: `2314299-LVDuc-Refresh-Token`)
  - Backend: `RefreshTokenCommandHandler` xác thực token cũ, thu hồi và cấp cặp token mới (phát hiện tái sử dụng Reuse Detection). Endpoint `POST /api/v1/auth/refresh`.
  - Frontend: Cấu hình Axios Interceptor trong `client.ts` tự bắt lỗi 401 để refresh ngầm và thử lại request cũ.

#### 3.3. Nguyễn Viết Toàn (MSSV: 2312777)
- **Chức năng 1: Tích hợp Redis Caching cho Danh mục** (Nhánh: `2312777-NVToan-Cache-Danh-Muc`)
  - Backend: Inject `ICacheService` vào `GetCategoriesQueryHandler`, kiểm tra Cache HIT/MISS, lưu cache TTL 30 phút. Xóa cache trong `CreateCategoryCommandHandler`.
  - Kiểm thử: Đo lường tốc độ phản hồi qua Seq và Scalar (< 5ms khi HIT).
- **Chức năng 2: Admin cập nhật danh mục (FR-CAT-004)** (Nhánh: `2312777-NVToan-Sua-Danh-Muc`)
  - Backend: `UpdateCategoryCommandHandler` cập nhật tên, mô tả, ảnh, thứ tự nhưng **giữ nguyên slug SEO theo Quyết định D12**. Xóa cache Redis sau khi sửa. Endpoint `PUT /api/v1/categories/{id}` (`AdminOnly`).
  - Frontend: Nút "Sửa" và Modal cập nhật thông tin danh mục trong trang `/admin/categories`.

#### 3.4. Nguyễn Đình Tuấn (MSSV: 2312792)
- **Chức năng 1: Quản lý nguyên liệu công thức (FR-RCP-009)** (Nhánh: `2312792-NDTuan-Nguyen-Lieu`)
  - Backend: Thư mục `ManageIngredients/*`: thêm, sửa, xóa nguyên liệu. Tuân thủ **Quyết định D10**: cho phép null đơn vị gia vị nêm nếm. Endpoints `POST/PUT/DELETE /api/v1/recipes/{id}/ingredients`.
  - Frontend: Component `IngredientListEditor.tsx` bảng danh sách nguyên liệu, định lượng, ghi chú và các nút thao tác.
- **Chức năng 2: Xem thông tin hồ sơ cá nhân (FR-AUTH-006)** (Nhánh: `2312792-NDTuan-Xem-Ho-So`)
  - Backend: `GetProfileQueryHandler` lấy thông tin user hiện tại từ `ICurrentUser` và Identity. Endpoint `GET /api/v1/auth/me`.
  - Frontend: Trang cá nhân `/profile` hiển thị Avatar lớn, Tên hiển thị, Email, Bio và Roles.

---

### 4. KẾ HOẠCH TỔNG QUAN CÁC TUẦN TIẾP THEO (TUẦN 5 → TUẦN 8)

* **Tuần 5**:
  - **Tiến**: `FR-RCP-004` (Sửa thông tin công thức) & `FR-RCP-006` (Lưu trữ công thức Archive).
  - **Đức**: `FR-AUTH-003` (Đăng nhập Google OAuth 2.0).
  - **Toàn**: `FR-CAT-005` (Admin xóa mềm danh mục - tuân thủ D1 không có recipe mới cho xóa).
  - **Tuấn**: `FR-AUTH-007` (Cập nhật hồ sơ cá nhân, đổi DisplayName, Bio và upload Avatar MinIO).

* **Tuần 6**:
  - **Tiến**: `FR-RCP-007` (Xóa mềm công thức theo D1 Soft Delete).
  - **Đức**: `FR-RCP-005` (Xuất bản công thức - tuân thủ điều kiện D11: $\ge 1$ bước và $\ge 1$ nguyên liệu).
  - **Toàn**: `FR-SRCH-001` (Full-Text Search unaccent) & `FR-SRCH-002..004` (Lọc đa tiêu chí, sắp xếp dual-syntax D8).
  - **Tuấn**: `FR-JOB-002` (Hangfire background job nén & resize ảnh thumbnail).

* **Tuần 7**:
  - **Tiến**: `FR-JOB-003` (Hangfire background job tự động sinh sitemap.xml SEO).
  - **Đức**: `FR-JOB-001` (Hangfire gửi Email chào mừng thành viên mới qua MailHog).
  - **Toàn**: `FR-INT-001` (Đánh giá sao công thức 1-5 sao, tính rating trung bình).
  - **Tuấn**: Tối ưu hóa toàn diện Media MinIO, xử lý ảnh lỗi và kiểm thử tải.

* **Tuần 8**:
  - Triển khai môi trường Production (Docker Compose + Nginx Reverse Proxy, cấu hình HTTPS SSL).
  - Kiểm thử tích hợp toàn diện luồng người dùng (E2E Testing).
  - Tổng kết nghiệm thu, hoàn thiện tài liệu báo cáo đồ án và slide thuyết trình vấn đáp.

---

## 🛠️ QUY TẮC NHÁNH GIT & QUY TRÌNH LÀM VIỆC

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

## 💻 HƯỚNG DẪN CHẠY DỰ ÁN NHANH

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
