using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Approval
{
    public int IdApproval { get; set; }

    public string? AppovalStatus { get; set; }

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();
}
