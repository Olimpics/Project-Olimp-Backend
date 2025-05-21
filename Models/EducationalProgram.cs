using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationalProgram
{
    public int IdEducationalProgram { get; set; }

    public string NameEducationalProgram { get; set; } = null!;

    public int? CountAddSemestr3 { get; set; }

    public int? CountAddSemestr4 { get; set; }

    public int? CountAddSemestr5 { get; set; }

    public int? CountAddSemestr6 { get; set; }

    public int? CountAddSemestr7 { get; set; }

    public int? CountAddSemestr8 { get; set; }

    public string Degree { get; set; } = null!;

    public string Speciality { get; set; } = null!;

    public sbyte Accreditation { get; set; }

    public string AccreditationType { get; set; } = null!;

    public uint StudentsAmount { get; set; }

    public virtual ICollection<BindMainDiscipline> BindMainDisciplines { get; set; } = new List<BindMainDiscipline>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
