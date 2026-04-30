using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MembersOfSg
{
    public int IdMembersOfSg { get; set; }

    public int? StudentId { get; set; }

    public int? BindsubdivisionRoleSgid { get; set; }

    public int? CreatedBy { get; set; }

    public virtual BindSubdivisionRoleSg? BindsubdivisionRoleSg { get; set; }

    public virtual Student? CreatedByNavigation { get; set; }

    public virtual Student? Student { get; set; }
}
