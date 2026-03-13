using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindMainDisciplineController : ControllerBase
    {
        private readonly IBindMainDisciplineService _service;

        public BindMainDisciplineController(IBindMainDisciplineService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BindMainDisciplineDto>> GetBindMainDiscipline(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<BindMainDisciplineDto>> CreateBindMainDiscipline(CreateBindMainDisciplineDto dto)
        {
            var resultDto = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetBindMainDiscipline), new { id = resultDto.IdBindMainDisciplines }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBindMainDiscipline(int id, UpdateBindMainDisciplineDto dto)
        {
            var (success, statusCode, errorMessage) = await _service.UpdateAsync(id, dto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBindMainDiscipline(int id)
        {
            var (success, statusCode, errorMessage) = await _service.DeleteAsync(id);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }
    }
}