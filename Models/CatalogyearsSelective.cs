using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class CatalogYearsSelective
{
    public string? NameCatalog { get; set; }

    public bool IsFormed { get; set; }

    public Guid IdCatalogYearSelective { get; set; }

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();
}
