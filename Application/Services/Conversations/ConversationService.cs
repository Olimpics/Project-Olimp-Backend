using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.Application.DTO.Conversations;
using OlimpBack.Infrastructure.Repositories.Conversations;
using OlimpBack.Models;

namespace OlimpBack.Application.Services.Conversations;

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _repository;
    private readonly IAnonymousConversationService _anonymousService;
    private readonly IMapper _mapper;

    public ConversationService(
        IConversationRepository repository,
        IAnonymousConversationService anonymousService,
        IMapper mapper)
    {
        _repository = repository;
        _anonymousService = anonymousService;
        _mapper = mapper;
    }

    public async Task<ConversationResponse> CreateConversationAsync(CreateConversationRequest request, Guid currentUserId)
    {
        var conversationId = Guid.NewGuid();
        var conversation = new Conversation
        {
            IdConversation = conversationId,
            ConversationToken = Guid.NewGuid(),
            IsAnonymous = request.IsAnonymous,
            CreatedAt = DateTime.UtcNow
        };

        // Ensure current user is a participant
        if (!request.ParticipantIds.Contains(currentUserId))
        {
            request.ParticipantIds.Add(currentUserId);
        }

        foreach (var userId in request.ParticipantIds)
        {
            var participant = new ConversationParticipant
            {
                IdConversationParticipants = Guid.NewGuid(),
                ConversationId = conversationId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsIdentityRevealed = !request.IsAnonymous, // revealed by default if not anonymous
                EncryptedParticipant = Array.Empty<byte>() // Stub for encryption
            };

            if (request.IsAnonymous)
            {
                participant.Pseudonym = _anonymousService.GeneratePseudonym(conversationId, userId);
            }

            conversation.ConversationParticipants.Add(participant);
        }

        var created = await _repository.CreateConversationAsync(conversation);
        return MapToResponse(created, currentUserId);
    }

    public async Task<ConversationResponse?> GetConversationByTokenAsync(Guid token, Guid currentUserId)
    {
        var conversation = await _repository.GetConversationByTokenAsync(token);
        if (conversation == null) return null;

        // Verify participant
        if (!conversation.ConversationParticipants.Any(p => p.UserId == currentUserId))
            return null;

        return MapToResponse(conversation, currentUserId);
    }

    public async Task<IEnumerable<ConversationResponse>> GetUserConversationsAsync(Guid userId)
    {
        var conversations = await _repository.GetUserConversationsAsync(userId);
        return conversations.Select(c => MapToResponse(c, userId));
    }

    public async Task<bool> RevealIdentityAsync(Guid conversationId, Guid userId)
    {
        return await _repository.RevealIdentityAsync(conversationId, userId);
    }

    private ConversationResponse MapToResponse(Conversation conversation, Guid currentUserId)
    {
        var response = new ConversationResponse
        {
            IdConversation = conversation.IdConversation,
            ConversationToken = conversation.ConversationToken,
            IsAnonymous = conversation.IsAnonymous,
            CreatedAt = conversation.CreatedAt
        };

        foreach (var p in conversation.ConversationParticipants)
        {
            var dto = new ConversationParticipantDto
            {
                IdParticipant = p.IdConversationParticipants,
                IsIdentityRevealed = p.IsIdentityRevealed,
                Pseudonym = p.Pseudonym
            };

            // If revealed, or not anonymous, or it's the current user themselves
            if (p.IsIdentityRevealed || !conversation.IsAnonymous || p.UserId == currentUserId)
            {
                dto.UserId = p.UserId;
                dto.DisplayName = p.User?.Email ?? "User"; // Using email as display name
            }
            else
            {
                dto.DisplayName = p.Pseudonym ?? "Anonymous";
            }

            response.Participants.Add(dto);
        }

        return response;
    }
}
