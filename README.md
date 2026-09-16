# Culinary Blog

Đồ án môn Phát triển Ứng dụng Web Nâng cao — Nhóm 21

Web chia sẻ công thức nấu ăn. Backend .NET 10 (Clean Architecture) và frontend
Next.js là hai ứng dụng tách rời, nói chuyện với nhau qua REST API. Toàn bộ
yêu cầu lấy từ tài liệu SRS v1.0.0 trong thư mục `docs/`.

- Mã dự án: CULINARY-BLOG-V1
- Phiên bản đặc tả: 1.0.0

---

## Mục lục

- [Sơ đồ hệ thống](#sơ-đồ-hệ-thống)
- [Công nghệ](#công-nghệ)
- [Ba loại tài khoản](#ba-loại-tài-khoản)
- [Cấu trúc thư mục](#cấu-trúc-thư-mục)
- [Phân công công việc](#phân-công-công-việc)
- [Quy tắc nhánh Git](#quy-tắc-nhánh-git)
- [Chạy dự án](#chạy-dự-án)
- [Tài liệu đi kèm](#tài-liệu-đi-kèm)
- [Thành viên nhóm](#thành-viên-nhóm)

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

Backend chia 4 tầng theo Clean Architecture, phụ thuộc chỉ đi từ ngoài vào
trong: `API -> Infrastructure -> Application -> Domain`. Tầng Application dùng
CQRS qua MediatR, mỗi chức năng là một Command hoặc Query riêng.

---

## Công nghệ

**Backend:** .NET 10 Minimal APIs, EF Core 10, PostgreSQL 16, Redis 7,
MediatR (CQRS), FluentValidation, ASP.NET Core Identity + JWT, Hangfire,
Serilog + OpenTelemetry, Scalar (tài liệu API)

**Frontend:** Next.js App Router, TypeScript, Tailwind CSS, TanStack Query,
Auth.js v5

**Hạ tầng:** Docker Compose, MinIO (lưu ảnh), Nginx (reverse proxy ở production)

---

## Ba loại tài khoản

| Vai trò | Làm được gì |
| --- | --- |
| Guest | Xem công thức đã xuất bản, xem danh mục, tìm kiếm — không cần đăng nhập |
| Author | Thêm quyền: tạo/sửa/xóa công thức **của chính mình**, upload ảnh, quản lý nguyên liệu và các bước, tự xuất bản hoặc lưu trữ |
| Admin | Thêm quyền: quản lý danh mục, sửa/xóa công thức của bất kỳ ai, vào Hangfire Dashboard, xem log |

Phân quyền gồm 3 tầng: theo role, theo quyền sở hữu tài nguyên (Author chỉ động
được vào bài của mình), và theo policy riêng.

---

## Cấu trúc thư mục

```
PTUDWNC-2026-Nhom21/
├── docker-compose.yml
├── README.md
├── docs/
│   ├── SRS_Culinary_Blog_v1.0.0.pdf   # tài liệu đặc tả gốc
│   ├── DECISIONS.md                   # chốt các chỗ SRS mâu thuẫn - ĐỌC TRƯỚC KHI CODE
│   └── SETUP.md                       # hướng dẫn chạy dự án
└── src/
    ├── Backend/
    │   ├── CulinaryBlog.Domain/          # Entities, Value Objects, Enums, Interfaces repository
    │   ├── CulinaryBlog.Application/     # Commands, Queries, Handlers, DTOs, Validators, Behaviors
    │   ├── CulinaryBlog.Infrastructure/  # EF Core, Repositories, JWT, Redis, MinIO, Hangfire
    │   └── CulinaryBlog.API/             # Minimal API Endpoints, Middleware, Program.cs
    └── Frontend/                         # Next.js App Router
```

Trong `CulinaryBlog.Application/Features/` đã dựng sẵn thư mục cho cả 27 chức
năng, mỗi chức năng có đủ Command/Query + Validator + Handler. Phần Handler để
trống (ném `NotImplementedException`) kèm comment ghi rõ các bước cần làm —
đó chính là phần việc của từng thành viên.

---

## Phân công công việc

Chia theo từng mã chức năng (FR-xxx-0xx) chứ không chia nguyên module, nên ai
cũng phải đụng vào nhiều phần khác nhau của hệ thống.

| MSSV | Tên | Chức năng phụ trách | Số FR |
| --- | --- | --- | :---: |
| 2312770 | Lê Nhật Tiến | FR-RCP-001 (danh sách công thức), FR-RCP-002 (chi tiết), FR-RCP-003 (tạo), FR-RCP-004 (sửa), FR-RCP-007 (xóa), FR-CAT-002 (chi tiết danh mục + công thức) | 6 |
| 2314299 | Lâm Văn Đức | FR-AUTH-001 (đăng ký), FR-AUTH-002 (đăng nhập), FR-AUTH-003 (Google OAuth), FR-AUTH-004 (refresh token), FR-AUTH-005 (đăng xuất), FR-RCP-005 (xuất bản/hủy), FR-RCP-006 (lưu trữ) | 7 |
| 2312777 | Nguyễn Viết Toàn | FR-CAT-001 (danh sách danh mục), FR-CAT-003 (tạo), FR-CAT-004 (sửa), FR-CAT-005 (xóa), FR-SRCH-001 (tìm kiếm toàn văn), FR-SRCH-002/003/004 (lọc, sắp xếp, phân trang), FR-RCP-009 (nguyên liệu) | 9 |
| 2312792 | Nguyễn Đình Tuấn | FR-FILE-001 (upload MinIO), FR-FILE-002 (xóa file), FR-JOB-001 (email chào mừng), FR-JOB-002 (resize ảnh), FR-JOB-003 (sitemap), FR-OBS-001 (health check), FR-OBS-002 (logging), FR-OBS-003 (tracing), FR-RCP-008 (ảnh công thức), FR-RCP-010 (các bước), FR-AUTH-006/007 (hồ sơ cá nhân) | 13 |

Tổng 27 FR (một số mã được nhóm lại nên số đếm ở cột cuối có chênh chút so với
số dòng liệt kê).

**Thứ tự nên làm:** FR-AUTH và FR-CAT trước (các module khác phụ thuộc vào),
rồi đến FR-RCP, cuối cùng là FR-SRCH / FR-FILE / FR-JOB / FR-OBS.

---

## Quy tắc nhánh Git

- `main` — chỉ chứa code ổn định, merge vào từ `develop`
- `develop` — nhánh tích hợp chung
- Mỗi người tạo nhánh riêng cho **từng chức năng**, tách từ `develop`

Cách đặt tên nhánh:

```
<MSSV>-<TênKhôngDấu>-<TênChứcNăng>
```

Ví dụ: `2312770-LeNhatTien-Tao_Cong_Thuc_Moi`

Xong chức năng thì push nhánh lên và mở Pull Request vào `develop`, nhờ một bạn
khác review. Chỉ merge khi không còn xung đột. Chuyển sang chức năng khác thì
tạo nhánh mới, giữ nguyên MSSV và tên, chỉ đổi phần cuối.

Quy ước commit: `<module>: <mô tả ngắn>` — ví dụ `auth: hien thuc dang nhap email`.

---

## Chạy dự án

Hướng dẫn chi tiết từng lệnh nằm ở [`docs/SETUP.md`](./docs/SETUP.md). Tóm tắt:

```powershell
dotnet new sln -n CulinaryBlog
# ... dotnet sln add cho 4 project (xem SETUP.md)
dotnet restore
docker compose up -d
dotnet ef database update --project src\Backend\CulinaryBlog.Infrastructure --startup-project src\Backend\CulinaryBlog.API
dotnet run --project src\Backend\CulinaryBlog.API
```

- API + tài liệu: http://localhost:5000/scalar/v1
- Frontend: http://localhost:3000

---

## Tài liệu đi kèm

| File | Nội dung |
| --- | --- |
| [`docs/SRS_Culinary_Blog_v1.0.0.pdf`](./docs/SRS_Culinary_Blog_v1.0.0.pdf) | Đặc tả gốc: 27 FR, NFR, mô hình dữ liệu, đặc tả API |
| [`docs/DECISIONS.md`](./docs/DECISIONS.md) | Các chỗ SRS mâu thuẫn và cách nhóm thống nhất xử lý |
| [`docs/SETUP.md`](./docs/SETUP.md) | Hướng dẫn cài đặt, chạy, và quy trình Git |

---

## Thành viên nhóm

| MSSV | Họ và tên | Email | GitHub |
| --- | --- | --- | --- |
| 2312770 | Lê Nhật Tiến | 2312770@dlu.edu.vn | https://github.com/2312770-coder |
| 2312792 | Nguyễn Đình Tuấn | 2312792@dlu.edu.vn | https://github.com/2312792-debug |
| 2312777 | Nguyễn Viết Toàn | 2312777@dlu.edu.vn | https://github.com/2312777-rgb |
| 2314299 | Lâm Văn Đức | 2314299@dlu.edu.vn | https://github.com/2314299-debug |

Nhóm 21 — Lớp học phần Phát triển Ứng dụng Web Nâng cao
