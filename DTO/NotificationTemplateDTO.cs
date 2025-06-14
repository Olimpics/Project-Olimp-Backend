using System.Collections.Generic;

namespace OlimpBack.DTO
{
    public class NotificationTemplateDto
    {
        public int IdNotificationTemplates { get; set; }
        public string NotificationType { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Message { get; set; }
    }

    public class CreateNotificationTemplateDto
    {
        public string NotificationType { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Message { get; set; }
    }

    public class UpdateNotificationTemplateDto
    {
        public string NotificationType { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Message { get; set; }
    }
} 