using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.GetRecipes;

// FR-RCP-001 + FR-SRCH-002/003/004 - CHƯA HIỆN THỰC.
//
// Gợi ý:
//   - Bắt đầu từ _recipeRepository.Query().AsNoTracking().
//   - CHỈ trả recipe Status = Published (Guest không thấy Draft/Archived).
//   - Lọc theo CategoryId / Difficulty / MaxTotalTime nếu client có gửi lên
//     (chỉ thêm .Where khi giá trị khác null).
//   - Sắp xếp theo Paging.SortBy (createdAt, publishedAt, title, cookTime...)
//     và Paging.IsDescending. Nhớ có nhánh default cho giá trị lạ.
//   - Đếm Total TRƯỚC khi Skip/Take.
//   - Select sang RecipeListItemDto (lấy ảnh có IsPrimary = true làm
//     PrimaryImageUrl) để SQL chỉ lấy đúng cột cần.
public class GetRecipesQueryHandler : IRequestHandler<GetRecipesQuery, PagedResult<RecipeListItemDto>>
{
    private readonly IRecipeRepository _recipeRepository;

    public GetRecipesQueryHandler(IRecipeRepository recipeRepository)
        => _recipeRepository = recipeRepository;

    public Task<PagedResult<RecipeListItemDto>> Handle(GetRecipesQuery request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-001 / FR-SRCH-002,003,004 (Danh sách + lọc + sắp xếp + phân trang) chưa được hiện thực.");
}
