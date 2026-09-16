using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Commands.RefreshToken;

// FR-AUTH-004: Làm mới Access Token. SRS mục 8.1: POST /auth/refresh
// Body: { refreshToken } -> 200 { accessToken, refreshToken, expiresIn }
public record RefreshTokenCommand(string RefreshToken, string? IpAddress = null)
    : IRequest<AuthTokensDto>;
