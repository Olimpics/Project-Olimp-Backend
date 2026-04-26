using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Department
{
    public int? IdDepartment { get; set; }

    public int? FacultyId { get; set; }

    public string? NameDepartment { get; set; }

    public string? Abbreviation { get; set; }

    public string? AdminId { get; set; }

    public string? Metadata { get; set; }

    public virtual Faculty? Faculty { get; set; }
}
