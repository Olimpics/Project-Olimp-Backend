using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class StudyForm
{
    public int IdStudyForm { get; set; }

    public string? NameStudyForm { get; set; }

    public virtual ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
