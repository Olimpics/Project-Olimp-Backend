using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Bindstudentsfavouritediscipline
{
    public int? IdBindStudentsFavouriteDisciplines { get; set; }

    public int? IdStudent { get; set; }

    public int? IdAddDiscipline { get; set; }

    public virtual Adddiscipline? IdAddDisciplineNavigation { get; set; }
}
