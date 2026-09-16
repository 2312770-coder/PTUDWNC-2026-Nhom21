using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(200).WithMessage("Tiêu đề tối đa 200 ký tự.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Mô tả không được để trống.")
            .MaximumLength(2000).WithMessage("Mô tả tối đa 2000 ký tự.");

        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Phải chọn danh mục.");

        RuleFor(x => x.PrepTime)
            .GreaterThan(0).WithMessage("Thời gian chuẩn bị phải lớn hơn 0 phút.");

        RuleFor(x => x.CookTime)
            .GreaterThanOrEqualTo(0).WithMessage("Thời gian nấu không được âm.");

        RuleFor(x => x.Servings)
            .GreaterThan(0).WithMessage("Số khẩu phần phải lớn hơn 0.");

        RuleFor(x => x.Difficulty).IsInEnum().WithMessage("Độ khó không hợp lệ.");
    }
}
