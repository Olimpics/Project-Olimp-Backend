namespace OlimpBack.DTO
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
} 