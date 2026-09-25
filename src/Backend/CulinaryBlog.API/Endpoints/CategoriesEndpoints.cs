using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Categories.Queries.GetCategories;
using CulinaryBlog.Application.Features.Categories.Queries.GetCategoryBySlug;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

public static class CategoriesEndpoints
{
    public static void MapCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/categories").WithTags("Categories");

        // FR-CAT-001: Lấy danh sách danh mục
        group.MapGet("/", async (IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetCategoriesQuery(), ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("GetCategories")
        .WithSummary("Lấy danh sách tất cả danh mục ẩm thực");

        // FR-CAT-002: Lấy chi tiết danh mục kèm danh sách công thức
        group.MapGet("/{slug}", async (string slug, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetCategoryBySlugQuery(slug), ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("GetCategoryBySlug")
        .WithSummary("Lấy chi tiết danh mục kèm danh sách công thức nấu ăn (FR-CAT-002)");
    }
}