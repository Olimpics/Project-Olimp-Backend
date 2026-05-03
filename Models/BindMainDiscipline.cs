using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindMainDiscipline
{
    public int IdBindMainDisciplines { get; set; }

    public int? StudentId { get; set; }

    public int? MainDisciplinesId { get; set; }

    public int? Semestr { get; set; }

    public string? Grade { get; set; }

    public BitArray? IsRedo { get; set; }

    public virtual MainDiscipline IdBindMainDisciplinesNavigation { get; set; } = null!;

    public virtual Student? Student { get; set; }
}
