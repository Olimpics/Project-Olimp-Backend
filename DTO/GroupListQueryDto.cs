namespace OlimpBack.DTO;

public class GroupListQueryDto
{
    public string? Search { get; set; }

    // Типізовані масиви замість рядків із комами
    public List<int>? FacultyIds { get; set; }
    public List<int>? DepartmentIds { get; set; }
    public List<int>? Courses { get; set; }
    public List<int>? DegreeLevelIds { get; set; }

    public int SortOrder { get; set; } = 0;
}

