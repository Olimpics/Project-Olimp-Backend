using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Message
{
    public Guid IdMessage { get; set; }

    public Guid ConversationTokenId { get; set; }

    public string SenderDevicePublicKey { get; set; } = null!;

    public byte[] EncryptedPayload { get; set; } = null!;

    public string Nonce { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
