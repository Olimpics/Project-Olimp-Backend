using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Branch
{
    public int IdBranch { get; set; }

    /// <summary>
    /// Код галузі (наприклад, 12)
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// Назва галузі (наприклад, Інформаційні технології)
    /// </summary>
    public string Name { get; set; } = null!;

    public virtual ICollection<Speciality> Specialities { get; set; } = new List<Speciality>();
}
