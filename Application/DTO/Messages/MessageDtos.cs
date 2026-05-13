using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO.Messages;

public class SendEncryptedMessageRequest
{
    public Guid ConversationId { get; set; }
    public Guid DeviceId { get; set; }
    public string SenderDevicePublicKey { get; set; } = null!;
    public byte[] EncryptedPayload { get; set; } = null!;
    public string Nonce { get; set; } = null!;
    public string MessageHash { get; set; } = null!; // Client-side SHA256 of payload
    public long Timestamp { get; set; } // Unix timestamp in milliseconds
}

public class EncryptedMessageResponse
{
    public Guid IdMessage { get; set; }
    public Guid ConversationId { get; set; }
    public Guid? SenderId { get; set; } // Null if anonymous and not current user
    public string SenderDevicePublicKey { get; set; } = null!;
    public byte[] EncryptedPayload { get; set; } = null!;
    public string Nonce { get; set; } = null!;
    public bool IsDelivered { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class MessageHistoryResponse
{
    public List<EncryptedMessageResponse> Messages { get; set; } = new();
    public string? NextCursor { get; set; }
}

public class MessageCursorPaginationRequest
{
    public Guid ConversationId { get; set; }
    public string? Cursor { get; set; } // Base64 encoded createdAt|idMessage
    public int Limit { get; set; } = 50;
}

public class SyncMessagesRequest
{
    public Guid ConversationId { get; set; }
    public DateTime Since { get; set; }
    public Guid? LastMessageId { get; set; }
}

public class MessageDeliveryDto
{
    public Guid MessageId { get; set; }
    public DateTime DeliveredAt { get; set; }
}

public class MessageReadDto
{
    public Guid MessageId { get; set; }
    public DateTime ReadAt { get; set; }
}

public class RealtimeEncryptedMessageDto : EncryptedMessageResponse
{
    // Additional fields for realtime if needed
}
