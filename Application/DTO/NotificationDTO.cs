using System;
using System.Text.Json;

namespace OlimpBack.Application.DTO;

public class NotificationDto
{
    public Guid IdNotification { get; set; }
    public Guid UserId { get; set; }
    public Guid TemplateId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; }
    public DateOnly CreatedAt { get; set; }
    public string NotificationType { get; set; } = null!;
    public JsonDocument? Metadata { get; set; }
}

public class CreateNotificationDto
{
    public Guid UserId { get; set; }
    public Guid TemplateId { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string NotificationType { get; set; } = null!;
    public JsonDocument? Metadata { get; set; }
}

public class UpdateNotificationDto
{
    public Guid TemplateId { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string NotificationType { get; set; } = null!;
    public JsonDocument? Metadata { get; set; }
} 