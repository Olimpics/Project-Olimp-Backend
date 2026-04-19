namespace OlimpBack.Application.DTO
{
    public class RoleDto
    {
        public int IdRole { get; set; }
        public string NameRole { get; set; }
        public int? ParentRoleId { get; set; }
        public long PermissionsMask { get; set; }
    }

}
