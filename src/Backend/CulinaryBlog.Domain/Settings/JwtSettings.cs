namespace CulinaryBlog.Domain.Settings;

// Bind từ appsettings.json, section "JwtSettings".
// CONS-004: access token 15 phút, refresh token 7 ngày.
// Riêng Key KHÔNG để trong appsettings - dùng dotnet user-secrets (xem docs/SETUP.md).
public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; init; } = 15;
    public int RefreshTokenExpiryDays { get; init; } = 7;
}
