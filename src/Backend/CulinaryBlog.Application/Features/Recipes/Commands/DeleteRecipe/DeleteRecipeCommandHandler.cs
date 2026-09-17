using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.DeleteRecipe;

// FR-RCP-007 - CHƯA HIỆN THỰC.
//
// QUYẾT ĐỊNH CHÍNH THỨC: Áp dụng cơ chế SOFT DELETE (theo mục 7.1, 8.3 và docs/DECISIONS.md mục D1).
//
// Gợi ý các bước:
//   1. Lấy recipe theo ID (GetByIdAsync), kiểm tra tồn tại -> NotFoundException (404).
//   2. Kiểm tra quyền sở hữu (AuthorId == currentUserId hoặc user có role Admin) -> ForbiddenException (403).
//   3. Gọi _recipeRepository.SoftDelete(recipe) (đánh dấu IsDeleted = true, UpdatedAt = UtcNow).
//      Global Query Filter trong DbContext sẽ tự động ẩn recipe này khỏi mọi truy vấn đọc thông thường.
//   4. Lưu ý: KHÔNG xóa ảnh trên MinIO ngay khi soft delete để bảo toàn dữ liệu phòng trường hợp khôi phục.
//   5. SaveChangesAsync(ct) và invalidate cache tag "recipes" / slug tương ứng.
public class DeleteRecipeCommandHandler : IRequestHandler<DeleteRecipeCommand>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteRecipeCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task Handle(DeleteRecipeCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-007 (Xóa công thức) chưa được hiện thực.");
}
