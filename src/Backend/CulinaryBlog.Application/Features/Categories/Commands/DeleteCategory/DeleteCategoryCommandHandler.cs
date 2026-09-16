using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Commands.DeleteCategory;

// FR-CAT-005 - CHƯA HIỆN THỰC.
//
// Gợi ý:
//   1. GetByIdAsync -> không thấy thì NotFoundException (404).
//   2. Gọi _categoryRepository.HasRecipesAsync(id) - còn công thức thì ném
//      ConflictException (409), KHÔNG xóa (SRS mục 8.2 quy định rõ).
//   3. Dùng SoftDelete(entity) chứ không phải Remove(entity), vì SRS mục 8.2
//      ghi rõ đây là soft delete.
public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public Task Handle(DeleteCategoryCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-CAT-005 (Xóa danh mục) chưa được hiện thực.");
}
