using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindTeacherMain
{
    public Guid? MainDisciplinesId { get; set; }

    public bool IsHead { get; set; }

    public Guid? AdminId { get; set; }

    public Guid IdBindTeacherMain { get; set; }

    public virtual AdminsPersonal? Admin { get; set; }

    public virtual MainDiscipline? MainDisciplines { get; set; }
}
