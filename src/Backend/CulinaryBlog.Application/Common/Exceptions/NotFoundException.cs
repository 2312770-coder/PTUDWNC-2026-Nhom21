namespace CulinaryBlog.Application.Common.Exceptions;

// GlobalExceptionMiddleware map thành HTTP 404 (Phụ lục A SRS).
public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"{name} với khóa \"{key}\" không tồn tại.") { }

    public NotFoundException(string message) : base(message) { }
}
