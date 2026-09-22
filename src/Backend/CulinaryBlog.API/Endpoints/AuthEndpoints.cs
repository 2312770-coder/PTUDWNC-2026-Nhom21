using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Auth.Commands.Register;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/register", async (
            RegisterRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var command = new RegisterCommand(
                Email: request.Email,
                Password: request.Password,
                DisplayName: request.DisplayName,
                UserName: request.UserName);

            var result = await mediator.Send(command, cancellationToken);
            return Results.Ok(ApiResponse.Ok(new
            {
                result.AccessToken,
                result.RefreshToken,
                result.User
            }));
        })
        .WithName("RegisterUser")
        .WithSummary("Đăng ký tài khoản mới (FR-AUTH-001)");

        return app;
    }

    public sealed record RegisterRequest(
        string Email,
        string Password,
        string DisplayName,
        string? UserName);
}
