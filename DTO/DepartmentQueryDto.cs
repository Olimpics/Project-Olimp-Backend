namespace OlimpBack.DTO;

public class DepartmentQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? FacultyIds { get; set; }
    public string? Search { get; set; }
    public int SortOrder { get; set; } = 0;
}

