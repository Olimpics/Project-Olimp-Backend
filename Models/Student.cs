using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Student
{
    public int IdStudents { get; set; }

    public int UserId { get; set; }

    public string NameStudent { get; set; } = null!;

    public int StatusId { get; set; }

    public DateOnly EducationStart { get; set; }

    public DateOnly EducationEnd { get; set; }

    public int FacultyId { get; set; }

    public int EducationalDegreeId { get; set; }

    public int StudyFormId { get; set; }

    public sbyte IsShort { get; set; }

    public int EducationalProgramId { get; set; }

    public int Course { get; set; }

    public int? DepartmentId { get; set; }

    public byte[]? Photo { get; set; }

    public virtual ICollection<BindAddDiscipline> BindAddDisciplines { get; set; } = new List<BindAddDiscipline>();

    public virtual EducationalDegree EducationalDegree { get; set; } = null!;

    public virtual EducationalProgram EducationalProgram { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual EducationStatus Status { get; set; } = null!;

    public virtual StudyForm StudyForm { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
