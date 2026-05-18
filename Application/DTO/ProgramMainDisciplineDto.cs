using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO;

public class ProgramMainDisciplineDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public Guid? TypeOfControl { get; set; }
    public int Loans { get; set; }
    public int Hours { get; set; }
}

public class ProgramDisciplinesBySemesterDto
{
    public int Semester { get; set; }
    public List<ProgramMainDisciplineDto> Disciplines { get; set; } = new();
}
