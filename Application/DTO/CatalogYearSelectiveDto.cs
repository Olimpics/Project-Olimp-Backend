using System;

namespace OlimpBack.Application.DTO;

public class CatalogYearSelectiveDto
{
    public Guid IdCatalogYear { get; set; }
    public string NameCatalog { get; set; } = null!;
    public bool IsFormed { get; set; }
}

public class CreateCatalogYearSelectiveDto
{
    public string NameCatalog { get; set; } = null!;
    public bool IsFormed { get; set; }
}

public class UpdateCatalogYearSelectiveDto
{
    public Guid IdCatalogYear { get; set; }
    public string NameCatalog { get; set; } = null!;
    public bool IsFormed { get; set; }
}
