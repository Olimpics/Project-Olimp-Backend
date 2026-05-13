using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.Application.DTO.Messages;
using OlimpBack.Infrastructure.Repositories.Conversations;
using OlimpBack.Infrastructure.Repositories.Messages;
using OlimpBack.Models;

using OlimpBack.Infrastructure.Security;

namespace OlimpBack.Application.Services.Messages;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IMapper _mapper;
    private readonly IReplayProtectionService _replayProtection;
    private readonly IDeviceTrustService _deviceTrust;
    private readonly IRateLimitService _rateLimit;

    public MessageService(
        IMessageRepository messageRepository,
        IConversationRepository conversationRepository,
        IMapper mapper,
        IReplayProtectionService replayProtection,
        IDeviceTrustService deviceTrust,
        IRateLimitService rateLimit)
    {
        _messageRepository = messageRepository;
        _conversationRepository = conversationRepository;
        _mapper = mapper;
        _replayProtection = replayProtection;
        _deviceTrust = deviceTrust;
        _rateLimit = rateLimit;
    }

    public async Task<EncryptedMessageResponse> SendMessageAsync(SendEncryptedMessageRequest request, Guid userId)
    {
        // 1. Validate conversation access
        if (!await _messageRepository.ValidateConversationAccessAsync(request.ConversationId, userId))
        {
            throw new UnauthorizedAccessException("You are not a participant of this conversation.");
        }

        // 2. Security Hardening Checks
        
        // 2a. Replay Protection
        if (await _replayProtection.IsReplayAsync(request.ConversationId, request.Nonce, request.Timestamp))
        {
            throw new InvalidOperationException("Security violation: Message replay or invalid timestamp detected.");
        }

        // 2b. Device Trust
        if (!await _deviceTrust.IsDeviceTrustedAsync(userId, request.DeviceId))
        {
            throw new UnauthorizedAccessException("Security violation: Device is not trusted for this user.");
        }

        // 2c. Integrity Check (Backend validates client-provided hash against payload)
        await _deviceTrust.ValidateMessageIntegrityAsync(request.EncryptedPayload, request.MessageHash);

        // 2d. Per-conversation rate limiting
        if (!await _rateLimit.IsAllowedAsync($"conv:{request.ConversationId}", 30, TimeSpan.FromMinutes(1)))
        {
            throw new InvalidOperationException("Rate limit exceeded for this conversation.");
        }

        // 3. Map to entity
        var message = new Message
        {
            IdMessage = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId = userId,
            SenderDevicePublicKey = request.SenderDevicePublicKey,
            EncryptedPayload = request.EncryptedPayload,
            Nonce = request.Nonce,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            IsDelivered = false,
            IsRead = false
        };

        // 4. Save
        var saved = await _messageRepository.SaveEncryptedMessageAsync(message);

        // 5. Map back to DTO
        return await MapToResponseAsync(saved, userId);
    }

    public async Task<MessageHistoryResponse> GetMessageHistoryAsync(MessageCursorPaginationRequest request, Guid userId)
    {
        if (!await _messageRepository.ValidateConversationAccessAsync(request.ConversationId, userId))
        {
            throw new UnauthorizedAccessException("Access denied.");
        }

        DateTime? beforeTimestamp = null;
        Guid? beforeId = null;

        if (!string.IsNullOrEmpty(request.Cursor))
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(request.Cursor));
            var parts = decoded.Split('|');
            if (parts.Length == 2 && DateTime.TryParse(parts[0], out var dt) && Guid.TryParse(parts[1], out var guid))
            {
                beforeTimestamp = dt;
                beforeId = guid;
            }
        }

        var (messages, nextCursor) = await _messageRepository.GetMessageHistoryAsync(request.ConversationId, beforeTimestamp, beforeId, request.Limit);

        var response = new MessageHistoryResponse
        {
            NextCursor = nextCursor,
            Messages = new List<EncryptedMessageResponse>()
        };

        foreach (var m in messages)
        {
            response.Messages.Add(await MapToResponseAsync(m, userId));
        }

        return response;
    }

    public async Task<IEnumerable<EncryptedMessageResponse>> SyncMessagesAsync(SyncMessagesRequest request, Guid userId)
    {
        if (!await _messageRepository.ValidateConversationAccessAsync(request.ConversationId, userId))
        {
            throw new UnauthorizedAccessException("Access denied.");
        }

        var messages = await _messageRepository.SyncMessagesAsync(request.ConversationId, request.Since, request.LastMessageId);
        
        var result = new List<EncryptedMessageResponse>();
        foreach (var m in messages)
        {
            result.Add(await MapToResponseAsync(m, userId));
        }
        return result;
    }

    public async Task<bool> SoftDeleteMessageAsync(Guid messageId, Guid userId)
    {
        return await _messageRepository.SoftDeleteMessageAsync(messageId, userId);
    }

    public async Task<bool> MarkDeliveredAsync(Guid messageId, Guid userId)
    {
        var message = await _messageRepository.GetMessageByIdAsync(messageId);
        if (message == null) return false;

        // Recipient is marking it delivered
        if (!await _messageRepository.ValidateConversationAccessAsync(message.ConversationId, userId))
            return false;

        return await _messageRepository.MarkMessageDeliveredAsync(messageId, DateTime.UtcNow);
    }

    public async Task<bool> MarkReadAsync(Guid messageId, Guid userId)
    {
        var message = await _messageRepository.GetMessageByIdAsync(messageId);
        if (message == null) return false;

        if (!await _messageRepository.ValidateConversationAccessAsync(message.ConversationId, userId))
            return false;

        return await _messageRepository.MarkMessageReadAsync(messageId, DateTime.UtcNow);
    }

    private async Task<EncryptedMessageResponse> MapToResponseAsync(Message message, Guid currentUserId)
    {
        var conversation = await _conversationRepository.GetConversationByIdAsync(message.ConversationId);
        var response = _mapper.Map<EncryptedMessageResponse>(message);

        // Anonymous handling: Recipient must not know sender identity
        if (conversation != null && conversation.IsAnonymous)
        {
            // If it's not the current user's own message
            if (message.SenderId != currentUserId)
            {
                // Check if sender has revealed identity
                var participant = conversation.ConversationParticipants.FirstOrDefault(p => p.UserId == message.SenderId);
                if (participant == null || !participant.IsIdentityRevealed)
                {
                    response.SenderId = null; // Hide sender ID
                }
            }
        }

        return response;
    }
}
