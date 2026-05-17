using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class CatalogYearsSelective
{
    public bool IsFormed { get; set; }

    public Guid IdCatalogYearSelective { get; set; }

    public int YearStart { get; set; }

    public int YearEnd { get; set; }

    public virtual ICollection<SelectiveDiscipline> SelectiveDisciplines { get; set; } = new List<SelectiveDiscipline>();
}
