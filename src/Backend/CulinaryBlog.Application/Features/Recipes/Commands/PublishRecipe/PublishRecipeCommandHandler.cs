using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.PublishRecipe;

// FR-RCP-005 - CHƯA HIỆN THỰC.
//
// Gợi ý:
//   1. Lấy recipe KÈM Steps và Ingredients (vì Recipe.Publish() cần đếm 2
//      collection này để kiểm tra điều kiện) - dùng GetDetailByIdAsync.
//   2. Kiểm tra quyền sở hữu (chỉ chủ recipe hoặc Admin).
//   3. Gọi recipe.Publish() hoặc recipe.Unpublish() tùy request.Publish.
//      Business rule "phải có ít nhất 1 bước và 1 nguyên liệu" đã nằm trong
//      Recipe.Publish(), nếu vi phạm sẽ ném DomainException -> trả 422.
//   4. SaveChangesAsync.
public class PublishRecipeCommandHandler : IRequestHandler<PublishRecipeCommand>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public PublishRecipeCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task Handle(PublishRecipeCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-005 (Xuất bản / hủy xuất bản công thức) chưa được hiện thực.");
}
