using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Repositories.Conversations;

public interface IConversationRepository
{
    Task<Conversation> CreateConversationAsync(Conversation conversation);
    Task<Conversation?> GetConversationByIdAsync(Guid conversationId);
    Task<Conversation?> GetConversationByTokenAsync(Guid token);
    Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId);
    Task<bool> ValidateParticipantAsync(Guid conversationId, Guid userId);
    Task<bool> RevealIdentityAsync(Guid conversationId, Guid userId);
    Task<ConversationParticipant?> GetParticipantAsync(Guid conversationId, Guid userId);
}
