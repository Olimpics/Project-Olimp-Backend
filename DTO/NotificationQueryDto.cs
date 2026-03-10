using System;
using System.Collections.Generic; // Додали using

namespace OlimpBack.DTO;

public class NotificationQueryDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Search { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    // Замість string? використовуємо типізований список
    public List<string>? NotificationTypes { get; set; }

    public bool? IsRead { get; set; }
    public int SortOrder { get; set; } = 0;
}