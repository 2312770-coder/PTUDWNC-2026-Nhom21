using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategoryBySlug;

// FR-CAT-002 - CHƯA HIỆN THỰC, dành cho người phụ trách FR-CAT-002.
//
// Gợi ý:
//   - Tìm danh mục theo SLUG (không phải Id) vì URL dùng slug cho SEO.
//     Không thấy -> NotFoundException (404).
//   - Lấy recipe thuộc danh mục, CHỈ những recipe Status = Published
//     (Guest không được thấy Draft/Archived của người khác).
//   - Áp dụng phân trang theo request.Paging, trả về PagedResult.
public class GetCategoryBySlugQueryHandler
    : IRequestHandler<GetCategoryBySlugQuery, CategoryWithRecipesDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IRecipeRepository _recipeRepository;

    public GetCategoryBySlugQueryHandler(ICategoryRepository categoryRepository,
        IRecipeRepository recipeRepository)
    {
        _categoryRepository = categoryRepository;
        _recipeRepository = recipeRepository;
    }

    public Task<CategoryWithRecipesDto> Handle(GetCategoryBySlugQuery request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-CAT-002 (Xem chi tiết danh mục + công thức) chưa được hiện thực.");
}
