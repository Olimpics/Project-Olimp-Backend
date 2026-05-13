using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Infrastructure.Repositories.Messages;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Message> SaveEncryptedMessageAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<(IEnumerable<Message> Messages, string? NextCursor)> GetMessageHistoryAsync(Guid conversationId, DateTime? beforeTimestamp, Guid? beforeId, int limit)
    {
        var query = _context.Messages
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted);

        if (beforeTimestamp.HasValue && beforeId.HasValue)
        {
            query = query.Where(m => m.CreatedAt < beforeTimestamp.Value || 
                                    (m.CreatedAt == beforeTimestamp.Value && m.IdMessage.CompareTo(beforeId.Value) < 0));
        }

        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.IdMessage)
            .Take(limit + 1)
            .ToListAsync();

        string? nextCursor = null;
        if (messages.Count > limit)
        {
            var last = messages[limit - 1];
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{last.CreatedAt:O}|{last.IdMessage}"));
            messages = messages.Take(limit).ToList();
        }

        return (messages, nextCursor);
    }

    public async Task<IEnumerable<Message>> SyncMessagesAsync(Guid conversationId, DateTime since, Guid? lastMessageId)
    {
        var query = _context.Messages
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted);

        if (lastMessageId.HasValue)
        {
             query = query.Where(m => m.CreatedAt > since || 
                                     (m.CreatedAt == since && m.IdMessage.CompareTo(lastMessageId.Value) > 0));
        }
        else
        {
            query = query.Where(m => m.CreatedAt > since);
        }

        return await query
            .OrderBy(m => m.CreatedAt)
            .ThenBy(m => m.IdMessage)
            .ToListAsync();
    }

    public async Task<bool> SoftDeleteMessageAsync(Guid messageId, Guid userId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null || message.SenderId != userId) return false;

        message.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidateConversationAccessAsync(Guid conversationId, Guid userId)
    {
        return await _context.ConversationParticipants
            .AnyAsync(p => p.ConversationId == conversationId && p.UserId == userId);
    }

    public async Task<Message?> GetMessageByIdAsync(Guid messageId)
    {
        return await _context.Messages.FindAsync(messageId);
    }

    public async Task<bool> MarkMessageDeliveredAsync(Guid messageId, DateTime deliveredAt)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null || message.IsDelivered) return false;

        message.IsDelivered = true;
        message.DeliveredAt = deliveredAt;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkMessageReadAsync(Guid messageId, DateTime readAt)
    {
        var message = await _context.Messages.FindAsync(messageId);
        if (message == null || message.IsRead) return false;

        message.IsRead = true;
        message.ReadAt = readAt;
        await _context.SaveChangesAsync();
        return true;
    }
}
