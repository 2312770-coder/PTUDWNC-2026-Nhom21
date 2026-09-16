using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.DeleteRecipe;

// FR-RCP-007: Xóa công thức [Author-Owner/Admin]. SRS mục 8.3: DELETE /recipes/{id}
// Xem docs/DECISIONS.md D1 về việc chọn hard delete hay soft delete cho recipe.
public record DeleteRecipeCommand(Guid Id) : IRequest;
