using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO;

public class StudentQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Search { get; set; }

    public List<Guid>? Faculties { get; set; }
    public List<Guid>? Specialities { get; set; }
    public List<Guid>? GroupIds { get; set; }
    public List<int>? Courses { get; set; }
    public List<Guid>? StudyFormIds { get; set; }
    public List<Guid>? DegreeLevelIds { get; set; }
    public bool? IsShort { get; set; }
    public int SortOrder { get; set; } = 0;
}
