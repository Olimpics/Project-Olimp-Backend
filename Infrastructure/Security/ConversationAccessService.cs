using System;
using System.Threading.Tasks;
using OlimpBack.Infrastructure.Repositories.Conversations;

namespace OlimpBack.Infrastructure.Security;

public class ConversationAccessService : IConversationAccessService
{
    private readonly IConversationRepository _conversationRepository;

    public ConversationAccessService(IConversationRepository conversationRepository)
    {
        _conversationRepository = conversationRepository;
    }

    public async Task<bool> CanUserAccessConversationAsync(Guid userId, Guid conversationId)
    {
        return await _conversationRepository.ValidateParticipantAsync(conversationId, userId);
    }
}
