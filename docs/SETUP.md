# Hướng dẫn chạy dự án trên máy cá nhân

Làm tuần tự từ trên xuống. Các lệnh viết cho **PowerShell** trên Windows.

## 0. Cài sẵn

- .NET 10 SDK
- Node.js 20+ và npm 10+
- Docker Desktop (đang chạy)
- Git

Nếu máy đã cài sẵn PostgreSQL bản desktop, nhớ **tắt service đó đi** (hoặc gỡ),
vì nó chiếm cổng 5432 làm container Docker không kết nối được.

## 1. Tạo file solution

Các file `.csproj` đã có sẵn, chỉ cần tạo `.sln` và gán 4 project vào:

```powershell
dotnet new sln -n CulinaryBlog

dotnet sln add src\Backend\CulinaryBlog.Domain\CulinaryBlog.Domain.csproj
dotnet sln add src\Backend\CulinaryBlog.Application\CulinaryBlog.Application.csproj
dotnet sln add src\Backend\CulinaryBlog.Infrastructure\CulinaryBlog.Infrastructure.csproj
dotnet sln add src\Backend\CulinaryBlog.API\CulinaryBlog.API.csproj
```

Tham chiếu giữa các tầng đã khai báo sẵn trong từng `.csproj`, không cần
`dotnet add reference`.

## 2. Restore và build

```powershell
dotnet restore
dotnet build
```

Nếu báo lỗi version package không tồn tại: mở `.csproj` tương ứng, chạy
`dotnet add package <TênGói>` để dotnet tự lấy bản mới nhất.

## 3. Đặt JWT secret (không commit lên Git)

```powershell
cd src\Backend\CulinaryBlog.API
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:Key" "doi-thanh-chuoi-bi-mat-cua-rieng-ban-toi-thieu-32-ky-tu"
cd ..\..\..
```

## 4. Khởi chạy hạ tầng

Bật Docker Desktop trước, rồi:

```powershell
docker compose up -d
docker compose ps
```

Phải thấy đủ 5 container ở trạng thái `Up`: postgres, redis, minio, seq, mailhog.

Các giao diện quản trị:
- MinIO: http://localhost:9001 (minioadmin / minioadmin)
- Seq (xem log): http://localhost:5341
- Mailhog (xem mail): http://localhost:8025

## 5. Tạo database

```powershell
dotnet tool install --global dotnet-ef
```

(bỏ qua nếu đã cài)

```powershell
dotnet ef migrations add InitialCreate `
  --project src\Backend\CulinaryBlog.Infrastructure `
  --startup-project src\Backend\CulinaryBlog.API `
  --output-dir Persistence\Migrations

dotnet ef database update `
  --project src\Backend\CulinaryBlog.Infrastructure `
  --startup-project src\Backend\CulinaryBlog.API
```

Dấu `` ` `` cuối dòng là ký tự xuống dòng của PowerShell, gõ đúng như vậy.

Khi nào cần Full-Text Search (FR-SRCH-001), bật thêm extension cho PostgreSQL:

```powershell
docker exec -it culinaryblog-postgres psql -U postgres -d culinary_blog -c "CREATE EXTENSION IF NOT EXISTS unaccent;"
```

## 6. Chạy Backend

```powershell
dotnet run --project src\Backend\CulinaryBlog.API
```

- Tài liệu API: http://localhost:5000/scalar/v1
- Health check: http://localhost:5000/health , `/health/live` , `/health/ready`

Các endpoint nghiệp vụ sẽ trả lỗi **501 Not Implemented** kèm mã FR tương ứng —
đó là đúng thiết kế, vì phần đó dành cho từng thành viên hiện thực.

## 7. Chạy Frontend

Mở terminal mới:

```powershell
cd src\Frontend
copy .env.example .env.local
```

Mở `.env.local`, đổi `NEXTAUTH_SECRET` thành chuỗi ngẫu nhiên của riêng bạn.
Sinh nhanh một chuỗi:

```powershell
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))
```

Rồi:

```powershell
npm install
npm run dev
```

Web chạy tại http://localhost:3000

Nếu PowerShell báo *"running scripts is disabled"*, chạy một lần:

```powershell
Set-ExecutionPolicy -Scope CurrentUser RemoteSigned
```

## 8. Quy trình làm việc với Git

Mỗi thành viên tạo nhánh riêng cho từng chức năng, tách từ `develop`:

```powershell
git checkout develop
git pull
git checkout -b 2312770-LeNhatTien-Tao_Cong_Thuc_Moi
```

Code xong thì:

```powershell
git add .
git commit -m "recipe: hien thuc FR-RCP-003 tao cong thuc moi"
git push -u origin 2312770-LeNhatTien-Tao_Cong_Thuc_Moi
```

Rồi vào GitHub mở Pull Request vào nhánh `develop`, nhờ một bạn khác review.
Chỉ merge khi không còn xung đột và build chạy được.

## 9. Bắt đầu code từ đâu

1. Mở `docs/DECISIONS.md` đọc trước — có mấy chỗ SRS mâu thuẫn đã được chốt sẵn.
2. Tìm file Handler ứng với mã FR mình phụ trách (xem bảng phân công ở README).
   Mỗi Handler đang là khung rỗng, bên trên có comment ghi rõ các bước cần làm.
3. Xóa dòng `throw new NotImplementedException(...)` và viết logic vào.
4. Test bằng Scalar tại http://localhost:5000/scalar/v1
