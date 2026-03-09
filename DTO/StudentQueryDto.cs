namespace OlimpBack.DTO;

public class StudentQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Search { get; set; }
    public string? Faculties { get; set; }
    public string? Speciality { get; set; }
    public string? Group { get; set; }
    public string? Courses { get; set; }
    public string? StudyForm { get; set; }
    public string? DegreeLevelIds { get; set; }
    public int SortOrder { get; set; } = 0;
}

