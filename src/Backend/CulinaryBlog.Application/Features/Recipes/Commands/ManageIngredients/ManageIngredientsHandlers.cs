using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ManageIngredients;

// FR-RCP-009 - CHƯA HIỆN THỰC.
//
// Gợi ý chung cho cả 3 handler: luôn kiểm tra recipe tồn tại + quyền sở hữu
// trước, rồi mới thao tác. Nếu client không gửi OrderIndex khi thêm mới thì
// tự gán = số nguyên liệu hiện có (thêm vào cuối danh sách).
public class AddIngredientCommandHandler : IRequestHandler<AddIngredientCommand, RecipeIngredientDto>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public AddIngredientCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task<RecipeIngredientDto> Handle(AddIngredientCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-009 (Thêm nguyên liệu) chưa được hiện thực.");
}

public class UpdateIngredientCommandHandler : IRequestHandler<UpdateIngredientCommand, RecipeIngredientDto>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateIngredientCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task<RecipeIngredientDto> Handle(UpdateIngredientCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-009 (Cập nhật nguyên liệu) chưa được hiện thực.");
}

public class DeleteIngredientCommandHandler : IRequestHandler<DeleteIngredientCommand>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteIngredientCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task Handle(DeleteIngredientCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-009 (Xóa nguyên liệu) chưa được hiện thực.");
}
