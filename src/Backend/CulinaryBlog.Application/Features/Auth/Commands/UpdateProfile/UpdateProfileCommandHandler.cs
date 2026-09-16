using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.DTOs;
using CulinaryBlog.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CulinaryBlog.Application.Features.Auth.Commands.UpdateProfile;

// FR-AUTH-007 - CHƯA HIỆN THỰC.
//
// Gợi ý: lấy user hiện tại theo _currentUser.UserId, gọi business method
// user.UpdateProfile(...) (đã viết sẵn trong ApplicationUser) rồi
// _userManager.UpdateAsync(user). Không gán trực tiếp property từ Handler.
public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDto>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUser _currentUser;

    public UpdateProfileCommandHandler(UserManager<ApplicationUser> userManager, ICurrentUser currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public Task<UserDto> Handle(UpdateProfileCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-AUTH-007 (Cập nhật hồ sơ cá nhân) chưa được hiện thực.");
}
