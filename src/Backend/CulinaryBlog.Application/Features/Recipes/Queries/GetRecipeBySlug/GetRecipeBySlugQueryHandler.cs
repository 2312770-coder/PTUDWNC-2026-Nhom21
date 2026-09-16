using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipeBySlug;

// FR-RCP-002 - CHƯA HIỆN THỰC.
//
// Gợi ý:
//   - Dùng _recipeRepository.GetDetailBySlugAsync (đã có sẵn Include đầy đủ
//     và AsSplitQuery để tránh Cartesian Explosion).
//   - Không thấy -> NotFoundException (404).
//   - Nếu Status != Published: chỉ cho xem khi _currentUser.UserId == AuthorId
//     hoặc _currentUser.IsAdmin, ngược lại ném NotFoundException (không ném 403,
//     để không lộ rằng slug đó có tồn tại).
//   - Steps sắp xếp theo StepNumber, Ingredients theo OrderIndex,
//     Images theo OrderIndex.
public class GetRecipeBySlugQueryHandler : IRequestHandler<GetRecipeBySlugQuery, RecipeDetailDto>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public GetRecipeBySlugQueryHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task<RecipeDetailDto> Handle(GetRecipeBySlugQuery request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-002 (Xem chi tiết công thức) chưa được hiện thực.");
}
