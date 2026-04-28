using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class CatalogYearsMain
{
    public int IdCatalogYear { get; set; }

    public string? NameCatalog { get; set; }

    public BitArray? IsFormed { get; set; }

    public virtual ICollection<MainDiscipline> MainDisciplines { get; set; } = new List<MainDiscipline>();
}
