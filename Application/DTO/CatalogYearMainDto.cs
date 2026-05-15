using System;

namespace OlimpBack.Application.DTO;

public class CatalogYearMainDto
{
    public Guid IdCatalogYear { get; set; }
    public string NameCatalog { get; set; } = null!;
    public bool IsFormed { get; set; }
}

public class CreateCatalogYearMainDto
{
    public string NameCatalog { get; set; } = null!;
    public bool IsFormed { get; set; }
}

public class UpdateCatalogYearMainDto
{
    public Guid IdCatalogYear { get; set; }
    public string NameCatalog { get; set; } = null!;
    public bool IsFormed { get; set; }
}
