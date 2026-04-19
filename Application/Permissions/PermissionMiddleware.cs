using OlimpBack.Utils;

namespace OlimpBack.Application.Permissions;

public class PermissionMiddleware
{
    private readonly RequestDelegate _next;

    public PermissionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var attr = endpoint?.Metadata.GetMetadata<RequirePermissionAttribute>();

        if (attr == null)
        {
            await _next(context);
            return;
        }

        var permClaim = context.User.FindFirst("perm")?.Value;
        if (string.IsNullOrWhiteSpace(permClaim) || !long.TryParse(permClaim, out var userMask))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var requiredMask = PermissionMaskHelper.ToMask(attr.BitIndex);
        if (!PermissionMaskHelper.HasAll(userMask, requiredMask))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await _next(context);
    }
}
