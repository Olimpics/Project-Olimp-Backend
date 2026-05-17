using System;

namespace OlimpBack.Application.DTO;

public class GradeStudentDto
{
    public Guid IdBind { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? SecondName { get; set; }
    public string? ThirdName { get; set; }
    public string FullName => $"{SecondName} {FirstName} {ThirdName}".Trim();
    public string GroupName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string FacultyName { get; set; } = string.Empty;
    public int? Score { get; set; }
}
