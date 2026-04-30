using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindTeachersSelective
{
    public int IdBindTeacherSelective { get; set; }

    public int? AdminId { get; set; }

    public int? SelectiveDisciplinesId { get; set; }

    public virtual AdminsPersonal? Admin { get; set; }

    public virtual SelectiveDiscipline? SelectiveDisciplines { get; set; }
}
