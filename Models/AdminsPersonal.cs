using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Adminspersonal
{
    public int? IdAdmins { get; set; }

    public int? UserId { get; set; }

    public string? NameAdmin { get; set; }

    public int? FacultyId { get; set; }

    public int? DepartmentId { get; set; }

    public string? Photo { get; set; }
}
