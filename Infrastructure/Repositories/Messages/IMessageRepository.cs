using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Repositories.Messages;

public interface IMessageRepository
{
    Task<Message> SaveEncryptedMessageAsync(Message message);
    Task<(IEnumerable<Message> Messages, string? NextCursor)> GetMessageHistoryAsync(Guid conversationId, DateTime? beforeTimestamp, Guid? beforeId, int limit);
    Task<IEnumerable<Message>> SyncMessagesAsync(Guid conversationId, DateTime since, Guid? lastMessageId);
    Task<bool> SoftDeleteMessageAsync(Guid messageId, Guid userId);
    Task<bool> ValidateConversationAccessAsync(Guid conversationId, Guid userId);
    Task<Message?> GetMessageByIdAsync(Guid messageId);
    Task<bool> MarkMessageDeliveredAsync(Guid messageId, DateTime deliveredAt);
    Task<bool> MarkMessageReadAsync(Guid messageId, DateTime readAt);
}
