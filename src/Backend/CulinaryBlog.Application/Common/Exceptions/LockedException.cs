namespace CulinaryBlog.Application.Common.Exceptions;

// HTTP 423 - SRS mục 3.1 & Phụ lục A:
// Tài khoản bị tạm khóa 15 phút sau khi nhập sai mật khẩu quá 5 lần.
public class LockedException : Exception
{
    public LockedException(string message) : base(message) { }
}
