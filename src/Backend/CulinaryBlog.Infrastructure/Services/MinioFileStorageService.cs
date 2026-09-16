using CulinaryBlog.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CulinaryBlog.Infrastructure.Services;

// FR-FILE-001 / FR-FILE-002 - CHƯA HIỆN THỰC, dành cho người phụ trách FR-FILE.
//
// Gợi ý:
//   1. Cài NuGet "Minio" (hoặc AWSSDK.S3 vì MinIO tương thích S3).
//   2. Đăng ký IMinioClient trong DependencyInjection với endpoint/accessKey/
//      secretKey đọc từ cấu hình "MinIO" (đã có sẵn trong appsettings.Development.json).
//   3. UploadAsync: tạo bucket "culinary-blog" nếu chưa có (policy public-read),
//      đặt tên file dạng {folder}/{Guid.NewGuid()}{ext} để chống path traversal,
//      giữ nguyên ContentType gốc, trả về public URL.
//   4. DeleteAsync: tách object name từ URL rồi gọi RemoveObjectAsync.
//      Phải idempotent - object không tồn tại thì bỏ qua, KHÔNG ném exception
//      (SRS mục 3.5).
//
// Việc validate file (5MB, MIME type, magic bytes - CONS-007) làm ở tầng
// Application (handler FR-RCP-008), không làm ở đây.
public class MinioFileStorageService : IFileStorageService
{
    private readonly ILogger<MinioFileStorageService> _logger;

    public MinioFileStorageService(ILogger<MinioFileStorageService> logger)
        => _logger = logger;

    public Task<string> UploadAsync(Stream content, string fileName, string contentType,
        string folder, CancellationToken ct = default)
        => throw new NotImplementedException(
            "FR-FILE-001 (Upload file lên MinIO) chưa được hiện thực.");

    public Task DeleteAsync(string fileUrl, CancellationToken ct = default)
        => throw new NotImplementedException(
            "FR-FILE-002 (Xóa file khỏi MinIO) chưa được hiện thực.");
}
