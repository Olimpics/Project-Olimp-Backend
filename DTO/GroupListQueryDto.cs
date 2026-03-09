namespace OlimpBack.DTO;

public class GroupListQueryDto
{
    public string? FacultyIds { get; set; }
    public string? DepartmentIds { get; set; }
    public string? Courses { get; set; }
    public string? DegreeLevelIds { get; set; }
    public string? Search { get; set; }
    public int SortOrder { get; set; } = 0;
}

