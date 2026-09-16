// SRS mục 8.3 đến 8.6 - Recipes Module (/api/v1/recipes) gồm cả các endpoint
// con cho ảnh, bước thực hiện và nguyên liệu.

using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Application.Features.Recipes.Commands.ArchiveRecipe;
using CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;
using CulinaryBlog.Application.Features.Recipes.Commands.DeleteRecipe;
using CulinaryBlog.Application.Features.Recipes.Commands.ManageImages;
using CulinaryBlog.Application.Features.Recipes.Commands.ManageIngredients;
using CulinaryBlog.Application.Features.Recipes.Commands.ManageSteps;
using CulinaryBlog.Application.Features.Recipes.Commands.PublishRecipe;
using CulinaryBlog.Application.Features.Recipes.Commands.UpdateRecipe;
using CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;
using CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;
using CulinaryBlog.Application.Features.Recipes.Queries.SearchRecipes;
using CulinaryBlog.Domain.Enums;
using MediatR;

namespace CulinaryBlog.API.Endpoints;

public static class RecipesEndpoints
{
    public static IEndpointRouteBuilder MapRecipesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/recipes").WithTags("Recipes");

        // FR-RCP-001 + FR-SRCH-002/003/004
        group.MapGet("/", async (
            [AsParameters] PagingParams paging,
            Guid? categoryId,
            RecipeDifficulty? difficulty,
            int? maxTotalTime,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetRecipesQuery(paging, categoryId, difficulty, maxTotalTime), ct);
            return Results.Ok(ApiResponse.FromPaged(result));
        })
        .WithName("GetRecipes")
        .WithSummary("Danh sách công thức đã xuất bản (phân trang, lọc, sắp xếp)")
        .AllowAnonymous();

        // FR-SRCH-001 - phải khai báo TRƯỚC /{slug}, nếu không "search" sẽ bị
        // hiểu nhầm thành một slug.
        group.MapGet("/search", async (
            string q,
            [AsParameters] PagingParams paging,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new SearchRecipesQuery(q, paging), ct);
            return Results.Ok(ApiResponse.FromPaged(result));
        })
        .WithName("SearchRecipes")
        .WithSummary("Tìm kiếm công thức bằng Full-Text Search")
        .AllowAnonymous();

        // FR-RCP-002
        group.MapGet("/{slug}", async (string slug, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetRecipeBySlugQuery(slug), ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("GetRecipeBySlug")
        .WithSummary("Chi tiết công thức kèm nguyên liệu, các bước và ảnh")
        .AllowAnonymous()
        .Produces<ApiResponse<RecipeDetailDto>>()
        .ProducesProblem(StatusCodes.Status404NotFound);

        // FR-RCP-003
        group.MapPost("/", async (CreateRecipeCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/v1/recipes/{result.Slug}", ApiResponse.Ok(result));
        })
        .WithName("CreateRecipe")
        .WithSummary("Tạo công thức mới (trạng thái Draft)")
        .RequireAuthorization("AuthorOrAdmin");

        // FR-RCP-004
        group.MapPut("/{id:guid}", async (
            Guid id, UpdateRecipeCommand body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(body with { Id = id }, ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("UpdateRecipe")
        .WithSummary("Cập nhật thông tin cơ bản của công thức")
        .RequireAuthorization("AuthorOrAdmin");

        // FR-RCP-005
        group.MapPatch("/{id:guid}/publish", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new PublishRecipeCommand(id, Publish: true), ct);
            return Results.NoContent();
        })
        .WithName("PublishRecipe")
        .WithSummary("Xuất bản công thức (Draft sang Published)")
        .RequireAuthorization("AuthorOrAdmin");

        group.MapPatch("/{id:guid}/unpublish", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new PublishRecipeCommand(id, Publish: false), ct);
            return Results.NoContent();
        })
        .WithName("UnpublishRecipe")
        .WithSummary("Hủy xuất bản công thức (Published về Draft)")
        .RequireAuthorization("AuthorOrAdmin");

        // FR-RCP-006
        group.MapPatch("/{id:guid}/archive", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new ArchiveRecipeCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("ArchiveRecipe")
        .WithSummary("Lưu trữ công thức")
        .RequireAuthorization("AuthorOrAdmin");

        // FR-RCP-007
        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteRecipeCommand(id), ct);
            return Results.NoContent();
        })
        .WithName("DeleteRecipe")
        .WithSummary("Xóa công thức")
        .RequireAuthorization("AuthorOrAdmin");

        MapImageEndpoints(group);
        MapStepEndpoints(group);
        MapIngredientEndpoints(group);

        return app;
    }

    // SRS mục 8.4 - FR-RCP-008
    private static void MapImageEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/images", async (
            Guid id, IFormFile file, string? altText, bool isPrimary,
            ISender sender, CancellationToken ct) =>
        {
            await using var stream = file.OpenReadStream();
            var command = new UploadRecipeImageCommand(
                id, stream, file.FileName, file.ContentType, file.Length, altText, isPrimary);
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/v1/recipes/{id}/images/{result.Id}", ApiResponse.Ok(result));
        })
        .WithName("UploadRecipeImage")
        .WithSummary("Upload ảnh cho công thức (tối đa 5MB)")
        .RequireAuthorization("AuthorOrAdmin")
        .DisableAntiforgery(); // dùng Bearer JWT nên không cần antiforgery token

        group.MapPatch("/{id:guid}/images/{imageId:guid}", async (
            Guid id, Guid imageId, UpdateRecipeImageCommand body,
            ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(body with { RecipeId = id, ImageId = imageId }, ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("UpdateRecipeImage")
        .WithSummary("Cập nhật metadata ảnh (altText, isPrimary, orderIndex)")
        .RequireAuthorization("AuthorOrAdmin");

        group.MapDelete("/{id:guid}/images/{imageId:guid}", async (
            Guid id, Guid imageId, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteRecipeImageCommand(id, imageId), ct);
            return Results.NoContent();
        })
        .WithName("DeleteRecipeImage")
        .WithSummary("Xóa ảnh của công thức")
        .RequireAuthorization("AuthorOrAdmin");
    }

    // SRS mục 8.5 - FR-RCP-010
    private static void MapStepEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/steps", async (
            Guid id, AddStepCommand body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(body with { RecipeId = id }, ct);
            return Results.Created($"/api/v1/recipes/{id}/steps/{result.Id}", ApiResponse.Ok(result));
        })
        .WithName("AddRecipeStep")
        .WithSummary("Thêm bước thực hiện mới")
        .RequireAuthorization("AuthorOrAdmin");

        group.MapPut("/{id:guid}/steps/{stepId:guid}", async (
            Guid id, Guid stepId, UpdateStepCommand body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(body with { RecipeId = id, StepId = stepId }, ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("UpdateRecipeStep")
        .WithSummary("Cập nhật một bước thực hiện")
        .RequireAuthorization("AuthorOrAdmin");

        group.MapDelete("/{id:guid}/steps/{stepId:guid}", async (
            Guid id, Guid stepId, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteStepCommand(id, stepId), ct);
            return Results.NoContent();
        })
        .WithName("DeleteRecipeStep")
        .WithSummary("Xóa một bước thực hiện")
        .RequireAuthorization("AuthorOrAdmin");
    }

    // SRS mục 8.6 - FR-RCP-009
    private static void MapIngredientEndpoints(RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/ingredients", async (
            Guid id, AddIngredientCommand body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(body with { RecipeId = id }, ct);
            return Results.Created($"/api/v1/recipes/{id}/ingredients/{result.Id}", ApiResponse.Ok(result));
        })
        .WithName("AddRecipeIngredient")
        .WithSummary("Thêm nguyên liệu")
        .RequireAuthorization("AuthorOrAdmin");

        group.MapPut("/{id:guid}/ingredients/{ingId:guid}", async (
            Guid id, Guid ingId, UpdateIngredientCommand body, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(body with { RecipeId = id, IngredientId = ingId }, ct);
            return Results.Ok(ApiResponse.Ok(result));
        })
        .WithName("UpdateRecipeIngredient")
        .WithSummary("Cập nhật nguyên liệu")
        .RequireAuthorization("AuthorOrAdmin");

        group.MapDelete("/{id:guid}/ingredients/{ingId:guid}", async (
            Guid id, Guid ingId, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteIngredientCommand(id, ingId), ct);
            return Results.NoContent();
        })
        .WithName("DeleteRecipeIngredient")
        .WithSummary("Xóa nguyên liệu")
        .RequireAuthorization("AuthorOrAdmin");
    }
}
