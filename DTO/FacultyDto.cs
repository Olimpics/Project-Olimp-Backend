namespace OlimpBack.DTO
{
    public class FacultyDto
    {
        public int IdFaculty { get; set; }
        public string NameFaculty { get; set; }
        public string Abbreviation { get; set;}
    }
    public class FacultyCreateDto
    {
        public string NameFaculty { get; set; }
        public string Abbreviation { get; set; }
    }

}
