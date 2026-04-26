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

    public int IsShort { get; set; }

    public int EducationalProgramId { get; set; }

    public int Course { get; set; }

    public int? DepartmentId { get; set; }

    public int GroupId { get; set; }

    public int IsInSg { get; set; }

    public virtual EducationStatus? EducationStatus { get; set; }

    public virtual User? User { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual EducationalProgram? EducationalProgram { get; set; }

    public virtual EducationalDegree? EducationalDegree { get; set; }

    public virtual StudyForm? StudyForm { get; set; }

    public virtual Group? Group { get; set; }

    public virtual ICollection<Bindadddiscipline> BindAddDisciplines { get; set; } = new List<Bindadddiscipline>();
}
