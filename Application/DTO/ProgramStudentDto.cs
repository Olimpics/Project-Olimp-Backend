using System;

namespace OlimpBack.Application.DTO;

public class ProgramStudentDto
{
    public int IdStudent { get; set; }
    public string NameStudent { get; set; } = null!;
    public string GroupName { get; set; } = null!;
    public bool IsShort { get; set; }
    public string Status { get; set; } = null!;
    public DateOnly EducationStart { get; set; }
}

public class ProgramStudentQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Search { get; set; }
    public string? SortBy { get; set; } // Group, IsShort, Status, EducationStart, Name
    public bool IsDescending { get; set; } = false;
}
