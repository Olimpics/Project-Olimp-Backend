namespace OlimpBack.Application.DTO
{
    public class GroupDto
    {
        public int IdGroup { get; set; }
        public string GroupCode { get; set; } = null!;
        public int? NumberOfStudents { get; set; }
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
        public int? DepartmentId { get; set; }
        public int? IdEducationalProgram { get; set; }
        public int? IdSpeciality { get; set; }
        public int? AdmissionYear { get; set; }
        public int? IdStudyForm { get; set; }
        public int? IdSpecialization { get; set; }
        public bool IsAccelerated { get; set; }
    }

    public class GroupStudentDto
    {
        public int IdStudent { get; set; }
        public int UserId { get; set; }
        public string NameStudent { get; set; } = null!;
        public int Course { get; set; }
        public int GroupId { get; set; }
    }
} 