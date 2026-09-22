using FluentValidation;

namespace CulinaryBlog.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .EmailAddress().WithMessage("Email không hợp lệ.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Tên hiển thị không được để trống.")
            .MaximumLength(100);

        RuleFor(x => x.UserName)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.UserName));
    }
}
