namespace OlimpBack.Application.DTO;

public class GrantablePermissionDto
{
    public Guid PermissionId { get; set; }

    public string Code { get; set; } = null!;

    public int BitIndex { get; set; }

    public int MinGrantLevel { get; set; }

    public string TypePermission { get; set; } = null!;

    public string TableName { get; set; } = null!;
}
