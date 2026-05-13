using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO.Encryption;
using OlimpBack.Application.Services.Encryption;

namespace OlimpBack.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EncryptionController : ControllerBase
{
    private readonly IEncryptionSessionService _encryptionService;
    private readonly ILogger<EncryptionController> _logger;

    public EncryptionController(
        IEncryptionSessionService encryptionService,
        ILogger<EncryptionController> logger)
    {
        _encryptionService = encryptionService;
        _logger = logger;
    }

    [HttpPost("devices")]
    public async Task<ActionResult<DeviceKeyResponse>> RegisterDevice(DeviceKeyUploadRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            var result = await _encryptionService.RegisterDeviceKeysAsync(request, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering device keys");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("devices")]
    public async Task<ActionResult<IEnumerable<DeviceKeyResponse>>> GetMyDevices()
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var result = await _encryptionService.GetUserDevicesAsync(userId);
        return Ok(result);
    }

    [HttpGet("bundle/{userId}")]
    public async Task<ActionResult<KeyBundleResponse>> GetKeyBundle(Guid userId, [FromQuery] Guid? deviceId)
    {
        var result = await _encryptionService.GetRecipientKeyBundleAsync(userId, deviceId);
        if (result == null) return NotFound("User or device not found, or no keys available.");

        return Ok(result);
    }

    [HttpPost("prekeys/{deviceId}")]
    public async Task<IActionResult> UploadPreKeys(Guid deviceId, UploadPreKeysRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        try
        {
            await _encryptionService.UploadOneTimePreKeysAsync(deviceId, request, userId);
            return Ok(new { message = "PreKeys uploaded successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost("rotate/{deviceId}")]
    public async Task<IActionResult> RotateSignedPreKey(Guid deviceId, KeyRotationRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == Guid.Empty) return Unauthorized();

        var success = await _encryptionService.RotateSignedPreKeyAsync(deviceId, request, userId);
        if (!success) return BadRequest("Could not rotate signed prekey.");

        return Ok(new { message = "Signed PreKey rotated successfully" });
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
