using System;

namespace OlimpBack.Application.DTO.Sg;

public class StudentSgDto
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = null!;
    public string FacultyName { get; set; } = null!;
    public string RoleInSg { get; set; } = null!;
}
