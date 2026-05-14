using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DeviceTransferController : ControllerBase
{
    private readonly IDeviceTransferService _service;

    public DeviceTransferController(IDeviceTransferService service)
    {
        _service = service;
    }

    [HttpPost("session")]
    public async Task<IActionResult> CreateSession([FromBody] CreateTransferSessionRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var (response, error) = await _service.CreateTransferSessionAsync(userId, request);
        
        if (error != null)
            return BadRequest(new { Message = error });

        return Ok(response);
    }

    [HttpGet("validate/{code}")]
    public async Task<IActionResult> ValidateCode(string code)
    {
        var (valid, error) = await _service.ValidateTransferCodeAsync(code);
        
        if (!valid)
            return BadRequest(new { Message = error });

        return Ok(new { Valid = true });
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinSession([FromBody] JoinTransferSessionRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var (response, error) = await _service.JoinTransferSessionAsync(userId, request);
        
        if (error != null)
            return BadRequest(new { Message = error });

        return Ok(response);
    }

    [HttpPost("payload")]
    public async Task<IActionResult> UploadPayload([FromBody] UploadTransferPayloadRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var (success, error) = await _service.UploadEncryptedPayloadAsync(userId, request);
        
        if (!success)
            return BadRequest(new { Message = error });

        return Ok(new { Success = true });
    }

    [HttpGet("payload/{token}")]
    public async Task<IActionResult> DownloadPayload(Guid token)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var (response, error) = await _service.DownloadEncryptedPayloadAsync(userId, token);
        
        if (error != null)
            return BadRequest(new { Message = error });

        return Ok(response);
    }

    [HttpPost("complete/{token}")]
    public async Task<IActionResult> CompleteTransfer(Guid token)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var (success, error) = await _service.CompleteTransferAsync(userId, token);
        
        if (!success)
            return BadRequest(new { Message = error });

        return Ok(new { Success = true });
    }

    [HttpGet("status/{token}")]
    public async Task<IActionResult> GetStatus(Guid token)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var (response, error) = await _service.GetTransferStatusAsync(userId, token);
        
        if (error != null)
            return BadRequest(new { Message = error });

        return Ok(response);
    }
}
