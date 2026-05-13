using System;

namespace OlimpBack.Application.DTO.Realtime;

public class RealtimeEncryptedMessageDto
{
    public Guid MessageId { get; set; }
    public Guid ConversationId { get; set; }
    public byte[] EncryptedPayload { get; set; } = null!;
    public string SenderDevicePublicKey { get; set; } = null!;
    public string Nonce { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public Guid? SenderId { get; set; } // Null for anonymous
}

public class DeliveryAckDto
{
    public Guid MessageId { get; set; }
    public Guid ConversationId { get; set; }
    public DateTime DeliveredAt { get; set; }
}

public class ReadAckDto
{
    public Guid MessageId { get; set; }
    public Guid ConversationId { get; set; }
    public DateTime ReadAt { get; set; }
}

public class ConnectionEventDto
{
    public Guid? UserId { get; set; }
    public Guid DeviceId { get; set; }
    public string ConnectionId { get; set; } = null!;
    public string Status { get; set; } = null!; // "Connected" or "Disconnected"
}
