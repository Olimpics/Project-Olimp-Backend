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
        var attrs = endpoint?.Metadata.GetOrderedMetadata<RequirePermissionAttribute>() ?? Array.Empty<RequirePermissionAttribute>();

        if (attrs.Count == 0)
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

        var requiredMask = PermissionMaskHelper.BuildMask(attrs.Select(attr => attr.BitIndex));
        if (!PermissionMaskHelper.HasAll(userMask, requiredMask))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await _next(context);
    }
}
