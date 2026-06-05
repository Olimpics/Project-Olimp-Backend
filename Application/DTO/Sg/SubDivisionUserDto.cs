using System;

namespace OlimpBack.Application.DTO.Sg;

public class SubDivisionUserDto
{
    public Guid SubDivisionId { get; set; }
    public string NameDivision { get; set; } = null!;
    public Guid? FacultyId { get; set; }
    public string? Abbreviation { get; set; }
}
