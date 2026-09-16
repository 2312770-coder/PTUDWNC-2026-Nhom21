using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Commands.Login;

// FR-AUTH-002: Đăng nhập email/mật khẩu. SRS mục 8.1: POST /auth/login
// Body: { email, password } -> 200 { accessToken, refreshToken, expiresIn }
// Endpoint này bị giới hạn 5 request/phút (rate limit) để chống brute-force.
public record LoginCommand(string Email, string Password, string? IpAddress = null)
    : IRequest<AuthTokensDto>;
