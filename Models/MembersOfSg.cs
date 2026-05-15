using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MembersOfSg
{
    public Guid StudentId { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? BindsubdivisionRoleSgid { get; set; }

    public bool Avail { get; set; }

    public Guid IdMembersOfSg { get; set; }

    public virtual BindSubdivisionRoleSg? BindsubdivisionRoleSg { get; set; }

    public virtual Student? CreatedByNavigation { get; set; }

    public virtual Student Student { get; set; } = null!;
}
