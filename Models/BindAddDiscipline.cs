using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class BindAddDiscipline
{
    public int IdBindAddDisciplines { get; set; }

    public int? StudentId { get; set; }

    public int? AddDisciplinesId { get; set; }

    public int? Semestr { get; set; }

    public int? Loans { get; set; }

    public BitArray? InProcess { get; set; }

    public string? Grade { get; set; }

    public string? CreatedAt { get; set; }

    public virtual AddDiscipline? AddDisciplines { get; set; }

    public virtual Student? Student { get; set; }
}
