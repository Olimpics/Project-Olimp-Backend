using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Department
{
    public int IdDepartment { get; set; }

    public int FacultyId { get; set; }

    public string NameDepartment { get; set; } = null!;

    public string Abbreviation { get; set; } = null!;

    public int? AdminId { get; set; }

    public string? Metadata { get; set; }

    public virtual ICollection<AddDetail> AddDetails { get; set; } = new List<AddDetail>();

    public virtual AdminsPersonal? Admin { get; set; }

    public virtual ICollection<AdminsPersonal> AdminsPersonals { get; set; } = new List<AdminsPersonal>();

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
