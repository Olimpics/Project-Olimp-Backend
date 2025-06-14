using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.DTO;
using System;
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
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications()
        {
            var notifications = await _context.Notifications
                .Include(n => n.Template)
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

            return Ok(response);
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

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications(
            int userId,
            [FromQuery] bool includeRead = false)
        {
            var query = _context.Notifications
                .Include(n => n.Template)
                .Where(n => n.UserId == userId);

            if (!includeRead)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query.ToListAsync();

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
                Title = notification.CustomTitle?? template?.Title ?? string.Empty,
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