using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Queries.GetCategories;

// FR-CAT-001 - Xem danh sách danh mục (kèm số công thức Published và sắp xếp theo OrderIndex).
// TODO: Người phụ trách FR-CAT-001 hiện thực handler này.
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
        => throw new NotImplementedException("FR-CAT-001 (Xem danh sách danh mục) chưa được hiện thực.");
}
