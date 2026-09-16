namespace CulinaryBlog.Application.Common.Interfaces;

// SRS mục 6.2 - cung cấp thông tin user của request hiện tại cho tầng Application
// mà không cần phụ thuộc HttpContext. Implement ở Infrastructure bằng
// IHttpContextAccessor.
public interface ICurrentUser
{
    string? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    IReadOnlyList<string> Roles { get; }
}
