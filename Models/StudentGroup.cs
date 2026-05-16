using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class StudentGroup
{
    public string GroupCode { get; set; } = null!;

    public int Course { get; set; }

    public DateOnly? AdmissionYear { get; set; }

    public Guid EducationalProgramId { get; set; }

    public Guid IdGroup { get; set; }

    public Guid StudyFormId { get; set; }

    public bool Avail { get; set; }

    public bool IsAccelerated { get; set; }

    public Guid AdminId { get; set; }

    public virtual AdminsPersonal Admin { get; set; } = null!;

    public virtual EducationalProgram EducationalProgram { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual StudyForm StudyForm { get; set; } = null!;
}
