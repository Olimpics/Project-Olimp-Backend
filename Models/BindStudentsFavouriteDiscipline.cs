using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindStudentsFavouriteDiscipline
{
    public int IdBindStudentsFavouriteDisciplines { get; set; }

    public int? IdStudent { get; set; }

    public int? IdAddDiscipline { get; set; }

    public virtual AddDiscipline? IdAddDisciplineNavigation { get; set; }

    public virtual Student? IdStudentNavigation { get; set; }
}
