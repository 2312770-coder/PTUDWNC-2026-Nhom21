using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Recipes.Commands.ManageSteps;

// FR-RCP-010 - CHƯA HIỆN THỰC.
//
// Điểm cần chú ý nhất: StepNumber là composite unique cùng RecipeId (SRS mục 7.3).
// Nghĩa là khi CHÈN một bước vào giữa, hoặc XÓA một bước, phải đánh số lại các
// bước phía sau cho liền mạch 1,2,3... (dùng RecipeStep.Renumber đã viết sẵn),
// nếu không sẽ vi phạm ràng buộc unique hoặc bị nhảy số.
public class AddStepCommandHandler : IRequestHandler<AddStepCommand, RecipeStepDto>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public AddStepCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task<RecipeStepDto> Handle(AddStepCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-010 (Thêm bước thực hiện) chưa được hiện thực.");
}

public class UpdateStepCommandHandler : IRequestHandler<UpdateStepCommand, RecipeStepDto>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateStepCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task<RecipeStepDto> Handle(UpdateStepCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-010 (Cập nhật bước thực hiện) chưa được hiện thực.");
}

public class DeleteStepCommandHandler : IRequestHandler<DeleteStepCommand>
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteStepCommandHandler(IRecipeRepository recipeRepository, ICurrentUser currentUser)
    {
        _recipeRepository = recipeRepository;
        _currentUser = currentUser;
    }

    public Task Handle(DeleteStepCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-RCP-010 (Xóa bước thực hiện) chưa được hiện thực.");
}
