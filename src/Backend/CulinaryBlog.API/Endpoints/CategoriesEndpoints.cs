// SRS mục 8.2 - Categories Module (/api/v1/categories).

using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;
using CulinaryBlog.Application.Features.Categories.Commands.DeleteCategory;
using CulinaryBlog.Application.Features.Categories.Commands.UpdateCategory;
using CulinaryBlog.Application.Features.Categories.Queries.GetCategories;
using CulinaryBlog.Application.Features.Categories.Queries.GetCategoryBySlug;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

public static class CategoriesEndpoints
{
    public static IEndpointRouteBuilder MapCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/categories").WithTags("Categories");

        // FR-CAT-001
        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCategoriesQuery(), ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("GetCategories")
        .WithSummary("Lấy danh sách tất cả danh mục")
        .AllowAnonymous()
        .Produces<ApiResponse<IReadOnlyList<CategoryDto>>>();

        // FR-CAT-002
        group.MapGet("/{slug}", async (
            string slug,
            [AsParameters] PagingParams paging,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCategoryBySlugQuery(slug, paging), ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("GetCategoryBySlug")
        .WithSummary("Lấy chi tiết danh mục kèm danh sách công thức")
        .AllowAnonymous()
        .Produces<ApiResponse<CategoryWithRecipesDto>>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        // FR-CAT-003
        group.MapPost("/", async (CreateCategoryCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/v1/categories/{result.Slug}", ApiResponse.Ok(result));
        })
        .WithName("CreateCategory")
        .WithSummary("Tạo danh mục mới (chỉ Admin)")
        .RequireAuthorization("AdminOnly")
        .Produces<ApiResponse<CategoryDto>>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status403Forbidden)
        .ProducesProblem(StatusCodes.Status409Conflict);

        // FR-CAT-004
        group.MapPut("/{id:guid}", async (
            Guid id, UpdateCategoryCommand body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(body with { Id = id }, ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("UpdateCategory")
        .WithSummary("Cập nhật danh mục (chỉ Admin)")
        .RequireAuthorization("AdminOnly")
        .Produces<ApiResponse<CategoryDto>>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        // FR-CAT-005
        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteCategoryCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("DeleteCategory")
        .WithSummary("Xóa danh mục (chỉ Admin)")
        .RequireAuthorization("AdminOnly")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict);

        return app;
    }
}
