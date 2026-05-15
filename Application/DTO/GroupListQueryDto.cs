namespace OlimpBack.Application.DTO;

public class GroupListQueryDto
{
    public string? Search { get; set; }

    // Типізовані масиви замість рядків із комами
    public List<Guid>? FacultyIds { get; set; }
    public List<Guid>? DepartmentIds { get; set; }
    public List<int>? Courses { get; set; }
    public List<Guid>? DegreeLevelIds { get; set; }

    public int SortOrder { get; set; } = 0;
}

