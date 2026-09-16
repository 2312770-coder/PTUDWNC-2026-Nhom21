namespace CulinaryBlog.Domain.Common;

// Lỗi vi phạm business rule trong Domain (vd: publish recipe chưa có bước nào).
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
