namespace OlimpBack.Application.DTO;

/// <summary>
/// Generic paginated response wrapper.
/// </summary>
public class PaginatedResponseDto<T>
{
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public List<T> Items { get; set; } = new();
    public object? Filters { get; set; }
}
