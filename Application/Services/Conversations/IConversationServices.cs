using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO.Conversations;

namespace OlimpBack.Application.Services.Conversations;

public interface IConversationService
{
    Task<ConversationResponse> CreateConversationAsync(CreateConversationRequest request, Guid currentUserId);
    Task<ConversationResponse?> GetConversationByTokenAsync(Guid token, Guid currentUserId);
    Task<IEnumerable<ConversationResponse>> GetUserConversationsAsync(Guid userId);
    Task<bool> RevealIdentityAsync(Guid conversationId, Guid userId);
}

public interface IAnonymousConversationService
{
    string GeneratePseudonym(Guid conversationId, Guid userId);
    Task<bool> CanRevealIdentityAsync(Guid conversationId, Guid userId);
    Task<AnonymousIdentityDto?> GetRevealedIdentityAsync(Guid conversationId, Guid userId);
}
