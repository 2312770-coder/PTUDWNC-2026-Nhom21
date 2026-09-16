namespace CulinaryBlog.Application.Common.Models;

// SRS mục 8: ?page=1&pageSize=10&sortBy=createdAt&sortOrder=desc
// Giới hạn pageSize tối đa 100 để tránh client yêu cầu quá nhiều dữ liệu một lần.
public record PagingParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    private int _page = 1;

    public int Page
    {
        get => _page;
        init => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value switch
        {
            < 1 => 10,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }

    public string? SortBy { get; init; }
    public string? SortOrder { get; init; } = "desc";

    public bool IsDescending => !string.Equals(SortOrder, "asc", StringComparison.OrdinalIgnoreCase);
}
