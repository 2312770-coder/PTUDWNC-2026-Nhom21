using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Commands.GoogleLogin;

// FR-AUTH-003: Đăng nhập Google OAuth 2.0. SRS mục 8.1: POST /auth/google
// Body: { idToken } - ID Token lấy từ Google Sign-In phía frontend.
public record GoogleLoginCommand(string IdToken, string? IpAddress = null)
    : IRequest<AuthTokensDto>;
