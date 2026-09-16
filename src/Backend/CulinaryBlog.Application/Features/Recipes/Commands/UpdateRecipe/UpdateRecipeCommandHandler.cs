using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.UpdateRecipe;

// FR-RCP-004 - CHƯA HIỆN THỰC.
//
// Gợi ý:
//   1. Lấy recipe theo Id, không thấy -> NotFoundException.
//   2. Kiểm tra quyền: chỉ tác giả (AuthorId == _currentUser.UserId) hoặc Admin
//      mới được sửa, ngược lại ném ForbiddenException (403).
//      Endpoint cũng đã kiểm tra bằng RecipeAuthorizationHandler, nhưng kiểm
//      tra ở đây nữa để chắc chắn (defense in depth).
//   3. Gọi recipe.UpdateDetails(...) - business method đã lo việc slug không
//      đổi sau khi publish.
//   4. SaveChangesAsync. Nếu có người khác sửa đồng thời, EF sẽ ném
//      DbUpdateConcurrencyException nhờ RowVersion -> bắt và trả 409 Conflict.
public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand, RecipeDetailDto>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateRecipeCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task<RecipeDetailDto> Handle(UpdateRecipeCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-004 (Cập nhật công thức) chưa được hiện thực.");
}
