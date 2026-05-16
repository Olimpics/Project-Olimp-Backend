using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class GroupSimilarSelective
{
    public string GroupName { get; set; } = null!;

    public Guid CentrallId { get; set; }

    public Guid IdGroup { get; set; }

    public virtual ICollection<BindSimilarSelectiveInGroup> BindSimilarSelectiveInGroups { get; set; } = new List<BindSimilarSelectiveInGroup>();

    public virtual SelectiveDiscipline Centrall { get; set; } = null!;
}
