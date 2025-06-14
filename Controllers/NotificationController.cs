using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.DTO;
using System;
using System.Linq;
using System.Text.Json;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public NotificationController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] string? notificationTypes = null,
            [FromQuery] bool? isRead = null,
            [FromQuery] int sortOrder = 0)
        {
            var query = _context.Notifications
                .Include(n => n.Template)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(n =>
                    EF.Functions.Like(n.CustomTitle.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(n.CustomMessage.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(n.Template.Title.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(n.Template.Message.ToLower(), $"%{lowerSearch}%"));
            }

            // Apply date range filter
            if (dateFrom.HasValue)
            {
                query = query.Where(n => n.CreatedAt >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                query = query.Where(n => n.CreatedAt <= dateTo.Value);
            }

            // Apply notification type filter
            if (!string.IsNullOrWhiteSpace(notificationTypes))
            {
                var types = notificationTypes.Split(',').Select(t => t.Trim()).ToList();
                query = query.Where(n => n.Template != null && types.Contains(n.Template.NotificationType));
            }

            // Apply read status filter
            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            // Get total count for pagination
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Apply sorting
            query = sortOrder switch
            {
                1 => query.OrderByDescending(n => n.CreatedAt), // Newest first
                2 => query.OrderBy(n => n.CreatedAt), // Oldest first
                _ => query.OrderByDescending(n => n.CreatedAt) // Default: newest first
            };

            // Apply pagination
            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = notifications.Select(n => new NotificationDto
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
            });

            return Ok(new
            {
                totalPages,
                totalItems,
                currentPage = page,
                pageSize,
                notifications = response,
                filters = new
                {
                    search,
                    dateFrom,
                    dateTo,
                    notificationTypes = notificationTypes?.Split(',').Select(t => t.Trim()).ToList(),
                    isRead
                }
            });
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<object>> GetUserNotifications(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] string? notificationTypes = null,
            [FromQuery] bool? isRead = null,
            [FromQuery] int sortOrder = 0)
        {
            var query = _context.Notifications
                .Include(n => n.Template)
                .Where(n => n.UserId == userId);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(n =>
                    EF.Functions.Like(n.CustomTitle.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(n.CustomMessage.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(n.Template.Title.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(n.Template.Message.ToLower(), $"%{lowerSearch}%"));
            }

            // Apply date range filter
            if (dateFrom.HasValue)
            {
                query = query.Where(n => n.CreatedAt >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                query = query.Where(n => n.CreatedAt <= dateTo.Value);
            }

            // Apply notification type filter
            if (!string.IsNullOrWhiteSpace(notificationTypes))
            {
                var types = notificationTypes.Split(',').Select(t => t.Trim()).ToList();
                query = query.Where(n => n.Template != null && types.Contains(n.Template.NotificationType));
            }

            // Apply read status filter
            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            // Get total count for pagination
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Apply sorting
            query = sortOrder switch
            {
                1 => query.OrderByDescending(n => n.CreatedAt), // Newest first
                2 => query.OrderBy(n => n.CreatedAt), // Oldest first
                3 => query.OrderByDescending(n => n.IsRead), // Unread first
                4 => query.OrderBy(n => n.IsRead), // Read first
                _ => query.OrderByDescending(n => n.CreatedAt) // Default: newest first
            };

            // Apply pagination
            var notifications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var response = notifications.Select(n => new NotificationDto
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
            });

            return Ok(new
            {
                totalPages,
                totalItems,
                currentPage = page,
                pageSize,
                notifications = response,
                filters = new
                {
                    search,
                    dateFrom,
                    dateTo,
                    notificationTypes = notificationTypes?.Split(',').Select(t => t.Trim()).ToList(),
                    isRead
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetNotification(int id)
        {
            var notification = await _context.Notifications
                .Include(n => n.Template)
                .FirstOrDefaultAsync(n => n.IdNotification == id);

            if (notification == null)
                return NotFound();

            var response = new NotificationDto
            {
                IdNotification = notification.IdNotification,
                UserId = notification.UserId,
                TemplateId = notification.TemplateId ?? 0,
                Title = notification.CustomTitle ?? notification.Template?.Title ?? string.Empty,
                Message = notification.CustomMessage ?? notification.Template?.Message ?? string.Empty,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                NotificationType = notification.NotificationType,
                Metadata = notification.Metadata != null ? JsonDocument.Parse(notification.Metadata) : null
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationDto dto)
        {
            var notification = new Notification
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
            var response = new NotificationDto
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

            return CreatedAtAction(nameof(GetNotification), new { id = notification.IdNotification }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, UpdateNotificationDto dto)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            notification.TemplateId = dto.TemplateId;
            notification.CustomTitle = dto.Title;
            notification.CustomMessage = dto.Message;
            notification.NotificationType = dto.NotificationType;
            notification.Metadata = dto.Metadata?.ToString();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!NotificationExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(n => n.IdNotification == id);
        }
    }
} 