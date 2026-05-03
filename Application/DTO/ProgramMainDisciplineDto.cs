using System.Collections.Generic;

namespace OlimpBack.Application.DTO;

public class ProgramMainDisciplineDto
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? FormControl { get; set; }
    public int Loans { get; set; }
    public int Hours { get; set; }
}

public class ProgramDisciplinesBySemesterDto
{
    public int Semester { get; set; }
    public List<ProgramMainDisciplineDto> Disciplines { get; set; } = new();
}
