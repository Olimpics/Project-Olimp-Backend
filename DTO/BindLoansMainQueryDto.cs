namespace OlimpBack.DTO;

public class BindLoansMainQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Search { get; set; }
    public string? AddDisciplinesIds { get; set; }
    public string? EducationalProgramIds { get; set; }
    public int SortOrder { get; set; } = 0;
}

