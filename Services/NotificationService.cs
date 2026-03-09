using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using System.Text.Json;

namespace OlimpBack.Services;

    public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponseDto<NotificationDto>> GetNotificationsAsync(
        NotificationQueryDto queryDto)
    {
        var query = _context.Notifications
            .Include(n => n.Template)
            .AsQueryable();

        ApplyCommonFilters(ref query, queryDto);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);

        query = ApplySorting(query, queryDto.SortOrder, forUser: false);

        var notifications = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToListAsync();

        var items = notifications.Select(MapToDto).ToList();

        return new PaginatedResponseDto<NotificationDto>
        {
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items,
            Filters = new
            {
                queryDto.Search,
                queryDto.DateFrom,
                queryDto.DateTo,
                notificationTypes = queryDto.NotificationTypes?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(t => t.Trim()).ToList(),
                queryDto.IsRead
            }
        };
    }

    public async Task<PaginatedResponseDto<NotificationDto>> GetUserNotificationsAsync(
        int userId,
        NotificationQueryDto queryDto)
    {
        var query = _context.Notifications
            .Include(n => n.Template)
            .Where(n => n.UserId == userId)
            .AsQueryable();

        ApplyCommonFilters(ref query, queryDto);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);

        query = ApplySorting(query, queryDto.SortOrder, forUser: true);

        var notifications = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .ToListAsync();

        var items = notifications.Select(MapToDto).ToList();

        return new PaginatedResponseDto<NotificationDto>
        {
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = items,
            Filters = new
            {
                queryDto.Search,
                queryDto.DateFrom,
                queryDto.DateTo,
                notificationTypes = queryDto.NotificationTypes?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(t => t.Trim()).ToList(),
                queryDto.IsRead
            }
        };
    }

    public async Task<NotificationDto?> GetNotificationAsync(int id)
    {
        var notification = await _context.Notifications
            .Include(n => n.Template)
            .FirstOrDefaultAsync(n => n.IdNotification == id);

        return notification == null ? null : MapToDto(notification);
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
    {
        var notification = new Models.Notification
        {
            UserId = dto.UserId,
            TemplateId = dto.TemplateId,
            CustomTitle = dto.Title,
            CustomMessage = dto.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            NotificationType = dto.NotificationType,
            Metadata = dto.Metadata?.ToString()
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        var template = await _context.NotificationTemplates.FindAsync(dto.TemplateId);

        return new NotificationDto
        {
            IdNotification = notification.IdNotification,
            UserId = notification.UserId,
            TemplateId = notification.TemplateId ?? 0,
            Title = notification.CustomTitle ?? template?.Title ?? string.Empty,
            Message = notification.CustomMessage ?? template?.Message ?? string.Empty,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            NotificationType = notification.NotificationType,
            Metadata = dto.Metadata
        };
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> MarkAsReadAsync(int id)
    {
        var notification = await _context.Notifications.FindAsync(id);
        if (notification == null)
            return (false, StatusCodes.Status404NotFound, "Notification not found");

        if (notification.IsRead)
            return (true, StatusCodes.Status204NoContent, null);

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return (true, StatusCodes.Status204NoContent, null);
    }

    private static void ApplyCommonFilters(
        ref IQueryable<Models.Notification> query,
        NotificationQueryDto queryDto)
    {
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(n =>
                EF.Functions.Like(n.CustomTitle.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(n.CustomMessage.ToLower(), $"%{lowerSearch}%") ||
                (n.Template != null &&
                 (EF.Functions.Like(n.Template.Title.ToLower(), $"%{lowerSearch}%") ||
                  EF.Functions.Like(n.Template.Message.ToLower(), $"%{lowerSearch}%"))));
        }

        if (queryDto.DateFrom.HasValue)
        {
            query = query.Where(n => n.CreatedAt >= queryDto.DateFrom.Value);
        }

        if (queryDto.DateTo.HasValue)
        {
            query = query.Where(n => n.CreatedAt <= queryDto.DateTo.Value);
        }

        if (!string.IsNullOrWhiteSpace(queryDto.NotificationTypes))
        {
            var types = queryDto.NotificationTypes
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
            query = query.Where(n => n.Template != null && types.Contains(n.Template.NotificationType));
        }

        if (queryDto.IsRead.HasValue)
        {
            query = query.Where(n => n.IsRead == queryDto.IsRead.Value);
        }
    }

    private static IQueryable<Models.Notification> ApplySorting(
        IQueryable<Models.Notification> query,
        int sortOrder,
        bool forUser)
    {
        if (forUser)
        {
            return sortOrder switch
            {
                1 => query.OrderByDescending(n => n.CreatedAt),
                2 => query.OrderBy(n => n.CreatedAt),
                3 => query.OrderByDescending(n => n.IsRead),
                4 => query.OrderBy(n => n.IsRead),
                _ => query.OrderByDescending(n => n.CreatedAt)
            };
        }

        return sortOrder switch
        {
            1 => query.OrderByDescending(n => n.CreatedAt),
            2 => query.OrderBy(n => n.CreatedAt),
            _ => query.OrderByDescending(n => n.CreatedAt)
        };
    }

    private static NotificationDto MapToDto(Models.Notification n) =>
        new()
        {
            IdNotification = n.IdNotification,
            UserId = n.UserId,
            TemplateId = n.TemplateId ?? 0,
            Title = n.CustomTitle ?? n.Template?.Title ?? string.Empty,
            Message = n.CustomMessage ?? n.Template?.Message ?? string.Empty,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt,
            NotificationType = n.NotificationType,
            Metadata = n.Metadata != null ? JsonDocument.Parse(n.Metadata) : null
        };
}

