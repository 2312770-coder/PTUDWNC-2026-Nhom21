using CulinaryBlog.Domain.Entities;
using System.Security.Claims;

namespace CulinaryBlog.Application.Common.Interfaces;

// SRS mục 6.2 - service interface khai báo ở Application, implement ở Infrastructure.
public interface IJwtService
{
    // Sinh Access Token (JWT, 15 phút) chứa claims: sub, email, role...
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);

    // Sinh raw refresh token ngẫu nhiên. Trả về (rawToken, tokenHash):
    // rawToken trả cho client, tokenHash mới là thứ lưu vào DB (SRS mục 7.8).
    (string RawToken, string TokenHash) GenerateRefreshToken();

    // Băm SHA-256 một raw refresh token để đối chiếu với TokenHash trong DB.
    string HashRefreshToken(string rawToken);

    // Đọc claims từ Access Token đã hết hạn (dùng khi refresh).
    // Trả null nếu chữ ký không hợp lệ.
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
