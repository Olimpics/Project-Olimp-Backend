namespace OlimpBack.Application.DTO
{

    // Це наша композитна частина (лише текстовий контент)
    public class TypeOfDisciplineDto
    {
        public int IdTypeOfDiscipline { get; set; }
        public string TypeName { get; set; } = null!;
    }

    public class CreateTypeOfDisciplineDto
    {
        public string TypeName { get; set; } = null!;
    }

} 