using System;

namespace OlimpBack.Application.DTO;

public class GradeQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;

    public Guid DisciplineId { get; set; }
    public Guid CatalogYearId { get; set; }
    public bool IsEvenSemester { get; set; }
}
