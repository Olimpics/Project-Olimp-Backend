using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Message
{
    public Guid IdMessage { get; set; }

    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }

    public string SenderDevicePublicKey { get; set; } = null!;

    public byte[] EncryptedPayload { get; set; } = null!;

    public string Nonce { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public bool IsDelivered { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;
}
