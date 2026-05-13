using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class ConversationParticipant
{
    public Guid IdConversationParticipants { get; set; }

    public Guid ConversationId { get; set; }

    public byte[] EncryptedParticipant { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public Guid UserId { get; set; }

    public string? Pseudonym { get; set; }

    public bool IsIdentityRevealed { get; set; }

    public virtual Conversation Conversation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
