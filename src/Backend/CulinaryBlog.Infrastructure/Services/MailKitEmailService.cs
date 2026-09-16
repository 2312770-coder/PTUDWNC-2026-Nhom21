using CulinaryBlog.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace CulinaryBlog.Infrastructure.Services;

// Dùng cho FR-JOB-001 (email chào mừng) - CHƯA HIỆN THỰC, dành cho người
// phụ trách FR-JOB.
//
// Gợi ý:
//   1. Cài NuGet "MailKit".
//   2. Đọc cấu hình SMTP từ section "Smtp" trong appsettings.Development.json
//      (đang trỏ vào Mailhog: localhost:1025, không cần user/password).
//   3. Gửi mail dạng HTML. Ở môi trường dev, mail không đi ra ngoài thật mà
//      hiện trong giao diện Mailhog tại http://localhost:8025 - rất tiện để test.
public class MailKitEmailService : IEmailService
{
    private readonly ILogger<MailKitEmailService> _logger;

    public MailKitEmailService(ILogger<MailKitEmailService> logger) => _logger = logger;

    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        => throw new NotImplementedException(
            "Gửi email (phục vụ FR-JOB-001) chưa được hiện thực.");
}
