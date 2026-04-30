namespace OlimpBack.Application.Permissions;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class RequirePermissionAttribute : Attribute
{
    public int BitIndex { get; }

    public RequirePermissionAttribute(int bitIndex)
    {
        BitIndex = bitIndex;
    }

    public RequirePermissionAttribute(RbacPermissions permission)
    {
        BitIndex = (int)permission;
    }
}
