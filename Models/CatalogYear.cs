using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

/// <summary>
/// Катлог за навчальним роками.
/// </summary>
public partial class CatalogYear
{
    public int IdCatalogYear { get; set; }

    public string NameCatalog { get; set; } = null!;

    public sbyte IsFormed { get; set; }

    public virtual ICollection<AddDiscipline> AddDisciplines { get; set; } = new List<AddDiscipline>();
}
