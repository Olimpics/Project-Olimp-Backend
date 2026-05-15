using System;

namespace OlimpBack.Application.DTO;

public class NormativeDto
{
    public Guid IdNormative { get; set; }
    public int Count { get; set; }
    public bool IsFaculty { get; set; }
    public Guid? DegreeLevelId { get; set; }

    //   
    public string? DegreeLevelName { get; set; }
}

public class CreateNormativeDto
{
    public int Count { get; set; }
    public bool IsFaculty { get; set; }
    public Guid? DegreeLevelId { get; set; }
}

public class UpdateNormativeDto
{
    public Guid IdNormative { get; set; }
    public int Count { get; set; }
    public bool IsFaculty { get; set; }
    public Guid? DegreeLevelId { get; set; }
}