namespace OlimpBack.Application.DTO
{
    public class GroupDto
    {
        public int IdGroup { get; set; }
        public string GroupCode { get; set; } = null!;
        public int? NumberOfStudents { get; set; }
    }

    public class GroupBindMainDisciplinesDTO
    {   
        //public int idGroup { get; set; }
        public int idBindMainDisciplines { get; set; }
        public string nameBindMainDisciplines { get; set; } = null!;
        public int? Semestr { get; set; }
        public int? Loans { get; set; }
        public int? Hours { get; set; }
    }

    public class GroupCurriculumDTO
    {
        public int IdGroup { get; set; }
        public string GroupCode { get; set; } = null!;
        public List<GroupBindMainDisciplinesDTO> BindMainDisciplines { get; set; } = new List<GroupBindMainDisciplinesDTO>();
    }


    public class CreateGroupDto
    {
        public string GroupCode { get; set; } = null!;
        public int? NumberOfStudents { get; set; }
    }

    public class UpdateGroupDto : CreateGroupDto
    {
        public int IdGroup { get; set; }
    }

    public class GroupDetailsDto
    {
        public int IdGroup { get; set; }
        public string GroupCode { get; set; } = null!;
        public int? NumberOfStudents { get; set; }
        public int? AdminId { get; set; }
        public int? DegreeId { get; set; }
        public int? Course { get; set; }
        public int? FacultyId { get; set; }
        public string? FacultyName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? IdEducationalProgram { get; set; }
        public string? EducationalProgramName { get; set; }
        public int? IdSpeciality { get; set; }
        public string? SpecialityName { get; set; }
        public int? AdmissionYear { get; set; }
        public int? IdStudyForm { get; set; }
        public int? IdSpecialization { get; set; }
        public string? SpecializationName { get; set; }
        public bool IsAccelerated { get; set; }
    }

    public class GroupStudentDto
    {
        public int IdStudent { get; set; }
        public int UserId { get; set; }
        public string NameStudent { get; set; } = null!;
        public string EmailStudent { get; set; } = null!;
        public string EductionalStatus { get; set; } = null!;
    }
} 