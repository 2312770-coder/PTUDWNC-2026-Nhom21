# Culinary Blog

Đồ án môn Phát triển Ứng dụng Web Nâng cao — Nhóm 21

Web chia sẻ công thức nấu ăn chuẩn vị Việt Nam. Backend .NET 10 (Clean Architecture) và Frontend Next.js 15 (App Router) là hai ứng dụng tách rời, giao tiếp qua REST API. Toàn bộ yêu cầu và quyết định kỹ thuật lấy từ [SRS v1.0.0](./docs/SRS_Culinary_Blog_v1.0.0.md) và [DECISIONS.md](./docs/DECISIONS.md).

---

## Kế hoạch chi tiết TUẦN 3

> 📖 **Bắt buộc đọc trước khi code**: file [docs/thanh-vien/](./docs/thanh-vien/) của bạn + [docs/DECISIONS.md](./docs/DECISIONS.md)

Các chức năng tuần 3 được thiết kế **hoàn toàn độc lập** theo từng nhánh riêng biệt. Thành viên có thể code song song, không cần đợi người khác xong mới làm. Khi làm xong, đẩy nhánh lên GitHub và báo **trưởng nhóm Tiến** review để merge vào `main`.

### 1. Lê Nhật Tiến (MSSV: 2312770 — Trưởng nhóm)

| # | Chức năng | Mã FR | Tên nhánh | Backend cần làm | Frontend cần làm |
| :---: | :--- | :--- | :--- | :--- | :--- |
| 🔲 | Xem chi tiết danh mục + bài viết | `FR-CAT-002` | `2312770-LNTien-Chi-Tiet-Danh-Muc` | `GetCategoryBySlugQueryHandler` (truy vấn danh mục + các công thức Published kèm theo) | Trang `/categories/[slug]`: Banner thông tin danh mục + Lưới RecipeCard |
| 🔲 | Quản lý gallery ảnh công thức | `FR-RCP-008` | `2312770-LNTien-Gallery-Anh` | Các Command thêm/xóa/đổi ảnh đại diện chính (IsPrimary) trong `Features/Recipes/Commands/ManageImages/*` | Component `RecipeGalleryEditor.tsx` cho phép xem lưới ảnh, chọn ảnh chính ⭐, xóa ảnh |

📘 **Xem hướng dẫn chi tiết**: [docs/thanh-vien/2312770_LeNhatTien.md](./docs/thanh-vien/2312770_LeNhatTien.md)

---

### 2. Lâm Văn Đức (MSSV: 2314299)

| # | Chức năng | Mã FR | Tên nhánh | Backend cần làm | Frontend cần làm |
| :---: | :--- | :--- | :--- | :--- | :--- |
| 🔲 | Đăng nhập Email/Mật khẩu + Rate Limiting | `FR-AUTH-002` | `2314299-LVDuc-Dang-Nhap` | `LoginCommandHandler` (kiểm tra mật khẩu, sinh JWT, lockout 5 lần) + Cấu hình Rate Limiting 5 req/phút | Trang `/login`: Form đăng nhập, lưu token, cập nhật trạng thái User trên Navbar |
| 🔲 | Đăng xuất & Thu hồi token | `FR-AUTH-005` | `2314299-LVDuc-Dang-Xuat` | `LogoutCommandHandler` (thu hồi refresh token trong CSDL, xóa session) | Nút "Đăng xuất" trong menu Navbar, xóa token và điều hướng về trang đăng nhập |

📘 **Xem hướng dẫn chi tiết**: [docs/thanh-vien/2314299_LamVanDuc.md](./docs/thanh-vien/2314299_LamVanDuc.md)

---

### 3. Nguyễn Viết Toàn (MSSV: 2312777)

| # | Chức năng | Mã FR | Tên nhánh | Backend cần làm | Frontend cần làm |
| :---: | :--- | :--- | :--- | :--- | :--- |
| 🔲 | Admin tạo danh mục mới | `FR-CAT-003` | `2312777-NVToan-Tao-Danh-Muc` | `CreateCategoryCommandHandler` (kiểm tra trùng tên, sinh slug chuẩn SEO, xóa cache danh mục) | Trang quản trị `/admin/categories`: Form nhập tên, mô tả, OrderIndex, ảnh đại diện |
| 🔲 | Tích hợp Redis Caching cho Danh mục | `FR-CAT-001` | `2312777-NVToan-Cache-Danh-Muc` | Tự inject `ICacheService` vào `GetCategoriesQueryHandler`, kiểm tra cache HIT / MISS và set TTL 30 phút | Kiểm tra thời gian phản hồi API nhanh vượt trội khi có Redis |

📘 **Xem hướng dẫn chi tiết**: [docs/thanh-vien/2312777_NguyenVietToan.md](./docs/thanh-vien/2312777_NguyenVietToan.md)

---

### 4. Nguyễn Đình Tuấn (MSSV: 2312792)

| # | Chức năng | Mã FR | Tên nhánh | Backend cần làm | Frontend cần làm |
| :---: | :--- | :--- | :--- | :--- | :--- |
| 🔲 | Quản lý các bước nấu (D9) | `FR-RCP-010` | `2312792-NDTuan-Cac-Buoc-Nau` | Các Command thêm/sửa/xóa bước nấu trong `Features/Recipes/Commands/ManageSteps/*` (D9: tự sinh stepNumber) | Component `StepListEditor.tsx`: Thêm bước nấu, hướng dẫn, thời gian hẹn giờ và gắn ảnh MinIO |
| 🔲 | Quản lý nguyên liệu (D10) | `FR-RCP-009` | `2312792-NDTuan-Nguyen-Lieu` | Các Command thêm/sửa/xóa nguyên liệu trong `Features/Recipes/Commands/ManageIngredients/*` (D10: null unit) | Component `IngredientListEditor.tsx`: Bảng danh sách nguyên liệu, định lượng, ghi chú nêm nếm |

📘 **Xem hướng dẫn chi tiết**: [docs/thanh-vien/2312792_NguyenDinhTuan.md](./docs/thanh-vien/2312792_NguyenDinhTuan.md)

---

## Phân công tổng thể (Lộ trình 6 Tuần — 7 chức năng / thành viên)

Toàn bộ 28 chức năng chia đều cho 4 thành viên — **đúng 7 chức năng mỗi người**, phân bổ đều đặn trong 6 tuần học phần:

| MSSV | Họ và tên | Chuyên môn phụ trách | Danh sách 7 chức năng | Số FR | Hướng dẫn riêng |
| :---: | :--- | :--- | :--- | :---: | :---: |
| **2312770** | **Lê Nhật Tiến** *(Trưởng nhóm)* | **Core Recipe & Author Experience** | `FR-RCP-003` (Tạo công thức nháp), `FR-CAT-002` (Chi tiết danh mục + recipes), `FR-RCP-008` (Quản lý gallery ảnh), `FR-RCP-004` (Sửa công thức), `FR-RCP-006` (Lưu trữ Archive), `FR-RCP-007` (Xóa mềm Recipe D1), `FR-JOB-003` (Sinh sitemap.xml) | **7** | [Xem file](./docs/thanh-vien/2312770_LeNhatTien.md) |
| **2314299** | **Lâm Văn Đức** | **Authentication & Security** | `FR-AUTH-001` (Đăng ký), `FR-AUTH-002` (Đăng nhập Email + Rate Limiting), `FR-AUTH-005` (Đăng xuất), `FR-AUTH-004` (Refresh token rotation), `FR-AUTH-003` (Google OAuth 2.0), `FR-RCP-005` (Xuất bản/Hủy xuất bản D11), `FR-JOB-001` (Email chào mừng Hangfire) | **7** | [Xem file](./docs/thanh-vien/2314299_LamVanDuc.md) |
| **2312777** | **Nguyễn Viết Toàn** | **Category, Cache & Search Engine** | `FR-CAT-001` (Danh sách danh mục + Redis Cache), `FR-CAT-003` (Tạo danh mục), `FR-CAT-004` (Sửa danh mục D12), `FR-CAT-005` (Xóa mềm danh mục D1), `FR-SRCH-001` (Full-Text Search unaccent), `FR-SRCH-002..004` (Lọc đa tiêu chí, sắp xếp D8, phân trang), `FR-INT-001` (Đánh giá sao Rating 1-5) | **7** | [Xem file](./docs/thanh-vien/2312777_NguyenVietToan.md) |
| **2312792** | **Nguyễn Đình Tuấn** | **Storage, Recipe Details & Profile** | `FR-FILE-001` (Upload ảnh MinIO 5MB), `FR-FILE-002` (Xóa ảnh MinIO), `FR-RCP-010` (Quản lý các bước nấu D9), `FR-RCP-009` (Quản lý nguyên liệu D10), `FR-AUTH-006` (Xem Profile), `FR-AUTH-007` (Cập nhật Profile & Avatar), `FR-JOB-002` (Resize ảnh thumbnail Hangfire) | **7** | [Xem file](./docs/thanh-vien/2312792_NguyenDinhTuan.md) |

---

## Quy tắc nhánh Git & Quy trình làm việc

> ⚠️ **Quan trọng**: Thành viên **KHÔNG tự merge vào `main`**. Chỉ có trưởng nhóm **Lê Nhật Tiến (2312770)** mới được merge sau khi review code và xác nhận không xung đột.

### Quy ước tên nhánh theo từng chức năng

Mỗi chức năng (FR) được làm trên **một nhánh riêng biệt**, tự tạo từ nhánh `main` theo cú pháp:

```
<MSSV>-<VietTatHoDemTen>-<Ten-Chuc-Nang>
```

| Thành viên | Cú pháp tiền tố | Ví dụ nhánh chức năng tuần 3 |
| :--- | :--- | :--- |
| Lê Nhật Tiến (Trưởng nhóm) | `2312770-LNTien-` | `2312770-LNTien-Chi-Tiet-Danh-Muc`, `2312770-LNTien-Gallery-Anh` |
| Lâm Văn Đức | `2314299-LVDuc-` | `2314299-LVDuc-Dang-Nhap`, `2314299-LVDuc-Dang-Xuat` |
| Nguyễn Viết Toàn | `2312777-NVToan-` | `2312777-NVToan-Tao-Danh-Muc`, `2312777-NVToan-Cache-Danh-Muc` |
| Nguyễn Đình Tuấn | `2312792-NDTuan-` | `2312792-NDTuan-Cac-Buoc-Nau`, `2312792-NDTuan-Nguyen-Lieu` |

### Quy trình làm việc cho từng chức năng

```powershell
# 1. Luôn cập nhật code mới nhất từ main trước khi tạo nhánh mới
git checkout main
git pull origin main

# 2. Tự tạo nhánh mới cho chức năng bạn sắp làm
# Ví dụ Đức làm đăng nhập:
git checkout -b 2314299-LVDuc-Dang-Nhap

# 3. Code chức năng của bạn...

# 4. Kiểm tra trước khi đẩy (bắt buộc 0 lỗi)
dotnet build CulinaryBlog.slnx     # 0 lỗi
npm run lint                        # 0 lỗi (chạy trong src/Frontend)

# 5. Commit theo chuẩn: <module>: <mô tả ngắn> <mã FR>
git add .
git commit -m "auth: hien thuc dang nhap email rate limiting FR-AUTH-002"

# 6. Đẩy nhánh chức năng lên GitHub
git push -u origin 2314299-LVDuc-Dang-Nhap

# 7. Báo trưởng nhóm Tiến qua Zalo để Tiến review và merge vào main
# Sau khi Tiến merge xong, chuyển lại về main để pull về làm chức năng tiếp theo:
git checkout main
git pull origin main
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
| 🗄️ [`docs/DATABASE.md`](./docs/DATABASE.md) | Hướng dẫn CSDL, EF Core Migration và cơ chế nạp dữ liệu mẫu Bogus |
| ⚖️ [`docs/DECISIONS.md`](./docs/DECISIONS.md) | 12 quyết định kiến trúc cốt lõi (Soft delete, Slug, Steps, Ingredients…) |
| 📄 [`docs/SRS_Culinary_Blog_v1.0.0.md`](./docs/SRS_Culinary_Blog_v1.0.0.md) | Toàn văn đặc tả SRS 27 FRs, schema CSDL, API contracts |
| 🛠️ [`docs/SETUP.md`](./docs/SETUP.md) | Hướng dẫn thiết lập môi trường phát triển cá nhân |

---

*Nhóm 21 — Lớp học phần Phát triển Ứng dụng Web Nâng cao — Năm học 2025–2026*
