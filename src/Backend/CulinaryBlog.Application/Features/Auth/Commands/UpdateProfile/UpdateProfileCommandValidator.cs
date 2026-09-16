using FluentValidation;

namespace CulinaryBlog.Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        // Chỉ validate khi field được gửi lên (partial update).
        RuleFor(x => x.DisplayName)
            .MaximumLength(100).WithMessage("Tên hiển thị tối đa 100 ký tự.")
            .When(x => x.DisplayName is not null);

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(500).WithMessage("URL avatar tối đa 500 ký tự.")
            .When(x => x.AvatarUrl is not null);
    }
}
