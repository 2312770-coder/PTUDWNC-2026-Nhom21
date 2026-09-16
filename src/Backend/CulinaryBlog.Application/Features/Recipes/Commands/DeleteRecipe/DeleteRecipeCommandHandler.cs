using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.DeleteRecipe;

// FR-RCP-007 - CHƯA HIỆN THỰC.
//
// LƯU Ý QUAN TRỌNG: SRS mâu thuẫn ở chỗ này - FR-RCP-007 nói hard delete,
// còn mục 7.1 và 8.3 nói soft delete. Nhóm đã chốt cách xử lý trong
// docs/DECISIONS.md (mục D1) - đọc trước khi code, đừng tự quyết một mình.
//
// Gợi ý:
//   1. Lấy recipe, kiểm tra quyền sở hữu (chỉ chủ hoặc Admin).
//   2. Xóa theo phương án đã chốt ở D1 (Remove hoặc SoftDelete).
//      Steps/Ingredients/Images có cascade delete nên tự xóa theo.
//   3. Ảnh trên MinIO xóa bất đồng bộ qua Hangfire fire-and-forget (FR-FILE-002),
//      KHÔNG xóa đồng bộ trong handler này để tránh làm chậm HTTP response.
//      Phối hợp với người làm FR-FILE/FR-JOB ở bước này.
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
