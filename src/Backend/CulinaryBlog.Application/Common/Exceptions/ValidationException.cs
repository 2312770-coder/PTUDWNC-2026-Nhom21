namespace CulinaryBlog.Application.Common.Exceptions;

// HTTP 400 - lỗi validation. Errors chứa lỗi theo từng field để frontend
// hiển thị ngay dưới ô nhập tương ứng (SRS mục 8: "errors":{"field":["msg"]}).
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("Đã xảy ra một hoặc nhiều lỗi validation.")
        => Errors = new Dictionary<string, string[]>();

    public ValidationException(IDictionary<string, string[]> errors) : this()
        => Errors = errors;
}
