using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;

// FR-RCP-002: Xem chi tiết công thức kèm đầy đủ các bước, nguyên liệu, ảnh và thông tin dinh dưỡng.
// TODO: Người phụ trách FR-RCP-002 hiện thực handler này.
public class GetRecipeBySlugQueryHandler : IRequestHandler<GetRecipeBySlugQuery, RecipeDetailDto>
{
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipeBySlugQueryHandler(IRecipeRepository recipeRepository)
        => _recipeRepository = recipeRepository;

    public Task<RecipeDetailDto> Handle(GetRecipeBySlugQuery request, CancellationToken ct)
        => throw new NotImplementedException("FR-RCP-002 (Xem chi tiết công thức) chưa được hiện thực.");
}
