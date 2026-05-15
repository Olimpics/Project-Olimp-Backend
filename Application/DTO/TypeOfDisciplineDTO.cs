using System;

namespace OlimpBack.Application.DTO
{

    // 
    public class TypeOfDisciplineDto
    {
        public Guid IdTypeOfDiscipline { get; set; }
        public string TypeName { get; set; } = null!;
    }

    public class CreateTypeOfDisciplineDto
    {
        public string TypeName { get; set; } = null!;
    }

} 
