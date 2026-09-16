namespace CulinaryBlog.API.Middleware;

// FR-OBS-002 - gắn một mã định danh duy nhất cho mỗi request.
// Client có thể gửi sẵn header X-Correlation-ID; nếu không có thì tự sinh.
// Mã này được đưa vào log scope và trả lại trong response header, nên khi có
// lỗi, chỉ cần lấy mã đó là tra được toàn bộ log của request.
public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await _next(context);
        }
    }
}
