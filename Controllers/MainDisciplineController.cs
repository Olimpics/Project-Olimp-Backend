using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainDisciplineController : ControllerBase
    {
        private readonly IMainDisciplineService _service;

        public MainDisciplineController(IMainDisciplineService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        [RequirePermission(RbacPermissions.MainDisciplinesRead)]
        public async Task<ActionResult<MainDisciplineDto>> GetMainDiscipline(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        [RequirePermission(RbacPermissions.MainDisciplinesCreate)]
        public async Task<ActionResult<MainDisciplineDto>> CreateMainDiscipline(CreateMainDisciplineDto dto)
        {
            var resultDto = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetMainDiscipline), new { id = resultDto.IdMainDisciplines }, resultDto);
        }

        [HttpPut("{id}")]
        [RequirePermission(RbacPermissions.MainDisciplinesUpdate)]
        public async Task<IActionResult> UpdateMainDiscipline(int id, UpdateMainDisciplineDto dto)
        {
            var (success, statusCode, errorMessage) = await _service.UpdateAsync(id, dto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [RequirePermission(RbacPermissions.MainDisciplinesDelete)]
        public async Task<IActionResult> DeleteMainDiscipline(int id)
        {
            var (success, statusCode, errorMessage) = await _service.DeleteAsync(id);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }
    }
}
