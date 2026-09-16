using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ArchiveRecipe;

// FR-RCP-006 - CHƯA HIỆN THỰC.
//
// Gợi ý: lấy recipe, kiểm tra quyền sở hữu, gọi recipe.Archive() rồi
// SaveChangesAsync. Đơn giản nhất trong nhóm FR-RCP, nên làm trước để quen.
public class ArchiveRecipeCommandHandler : IRequestHandler<ArchiveRecipeCommand>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public ArchiveRecipeCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task Handle(ArchiveRecipeCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-006 (Lưu trữ công thức) chưa được hiện thực.");
}
