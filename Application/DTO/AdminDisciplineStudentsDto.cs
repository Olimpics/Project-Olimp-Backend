namespace OlimpBack.Application.DTO;

public class AdminStudentByAddDisciplineDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int Year { get; set; }
    public string EducationLevel { get; set; } = string.Empty;
    public sbyte IsShort { get; set; }
    public DateTime ChoiceDate { get; set; }
}

public class AdminStudentByMainDisciplineDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int FacultyId { get; set; }
    public string FacultyName { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public int Course { get; set; }
}

public class GetStudentsByAddDisciplineQueryDto
{
    public int DisciplineId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int? FacultyId { get; set; }
    public int? GroupId { get; set; }
    public string? Search { get; set; }

    /// <summary>
    /// 0 - student name asc, 1 - student name desc,
    /// 2 - group asc, 3 - group desc,
    /// 4 - department asc, 5 - department desc,
    /// 6 - year asc, 7 - year desc,
    /// 8 - education level asc, 9 - education level desc,
    /// 10 - isShort asc, 11 - isShort desc,
    /// 12 - choice date asc, 13 - choice date desc (default)
    /// </summary>
    public int SortOrder { get; set; } = 13;
}

public class GetStudentsByMainDisciplineQueryDto
{
    public int DisciplineId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int? FacultyId { get; set; }
    public int? GroupId { get; set; }
    public string? Search { get; set; }

    /// <summary>
    /// 0 - by student name (asc, default), 1 - by student name (desc), 2 - by group code (asc), 3 - by group code (desc)
    /// </summary>
    public int SortOrder { get; set; } = 0;
}

