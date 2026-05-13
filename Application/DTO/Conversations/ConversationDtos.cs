using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO.Conversations;

public class CreateConversationRequest
{
    public List<Guid> ParticipantIds { get; set; } = new();
    public bool IsAnonymous { get; set; } = false;
}

public class CreateAnonymousConversationRequest
{
    public List<Guid> ParticipantIds { get; set; } = new();
}

public class ConversationResponse
{
    public Guid IdConversation { get; set; }
    public Guid ConversationToken { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ConversationParticipantDto> Participants { get; set; } = new();
}

public class ConversationParticipantDto
{
    public Guid IdParticipant { get; set; }
    public Guid? UserId { get; set; }
    public string? Pseudonym { get; set; }
    public bool IsIdentityRevealed { get; set; }
    public string? DisplayName { get; set; } // Real name if revealed or not anonymous
}

public class AnonymousIdentityDto
{
    public Guid UserId { get; set; }
    public string Pseudonym { get; set; }
    public string RealName { get; set; }
}

public class RevealIdentityRequest
{
    public Guid ConversationId { get; set; }
}
