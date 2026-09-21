# Hướng Dẫn Quản Lý Cơ Sở Dữ Liệu & Nạp Dữ Liệu Mẫu (Database Guide)
### Dự án: Culinary Blog — Nhóm 21 (PTUDWNC-2026)

Tài liệu này hướng dẫn chi tiết về cấu trúc cơ sở dữ liệu PostgreSQL, các lệnh EF Core Migration, cơ chế tự động nạp dữ liệu mẫu (Seeder) và cách kiểm tra dữ liệu ngẫu nhiên trong hệ thống.

---

## 1. Tổng Quan Kiến Trúc Cơ Sở Dữ Liệu

Hệ thống sử dụng **PostgreSQL 16** kết hợp **EF Core 10** theo mô hình Code-First:
- **Tự động áp dụng Migration**: Khi chạy ứng dụng Backend ở môi trường `Development`, `Program.cs` sẽ tự động thực thi `db.Database.MigrateAsync()`.
- **Global Query Filter (Soft Delete)**: Mọi bảng kế thừa `BaseEntity` (`Recipes`, `Categories`, v.v.) tự động áp dụng bộ lọc `WHERE "IsDeleted" = false`. Khi xóa mềm, dữ liệu không bị mất khỏi CSDL.
- **Optimistic Concurrency Control**: Cột `RowVersion` (kiểu `bytea`) ngăn chặn xung đột khi nhiều người cùng chỉnh sửa một bản ghi.

### Các bảng chính trong CSDL:
| Tên Bảng | Thực thể tương ứng | Mô tả |
| :--- | :--- | :--- |
| `AspNetUsers`, `AspNetRoles`... | `ApplicationUser` | Quản lý người dùng, vai trò phân quyền (ASP.NET Core Identity). |
| `Categories` | `Category` | Danh mục ẩm thực (Món chính, Món nước, Món nướng BBQ, v.v.). |
| `Recipes` | `Recipe` | Công thức nấu ăn (Aggregate Root), nhúng kèm cột dinh dưỡng `Nutrition_*`. |
| `RecipeIngredients` | `RecipeIngredient` | Danh sách nguyên liệu chi tiết cho từng công thức. |
| `RecipeSteps` | `RecipeStep` | Các bước chế biến (kèm hẹn giờ và ảnh minh họa). |
| `RecipeImages` | `RecipeImage` | Gallery ảnh công thức (lưu đường dẫn MinIO S3). |
| `RefreshTokens` | `RefreshToken` | Quản lý phiên đăng nhập JWT, chống tấn công replay token. |

---

## 2. Dữ Liệu Mẫu Ngẫu Nhiên (Database Seeder)

Lớp `DatabaseSeeder.cs` đặt tại `src/Backend/CulinaryBlog.Infrastructure/Persistence/DatabaseSeeder.cs`.

### Tiêu chuẩn dữ liệu nạp sẵn:
1. **Vai trò (Roles)**: `Admin`, `Author`, `User`.
2. **Tài khoản mặc định**:
   - **Quản trị viên**: `admin@culinary.local` / Mật khẩu: `Admin@123` (Role: `Admin`)
   - **Bếp Trưởng An**: `bep_truong_an@culinary.local` / Mật khẩu: `Author@123` (Role: `Author`)
   - Các tác giả phụ: `ha_bep_nha@culinary.local`, `quang_am_thuc_viet@culinary.local`, `mai_chay_tam@culinary.local`, `vinh_bbq@culinary.local`.
3. **Danh mục (Categories)**: **22 danh mục** ẩm thực đa dạng (Món chính, Món nước & canh, Món nướng BBQ, Ẩm thực 3 miền, Món chay, v.v.) — *Đạt chỉ tiêu $\ge 20$ danh mục*.
4. **Công thức (Recipes)**: Tối thiểu **100 Recipes** được sinh ngẫu nhiên bằng thư viện `Bogus` kết hợp các dữ liệu thực tế:
   - **Mỗi Recipe có từ 10 - 14 nguyên liệu** (`RecipeIngredient`) — *Đạt chỉ tiêu $\ge 10$ nguyên liệu*.
   - **Mỗi Recipe có từ 5 - 7 bước chế biến chuẩn** (`RecipeStep`) — *Đạt chỉ tiêu $\ge 5$ bước*.
   - Có đầy đủ thông tin dinh dưỡng (`RecipeNutrition`), ảnh bìa (`RecipeImage`), thời gian chuẩn bị và nấu.

---

## 3. Các Bước Khởi Chạy & Nạp Dữ Liệu Mới

### Bước 1: Khởi động container PostgreSQL
Đảm bảo **Docker Desktop đang chạy**, sau đó mở terminal tại thư mục gốc dự án:
```powershell
docker compose up -d postgres
```
Kiểm tra container `culinaryblog-postgres` đang ở trạng thái `Up` (cổng `5432`).

### Bước 2: Chạy Backend để tự động Migrate & Seed Data
Chỉ cần chạy lệnh sau từ thư mục gốc:
```powershell
dotnet run --project src/Backend/CulinaryBlog.API
```
> **Cơ chế hoạt động**:
> - Backend phát hiện môi trường `Development`.
> - Tự động gọi `db.Database.MigrateAsync()` để tạo các bảng nếu chưa có.
> - Tự động gọi `DatabaseSeeder.SeedAsync()` để nạp 22 danh mục và 100 công thức ngẫu nhiên.
> - Bạn sẽ thấy log chi tiết:
>   `Tổng kết cơ sở dữ liệu hiện có: 22 Categories, 100 Recipes.`

---

## 4. Các Lệnh Quản Lý EF Core Migration Thường Dùng

Nếu cần thay đổi cấu trúc bảng hoặc reset database:

### Tạo Migration mới:
```powershell
dotnet ef migrations add <TenMigration> `
  --project src/Backend/CulinaryBlog.Infrastructure `
  --startup-project src/Backend/CulinaryBlog.API `
  --output-dir Persistence/Migrations
```

### Cập nhật database theo Migration:
```powershell
dotnet ef database update `
  --project src/Backend/CulinaryBlog.Infrastructure `
  --startup-project src/Backend/CulinaryBlog.API
```

### Xóa và làm mới database từ đầu (Reset Data):
Nếu muốn xóa sạch toàn bộ dữ liệu để nạp lại mới 100 công thức:
```powershell
# Xóa container postgres kèm volume dữ liệu cũ
docker compose down -v
# Khởi động lại container postgres sạch
docker compose up -d postgres
# Chạy backend để tự động tạo bảng và nạp lại dữ liệu
dotnet run --project src/Backend/CulinaryBlog.API
```

---

## 5. Hướng Dẫn Kết Nối Trực Tiếp Kiểm Tra CSDL

Bạn có thể kết nối vào PostgreSQL bằng **DBeaver**, **pgAdmin**, hoặc extension **Database Client** trong VS Code:

- **Host**: `localhost`
- **Port**: `5432`
- **Database**: `culinary_blog`
- **Username**: `postgres`
- **Password**: `postgres`

### Các câu lệnh SQL kiểm tra nhanh số lượng:
```sql
-- Kiểm tra tổng số danh mục (kết quả >= 20)
SELECT COUNT(*) AS total_categories FROM "Categories" WHERE "IsDeleted" = false;

-- Kiểm tra tổng số công thức (kết quả >= 100)
SELECT COUNT(*) AS total_recipes FROM "Recipes" WHERE "IsDeleted" = false;

-- Kiểm tra số lượng nguyên liệu trong từng công thức (tất cả đều >= 10)
SELECT "RecipeId", COUNT(*) AS ingredient_count 
FROM "RecipeIngredients" 
GROUP BY "RecipeId" 
ORDER BY ingredient_count ASC;

-- Kiểm tra số lượng bước chế biến trong từng công thức (tất cả đều >= 5)
SELECT "RecipeId", COUNT(*) AS step_count 
FROM "RecipeSteps" 
GROUP BY "RecipeId" 
ORDER BY step_count ASC;
```

---

## 6. Kiểm Tra Qua Giao Diện API & Web

1. **Kiểm tra API trên Scalar**:
   - Truy cập: `http://localhost:5000/scalar/v1`
   - Gọi `GET /api/v1/categories` -> Trả về danh sách 22 danh mục.
   - Gọi `GET /api/v1/recipes?page=1&pageSize=12` -> Trả về danh sách phân trang công thức.
2. **Kiểm tra trên Giao diện Web Frontend**:
   - Truy cập: `http://localhost:3000` hoặc `http://localhost:3000/recipes`
   - Khám phá các món ăn ngẫu nhiên đã được nạp sẵn.
