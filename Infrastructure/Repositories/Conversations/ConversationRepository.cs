using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Repositories.Conversations;

public class ConversationRepository : IConversationRepository
{
    private readonly AppDbContext _context;

    public ConversationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Conversation> CreateConversationAsync(Conversation conversation)
    {
        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();
        return conversation;
    }

    public async Task<Conversation?> GetConversationByIdAsync(Guid conversationId)
    {
        return await _context.Conversations
            .Include(c => c.ConversationParticipants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(c => c.IdConversation == conversationId);
    }

    public async Task<Conversation?> GetConversationByTokenAsync(Guid token)
    {
        return await _context.Conversations
            .Include(c => c.ConversationParticipants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(c => c.ConversationToken == token);
    }

    public async Task<IEnumerable<Conversation>> GetUserConversationsAsync(Guid userId)
    {
        return await _context.Conversations
            .Include(c => c.ConversationParticipants)
                .ThenInclude(p => p.User)
            .Where(c => c.ConversationParticipants.Any(p => p.UserId == userId))
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ValidateParticipantAsync(Guid conversationId, Guid userId)
    {
        return await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);
    }

    public async Task<bool> RevealIdentityAsync(Guid conversationId, Guid userId)
    {
        var participant = await _context.ConversationParticipants
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);

        if (participant == null) return false;

        participant.IsIdentityRevealed = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ConversationParticipant?> GetParticipantAsync(Guid conversationId, Guid userId)
    {
        return await _context.ConversationParticipants
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.ConversationId == conversationId && p.UserId == userId);
    }
}
