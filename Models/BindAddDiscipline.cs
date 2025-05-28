using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OlimpBack.Models;

public partial class BindAddDiscipline
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdBindAddDisciplines { get; set; }

    public int StudentId { get; set; }

    public int AddDisciplinesId { get; set; }

    public int Semestr { get; set; }

    public int Loans { get; set; }

    public sbyte InProcess { get; set; }

    public virtual AddDiscipline AddDisciplines { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
