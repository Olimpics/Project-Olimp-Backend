using System;

namespace OlimpBack.Application.DTO
{
    public class FacultyDto
    {
        public Guid IdFaculty { get; set; }
        public string NameFaculty { get; set; } = null!;
        public string? Abbreviation { get; set;}
    }
    public class FacultyCreateDto
    {
        public string NameFaculty { get; set; } = null!;
        public string? Abbreviation { get; set; }
    }

}
