using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OlimpBack.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string? Email { get; set; }

    public byte[]? Passwordhash { get; set; }

    public byte[]? Passwordsalt { get; set; }

    public int Roleid { get; set; }

    public int Isfirstlogin { get; set; }

    public DateTime? Passwordchangedat { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Lastloginat { get; set; }

    /// <summary>Primary role from users.roleid (roles.id).</summary>
    [ForeignKey(nameof(Roleid))]
    public virtual Role1? PrimaryRole { get; set; }

    public virtual ICollection<Role1> Roles { get; set; } = new List<Role1>();
}
