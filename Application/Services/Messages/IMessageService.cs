using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO.Messages;

namespace OlimpBack.Application.Services.Messages;

public interface IMessageService
{
    Task<EncryptedMessageResponse> SendMessageAsync(SendEncryptedMessageRequest request, Guid userId);
    Task<MessageHistoryResponse> GetMessageHistoryAsync(MessageCursorPaginationRequest request, Guid userId);
    Task<IEnumerable<EncryptedMessageResponse>> SyncMessagesAsync(SyncMessagesRequest request, Guid userId);
    Task<bool> SoftDeleteMessageAsync(Guid messageId, Guid userId);
    Task<bool> MarkDeliveredAsync(Guid messageId, Guid userId);
    Task<bool> MarkReadAsync(Guid messageId, Guid userId);
}
