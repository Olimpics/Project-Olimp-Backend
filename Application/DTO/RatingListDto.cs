using System;

namespace OlimpBack.Application.DTO;

public class RatingListQueryDto
{
    public Guid SpecialityId { get; set; }
    public int Course { get; set; }
    public int Semester { get; set; }
    public Guid CatalogYearId { get; set; }
    public bool IsAccelerated { get; set; }
    
    public bool? IsFundedOnly { get; set; }
    public bool? NoRetakesOnly { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class RatingStudentDto
{
    public string FullName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public float Score { get; set; }
}
