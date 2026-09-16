using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Commands.UpdateCategory;

// FR-CAT-004 - CHƯA HIỆN THỰC.
//
// Gợi ý: GetByIdAsync (không thấy -> NotFoundException 404); nếu đổi Name thì
// kiểm tra trùng với danh mục KHÁC (truyền excludeId = request.Id vào
// NameExistsAsync); gọi category.Update(...) rồi SaveChangesAsync.
public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-CAT-004 (Cập nhật danh mục) chưa được hiện thực.");
}
