using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class StudyForm
{
    public int IdStudyForm { get; set; }

    public string NameStudyForm { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
