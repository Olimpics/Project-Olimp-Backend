namespace OlimpBack.DTO;

public class StudentQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Search { get; set; }

    // Типізовані списки замість string!
    public List<string>? Faculties { get; set; }
    public List<string>? Specialities { get; set; }
    public List<int>? GroupIds { get; set; }
    public List<int>? Courses { get; set; }
    public List<int>? StudyFormIds { get; set; }
    public List<int>? DegreeLevelIds { get; set; }

    public int SortOrder { get; set; } = 0;
}