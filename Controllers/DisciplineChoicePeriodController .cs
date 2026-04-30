using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisciplineChoicePeriodController : ControllerBase
    {
        private readonly IDisciplineChoicePeriodService _service;

        public DisciplineChoicePeriodController(IDisciplineChoicePeriodService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(RbacPermissions.DisciplineChoicePeriodsRead)]
        public async Task<ActionResult<IEnumerable<DisciplineChoicePeriodDto>>> GetAll([FromQuery] GetDisciplineChoicePeriodsQueryDto queryDto)
        {
            var result = await _service.GetAllAsync(queryDto);
            return Ok(result);
        }

        [HttpPost]
        [RequirePermission(RbacPermissions.DisciplineChoicePeriodsCreate)]
        public async Task<ActionResult<DisciplineChoicePeriodDto>> Create([FromBody] CreateDisciplineChoicePeriodDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [RequirePermission(RbacPermissions.DisciplineChoicePeriodsUpdate)]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateDisciplineChoicePeriodDto dto)
        {
            var (success, statusCode, errorMessage) = await _service.UpdateAsync(id, dto);
            if (!success) return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

        [HttpPut("UpdateAfterStart/{id}")]
        [RequirePermission(RbacPermissions.DisciplineChoicePeriodsUpdate)]
        public async Task<ActionResult> UpdateAfterStart(int id, [FromBody] UpdateDisciplineChoicePeriodAfterStartDto dto)
        {
            var (success, statusCode, errorMessage) = await _service.UpdateAfterStartAsync(id, dto);
            if (!success) return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

        [HttpPut("OpenOrClose/{id}")]
        [RequirePermission(RbacPermissions.DisciplineChoicePeriodsUpdate)]
        public async Task<ActionResult> OpenOrClose(int id, [FromBody] UpdateDisciplineChoicePeriodOpenOrCloseDto dto)
        {
            var (success, statusCode, errorMessage) = await _service.OpenOrCloseAsync(id, dto);
            if (!success) return StatusCode(statusCode, errorMessage);

            return NoContent();
        }
    }
}