using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;
using CulinaryBlog.Application.Features.Categories.Queries.GetCategories;
using CulinaryBlog.Application.Features.Categories.Queries.GetCategoryBySlug;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

public static class CategoriesEndpoints
{
    public static void MapCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/categories").WithTags("Categories");

        // FR-CAT-001: Xem danh sách danh mục (public)
        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetCategoriesQuery(), ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("GetCategories")
        .WithSummary("Xem danh sách danh mục kèm số công thức");

        // FR-CAT-002: Lấy chi tiết danh mục kèm danh sách công thức (FR-CAT-002 - Lê Nhật Tiến)
        group.MapGet("/{slug}", async (string slug, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetCategoryBySlugQuery(slug), ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("GetCategoryBySlug")
        .WithSummary("Lấy chi tiết danh mục kèm danh sách công thức nấu ăn (FR-CAT-002)");

        // FR-CAT-003: Admin tạo danh mục mới (FR-CAT-003 - Nguyễn Viết Toàn)
        group.MapPost("/", async (CreateCategoryCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/v1/categories/{result.Slug}", ApiResponse.Ok(result));
        })
        .WithName("CreateCategory")
        .WithSummary("Admin tạo danh mục mới")
        .RequireAuthorization("AdminOnly");
    }
}