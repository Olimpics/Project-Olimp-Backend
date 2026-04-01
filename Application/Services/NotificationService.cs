using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using System.Text.Json;

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

        query = ApplyCommonFilters(query, queryDto); // Більше ніяких ref

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)queryDto.PageSize);

        query = ApplySorting(query, queryDto.SortOrder, forUser: false);

        // ПРОЕКЦІЯ: Тягнемо лише потрібні поля, бази даних сама зробить LEFT JOIN для Template
        var projectedData = await query
            .Skip((queryDto.Page - 1) * queryDto.PageSize)
            .Take(queryDto.PageSize)
            .Select(n => new NotificationProjection(
                n.IdNotification,
                n.UserId,
                n.TemplateId,
                n.Template != null ? n.Template.Title : null,
                n.Template != null ? n.Template.Message : null,
                n.CustomTitle,
                n.CustomMessage,
                n.IsRead,
                n.CreatedAt,
                n.NotificationType,
                n.Metadata
            ))
            .ToListAsync();

        return new PaginatedResponseDto<NotificationDto>
        {
            TotalItems = totalItems,
            TotalPages = totalPages,
            CurrentPage = queryDto.Page,
            PageSize = queryDto.PageSize,
            Items = projectedData.Select(MapProjectedToDto).ToList(), // Мапимо в пам'яті
            Filters = queryDto // Віддаємо DTO як є, без зайвих Split-ів
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
                n.IdNotification,
                n.UserId,
                n.TemplateId,
                n.Template != null ? n.Template.Title : null,
                n.Template != null ? n.Template.Message : null,
                n.CustomTitle,
                n.CustomMessage,
                n.IsRead,
                n.CreatedAt,
                n.NotificationType,
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
                n.IdNotification,
                n.UserId,
                n.TemplateId,
                n.Template != null ? n.Template.Title : null,
                n.Template != null ? n.Template.Message : null,
                n.CustomTitle,
                n.CustomMessage,
                n.IsRead,
                n.CreatedAt,
                n.NotificationType,
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
            CustomTitle = dto.Title,
            CustomMessage = dto.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            NotificationType = dto.NotificationType,
            Metadata = dto.Metadata?.ToString()
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // Оптимізація: беремо шаблон через AsNoTracking, бо він нам потрібен тільки для читання Title/Message
        var template = await _context.NotificationTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdNotificationTemplates == dto.TemplateId);

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
        // СУПЕР-ОПТИМІЗАЦІЯ: Оновлюємо без завантаження в пам'ять! (Працює на EF Core 7.0+)
        var updatedRows = await _context.Notifications
            .Where(n => n.IdNotification == id && !n.IsRead) // Оновлюємо тільки якщо воно ще не прочитане
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));

        if (updatedRows > 0)
            return (true, StatusCodes.Status204NoContent, null);

        // Якщо оновлено 0 рядків, це означає або що запису немає, або що IsRead вже було true.
        var exists = await _context.Notifications.AnyAsync(n => n.IdNotification == id);
        if (!exists)
            return (false, StatusCodes.Status404NotFound, "Notification not found");

        return (true, StatusCodes.Status204NoContent, null); // Сповіщення вже прочитане
    }

    // Змінено з ref IQueryable на повернення IQueryable (це правильний патерн для LINQ)
    private static IQueryable<Models.Notification> ApplyCommonFilters(IQueryable<Models.Notification> query, NotificationQueryDto queryDto)
    {
        if (!string.IsNullOrWhiteSpace(queryDto.Search))
        {
            var lowerSearch = queryDto.Search.Trim().ToLower();
            query = query.Where(n =>
                EF.Functions.Like(n.CustomTitle.ToLower(), $"%{lowerSearch}%") ||
                EF.Functions.Like(n.CustomMessage.ToLower(), $"%{lowerSearch}%") ||
                n.Template != null &&
                 (EF.Functions.Like(n.Template.Title.ToLower(), $"%{lowerSearch}%") ||
                  EF.Functions.Like(n.Template.Message.ToLower(), $"%{lowerSearch}%")));
        }

        if (queryDto.DateFrom.HasValue)
            query = query.Where(n => n.CreatedAt >= queryDto.DateFrom.Value);

        if (queryDto.DateTo.HasValue)
            query = query.Where(n => n.CreatedAt <= queryDto.DateTo.Value);

        if (queryDto.NotificationTypes != null && queryDto.NotificationTypes.Any())
        {
            // Зверни увагу: якщо тип зберігається в самому Notification, краще фільтрувати по n.NotificationType, 
            // щоб уникнути JOIN таблиці Template. Але я залишив твою логіку.
            query = query.Where(n => n.Template != null && queryDto.NotificationTypes.Contains(n.Template.NotificationType));
        }

        if (queryDto.IsRead.HasValue)
            query = query.Where(n => n.IsRead == queryDto.IsRead.Value);

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

    // Спеціальний record для легкої вибірки з БД без тягання важких сутностей
    private record NotificationProjection(
        int IdNotification,
        int UserId,
        int? TemplateId,
        string? TemplateTitle,
        string? TemplateMessage,
        string? CustomTitle,
        string? CustomMessage,
        bool IsRead,
        DateTime CreatedAt,
        string NotificationType,
        string? Metadata
    );

    // Мапер з нашої легкої проекції в DTO
    private static NotificationDto MapProjectedToDto(NotificationProjection p) =>
        new()
        {
            IdNotification = p.IdNotification,
            UserId = p.UserId,
            TemplateId = p.TemplateId ?? 0,
            Title = p.CustomTitle ?? p.TemplateTitle ?? string.Empty,
            Message = p.CustomMessage ?? p.TemplateMessage ?? string.Empty,
            IsRead = p.IsRead,
            CreatedAt = p.CreatedAt,
            NotificationType = p.NotificationType,
            Metadata = !string.IsNullOrWhiteSpace(p.Metadata) ? JsonDocument.Parse(p.Metadata) : null
        };
}