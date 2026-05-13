using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO.Messages;
using OlimpBack.Application.Services.Messages;

namespace OlimpBack.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<MessageController> _logger;

    public MessageController(
        IMessageService messageService,
        ILogger<MessageController> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<EncryptedMessageResponse>> SendMessage(SendEncryptedMessageRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var result = await _messageService.SendMessageAsync(request, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("history")]
    public async Task<ActionResult<MessageHistoryResponse>> GetHistory([FromQuery] MessageCursorPaginationRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var result = await _messageService.GetMessageHistoryAsync(request, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("sync")]
    public async Task<ActionResult<IEnumerable<EncryptedMessageResponse>>> Sync([FromQuery] SyncMessagesRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var result = await _messageService.SyncMessagesAsync(request, userId);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var success = await _messageService.SoftDeleteMessageAsync(id, userId);
        if (!success) return BadRequest("Could not delete message. Either not the sender or message not found.");

        return Ok(new { message = "Message deleted successfully" });
    }

    [HttpPost("{id}/delivered")]
    public async Task<IActionResult> MarkDelivered(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var success = await _messageService.MarkDeliveredAsync(id, userId);
        return success ? Ok() : BadRequest();
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkRead(Guid id)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var success = await _messageService.MarkReadAsync(id, userId);
        return success ? Ok() : BadRequest();
    }

    private Guid GetCurrentUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdStr, out var userId))
        {
            return userId;
        }
        return Guid.Empty;
    }
}
