using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Department
{
    public string NameDepartment { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public bool Avail { get; set; }

    public Guid FacultyId { get; set; }

    public Guid IdDepartment { get; set; }

    public string? NameInDock { get; set; }

    public virtual ICollection<AdminsPersonal> AdminsPersonals { get; set; } = new List<AdminsPersonal>();

    public virtual ICollection<DisciplineChoicePeriod> DisciplineChoicePeriods { get; set; } = new List<DisciplineChoicePeriod>();

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();

    public virtual ICollection<Speciality> Specialities { get; set; } = new List<Speciality>();

    public virtual ICollection<UserHierarchyAssignment> UserHierarchyAssignments { get; set; } = new List<UserHierarchyAssignment>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
