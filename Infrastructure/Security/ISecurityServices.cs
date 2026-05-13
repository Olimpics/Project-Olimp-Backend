using System;
using System.Threading.Tasks;

namespace OlimpBack.Infrastructure.Security;

public interface IReplayProtectionService
{
    Task<bool> IsReplayAsync(Guid conversationId, string nonce, long timestamp);
}

public interface IRateLimitService
{
    Task<bool> IsAllowedAsync(string key, int limit, TimeSpan window);
}

public interface IDeviceTrustService
{
    Task<bool> IsDeviceTrustedAsync(Guid userId, Guid deviceId);
    Task ValidateMessageIntegrityAsync(byte[] payload, string providedHash);
}

public interface IConversationAccessService
{
    Task<bool> CanUserAccessConversationAsync(Guid userId, Guid conversationId);
}
