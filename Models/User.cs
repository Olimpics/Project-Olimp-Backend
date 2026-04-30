using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string? Email { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public BitArray IsFirstLogin { get; set; } = null!;

    public DateTime? PasswordChangedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastLoginAt { get; set; }

    public byte[]? Passwordhash { get => PasswordHash; set => PasswordHash = value; }

    public byte[]? Passwordsalt { get => PasswordSalt; set => PasswordSalt = value; }

    public BitArray Isfirstlogin { get => IsFirstLogin; set => IsFirstLogin = value; }

    public DateTime? Passwordchangedat { get => PasswordChangedAt; set => PasswordChangedAt = value; }

    public DateTime Createdat { get => CreatedAt; set => CreatedAt = value; }

    public DateTime Lastloginat { get => LastLoginAt; set => LastLoginAt = value; }

    public virtual ICollection<AdminsPersonal> AdminsPersonals { get; set; } = new List<AdminsPersonal>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
