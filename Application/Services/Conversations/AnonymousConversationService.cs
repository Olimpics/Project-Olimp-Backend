using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OlimpBack.Application.DTO.Conversations;
using OlimpBack.Infrastructure.Repositories.Conversations;

namespace OlimpBack.Application.Services.Conversations;

public class AnonymousConversationService : IAnonymousConversationService
{
    private readonly IConversationRepository _repository;
    
    private static readonly string[] Adjectives = { "Silent", "Brave", "Clever", "Swift", "Mystic", "Golden", "Sharp", "Quiet", "Bright", "Dark" };
    private static readonly string[] Animals = { "Wolf", "Eagle", "Fox", "Lion", "Owl", "Tiger", "Bear", "Falcon", "Shark", "Panther" };

    public AnonymousConversationService(IConversationRepository repository)
    {
        _repository = repository;
    }

    public string GeneratePseudonym(Guid conversationId, Guid userId)
    {
        // Use a hash of conversationId and userId to pick adjective and animal
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes($"{conversationId}-{userId}");
        var hash = sha256.ComputeHash(bytes);
        
        int adjIndex = hash[0] % Adjectives.Length;
        int animalIndex = hash[1] % Animals.Length;
        int suffix = (hash[2] << 8 | hash[3]) % 1000;

        return $"{Adjectives[adjIndex]} {Animals[animalIndex]} #{suffix:D3}";
    }

    public async Task<bool> CanRevealIdentityAsync(Guid conversationId, Guid userId)
    {
        var participant = await _repository.GetParticipantAsync(conversationId, userId);
        return participant != null && !participant.IsIdentityRevealed;
    }

    public async Task<AnonymousIdentityDto?> GetRevealedIdentityAsync(Guid conversationId, Guid userId)
    {
        var participant = await _repository.GetParticipantAsync(conversationId, userId);
        
        if (participant == null || !participant.IsIdentityRevealed)
            return null;

        return new AnonymousIdentityDto
        {
            UserId = participant.UserId,
            Pseudonym = participant.Pseudonym ?? "Anonymous",
            RealName = participant.User.Email ?? "Unknown User" // Using email as real name for now
        };
    }
}
