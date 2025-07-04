﻿using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class User
{
    public int IdUsers { get; set; }

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public int RoleId { get; set; }

    public DateTime LastLoginAt { get; set; }

    public virtual ICollection<AdminsPersonal> AdminsPersonals { get; set; } = new List<AdminsPersonal>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
