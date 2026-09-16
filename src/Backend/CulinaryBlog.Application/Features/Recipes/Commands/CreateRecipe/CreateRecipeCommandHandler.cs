using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

// FR-RCP-003 - CHƯA HIỆN THỰC.
//
// Gợi ý:
//   1. Kiểm tra CategoryId có tồn tại không -> NotFoundException nếu không.
//   2. Tạo entity qua Recipe.Create(...), AuthorId lấy từ _currentUser.UserId
//      (KHÔNG nhận AuthorId từ client, tránh giả mạo tác giả).
//   3. Kiểm tra trùng slug bằng SlugExistsAsync; nếu trùng thì nối thêm hậu tố
//      (vd "-2") cho tới khi không trùng.
//   4. Nếu có Nutrition thì gọi recipe.SetNutrition(...).
//   5. AddAsync + SaveChangesAsync, trả về RecipeDetailDto.
public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, RecipeDetailDto>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrentUser _currentUser;

    public CreateRecipeCommandHandler(IRecipeRepository recipeRepository,
        ICategoryRepository categoryRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _categoryRepository = categoryRepository;
        _currentUser = currentUser;
    }

    public Task<RecipeDetailDto> Handle(CreateRecipeCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-003 (Tạo công thức mới) chưa được hiện thực.");
}
