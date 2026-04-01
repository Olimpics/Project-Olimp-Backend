using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindStudentsFavouriteDiscipline
{
    public int IdBindStudentsFavouriteDisciplines { get; set; }

    public int? IdStudent { get; set; }

    public int? IdAddDisciplines { get; set; }

    public virtual AddDiscipline? IdAddDisciplinesNavigation { get; set; }

    public virtual Student? IdStudentNavigation { get; set; }
}
