using System;
using System.Text.Json;

namespace OlimpBack.DTO;

public class NotificationDto
{
    public int IdNotification { get; set; }
    public int UserId { get; set; }
    public int TemplateId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public string NotificationType { get; set; } = null!;
    public JsonDocument? Metadata { get; set; }
}

public class CreateNotificationDto
{
    public int UserId { get; set; }
    public int TemplateId { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string NotificationType { get; set; } = null!;
    public JsonDocument? Metadata { get; set; }
}

public class UpdateNotificationDto
{
    public int TemplateId { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string NotificationType { get; set; } = null!;
    public JsonDocument? Metadata { get; set; }
} 