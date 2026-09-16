namespace CulinaryBlog.Application.Common.Exceptions;

// HTTP 401 - chưa đăng nhập, token sai hoặc hết hạn.
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
