using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Group
{
    public int IdGroup { get; set; }

    public string GroupCode { get; set; } = null!;

    public int? NumberOfStudents { get; set; }

    public int? AdminId { get; set; }

    public int? DegreeId { get; set; }

    public int? Course { get; set; }

    public int? FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    /// <summary>
    /// Освітня програма
    /// </summary>
    public int? IdEducationalProgram { get; set; }

    /// <summary>
    /// Спеціальність
    /// </summary>
    public int? IdSpeciality { get; set; }

    /// <summary>
    /// Рік вступу
    /// </summary>
    public int? AdmissionYear { get; set; }

    /// <summary>
    /// Форма навчання
    /// </summary>
    public int? IdStudyForm { get; set; }

    /// <summary>
    /// Спеціалізація
    /// </summary>
    public int? IdSpecialization { get; set; }

    /// <summary>
    /// Чи прискорений (0 - ні, 1 - так)
    /// </summary>
    public bool IsAccelerated { get; set; }

    public virtual AdminsPersonal? Admin { get; set; }

    public virtual EducationalDegree? Degree { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Faculty? Faculty { get; set; }

    public virtual EducationalProgram? IdEducationalProgramNavigation { get; set; }

    public virtual Speciality? IdSpecialityNavigation { get; set; }

    public virtual Specialization? IdSpecializationNavigation { get; set; }

    public virtual StudyForm? IdStudyFormNavigation { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
