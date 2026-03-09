using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.DTO;
using OlimpBack.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplineTabAdminController : ControllerBase
    {
        private readonly IDisciplineTabAdminService _service;

        public DisciplineTabAdminController(IDisciplineTabAdminService service)
        {
            _service = service;
        }
        [HttpGet("GetStudentsWithDisciplineChoices")]
        public async Task<ActionResult<object>> GetStudentsWithDisciplineChoices(
            [FromQuery] GetStudentsWithDisciplineChoicesQueryDto query)
        {
            var result = await _service.GetStudentsWithDisciplineChoicesAsync(query);
            return Ok(result);
        }

        [HttpPut("UpdateChoice")]
        public async Task<ActionResult<object>> UpdateChoice(ConfirmOrRejectChoiceDto[] items)
        {
            if (items == null || items.Length == 0)
                return BadRequest(new { error = "At least one item is required" });

            var result = await _service.UpdateChoiceAsync(items);
            return Ok(result);
        }

        [HttpGet("GetDisciplinesWithStatus")]
        public async Task<ActionResult<object>> GetDisciplinesWithStatus(
            [FromQuery] GetDisciplinesWithStatusQueryDto query)
        {
            var result = await _service.GetDisciplinesWithStatusAsync(query);
            return Ok(result);
        }

        [HttpPut("UpdateDisciplineStatus")]
        public async Task<ActionResult<object>> UpdateDisciplineStatus(UpdateDisciplineStatusDto dto)
        {
            if (dto.Status < 1 || dto.Status > 4)
                return BadRequest(new { error = "Status must be 1 (Not Selected), 2 (Intellectually Selected), 3 (Selected) or 4 (Collected)" });

            var result = await _service.UpdateDisciplineStatusAsync(dto);
            if (result == null)
                return NotFound(new { error = "Discipline not found" });
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BindAddDisciplineDto>> GetBind(int id)
        {
            var result = await _service.GetBindAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("GetStudentWithChoices/{studentId}")]
        public async Task<ActionResult<StudentWithDisciplineChoicesDto>> GetStudentWithChoices(int studentId)
        {
            var result = await _service.GetStudentWithChoicesAsync(studentId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

    }
}
