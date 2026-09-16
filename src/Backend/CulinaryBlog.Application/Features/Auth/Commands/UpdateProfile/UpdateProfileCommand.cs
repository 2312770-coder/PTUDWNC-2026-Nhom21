using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Commands.UpdateProfile;

// FR-AUTH-007: Cập nhật hồ sơ. SRS mục 8.1: PATCH /auth/me (cần Bearer JWT)
// Body: { displayName?, avatarUrl?, bio? } - tất cả optional (partial update).
public record UpdateProfileCommand(string? DisplayName, string? AvatarUrl, string? Bio)
    : IRequest<UserDto>;
