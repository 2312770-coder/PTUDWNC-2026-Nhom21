using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;

// FR-RCP-001 + FR-SRCH-002/003/004: Danh sách công thức (phân trang, lọc theo category/độ khó/thời gian, sắp xếp).
// TODO: Người phụ trách FR-RCP-001 hiện thực handler này.
public class GetRecipesQueryHandler : IRequestHandler<GetRecipesQuery, PagedResult<RecipeListItemDto>>
{
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipesQueryHandler(IRecipeRepository recipeRepository)
        => _recipeRepository = recipeRepository;

    public Task<PagedResult<RecipeListItemDto>> Handle(GetRecipesQuery request, CancellationToken ct)
        => throw new NotImplementedException("FR-RCP-001 (Danh sách công thức) chưa được hiện thực.");
}
