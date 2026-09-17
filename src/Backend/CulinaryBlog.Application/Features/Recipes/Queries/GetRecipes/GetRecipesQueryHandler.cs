using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Enums;
using CulinaryBlog.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;

// FR-RCP-001 + FR-SRCH-002/003/004: Danh sách công thức (phân trang, lọc theo category/độ khó/thời gian, sắp xếp).
public class GetRecipesQueryHandler : IRequestHandler<GetRecipesQuery, PagedResult<RecipeListItemDto>>
{
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipesQueryHandler(IRecipeRepository recipeRepository)
        => _recipeRepository = recipeRepository;

    public async Task<PagedResult<RecipeListItemDto>> Handle(GetRecipesQuery request, CancellationToken ct)
    {
        var query = _recipeRepository.Query()
            .AsNoTracking()
            .Where(r => r.Status == RecipeStatus.Published);

        if (request.CategoryId.HasValue)
        {
            query = query.Where(r => r.CategoryId == request.CategoryId.Value);
        }

        if (request.Difficulty.HasValue)
        {
            query = query.Where(r => r.Difficulty == request.Difficulty.Value);
        }

        if (request.MaxCookTime.HasValue)
        {
            query = query.Where(r => r.CookTime <= request.MaxCookTime.Value);
        }

        if (request.MaxTotalTime.HasValue)
        {
            query = query.Where(r => (r.PrepTime + r.CookTime) <= request.MaxTotalTime.Value);
        }

        var isDesc = request.Paging.IsDescending;
        var sortBy = request.Paging.SortBy?.ToLowerInvariant() ?? "createdat";

        query = (sortBy, isDesc) switch
        {
            ("title", false) => query.OrderBy(r => r.Title),
            ("title", true) => query.OrderByDescending(r => r.Title),
            ("cooktime", false) => query.OrderBy(r => r.CookTime),
            ("cooktime", true) => query.OrderByDescending(r => r.CookTime),
            ("preptime", false) => query.OrderBy(r => r.PrepTime),
            ("preptime", true) => query.OrderByDescending(r => r.PrepTime),
            ("publishedat", false) => query.OrderBy(r => r.PublishedAt),
            ("publishedat", true) => query.OrderByDescending(r => r.PublishedAt),
            (_, false) => query.OrderBy(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt),
        };

        var total = await query.CountAsync(ct);
        var page = request.Paging.Page ?? 1;
        var pageSize = request.Paging.PageSize ?? 10;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new RecipeListItemDto(
                r.Id,
                r.Title,
                r.Slug,
                r.Description,
                r.PrepTime,
                r.CookTime,
                r.Servings,
                r.Difficulty.ToString(),
                r.Status.ToString(),
                r.Images.Where(i => i.IsPrimary).Select(i => i.OriginalUrl).FirstOrDefault()
                    ?? r.Images.OrderBy(i => i.OrderIndex).Select(i => i.OriginalUrl).FirstOrDefault(),
                r.Category != null ? r.Category.Name : string.Empty,
                r.Author != null ? r.Author.DisplayName : string.Empty,
                r.PublishedAt
            ))
            .ToListAsync(ct);

        return new PagedResult<RecipeListItemDto>(items, page, pageSize, total);
    }
}

