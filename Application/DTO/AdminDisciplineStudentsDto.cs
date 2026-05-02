namespace OlimpBack.Application.DTO;

public class AdminStudentBySelectiveDisciplineDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int Year { get; set; }
    public string EducationLevel { get; set; } = string.Empty;
    public sbyte IsShort { get; set; }
    public string Faculty { get; set; } = string.Empty;
}

/// <summary>Minimal student row (e.g. reports).</summary>
public class StudentIdNameDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
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

public class GetStudentsBySelectiveDisciplineQueryDto
{
    public int DisciplineId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int? FacultyId { get; set; }
    public int? GroupId { get; set; }
    /// <summary>When set, only students whose group belongs to this department (group.departmentId).</summary>
    public int? DepartmentId { get; set; }
    public string? Search { get; set; }

    /// <summary>
    /// Sort by response fields (studentId excluded). Even = asc, odd = desc:
    /// 0–1 studentName, 2–3 groupCode, 4–5 departmentName, 6–7 year (course),
    /// 8–9 educationLevel, 10–11 faculty.
    /// </summary>
    public int SortOrder { get; set; } = 0;
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

