using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindSimilarSelectiveInGroup
{
    public int? IdBind { get; set; }

    public int? GroupId { get; set; }

    public int? SelectiveId { get; set; }

    public virtual GroupSimilarSelective? Group { get; set; }

    public virtual SelectiveDiscipline? Selective { get; set; }
}
