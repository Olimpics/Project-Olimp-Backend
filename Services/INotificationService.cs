using OlimpBack.DTO;

namespace OlimpBack.Services;

public interface INotificationService
{
    Task<PaginatedResponseDto<NotificationDto>> GetNotificationsAsync(NotificationQueryDto queryDto);

    Task<PaginatedResponseDto<NotificationDto>> GetUserNotificationsAsync(
        int userId,
        NotificationQueryDto queryDto);

    Task<NotificationDto?> GetNotificationAsync(int id);

    Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);

    Task<(bool success, int statusCode, string? errorMessage)> MarkAsReadAsync(int id);
}

