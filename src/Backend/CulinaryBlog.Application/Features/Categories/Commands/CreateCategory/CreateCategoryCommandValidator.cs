using FluentValidation;

namespace CulinaryBlog.Application.Features.Categories.Commands.CreateCategory;

// FR-CAT-003: Validator cho CreateCategoryCommand (SRS mục 7.6 và 8.2)
public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên danh mục không được để trống.")
            .MaximumLength(100).WithMessage("Tên danh mục không được vượt quá 100 ký tự.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500).WithMessage("Đường dẫn ảnh không được vượt quá 500 ký tự.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Thứ tự hiển thị phải lớn hơn hoặc bằng 0.");
    }
}
