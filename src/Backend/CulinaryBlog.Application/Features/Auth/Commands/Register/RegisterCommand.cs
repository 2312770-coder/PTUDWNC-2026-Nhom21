using MediatR;
using CulinaryBlog.Application.DTOs;

namespace CulinaryBlog.Application.Features.Auth.Commands.Register;

// FR-AUTH-001. UserName tùy chọn - nếu bỏ trống thì ApplicationUser.Create sẽ lấy theo Email.
public record RegisterCommand(
    string Email,
    string Password,
    string DisplayName,
    string? UserName) : IRequest<AuthResponseDto>;
