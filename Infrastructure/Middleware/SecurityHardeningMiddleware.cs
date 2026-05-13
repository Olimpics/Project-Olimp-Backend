using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OlimpBack.Infrastructure.Security;

namespace OlimpBack.Infrastructure.Middleware;

public class SecurityHardeningMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityHardeningMiddleware> _logger;

    public SecurityHardeningMiddleware(RequestDelegate next, ILogger<SecurityHardeningMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IRateLimitService rateLimitService)
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        // 1. Global IP Rate Limiting
        if (!await rateLimitService.IsAllowedAsync($"ip:{ipAddress}", 100, TimeSpan.FromMinutes(1)))
        {
            _logger.LogWarning("IP {IP} exceeded global rate limit", ipAddress);
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            await context.Response.WriteAsJsonAsync(new { error = "Rate limit exceeded. Please try again later." });
            return;
        }

        // 2. User-based Rate Limiting (if authenticated)
        var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(userIdStr))
        {
            if (!await rateLimitService.IsAllowedAsync($"user:{userIdStr}", 50, TimeSpan.FromMinutes(1)))
            {
                _logger.LogWarning("User {UserId} exceeded rate limit", userIdStr);
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsJsonAsync(new { error = "User rate limit exceeded." });
                return;
            }
        }

        // 3. Payload Size Limit Enforcement (Global)
        if (context.Request.ContentLength > 5 * 1024 * 1024) // 5MB limit
        {
            _logger.LogWarning("Request from {IP} exceeded payload size limit", ipAddress);
            context.Response.StatusCode = (int)HttpStatusCode.RequestEntityTooLarge;
            return;
        }

        await _next(context);
    }
}
