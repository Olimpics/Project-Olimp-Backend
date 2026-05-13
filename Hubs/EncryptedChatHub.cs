using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OlimpBack.Application.DTO.Messages;
using OlimpBack.Application.DTO.Realtime;
using OlimpBack.Application.Services.Conversations;
using OlimpBack.Application.Services.Messages;
using OlimpBack.Infrastructure.Realtime;

namespace OlimpBack.Hubs;

[Authorize]
public class EncryptedChatHub : Hub<IEncryptedChatClient>
{
    private readonly ISignalRConnectionManager _connectionManager;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;
    private readonly ILogger<EncryptedChatHub> _logger;

    public EncryptedChatHub(
        ISignalRConnectionManager connectionManager,
        IConversationService conversationService,
        IMessageService messageService,
        ILogger<EncryptedChatHub> logger)
    {
        _connectionManager = connectionManager;
        _conversationService = conversationService;
        _messageService = messageService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        if (userId != Guid.Empty)
        {
            await _connectionManager.AddConnectionAsync(userId, Context.ConnectionId);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _connectionManager.RemoveConnectionAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task RegisterDeviceConnection(Guid deviceId)
    {
        // In a real scenario, we might want to validate that this device belongs to the user
        await _connectionManager.BindDeviceToConnectionAsync(Context.ConnectionId, deviceId);
    }

    public async Task ConnectToConversation(Guid conversationId)
    {
        var userId = GetCurrentUserId();
        // Validate membership
        var conversations = await _conversationService.GetUserConversationsAsync(userId);
        if (conversations.Any(c => c.IdConversation == conversationId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }
        else
        {
            _logger.LogWarning("User {UserId} tried to join conversation {ConversationId} without access", userId, conversationId);
        }
    }

    public async Task DisconnectFromConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    public async Task SendEncryptedMessage(SendEncryptedMessageRequest request)
    {
        var userId = GetCurrentUserId();
        try
        {
            // 1. Save to DB first (blind relay)
            var response = await _messageService.SendMessageAsync(request, userId);

            // 2. Broadcast to conversation group
            var realtimeMsg = new RealtimeEncryptedMessageDto
            {
                MessageId = response.IdMessage,
                ConversationId = response.ConversationId,
                EncryptedPayload = response.EncryptedPayload,
                SenderDevicePublicKey = response.SenderDevicePublicKey,
                Nonce = response.Nonce,
                CreatedAt = response.CreatedAt,
                SenderId = response.SenderId
            };

            await Clients.Group(request.ConversationId.ToString()).ReceiveEncryptedMessage(realtimeMsg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending realtime message");
            throw new HubException("Failed to send message.");
        }
    }

    public async Task AcknowledgeDelivered(Guid messageId, Guid conversationId)
    {
        var userId = GetCurrentUserId();
        var success = await _messageService.MarkDeliveredAsync(messageId, userId);
        if (success)
        {
            await Clients.Group(conversationId.ToString()).MessageDelivered(new DeliveryAckDto
            {
                MessageId = messageId,
                ConversationId = conversationId,
                DeliveredAt = DateTime.UtcNow
            });
        }
    }

    public async Task AcknowledgeRead(Guid messageId, Guid conversationId)
    {
        var userId = GetCurrentUserId();
        var success = await _messageService.MarkReadAsync(messageId, userId);
        if (success)
        {
            await Clients.Group(conversationId.ToString()).MessageRead(new ReadAckDto
            {
                MessageId = messageId,
                ConversationId = conversationId,
                ReadAt = DateTime.UtcNow
            });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            return userId;
        }
        return Guid.Empty;
    }
}
