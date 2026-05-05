namespace OlimpBack.Application.DTO;

public class GradeStudentDto
{
    public int IdBind { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string FacultyName { get; set; } = string.Empty;
    public string? Score { get; set; }
}
