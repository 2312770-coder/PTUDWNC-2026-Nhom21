using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategories;

// FR-CAT-001 - CHƯA HIỆN THỰC, dành cho người phụ trách FR-CAT-001.
//
// Gợi ý:
//   - Query từ _categoryRepository.Query(), sắp xếp theo OrderIndex rồi Name.
//   - recipeCount đếm số recipe đã Published thuộc danh mục đó.
//   - Dùng AsNoTracking (chỉ đọc) và Select thẳng sang CategoryDto để SQL chỉ
//     lấy đúng cột cần, không SELECT *.
//   - KHÔNG cần viết code cache: Query đã implement ICacheable, CachingBehavior
//     lo phần đó.
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-CAT-001 (Xem danh sách danh mục) chưa được hiện thực.");
}
