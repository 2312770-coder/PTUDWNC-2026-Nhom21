using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Queries.SearchRecipes;

// FR-SRCH-001 - CHƯA HIỆN THỰC.
//
// Gợi ý:
//   - Phần truy vấn Full-Text Search hiện thực trong RecipeRepository.SearchAsync
//     (Infrastructure), Handler chỉ gọi và map sang DTO.
//   - SRS yêu cầu dùng PostgreSQL tsvector/tsquery + extension unaccent để
//     gõ không dấu vẫn tìm được có dấu ("pho bo" -> "Phở bò").
//   - Cần thêm cột SearchVector + GIN index vào RecipeConfiguration trước
//     (xem TODO trong file đó), rồi tạo migration mới.
//   - Sắp xếp kết quả theo độ liên quan (ts_rank), không phải theo ngày tạo.
public class SearchRecipesQueryHandler : IRequestHandler<SearchRecipesQuery, PagedResult<RecipeListItemDto>>
{
    private readonly IRecipeRepository _recipeRepository;

    public SearchRecipesQueryHandler(IRecipeRepository recipeRepository)
        => _recipeRepository = recipeRepository;

    public Task<PagedResult<RecipeListItemDto>> Handle(SearchRecipesQuery request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-SRCH-001 (Tìm kiếm toàn văn bản) chưa được hiện thực.");
}
