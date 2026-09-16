using Microsoft.AspNetCore.Identity;

namespace CulinaryBlog.Domain.Entities;

// SRS mục 7.7 - kế thừa IdentityUser<string>, bảng "AspNetUsers".
// Identity đã có sẵn: Id, UserName, Email, PasswordHash, LockoutEnd, ... nên
// ở đây chỉ khai báo thêm các cột nghiệp vụ riêng của Culinary Blog.
public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    // Dùng cho đăng ký thông thường (FR-AUTH-001).
    public static ApplicationUser Create(string email, string displayName)
        => new()
        {
            Email = email,
            UserName = email, // SRS mục 8.1: đăng ký chỉ cần email + password + displayName
            DisplayName = displayName,
        };

    // Dùng khi đăng nhập Google lần đầu (FR-AUTH-003).
    public static ApplicationUser CreateFromGoogle(string email, string displayName, string? avatarUrl)
        => new()
        {
            Email = email,
            UserName = email,
            DisplayName = displayName,
            AvatarUrl = avatarUrl,
            EmailConfirmed = true, // Google đã xác nhận email sẵn
        };

    // FR-AUTH-007: cập nhật hồ sơ cá nhân.
    public void UpdateProfile(string? displayName, string? avatarUrl, string? bio)
    {
        if (!string.IsNullOrWhiteSpace(displayName)) DisplayName = displayName.Trim();
        AvatarUrl = avatarUrl ?? AvatarUrl;
        Bio = bio ?? Bio;
    }
}
