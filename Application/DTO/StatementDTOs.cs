using System;

namespace OlimpBack.Application.DTO;

public class GroupShortDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
}

public class StatementFileDto
{
    public string FileName { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
    public string ContentType { get; set; } = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
}
