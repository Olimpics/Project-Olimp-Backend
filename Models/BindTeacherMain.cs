using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindTeacherMain
{
    public int IdBindTeacherMain { get; set; }

    public int? AdminId { get; set; }

    public int? MainDisciplinesId { get; set; }

    public virtual AdminsPersonal? Admin { get; set; }

    public virtual MainDiscipline? MainDisciplines { get; set; }
}
