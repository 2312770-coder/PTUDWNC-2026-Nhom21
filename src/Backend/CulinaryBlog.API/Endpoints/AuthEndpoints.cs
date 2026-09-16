// SRS mục 8.1 - Authentication Module (/api/v1/auth).
// Endpoint chỉ làm 3 việc: nhận request, gửi qua MediatR, trả response.
// Toàn bộ logic nghiệp vụ nằm trong Handler ở tầng Application.

using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Application.Features.Auth.Commands.GoogleLogin;
using CulinaryBlog.Application.Features.Auth.Commands.Login;
using CulinaryBlog.Application.Features.Auth.Commands.Logout;
using CulinaryBlog.Application.Features.Auth.Commands.RefreshToken;
using CulinaryBlog.Application.Features.Auth.Commands.Register;
using CulinaryBlog.Application.Features.Auth.Commands.UpdateProfile;
using CulinaryBlog.Application.Features.Auth.Queries.GetCurrentUser;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Authentication");

        // FR-AUTH-001
        group.MapPost("/register", async (RegisterCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/v1/users/{result.UserId}", ApiResponse.Ok(result));
        })
        .WithName("Register")
        .WithSummary("Đăng ký tài khoản mới")
        .AllowAnonymous()
        .Produces<ApiResponse<RegisterResponseDto>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);

        // FR-AUTH-002 - giới hạn 5 request/phút chống dò mật khẩu
        group.MapPost("/login", async (LoginCommand command, HttpContext http, ISender sender, CancellationToken ct) =>
        {
            var ip = http.Connection.RemoteIpAddress?.ToString();
            var result = await sender.Send(command with { IpAddress = ip }, ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("Login")
        .WithSummary("Đăng nhập bằng email và mật khẩu")
        .AllowAnonymous()
        .RequireRateLimiting("login-policy")
        .Produces<ApiResponse<AuthTokensDto>>()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status429TooManyRequests);

        // FR-AUTH-003
        group.MapPost("/google", async (GoogleLoginCommand command, HttpContext http, ISender sender, CancellationToken ct) =>
        {
            var ip = http.Connection.RemoteIpAddress?.ToString();
            var result = await sender.Send(command with { IpAddress = ip }, ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("GoogleLogin")
        .WithSummary("Đăng nhập bằng Google OAuth 2.0")
        .AllowAnonymous()
        .Produces<ApiResponse<AuthTokensDto>>()
        .ProducesProblem(StatusCodes.Status400BadRequest);

        // FR-AUTH-004
        group.MapPost("/refresh", async (RefreshTokenCommand command, HttpContext http, ISender sender, CancellationToken ct) =>
        {
            var ip = http.Connection.RemoteIpAddress?.ToString();
            var result = await sender.Send(command with { IpAddress = ip }, ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("RefreshToken")
        .WithSummary("Làm mới Access Token bằng Refresh Token")
        .AllowAnonymous()
        .Produces<ApiResponse<AuthTokensDto>>()
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        // FR-AUTH-005
        group.MapPost("/logout", async (LogoutCommand command, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(command, ct);
            return Results.NoContent();
        })
        .WithName("Logout")
        .WithSummary("Đăng xuất và thu hồi Refresh Token")
        .RequireAuthorization()
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        // FR-AUTH-006
        group.MapGet("/me", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCurrentUserQuery(), ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("GetCurrentUser")
        .WithSummary("Lấy thông tin người dùng đang đăng nhập")
        .RequireAuthorization()
        .Produces<ApiResponse<UserDto>>()
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        // FR-AUTH-007
        group.MapPatch("/me", async (UpdateProfileCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("UpdateProfile")
        .WithSummary("Cập nhật hồ sơ cá nhân")
        .RequireAuthorization()
        .Produces<ApiResponse<UserDto>>()
        .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }
}
