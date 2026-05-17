using System;

namespace OlimpBack.Application.DTO;

public class GenerateRatingQueryDto
{
    public Guid SpecialityId { get; set; }
    public int Course { get; set; }
    public int SemesterType { get; set; } // 1 or 2
    public Guid CatalogYearId { get; set; }
    public bool IsAccelerated { get; set; }
}
