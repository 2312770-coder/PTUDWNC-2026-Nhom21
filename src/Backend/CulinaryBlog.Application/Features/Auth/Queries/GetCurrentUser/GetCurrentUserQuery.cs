using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Queries.GetCurrentUser;

// FR-AUTH-006: Xem hồ sơ cá nhân. SRS mục 8.1: GET /auth/me (cần Bearer JWT)
// -> 200 { id, email, displayName, avatarUrl, bio, roles }
public record GetCurrentUserQuery : IRequest<UserDto>;
