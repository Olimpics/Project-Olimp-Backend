using System;

namespace OlimpBack.Application.DTO.Sg;

public class MemberSgDto
{
    public Guid IdMembersOfSg { get; set; }
    public Guid StudentId { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid BindSubdivisionRoleInSgId { get; set; }
    public Guid? FacultyId { get; set; }
    public bool Avail { get; set; }
}

public class MemberSgCreateUpdateDto
{
    public Guid StudentId { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid BindSubdivisionRoleInSgId { get; set; }
    public Guid? FacultyId { get; set; }
    public bool Avail { get; set; }
}
