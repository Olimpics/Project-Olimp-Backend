using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;
using OlimpBack.Application.Permissions;

namespace OlimpBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpPost("generate")]
    // [RequirePermission(RbacPermissions.StudentsRead)] // Need to check appropriate permission
    public async Task<IActionResult> GenerateRating([FromBody] GenerateRatingQueryDto query)
    {
        try
        {
            await _ratingService.GenerateRatingAsync(query);
            return Ok();
        }
        catch (Exception ex)
        {
            // Log error here
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
