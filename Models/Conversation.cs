using System;
using System.Collections;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Conversation
{
    public Guid IdConversation { get; set; }

    public Guid ConversationToken { get; set; }

    public bool IsAnonymous { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ConversationParticipant> ConversationParticipants { get; set; } = new List<ConversationParticipant>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
