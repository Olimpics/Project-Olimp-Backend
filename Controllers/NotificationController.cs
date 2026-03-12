using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetNotifications(
            [FromQuery] NotificationQueryDto query)
        {
            var result = await _notificationService.GetNotificationsAsync(query);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<object>> GetUserNotifications(
            int userId,
            [FromQuery] NotificationQueryDto query)
        {
            var result = await _notificationService.GetUserNotificationsAsync(userId, query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetNotification(int id)
        {
            var notification = await _notificationService.GetNotificationAsync(id);
            if (notification == null)
                return NotFound();

            return Ok(notification);
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationDto dto)
        {
            var response = await _notificationService.CreateNotificationAsync(dto);
            return CreatedAtAction(nameof(GetNotification), new { id = response.IdNotification }, response);
        }

        [HttpPost("{id}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var (success, statusCode, errorMessage) = await _notificationService.MarkAsReadAsync(id);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }
    }
} 