using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Department
{
    public int IdDepartment { get; set; }

    public int? FacultyId { get; set; }

    public string? NameDepartment { get; set; }

    public string? Abbreviation { get; set; }

    public virtual ICollection<AdminsPersonal> AdminsPersonals { get; set; } = new List<AdminsPersonal>();

    public virtual ICollection<DisciplineChoicePeriod> DisciplineChoicePeriods { get; set; } = new List<DisciplineChoicePeriod>();

    public virtual Faculty? Faculty { get; set; }

    public virtual ICollection<SelectiveDetail> SelectiveDetails { get; set; } = new List<SelectiveDetail>();

    public virtual ICollection<Speciality> Specialities { get; set; } = new List<Speciality>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
