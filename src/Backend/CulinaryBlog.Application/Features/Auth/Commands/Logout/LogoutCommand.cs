using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Commands.Logout;

// FR-AUTH-005: Đăng xuất. SRS mục 8.1: POST /auth/logout (cần Bearer JWT)
// Body: { refreshToken } -> 204 No Content
public record LogoutCommand(string RefreshToken) : IRequest;
