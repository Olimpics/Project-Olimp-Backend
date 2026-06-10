using System;

namespace OlimpBack.Application.DTO
{
    public class TypeOfControlDto
    {
        public Guid IdTypeOfControl { get; set; }
        public string Type { get; set; } = null!;
        public string? NameInDock { get; set; }
    }

    public class CreateTypeOfControlDto
    {
        public string Type { get; set; } = null!;
        public string? NameInDock { get; set; }
    }

    public class UpdateTypeOfControlDto : CreateTypeOfControlDto
    {
        public Guid IdTypeOfControl { get; set; }
    }
}
