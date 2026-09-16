using CulinaryBlog.Application.Common.Interfaces;
using MediatR;

namespace CulinaryBlog.Application.Features.Auth.Commands.Logout;

// FR-AUTH-005 - CHƯA HIỆN THỰC.
//
// Gợi ý: băm raw token, tìm theo TokenHash và đối chiếu UserId với
// _currentUser.UserId (tránh user A thu hồi token của user B), gọi Revoke()
// rồi SaveChangesAsync. Đây là chức năng đơn giản nhất trong FR-AUTH,
// nên làm trước để quen luồng CQRS.
public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ICurrentUser _currentUser;

    public LogoutCommandHandler(IApplicationDbContext context, IJwtService jwtService, ICurrentUser currentUser)
    {
        _context = context;
        _jwtService = jwtService;
        _currentUser = currentUser;
    }

    public Task Handle(LogoutCommand request, CancellationToken ct)
        => throw new NotImplementedException(
            "FR-AUTH-005 (Đăng xuất / thu hồi Refresh Token) chưa được hiện thực.");
}
