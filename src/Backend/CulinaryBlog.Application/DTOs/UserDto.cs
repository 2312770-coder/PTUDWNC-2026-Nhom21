namespace CulinaryBlog.Application.DTOs;

// SRS mục 8.1 - response của GET /auth/me
public record UserDto(
    string Id,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    string? Bio,
    IReadOnlyList<string> Roles
);

// SRS mục 8.1 - response 201 của POST /auth/register
public record RegisterResponseDto(
    string UserId,
    string Email,
    string DisplayName
);

// SRS mục 8.1 - response của /auth/login, /auth/google, /auth/refresh
public record AuthTokensDto(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn   // số giây còn lại của Access Token
);
