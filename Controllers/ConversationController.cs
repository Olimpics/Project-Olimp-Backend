using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO.Conversations;
using OlimpBack.Application.Services.Conversations;

namespace OlimpBack.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationService;
    private readonly ILogger<ConversationController> _logger;

    public ConversationController(
        IConversationService conversationService,
        ILogger<ConversationController> logger)
    {
        _conversationService = conversationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ConversationResponse>> CreateConversation(CreateConversationRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var result = await _conversationService.CreateConversationAsync(request, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conversation");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("token/{token}")]
    public async Task<ActionResult<ConversationResponse>> GetConversationByToken(Guid token)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _conversationService.GetConversationByTokenAsync(token, userId);
        if (result == null) return NotFound("Conversation not found or access denied");

        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<ConversationResponse>>> GetMyConversations()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _conversationService.GetUserConversationsAsync(userId);
        return Ok(result);
    }

    [HttpPost("reveal")]
    public async Task<IActionResult> RevealIdentity(RevealIdentityRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var success = await _conversationService.RevealIdentityAsync(request.ConversationId, userId);
        if (!success) return BadRequest("Could not reveal identity. Either not a participant or already revealed.");

        return Ok(new { message = "Identity revealed successfully" });
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
