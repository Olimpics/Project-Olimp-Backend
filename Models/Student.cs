using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Student
{
    public int IdStudent { get; set; }

    public int UserId { get; set; }

    public string? NameStudent { get; set; }

    public int EducationStatusId { get; set; }

    public DateOnly EducationStart { get; set; }

    public DateOnly EducationEnd { get; set; }

    public int FacultyId { get; set; }

    public int EducationalDegreeId { get; set; }

    public int StudyFormId { get; set; }

    public short IsShort { get; set; }

    public int EducationalProgramId { get; set; }

    public int Course { get; set; }

    public int GroupId { get; set; }

    public int IsInSg { get; set; }

    public List<int>? Idfav { get; set; }

    public int? Departmentid { get; set; }

    public virtual ICollection<BindAddDiscipline> BindAddDisciplines { get; set; } = new List<BindAddDiscipline>();

    public virtual ICollection<BindEvent> BindEvents { get; set; } = new List<BindEvent>();

    public virtual ICollection<BindExtraActivity> BindExtraActivities { get; set; } = new List<BindExtraActivity>();

    public virtual Department? Department { get; set; }

    public virtual EducationStatus EducationStatus { get; set; } = null!;

    public virtual EducationalDegree EducationalDegree { get; set; } = null!;

    public virtual EducationalProgram EducationalProgram { get; set; } = null!;

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual StudentGroup Group { get; set; } = null!;

    public virtual MembersOfSg IsInSgNavigation { get; set; } = null!;

    public virtual ICollection<MainGrade> MainGrades { get; set; } = new List<MainGrade>();

    public virtual StudyForm StudyForm { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
