using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string? Email { get; set; }

    public byte[]? Passwordhash { get; set; }

    public byte[]? Passwordsalt { get; set; }

    public BitArray Isfirstlogin { get; set; } = null!;

    public DateTime? Passwordchangedat { get; set; }

    public DateTime Createdat { get; set; }

    public DateTime Lastloginat { get; set; }

    public virtual ICollection<AdminsPersonal> AdminsPersonals { get; set; } = new List<AdminsPersonal>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
