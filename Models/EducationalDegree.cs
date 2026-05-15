using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class EducationalDegree
{
    public string? NameEducationalDegree { get; set; }

    public string? NameInDocuments { get; set; }

    public Guid Ideducationaldegree { get; set; }

    public virtual ICollection<DefaultUniNeed> DefaultUniNeeds { get; set; } = new List<DefaultUniNeed>();

    public virtual ICollection<DisciplineChoicePeriod> DisciplineChoicePeriods { get; set; } = new List<DisciplineChoicePeriod>();

    public virtual ICollection<EducationalProgram> EducationalPrograms { get; set; } = new List<EducationalProgram>();

    public virtual ICollection<Normative> Normatives { get; set; } = new List<Normative>();

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();

    public virtual ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();
}
