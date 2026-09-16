using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Commands.Register;

// FR-AUTH-001: Đăng ký tài khoản. SRS mục 8.1: POST /auth/register
// Body: { email, password, displayName } -> 201 { userId, email, displayName }
public record RegisterCommand(string Email, string Password, string DisplayName)
    : IRequest<RegisterResponseDto>;
