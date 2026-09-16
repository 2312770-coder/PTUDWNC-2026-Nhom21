using FluentValidation;

namespace CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên danh mục không được để trống.")
            .MaximumLength(100).WithMessage("Tên danh mục tối đa 100 ký tự.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("URL ảnh tối đa 500 ký tự.")
            .When(x => x.ImageUrl is not null);
    }
}
