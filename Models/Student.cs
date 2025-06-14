using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Student
{
    public int IdStudent { get; set; }

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

    public int GroupId { get; set; }

    public byte[]? Photo { get; set; }

    public bool IsInSg { get; set; }

    public string? Achievement { get; set; }

    public virtual ICollection<BindAddDiscipline> BindAddDisciplines { get; set; } = new List<BindAddDiscipline>();

    public virtual ICollection<BindEvent> BindEvents { get; set; } = new List<BindEvent>();

    public virtual ICollection<BindExtraActivity> BindExtraActivities { get; set; } = new List<BindExtraActivity>();

    public virtual EducationalDegree EducationalDegree { get; set; } = null!;

    public virtual EducationalProgram EducationalProgram { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual Group Group { get; set; } = null!;

    public virtual ICollection<MainGrade> MainGrades { get; set; } = new List<MainGrade>();

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public virtual EducationStatus Status { get; set; } = null!;

    public virtual StudyForm StudyForm { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
