using System.Globalization;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;

namespace OlimpBack.Application.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResponseDto<NotificationDto>> GetNotificationsAsync(NotificationQueryDto queryDto)
    {
        var query = _context.Notifications.AsNoTracking().AsQueryable();

        query = ApplyCommonFilters(query, queryDto); // ˜˜˜˜˜˜ ˜˜˜˜˜˜ ref

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);

        query = ApplySorting(query, queryDto.SortOrder, forUser: false);

        // ˜˜˜˜˜?˜: ˜˜˜˜˜˜˜ ˜˜˜˜ ˜˜˜˜˜˜˜ ˜˜˜˜, ˜˜˜˜ ˜˜˜˜˜ ˜˜˜˜ ˜˜˜˜˜˜˜ LEFT JOIN ˜˜˜ Template
        var projectedData = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(n => new NotificationProjection(
                n.IdNotification ?? 0,
                n.UserId ?? 0,
                n.TemplateId,
                n.Template != null ? n.Template.Title : null,
                n.Template != null ? n.Template.Message : null,
                n.CustomMessage,
                n.IsRead,
                n.CreatedAt,
                n.Template != null ? n.Template.NotificationType : "",
                n.Metadata
            ))
            .ToListAsync();

        return new PaginatedResponseDto<NotificationDto>
        {
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = projectedData.Select(MapProjectedToDto).ToList(), // ˜˜˜˜˜˜ ˜ ˜˜˜'˜˜
            Filters = queryDto // ?˜˜˜˜˜ DTO ˜˜ ˜, ˜˜˜ ˜˜˜˜˜˜ Split-˜˜
        };
    }

    public async Task<PaginatedResponseDto<NotificationDto>> GetUserNotificationsAsync(int userId, NotificationQueryDto queryDto)
    {
        var query = _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .AsQueryable();

        query = ApplyCommonFilters(query, queryDto);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);

        query = ApplySorting(query, queryDto.SortOrder, forUser: true);

        var projectedData = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(n => new NotificationProjection(
                n.IdNotification ?? 0,
                n.UserId ?? 0,
                n.TemplateId,
                n.Template != null ? n.Template.Title : null,
                n.Template != null ? n.Template.Message : null,
                n.CustomMessage,
                n.IsRead,
                n.CreatedAt,
                n.Template != null ? n.Template.NotificationType : "",
                n.Metadata
            ))
            .ToListAsync();

        return new PaginatedResponseDto<NotificationDto>
        {
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = projectedData.Select(MapProjectedToDto).ToList()
        };
    }

    public async Task<NotificationDto?> GetNotificationAsync(int id)
    {
        var projected = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.IdNotification == id)
            .Select(n => new NotificationProjection(
                n.IdNotification ?? 0,
                n.UserId ?? 0,
                n.TemplateId,
                n.Template != null ? n.Template.Title : null,
                n.Template != null ? n.Template.Message : null,
                n.CustomMessage,
                n.IsRead,
                n.CreatedAt,
                n.Template != null ? n.Template.NotificationType : "",
                n.Metadata
            ))
            .FirstOrDefaultAsync();

        return projected == null ? null : MapProjectedToDto(projected);
    }

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
    {
        var notification = new Models.Notification
        {
            UserId = dto.UserId,
            TemplateId = dto.TemplateId,
            CustomMessage = string.IsNullOrWhiteSpace(dto.Message) ? dto.Title : dto.Message,
            IsRead = 0,
            CreatedAt = DateTime.UtcNow.ToString("o"),
            Metadata = dto.Metadata?.RootElement.GetRawText()
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // ˜˜˜˜˜˜˜˜˜˜: ˜˜˜˜˜˜ ˜˜˜˜˜˜ ˜˜˜˜˜ AsNoTracking, ˜˜ ˜˜ ˜˜˜ ˜˜˜˜˜˜˜ ˜˜˜˜˜ ˜˜˜ ˜˜˜˜˜˜˜ Title/Message
        var template = await _context.NotificationTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdNotificationTemplates == dto.TemplateId);

        return new NotificationDto
        {
            IdNotification = notification.IdNotification ?? 0,
            UserId = notification.UserId ?? 0,
            TemplateId = notification.TemplateId ?? 0,
            Title = template?.Title ?? dto.Title ?? string.Empty,
            Message = notification.CustomMessage ?? template?.Message ?? string.Empty,
            IsRead = (notification.IsRead ?? 0) != 0,
            CreatedAt = DateTime.TryParse(notification.CreatedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var ca) ? ca : default,
            NotificationType = template?.NotificationType ?? dto.NotificationType,
            Metadata = dto.Metadata
        };
    }

    public async Task<(bool success, int statusCode, string? errorMessage)> MarkAsReadAsync(int id)
    {
        // ˜˜˜˜˜-˜˜˜˜?˜˜?˜: ˜˜˜˜˜˜˜˜˜ ˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜˜ ˜ ˜˜˜'˜˜˜! (˜˜˜˜˜˜ ˜˜ EF Core 7.0+)
        var updatedRows = await _context.Notifications
            .Where(n => n.IdNotification == id && (n.IsRead ?? 0) == 0) // ˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜ ˜˜˜˜ ˜˜˜˜ ˜˜ ˜˜ ˜˜˜˜˜˜˜˜˜
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, 1));

        if (updatedRows > 0)
            return (true, StatusCodes.Status204NoContent, null);

        // ˜˜˜˜ ˜˜˜˜˜˜˜˜ 0 ˜˜˜˜˜, ˜˜ ˜˜˜˜˜˜ ˜˜˜ ˜˜ ˜˜˜˜˜˜ ˜˜˜˜, ˜˜˜ ˜˜ IsRead ˜˜˜ ˜˜˜˜ true.
        var exists = await _context.Notifications.AnyAsync(n => n.IdNotification == id);
        if (!exists)
            return (false, StatusCodes.Status404NotFound, "Notification not found");

        return (true, StatusCodes.Status204NoContent, null); // ˜˜˜˜˜˜˜˜˜ ˜˜˜ ˜˜˜˜˜˜˜˜˜
    }

    // ˜˜˜˜˜˜ ˜ ref IQueryable ˜˜ ˜˜˜˜˜˜˜˜˜˜ IQueryable (˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜˜˜˜˜˜ ˜˜˜ LINQ)
    private static IQueryable<Models.Notification> ApplyCommonFilters(IQueryable<Models.Notification> query, NotificationQueryDto queryDto)
    {
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(n =>
                (n.CustomMessage != null && EF.Functions.Like(n.CustomMessage.ToLower(), $"%{lowerSearch}%")) ||
                (n.Template != null &&
                 ((n.Template.Title != null && EF.Functions.Like(n.Template.Title.ToLower(), $"%{lowerSearch}%")) ||
                  (n.Template.Message != null && EF.Functions.Like(n.Template.Message.ToLower(), $"%{lowerSearch}%")))));
        }

        if (queryDto.NotificationTypes != null && queryDto.NotificationTypes.Any())
        {
            // ˜˜˜˜˜˜ ˜˜˜˜˜: ˜˜˜˜ ˜˜˜ ˜˜˜˜˜˜˜˜˜˜ ˜ ˜˜˜˜˜˜ Notification, ˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜˜˜ ˜˜ n.NotificationType, 
            // ˜˜˜ ˜˜˜˜˜˜˜˜ JOIN ˜˜˜˜˜˜˜ Template. ˜˜˜ ˜ ˜˜˜˜˜˜˜ ˜˜˜˜ ˜˜˜˜˜.
            query = query.Where(n => n.Template != null && n.Template.NotificationType != null && queryDto.NotificationTypes.Contains(n.Template.NotificationType));
        }

        if (queryDto.IsRead.HasValue)
        {
            var wantRead = queryDto.IsRead.Value ? 1 : 0;
            query = query.Where(n => (n.IsRead ?? 0) == wantRead);
        }

        return query;
    }

    private static IQueryable<Models.Notification> ApplySorting(IQueryable<Models.Notification> query, int sortOrder, bool forUser)
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

    // ˜˜˜˜˜˜˜˜˜˜˜ record ˜˜˜ ˜˜˜˜˜ ˜˜˜˜˜˜ ˜ ˜˜ ˜˜˜ ˜˜˜˜˜˜˜ ˜˜˜˜˜˜ ˜˜˜˜˜˜˜˜˜
    private record NotificationProjection(
        int IdNotification,
        int UserId,
        int? TemplateId,
        string? TemplateTitle,
        string? TemplateMessage,
        string? CustomMessage,
        int? IsRead,
        string? CreatedAt,
        string NotificationType,
        string? Metadata
    );

    // ˜˜˜˜˜ ˜ ˜˜˜˜ ˜˜˜˜˜ ˜˜˜˜˜˜˜˜ ˜ DTO
    private static NotificationDto MapProjectedToDto(NotificationProjection p)
    {
        var createdAt = DateTime.TryParse(
            p.CreatedAt,
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out var dt)
            ? dt
            : default;

        return new NotificationDto
        {
            IdNotification = p.IdNotification,
            UserId = p.UserId,
            TemplateId = p.TemplateId ?? 0,
            Title = p.TemplateTitle ?? string.Empty,
            Message = p.TemplateMessage ?? p.CustomMessage ?? string.Empty,
            IsRead = (p.IsRead ?? 0) != 0,
            CreatedAt = createdAt,
            NotificationType = p.NotificationType ?? string.Empty,
            Metadata = !string.IsNullOrWhiteSpace(p.Metadata) ? JsonDocument.Parse(p.Metadata) : null
        };
    }
}