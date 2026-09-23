using MediatR;
using CulinaryBlog.Application.DTOs;

namespace CulinaryBlog.Application.Features.Auth.Commands.Login;

// FR-AUTH-002: Đăng nhập bằng Email và Mật khẩu.
public record LoginCommand(
    string Email,
    string Password,
    string? ClientIp = null) : IRequest<AuthResponseDto>;
