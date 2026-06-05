using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class MembersOfSg
{
    public Guid StudentId { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid BindSubdivisionRoleInSgId { get; set; }

    public bool Avail { get; set; }

    public Guid IdMembersOfSg { get; set; }

    public Guid? FacultyId { get; set; }

    public virtual BindSubdivisionRoleSg BindSubdivisionRoleInSg { get; set; } = null!;

    public virtual Student CreatedByNavigation { get; set; } = null!;

    public virtual Faculty? Faculty { get; set; }

    public virtual Student Student { get; set; } = null!;
}
