using System;

namespace OlimpBack.Application.DTO;

public class GenerateRatingQueryDto
{
    public int EducationalProgramId { get; set; }
    public int Course { get; set; }
    public int SemesterType { get; set; } // 1 or 2
    public int CatalogYearId { get; set; }
    public bool IsAccelerated { get; set; }
}
