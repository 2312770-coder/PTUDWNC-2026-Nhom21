// SRS mục 8.3 đến 8.6 - Recipes Module (/api/v1/recipes)

using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

public static class RecipesEndpoints
{
    public static IEndpointRouteBuilder MapRecipesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/recipes").WithTags("Recipes");

        // FR-RCP-003: Tạo công thức mới (Đã hoàn thành bởi Lê Nhật Tiến)
        group.MapPost("/", async (CreateRecipeCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/v1/recipes/{result.Slug}", ApiResponse.Ok(result));
        })
        .WithName("CreateRecipe")
        .WithSummary("Tạo công thức mới (trạng thái Draft)")
        .RequireAuthorization("AuthorOrAdmin");

        return app;
    }
}
