using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationStatusController : ControllerBase
    {
        private readonly IEducationStatusService _service;

        public EducationStatusController(IEducationStatusService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EducationStatusDto>>> GetEducationStatuses()
        {
            var statuses = await _service.GetAllAsync();
            return Ok(statuses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EducationStatusDto>> GetEducationStatus(int id)
        {
            var status = await _service.GetByIdAsync(id);
            if (status == null)
                return NotFound();

            return Ok(status);
        }

        [HttpPost]
        public async Task<ActionResult<EducationStatusDto>> CreateEducationStatus(EducationStatusDto statusDto)
        {
            var resultDto = await _service.CreateAsync(statusDto);
            return CreatedAtAction(nameof(GetEducationStatus), new { id = resultDto.IdEducationStatus }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducationStatus(int id, EducationStatusDto statusDto)
        {
            var (success, statusCode, errorMessage) = await _service.UpdateAsync(id, statusDto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducationStatus(int id)
        {
            var (success, statusCode, errorMessage) = await _service.DeleteAsync(id);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }
    }
}