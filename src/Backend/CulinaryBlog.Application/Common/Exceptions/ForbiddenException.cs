namespace CulinaryBlog.Application.Common.Exceptions;

// HTTP 403 - đã đăng nhập nhưng không đủ quyền
// (vd: Author sửa recipe của người khác).
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Bạn không có quyền thực hiện thao tác này.")
        : base(message) { }
}
