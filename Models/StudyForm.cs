using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class StudyForm
{
    public string NameStudyForm { get; set; } = null!;

    public Guid IdStudyForm { get; set; }

    public virtual ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();
}
