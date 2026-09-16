# Thư mục Background Jobs (FR-JOB) — chưa hiện thực

Dành cho người phụ trách module FR-JOB. SRS mục 3.6 yêu cầu 3 job chạy bằng
Hangfire (lưu queue trong PostgreSQL, dashboard tại `/hangfire`, chỉ Admin vào được):

| Mã FR | Job | Loại | Kích hoạt khi |
| --- | --- | --- | --- |
| FR-JOB-001 | Gửi email chào mừng | Fire-and-forget | Sau khi đăng ký thành công (FR-AUTH-001) |
| FR-JOB-002 | Tạo thumbnail 300x300 + medium 800x600 | Fire-and-forget | Sau khi upload ảnh thành công (FR-RCP-008) |
| FR-JOB-003 | Sinh sitemap.xml | Recurring | Hằng ngày 02:00 UTC (cron `0 2 * * *`) |

Các bước gợi ý:

1. Cài NuGet `Hangfire.AspNetCore` và `Hangfire.PostgreSql`.
2. Đăng ký trong `DependencyInjection.cs` của Infrastructure, dùng chung
   connection string với database chính.
3. Tạo class job tương ứng trong thư mục này, ví dụ `WelcomeEmailJob.cs`,
   `ImageResizeJob.cs`, `SitemapGenerationJob.cs`.
4. Map dashboard `/hangfire` trong `Program.cs` với authorization chỉ cho Admin.
5. Chính sách retry theo đúng bảng trong SRS mục 3.6 (job email retry 3 lần
   với exponential backoff 1/5/30 phút, job ảnh retry 3 lần, sitemap retry 2 lần).

Xóa file này sau khi đã hiện thực xong.
