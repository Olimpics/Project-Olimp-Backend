using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindTeachersSelective
{
    public Guid AdminId { get; set; }

    public Guid SelectiveDisciplinesId { get; set; }

    public bool IsHead { get; set; }

    public Guid IdBindTeacherSelective { get; set; }

    public virtual AdminsPersonal Admin { get; set; } = null!;

    public virtual SelectiveDiscipline SelectiveDisciplines { get; set; } = null!;
}
