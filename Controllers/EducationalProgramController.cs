using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OlimpBack.Application.Services;
using OlimpBack.Application.DTO;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationalProgramController : ControllerBase
    {
        private readonly IEducationalProgramService _educationalProgramService;

        public EducationalProgramController(IEducationalProgramService educationalProgramService)
        {
            _educationalProgramService = educationalProgramService;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetEducationalPrograms(
            [FromQuery] EducationalProgramListQueryDto query)
        {
            var result = await _educationalProgramService.GetEducationalProgramsAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EducationalProgramDto>> GetEducationalProgram(int id)
        {
            var program = await _educationalProgramService.GetEducationalProgramAsync(id);

            if (program == null)
                return NotFound("Educational program not found");

            return Ok(program);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<EducationalProgramDto>> CreateEducationalProgram(CreateEducationalProgramDto dto)
        {
            var result = await _educationalProgramService.CreateEducationalProgramAsync(dto);

            return CreatedAtAction(nameof(GetEducationalProgram), new { id = result.IdEducationalProgram }, result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducationalProgram(int id, UpdateEducationalProgramDto dto)
        {
            if (id != dto.IdEducationalProgram)
                return BadRequest("Route id does not match body id.");

            var (success, statusCode, errorMessage) =
                await _educationalProgramService.UpdateEducationalProgramAsync(id, dto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

    }
}