using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;

// FR-CAT-003 - CHƯA HIỆN THỰC, dành cho người phụ trách FR-CAT-003.
//
// Gợi ý:
//   1. Kiểm tra trùng tên qua _categoryRepository.NameExistsAsync
//      -> trùng thì ném ConflictException (409, theo SRS mục 8.2).
//   2. Tạo entity bằng Category.Create(...) - slug tự sinh trong đó,
//      không tự ghép chuỗi ở Handler.
//   3. AddAsync + SaveChangesAsync.
//   4. Map sang CategoryDto trả về (recipeCount = 0 vì danh mục mới tạo).
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
        => _categoryRepository = categoryRepository;

    public Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-CAT-003 (Tạo danh mục mới) chưa được hiện thực.");
}
