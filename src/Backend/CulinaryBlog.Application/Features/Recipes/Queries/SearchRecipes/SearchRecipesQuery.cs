using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.DTOs;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.SearchRecipes;

// FR-SRCH-001: Tìm kiếm toàn văn bản. SRS mục 8.3: GET /recipes/search?q=...
public record SearchRecipesQuery(string Keyword, PagingParams Paging)
    : IRequest<PagedResult<RecipeListItemDto>>;
