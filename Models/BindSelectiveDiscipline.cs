using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindSelectiveDiscipline
{
    public int IdBindSelectiveDisciplines { get; set; }

    public int? StudentId { get; set; }

    public int? SelectiveDisciplinesId { get; set; }

    public int? Semestr { get; set; }

    public int? Loans { get; set; }

    public BitArray? InProcess { get; set; }

    public string? Grade { get; set; }

    public string? CreatedAt { get; set; }

    public BitArray? IsRedo { get; set; }

    public virtual SelectiveDiscipline? SelectiveDisciplines { get; set; }

    public virtual Student? Student { get; set; }
}
