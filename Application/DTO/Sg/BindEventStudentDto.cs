using System;

namespace OlimpBack.Application.DTO.Sg;

public class BindEventStudentDto
{
    public Guid IdBindEventStudent { get; set; }
    public Guid EventId { get; set; }
    public Guid StudentId { get; set; }
    public Guid RoleId { get; set; }
    public int Point { get; set; }
    public string? OtherOption { get; set; }
}

public class BindEventStudentCreateUpdateDto
{
    public Guid EventId { get; set; }
    public Guid StudentId { get; set; }
    public Guid RoleId { get; set; }
    public int Point { get; set; }
    public string? OtherOption { get; set; }
}
