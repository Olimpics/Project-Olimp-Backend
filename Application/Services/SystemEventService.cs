using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

public class SystemEventService : ISystemEventService
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public SystemEventService(AppDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task DispatchEventAsync(string eventName)
    {
        var systemEvent = await _context.SystemEvents
            .Include(e => e.BindRoleSystemEvents)
            .FirstOrDefaultAsync(e => e.NameSystemEvent == eventName);

        if (systemEvent == null) return;

        var roleIds = systemEvent.BindRoleSystemEvents
            .Where(b => b.RoleId.HasValue)
            .Select(b => b.RoleId.Value)
            .ToList();

        if (!roleIds.Any()) return;

        var userIds = await _context.UserRoles
            .Where(ur => roleIds.Contains(ur.RoleId))
            .Select(ur => ur.UserId)
            .Distinct()
            .ToListAsync();

        foreach (var userId in userIds)
        {
            await _notificationService.CreateNotificationAsync(new CreateNotificationDto
            {
                UserId = userId,
                Title = "System Event",
                Message = systemEvent.MessegeSystemEvent,
                NotificationType = "System",
                TemplateId = Guid.Empty // Assuming NotificationService handles Guid.Empty
            });
        }
    }
}
