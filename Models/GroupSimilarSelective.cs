using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class GroupSimilarSelective
{
    public int IdGroup { get; set; }

    public string? GroupName { get; set; }

    public int? CentralId { get; set; }

    public virtual SelectiveDiscipline? Central { get; set; }
}
