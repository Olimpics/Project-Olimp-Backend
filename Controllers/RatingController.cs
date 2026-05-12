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
    [RequirePermission(RbacPermissions.StudentsRead)]
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

    [HttpGet("status")]
    [RequirePermission(RbacPermissions.StudentsRead)]
    public async Task<ActionResult<RatingStatusResponseDto>> GetRatingStatus([FromQuery] RatingStatusQueryDto query)
    {
        try
        {
            var result = await _ratingService.GetRatingStatusAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("list")]
    [RequirePermission(RbacPermissions.StudentsRead)]
    public async Task<ActionResult<PaginatedResponseDto<RatingStudentDto>>> GetRatingList([FromQuery] RatingListQueryDto query)
    {
        try
        {
            var result = await _ratingService.GetPaginatedRatingsAsync(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
