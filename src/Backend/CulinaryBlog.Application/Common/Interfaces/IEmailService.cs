namespace CulinaryBlog.Application.Common.Interfaces;

// SRS mục 6.2 - gửi email. Ở môi trường dev trỏ vào Mailhog (cổng 1025),
// xem mail đã gửi tại http://localhost:8025.
public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
}
