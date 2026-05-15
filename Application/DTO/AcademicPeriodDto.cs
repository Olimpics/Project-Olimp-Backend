using System;

namespace OlimpBack.Application.DTO;

public class AcademicPeriodDto
{
    public Guid CatalogYearId { get; set; }
    public int Semester { get; set; } // 1 or 2
}
