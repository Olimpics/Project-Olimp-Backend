namespace OlimpBack.DTO
{
    // DTO/StudentDto.cs
    public class StudentDto
    {
        public int IdStudents { get; set; }
        public string NameStudent { get; set; }
        public string StatusName { get; set; }
        public string FacultyName { get; set; }
        public string ProgramName { get; set; }
        public string DegreeName { get; set; }
        public string StudyFormName { get; set; }
        public string GroupName { get; set; }
        public DateOnly EducationStart { get; set; }
        public DateOnly EducationEnd { get; set; }
        public int Course { get; set; }
        public SByte IsShort { get; set; }
    }
    public class StudentForCatalogDto
    {
        public int IdStudents { get; set; }
        public string NameStudent { get; set; }
        public string FacultyAbbreviation { get; set; }
        public string Speciality { get; set; }
        public string DegreeName { get; set; }
        public string GroupName { get; set; }
        public int Course { get; set; }
    }

    // DTO/CreateStudentDto.cs
    public class CreateStudentDto
    {
        public int UserId { get; set; }
        public string NameStudent { get; set; }
        public int StatusId { get; set; }
        public DateOnly EducationStart { get; set; }
        public DateOnly EducationEnd { get; set; }
        public int Course { get; set; }
        public int FacultyId { get; set; }
        public int EducationalDegreeId { get; set; }
        public int StudyFormId { get; set; }
        public SByte IsShort { get; set; }
        public int EducationalProgramId { get; set; }
    }

    // DTO/UpdateStudentDto.cs
    public class UpdateStudentDto : CreateStudentDto
    {
    }

}
