# Culinary Blog

Đồ án môn Phát triển Ứng dụng Web Nâng cao — Nhóm 21

Web chia sẻ công thức nấu ăn chuẩn vị Việt Nam. Backend .NET 10 (Clean Architecture) và Frontend Next.js 15 (App Router) là hai ứng dụng tách rời, giao tiếp qua REST API. Toàn bộ yêu cầu và quyết định kỹ thuật lấy từ [SRS v1.0.0](./docs/SRS_Culinary_Blog_v1.0.0.md) và [DECISIONS.md](./docs/DECISIONS.md).

---

## Kế hoạch chi tiết TUẦN 2

> 📖 **Bắt buộc đọc trước khi code**: file [docs/thanh-vien/](./docs/thanh-vien/) của bạn + [docs/DECISIONS.md](./docs/DECISIONS.md)

Mỗi chức năng làm trên **một nhánh riêng biệt** (xem mục [Quy tắc nhánh Git](#quy-tắc-nhánh-git--quy-trình-làm-việc) bên dưới). Sau khi làm xong mỗi chức năng, thành viên đẩy nhánh đó lên GitHub và báo **trưởng nhóm Tiến** review và merge vào `main`.

### 1. Lê Nhật Tiến (MSSV: 2312770 — Trưởng nhóm)

| # | Chức năng | Mã FR | Tên nhánh | Backend cần làm | Frontend cần làm |
| :---: | :--- | :---: | :--- | :--- | :--- |
| ✅ | Dựng hạ tầng & Database | — | `main` | Migration DB, DatabaseSeeder (2 users, 6 danh mục, 6 công thức mẫu chuẩn dữ liệu Việt) | — |
| ✅ | Layout chung & Trang chủ | — | `main` | `GetCategoriesQueryHandler`, `GetRecipesQueryHandler`, `GetRecipeBySlugQueryHandler` | `Navbar.tsx`, `Footer.tsx`, `RecipeCard.tsx`, `app/page.tsx` (Hero, Category filter, Recipe grid) |
| 🔲 | Tạo công thức mới (Draft) | `FR-RCP-003` | `2312770-LNTien-Tao-Cong-Thuc` | `CreateRecipeCommandHandler` + Validator | Form tạo công thức `/recipes/create`: Tiêu đề, Mô tả, Danh mục, Thời gian, Khẩu phần, Độ khó |
| 🔲 | Xem chi tiết danh mục | `FR-CAT-002` | `2312770-LNTien-Chi-Tiet-Danh-Muc` | `GetCategoryBySlugQueryHandler` | Trang `/categories/[slug]`: Banner danh mục + lưới RecipeCard |

📘 **Xem hướng dẫn chi tiết**: [docs/thanh-vien/2312770_LeNhatTien.md](./docs/thanh-vien/2312770_LeNhatTien.md)

---

### 2. Lâm Văn Đức (MSSV: 2314299)

| # | Chức năng | Mã FR | Tên nhánh | Backend cần làm | Frontend cần làm |
| :---: | :--- | :---: | :--- | :--- | :--- |
| 🔲 | Đăng ký tài khoản | `FR-AUTH-001` | `2314299-LVDuc-Dang-Ky` | `RegisterCommandHandler` + Validator (email hợp lệ, mật khẩu ≥ 8 ký tự), gán role Author, sinh JWT | Trang `/register`: Form Email, Tên hiển thị, Tên đăng nhập (tùy chọn), Mật khẩu, Xác nhận mật khẩu |
| 🔲 | Đăng nhập Email/Mật khẩu | `FR-AUTH-002` | `2314299-LVDuc-Dang-Nhap` | `LoginCommandHandler` (kiểm tra mật khẩu, sinh Access + Refresh Token, xử lý lockout 5 lần sai) | Trang `/login`: Form Email, Mật khẩu, nút Đăng nhập; lưu token vào cookie và cập nhật Navbar |

📘 **Xem hướng dẫn chi tiết**: [docs/thanh-vien/2314299_LamVanDuc.md](./docs/thanh-vien/2314299_LamVanDuc.md)

---

### 3. Nguyễn Viết Toàn (MSSV: 2312777)

| # | Chức năng | Mã FR | Tên nhánh | Backend cần làm | Frontend cần làm |
| :---: | :--- | :---: | :--- | :--- | :--- |
| 🔲 | Danh sách danh mục | `FR-CAT-001` | `2312777-NVToan-Danh-Sach-Danh-Muc` | Kiểm tra & bổ sung comment cho `GetCategoriesQueryHandler` (đã có khung mẫu) | Trang `/categories`: Lưới card danh mục với hình ảnh đại diện, mô tả và đếm số bài viết |
| 🔲 | Admin tạo danh mục | `FR-CAT-003` | `2312777-NVToan-Tao-Danh-Muc` | `CreateCategoryCommandHandler` (kiểm tra tên trùng, tự sinh slug, lưu DB) | Trang admin `/admin/categories`: Bảng danh mục + Form nhập tên, mô tả, ảnh, thứ tự OrderIndex |

📘 **Xem hướng dẫn chi tiết**: [docs/thanh-vien/2312777_NguyenVietToan.md](./docs/thanh-vien/2312777_NguyenVietToan.md)

---

### 4. Nguyễn Đình Tuấn (MSSV: 2312792)

| # | Chức năng | Mã FR | Tên nhánh | Backend cần làm | Frontend cần làm |
| :---: | :--- | :---: | :--- | :--- | :--- |
| 🔲 | Upload ảnh lên MinIO | `FR-FILE-001` | `2312792-NDTuan-Upload-Minio` | `MinioStorageService.UploadAsync()`: Validate ≤ 5MB, chỉ JPG/PNG/WebP, sinh tên file UUID, upload vào bucket `culinary-blog` | Component `ImageUploader.tsx`: Kéo thả hoặc click chọn ảnh, preview ảnh sau upload, hiển thị URL |
| 🔲 | Xóa ảnh trên MinIO | `FR-FILE-002` | `2312792-NDTuan-Xoa-Anh-Minio` | `MinioStorageService.DeleteAsync()`: Phân tích object key từ URL, gọi MinIO RemoveObject | Nút xóa (icon thùng rác / dấu ×) trên ảnh preview, xác nhận trước khi xóa |

📘 **Xem hướng dẫn chi tiết**: [docs/thanh-vien/2312792_NguyenDinhTuan.md](./docs/thanh-vien/2312792_NguyenDinhTuan.md)

---

## Phân công tổng thể (7 chức năng / thành viên)

Toàn bộ 28 chức năng chia đều cho 4 thành viên — **đúng 7 chức năng mỗi người**, tất cả đều gồm cả Backend lẫn Frontend tương ứng:

| MSSV | Họ và tên | Chuyên môn phụ trách | Danh sách 7 chức năng | Số FR | Hướng dẫn riêng |
| :---: | :--- | :--- | :--- | :---: | :---: |
| **2312770** | **Lê Nhật Tiến** *(Trưởng nhóm)* | **Core Recipe & Author Experience** | `FR-RCP-003` (Tạo công thức nháp), `FR-CAT-002` (Chi tiết danh mục + recipes), `FR-RCP-004` (Sửa công thức), `FR-RCP-008` (Quản lý gallery ảnh), `FR-RCP-006` (Lưu trữ Archive), `FR-RCP-007` (Xóa mềm Recipe D1), `FR-JOB-003` (Sinh sitemap.xml) | **7** | [Xem file](./docs/thanh-vien/2312770_LeNhatTien.md) |
| **2314299** | **Lâm Văn Đức** | **Authentication & Recipe Publishing** | `FR-AUTH-001` (Đăng ký), `FR-AUTH-002` (Đăng nhập Email), `FR-AUTH-004` (Refresh token rotation), `FR-AUTH-005` (Đăng xuất), `FR-AUTH-003` (Google OAuth 2.0), `FR-RCP-005` (Xuất bản/Hủy xuất bản D11), `FR-JOB-001` (Email chào mừng Hangfire) | **7** | [Xem file](./docs/thanh-vien/2314299_LamVanDuc.md) |
| **2312777** | **Nguyễn Viết Toàn** | **Category & Search Engine** | `FR-CAT-001` (Danh sách danh mục), `FR-CAT-003` (Tạo danh mục), `FR-CAT-004` (Sửa danh mục D12), `FR-CAT-005` (Xóa mềm danh mục D1), `FR-SRCH-001` (Full-Text Search unaccent), `FR-SRCH-002..004` (Lọc đa tiêu chí, sắp xếp D8, phân trang), `FR-INT-001` (Đánh giá sao Rating 1-5) | **7** | [Xem file](./docs/thanh-vien/2312777_NguyenVietToan.md) |
| **2312792** | **Nguyễn Đình Tuấn** | **Storage, Steps/Ingredients, Profile** | `FR-FILE-001` (Upload ảnh MinIO 5MB), `FR-FILE-002` (Xóa ảnh MinIO), `FR-RCP-010` (Quản lý các bước nấu D9), `FR-RCP-009` (Quản lý nguyên liệu D10), `FR-AUTH-006` (Xem Profile), `FR-AUTH-007` (Cập nhật Profile & Avatar), `FR-JOB-002` (Resize ảnh thumbnail Hangfire) | **7** | [Xem file](./docs/thanh-vien/2312792_NguyenDinhTuan.md) |

---

## Quy tắc nhánh Git & Quy trình làm việc

> ⚠️ **Quan trọng**: Thành viên **KHÔNG tự merge vào `main`**. Chỉ có trưởng nhóm **Lê Nhật Tiến (2312770)** mới được merge sau khi review code và xác nhận không xung đột.

### Quy ước tên nhánh theo từng chức năng

Mỗi chức năng (FR) được làm trên **một nhánh riêng biệt**, tự tạo từ nhánh `main` theo cú pháp:

```
<MSSV>-<VietTatHoDemTen>-<Ten-Chuc-Nang>
```

| Thành viên | Cú pháp tiền tố | Ví dụ nhánh chức năng |
| :--- | :--- | :--- |
| Lê Nhật Tiến (Trưởng nhóm) | `2312770-LNTien-` | `2312770-LNTien-Tao-Cong-Thuc`, `2312770-LNTien-Chi-Tiet-Danh-Muc` |
| Lâm Văn Đức | `2314299-LVDuc-` | `2314299-LVDuc-Dang-Ky`, `2314299-LVDuc-Dang-Nhap` |
| Nguyễn Viết Toàn | `2312777-NVToan-` | `2312777-NVToan-Danh-Sach-Danh-Muc`, `2312777-NVToan-Tao-Danh-Muc` |
| Nguyễn Đình Tuấn | `2312792-NDTuan-` | `2312792-NDTuan-Upload-Minio`, `2312792-NDTuan-Xoa-Anh-Minio` |

### Quy trình làm việc cho từng chức năng

```powershell
# 1. Luôn cập nhật code mới nhất từ main trước khi tạo nhánh mới
git checkout main
git pull origin main

# 2. Tự tạo nhánh mới cho chức năng bạn sắp làm
# Ví dụ Tuấn làm upload MinIO:
git checkout -b 2312792-NDTuan-Upload-Minio

# 3. Code chức năng của bạn...

# 4. Kiểm tra trước khi đẩy (bắt buộc 0 lỗi)
dotnet build CulinaryBlog.slnx     # 0 lỗi
npm run lint                        # 0 lỗi (chạy trong src/Frontend)

# 5. Commit theo chuẩn: <module>: <mô tả ngắn> <mã FR>
git add .
git commit -m "file: upload anh len minio 5mb FR-FILE-001"

# 6. Đẩy nhánh chức năng lên GitHub
git push -u origin 2312792-NDTuan-Upload-Minio

# 7. Báo trưởng nhóm Tiến qua Zalo để Tiến review và merge vào main
# Sau khi Tiến merge xong, chuyển lại về main để pull về làm chức năng tiếp theo:
git checkout main
git pull origin main
```

### Quy ước commit message

```
<module>: <mô tả ngắn bằng tiếng Anh hoặc Việt không dấu> <mã FR>

Ví dụ:
  auth: hien thuc dang ky tai khoan FR-AUTH-001
  auth: hien thuc dang nhap email password FR-AUTH-002
  category: them api danh sach danh muc voi recipe count FR-CAT-001
  file: upload anh len minio 5mb validate FR-FILE-001
```


---

## Sơ đồ hệ thống

```
+-------------------------------------------------------------------+
|                     CULINARY BLOG SYSTEM                          |
|                                                                   |
|   +-------------------+        +------------------------------+   |
|   |  NEXT.JS FRONTEND |<------>|     .NET 10 BACKEND API      |   |
|   |  (App Router)     |  REST  |  (Minimal APIs + Clean Arch) |   |
|   |  Port: 3000       |  JSON  |  Port: 5000                  |   |
|   +-------------------+        +--------------+---------------+   |
|                                               |                   |
|   +--------+  +--------+  +--------+  +---------+  +-----------+  |
|   | Pgsql  |  | Redis  |  | MinIO  |  |Hangfire |  |Google Auth|  |
|   | :5432  |  | :6379  |  | :9000  |  | Jobs    |  | OAuth 2.0 |  |
|   +--------+  +--------+  +--------+  +---------+  +-----------+  |
+-------------------------------------------------------------------+
```

Backend chia 4 tầng Clean Architecture: `API → Infrastructure → Application → Domain`.
Tầng Application dùng CQRS qua MediatR — mỗi chức năng là một Command hoặc Query riêng.

---

## Công nghệ

- **Backend:** .NET 10 Minimal APIs, EF Core 10, PostgreSQL 16, Redis 7, MediatR (CQRS), FluentValidation, ASP.NET Core Identity + JWT, Hangfire, Serilog + OpenTelemetry, Scalar (API Docs).
- **Frontend:** Next.js 15 App Router, TypeScript, Tailwind CSS, TanStack Query, Axios.
- **Hạ tầng:** Docker Compose, MinIO (Object Storage), MailHog (SMTP Mock), Seq (Log Viewer).

---

## Ba loại tài khoản

| Vai trò | Quyền hạn chính |
| :--- | :--- |
| **Guest** | Xem công thức đã xuất bản, xem danh mục, tìm kiếm — không cần đăng nhập |
| **Author** | Thêm quyền: tạo/sửa/xóa/publish công thức **của chính mình**, upload ảnh, quản lý nguyên liệu & các bước, chỉnh hồ sơ cá nhân |
| **Admin** | Thêm quyền: CRUD toàn quyền danh mục, sửa/xóa công thức bất kỳ ai, vào Hangfire Dashboard, xem logs |

---

## Cấu trúc thư mục

```
PTUDWNC-2026-Nhom21/
├── CulinaryBlog.slnx                  # Solution file .NET 10 (4 project backend)
├── docker-compose.yml                 # 5 dịch vụ hạ tầng (Postgres, Redis, MinIO, Seq, MailHog)
├── README.md                          # File này
├── docs/
│   ├── SRS_Culinary_Blog_v1.0.0.md    # Toàn văn đặc tả SRS chuẩn sau khi chốt mâu thuẫn
│   ├── DECISIONS.md                   # 12 quyết định kiến trúc cốt lõi (ĐỌC TRƯỚC KHI CODE)
│   ├── HUONG_DAN_THANH_VIEN.md        # Hướng dẫn cài đặt, môi trường và Git workflow
│   ├── SETUP.md                       # Hướng dẫn chạy dự án trên máy cá nhân
│   ├── SRS_Culinary_Blog_v1.0.0.pdf   # File đặc tả gốc PDF
│   └── thanh-vien/                    # Hướng dẫn chi tiết riêng từng người
│       ├── 2312770_LeNhatTien.md
│       ├── 2314299_LamVanDuc.md
│       ├── 2312777_NguyenVietToan.md
│       └── 2312792_NguyenDinhTuan.md
└── src/
    ├── Backend/
    │   ├── CulinaryBlog.Domain/          # Entities, Value Objects, Enums, Interfaces
    │   ├── CulinaryBlog.Application/     # Commands, Queries, Handlers, DTOs, Validators
    │   ├── CulinaryBlog.Infrastructure/  # EF Core, Repositories, JWT, Redis, MinIO, Hangfire
    │   └── CulinaryBlog.API/             # Minimal API Endpoints, Middleware, Program.cs
    └── Frontend/                         # Next.js 15 App Router (TypeScript, Tailwind CSS)
```

---

## Chạy dự án

Hướng dẫn cài đặt đầy đủ: xem [docs/SETUP.md](./docs/SETUP.md).

```powershell
# 1. Khởi động hạ tầng Docker
docker compose up -d

# 2. Chạy Backend (.NET 10 API — tự migrate & seed DB)
dotnet run --project src\Backend\CulinaryBlog.API
# -> Tài liệu API: http://localhost:5000/scalar/v1

# 3. Chạy Frontend (Next.js 15)
cd src\Frontend
npm install
npm run dev
# -> Website: http://localhost:3000
```

Tài khoản mẫu đã có sẵn trong DB:
- **Admin**: `admin@culinary.local` / `Admin@123`
- **Author**: `bep_truong_an@culinary.local` / `Author@123`

---

## Tài liệu đi kèm

| Tập tin | Mô tả |
| :--- | :--- |
| 📘 [`docs/thanh-vien/2312770_LeNhatTien.md`](./docs/thanh-vien/2312770_LeNhatTien.md) | Hướng dẫn chi tiết 7 chức năng của **Lê Nhật Tiến** |
| 📘 [`docs/thanh-vien/2314299_LamVanDuc.md`](./docs/thanh-vien/2314299_LamVanDuc.md) | Hướng dẫn chi tiết 7 chức năng của **Lâm Văn Đức** |
| 📘 [`docs/thanh-vien/2312777_NguyenVietToan.md`](./docs/thanh-vien/2312777_NguyenVietToan.md) | Hướng dẫn chi tiết 7 chức năng của **Nguyễn Viết Toàn** |
| 📘 [`docs/thanh-vien/2312792_NguyenDinhTuan.md`](./docs/thanh-vien/2312792_NguyenDinhTuan.md) | Hướng dẫn chi tiết 7 chức năng của **Nguyễn Đình Tuấn** |
| 📋 [`docs/HUONG_DAN_THANH_VIEN.md`](./docs/HUONG_DAN_THANH_VIEN.md) | Hướng dẫn cài đặt, môi trường, và quy trình Git |
| ⚖️ [`docs/DECISIONS.md`](./docs/DECISIONS.md) | 12 quyết định kiến trúc cốt lõi (Soft delete, Slug, Steps, Ingredients…) |
| 📄 [`docs/SRS_Culinary_Blog_v1.0.0.md`](./docs/SRS_Culinary_Blog_v1.0.0.md) | Toàn văn đặc tả SRS 27 FRs, schema CSDL, API contracts |
| 🛠️ [`docs/SETUP.md`](./docs/SETUP.md) | Hướng dẫn thiết lập môi trường phát triển cá nhân |

---

## Thành viên nhóm

| MSSV | Họ và tên | Email | GitHub | Vai trò | Số FR |
| :---: | :--- | :--- | :--- | :--- | :---: |
| **2312770** | **Lê Nhật Tiến** | 2312770@dlu.edu.vn | [@2312770-coder](https://github.com/2312770-coder) | Trưởng nhóm, Core Recipe & Author Experience | **7** |
| **2314299** | **Lâm Văn Đức** | 2314299@dlu.edu.vn | [@2314299-debug](https://github.com/2314299-debug) | Thành viên, Authentication & Recipe Publishing | **7** |
| **2312777** | **Nguyễn Viết Toàn** | 2312777@dlu.edu.vn | [@2312777-rgb](https://github.com/2312777-rgb) | Thành viên, Category Management & Search Engine | **7** |
| **2312792** | **Nguyễn Đình Tuấn** | 2312792@dlu.edu.vn | [@2312792-debug](https://github.com/2312792-debug) | Thành viên, Storage, Steps/Ingredients, Profile | **7** |

*Nhóm 21 — Lớp học phần Phát triển Ứng dụng Web Nâng cao — Năm học 2025–2026*
