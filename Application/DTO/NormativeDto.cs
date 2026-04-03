namespace OlimpBack.Application.DTO;

public class NormativeDto
{
    public int IdNormative { get; set; }
    public int Count { get; set; }
    public sbyte IsFaculty { get; set; }
    public int? DegreeLevelId { get; set; }

    // Додаткове поле для зручності фронтенду
    public string? DegreeLevelName { get; set; }
}

public class CreateNormativeDto
{
    public int Count { get; set; }
    public sbyte IsFaculty { get; set; }
    public int? DegreeLevelId { get; set; }
}

public class UpdateNormativeDto
{
    public int IdNormative { get; set; }
    public int Count { get; set; }
    public sbyte IsFaculty { get; set; }
    public int? DegreeLevelId { get; set; }
}