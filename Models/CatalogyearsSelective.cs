using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class CatalogYearsSelective
{
    public int IdCatalogYear { get; set; }

    public string? NameCatalog { get; set; }

    public BitArray? IsFormed { get; set; }

    public virtual ICollection<AddDiscipline> AddDisciplines { get; set; } = new List<AddDiscipline>();
}
