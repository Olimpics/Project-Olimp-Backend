using System.Security.Claims;

namespace OlimpBack.Application.Permissions;

public static class HttpContextUserExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var raw = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                  ?? user.FindFirst(ClaimTypes.Name)?.Value;

        return Guid.TryParse(raw, out var userId) ? userId : null;
    }
}
