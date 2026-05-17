using System;

namespace OlimpBack.Application.DTO;

public class RatingStatusQueryDto
{
    public Guid SpecialityId { get; set; }
    public int Course { get; set; }
    public int Semester { get; set; }
    public Guid CatalogYearId { get; set; }
    public bool IsAccelerated { get; set; }
}

public class RatingStatusResponseDto
{
    public bool Exists { get; set; }
    public DateOnly CalculationTime { get; set; }
}
