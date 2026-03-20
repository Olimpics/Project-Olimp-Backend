using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplineTabStudentController : ControllerBase
    {
        private readonly IDisciplineTabService _service;

        public DisciplineTabStudentController(IDisciplineTabService service)
        {
            _service = service;
        }

        [HttpGet("GetAllDisciplinesWithAvailability")]
        public async Task<ActionResult<object>> GetAllDisciplinesWithAvailability(
            [FromQuery] GetAllDisciplinesWithAvailabilityQueryDto query)
        {
            var result = await _service.GetAllDisciplinesWithAvailabilityAsync(query);
            if (result == null)
                return NotFound("Student not found");
            return Ok(result);
        }

        [HttpPost("AddDisciplineBind")]
        public async Task<ActionResult> AddDisciplineBind(AddDisciplineBindDto dto)
        {
            try
            {
                var (bindId, error) = await _service.AddDisciplineBindAsync(dto);
                if (error != null)
                {
                    if (error.StartsWith("Student not found"))
                        return NotFound(new { error });
                    return BadRequest(new { error });
                }
                return Ok(new
                {
                    message = "Discipline successfully bound to student",
                    bindId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while processing your request",
                    details = ex.Message
                });
            }
        }

        [HttpGet("GetDisciplineWithDetails/{id}")]
        public async Task<ActionResult<FullDisciplineWithDetailsDto>> GetDisciplineWithDetails(int id)
        {
            var result = await _service.GetDisciplineWithDetailsAsync(id);
            if (result == null)
                return NotFound("Discipline not found");
            return Ok(result);
        }

        [HttpGet("GetDisciplinesBySemester")]
        public async Task<ActionResult<DisciplineTabResponseDto>> GetDisciplinesBySemester(
           [FromQuery] GetDisciplinesBySemesterQueryDto query)
        {
            var result = await _service.GetDisciplinesBySemesterAsync(query);

            if (result == null)
                return BadRequest(new { message = "Student not found or choice period is not active" });

            return Ok(result);
        }

        [HttpPost("CreateDisciplineWithDetails")]
        public async Task<ActionResult<FullDisciplineWithDetailsDto>> CreateDisciplineWithDetails(CreateAddDisciplineWithDetailsDto dto)
        {
            var result = await _service.CreateDisciplineWithDetailsAsync(dto);
            if (result == null)
                return NotFound("Discipline details not found");
            return CreatedAtAction(nameof(GetDisciplineWithDetails), new { id = result.IdAddDisciplines }, result);
        }

        [HttpPut("UpdateDisciplineWithDetails/{id}")]
        public async Task<IActionResult> UpdateDisciplineWithDetails(int id, UpdateAddDisciplineWithDetailsDto dto)
        {
            if (id != dto.IdAddDisciplines)
                return BadRequest();

            var (success, error) = await _service.UpdateDisciplineWithDetailsAsync(id, dto);
            if (!success)
            {
                if (error == "Discipline not found")
                    return NotFound("Discipline not found");
                return BadRequest(error);
            }
            return NoContent();
        }
    }
}
