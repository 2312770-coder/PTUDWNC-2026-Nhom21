namespace CulinaryBlog.Application.Common.Models;

// SRS mục 8: ?page=1&pageSize=10&sortBy=createdAt&sortOrder=desc
// Giới hạn pageSize tối đa 100 để tránh client yêu cầu quá nhiều dữ liệu một lần.
public record PagingParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    private int _page = 1;

    public int? Page
    {
        get => _page;
        init => _page = (!value.HasValue || value.Value < 1) ? 1 : value.Value;
    }

    public int? PageSize
    {
        get => _pageSize;
        init => _pageSize = value switch
        {
            null => 10,
            < 1 => 10,
            > MaxPageSize => MaxPageSize,
            _ => value.Value
        };
    }


    private string? _sortBy;
    private string? _sortOrder = "desc";
    private string? _sort;

    // Hỗ trợ cú pháp gộp: ?sort=-createdAt (dấu - là desc, không dấu là asc)
    public string? Sort
    {
        get => _sort;
        init
        {
            _sort = value;
            if (!string.IsNullOrWhiteSpace(value) && string.IsNullOrWhiteSpace(_sortBy))
            {
                if (value.StartsWith('-'))
                {
                    _sortBy = value[1..];
                    _sortOrder = "desc";
                }
                else
                {
                    _sortBy = value;
                    _sortOrder = "asc";
                }
            }
        }
    }

    // Chuẩn chính thức: ?sortBy=createdAt&sortOrder=desc
    public string? SortBy
    {
        get => _sortBy;
        init => _sortBy = value;
    }

    public string? SortOrder
    {
        get => _sortOrder;
        init => _sortOrder = value;
    }

    public bool IsDescending => !string.Equals(SortOrder, "asc", StringComparison.OrdinalIgnoreCase);
}
