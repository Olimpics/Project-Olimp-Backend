using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class GroupSimilarEducationalProgram
{
    public string? GroupName { get; set; }

    public Guid IdGroup { get; set; }

    public Guid? CentralId { get; set; }

    public virtual ICollection<BindSimilaEducationalProgramInGroup> BindSimilaEducationalProgramInGroups { get; set; } = new List<BindSimilaEducationalProgramInGroup>();
}
