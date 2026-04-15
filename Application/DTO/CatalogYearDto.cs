namespace OlimpBack.Application.DTO;

public class CatalogYearDto
{
    public int IdCatalogYear { get; set; }
    public string NameCatalog { get; set; } = null!;
    public sbyte IsFormed { get; set; }
}

public class CreateCatalogYearDto
{
    public string NameCatalog { get; set; } = null!;
    public sbyte IsFormed { get; set; }
}

public class UpdateCatalogYearDto
{
    public int IdCatalogYear { get; set; }
    public string NameCatalog { get; set; } = null!;
    public sbyte IsFormed { get; set; }
}
