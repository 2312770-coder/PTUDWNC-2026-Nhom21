using FluentValidation;

namespace CulinaryBlog.Application.Features.Recipes.Commands.CreateRecipe;

/// <summary>
/// Validator cho CreateRecipeCommand sử dụng FluentValidation (CONS-008).
/// Đảm bảo tính hợp lệ của dữ liệu trước khi chuyển tiếp vào Command Handler trong MediatR Pipeline.
/// </summary>
public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        // Kiểm tra thông tin cơ bản của công thức
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(200).WithMessage("Tiêu đề tối đa 200 ký tự.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Mô tả không được để trống.")
            .MaximumLength(2000).WithMessage("Mô tả tối đa 2000 ký tự.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Phải chọn danh mục cho công thức.");

        RuleFor(x => x.PrepTime)
            .GreaterThan(0).WithMessage("Thời gian chuẩn bị phải lớn hơn 0 phút.");

        RuleFor(x => x.CookTime)
            .GreaterThanOrEqualTo(0).WithMessage("Thời gian nấu không được âm (nhập 0 nếu là món không cần nấu).");

        RuleFor(x => x.Servings)
            .GreaterThan(0).WithMessage("Số khẩu phần phải lớn hơn 0.");

        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("Độ khó không hợp lệ (1: Dễ, 2: Trung bình, 3: Khó, 4: Chuyên nghiệp).");

        // Validate danh sách nguyên liệu nếu client có gửi kèm
        When(x => x.Ingredients != null, () =>
        {
            RuleForEach(x => x.Ingredients).ChildRules(ing =>
            {
                ing.RuleFor(i => i.Name)
                    .NotEmpty().WithMessage("Tên nguyên liệu không được để trống.")
                    .MaximumLength(200).WithMessage("Tên nguyên liệu tối đa 200 ký tự.");

                ing.RuleFor(i => i.Quantity)
                    .GreaterThan(0).When(i => i.Quantity.HasValue)
                    .WithMessage("Định lượng nguyên liệu phải lớn hơn 0 nếu có khai báo.");

                ing.RuleFor(i => i.Unit)
                    .MaximumLength(50).WithMessage("Đơn vị đo tối đa 50 ký tự.");

                ing.RuleFor(i => i.Notes)
                    .MaximumLength(500).WithMessage("Ghi chú nguyên liệu tối đa 500 ký tự.");
            });
        });

        // Validate danh sách các bước thực hiện nếu client có gửi kèm
        When(x => x.Steps != null, () =>
        {
            RuleForEach(x => x.Steps).ChildRules(step =>
            {
                step.RuleFor(s => s.Title)
                    .NotEmpty().WithMessage("Tên bước thực hiện không được để trống.")
                    .MaximumLength(200).WithMessage("Tên bước tối đa 200 ký tự.");

                step.RuleFor(s => s.Description)
                    .NotEmpty().WithMessage("Nội dung hướng dẫn chi tiết của bước không được để trống.");

                step.RuleFor(s => s.TimerMinutes)
                    .GreaterThan(0).When(s => s.TimerMinutes.HasValue)
                    .WithMessage("Thời gian hẹn giờ của bước phải lớn hơn 0 phút nếu có khai báo.");

                step.RuleFor(s => s.StepNumber)
                    .GreaterThan(0).When(s => s.StepNumber.HasValue)
                    .WithMessage("Thứ tự bước phải lớn hơn 0.");
            });
        });
    }
}

