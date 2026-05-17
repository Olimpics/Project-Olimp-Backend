using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class CatalogYearsMain
{
    public bool IsFormed { get; set; }

    public Guid IdCatalogYear { get; set; }

    public int YearStart { get; set; }

    public int YearEnd { get; set; }

    public virtual ICollection<EducationalProgram> EducationalPrograms { get; set; } = new List<EducationalProgram>();
}
