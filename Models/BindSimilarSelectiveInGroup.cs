using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindSimilarSelectiveInGroup
{
    public Guid? GroupId { get; set; }

    public Guid? SelectiveId { get; set; }

    public Guid IdBind { get; set; }

    public virtual GroupSimilarSelective? Group { get; set; }

    public virtual SelectiveDiscipline? Selective { get; set; }
}
