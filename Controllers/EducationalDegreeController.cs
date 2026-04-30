using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationalDegreeController : ControllerBase
    {
        private readonly IEducationalDegreeService _service;

        public EducationalDegreeController(IEducationalDegreeService service)
        {
            _service = service;
        }

        [HttpGet]
        [RequirePermission(RbacPermissions.EducationalDegreesRead)]
        public async Task<ActionResult<IEnumerable<EducationalDegreeDto>>> GetEducationalDegrees()
        {
            var degrees = await _service.GetAllAsync();
            return Ok(degrees);
        }

        [HttpGet("{id}")]
        [RequirePermission(RbacPermissions.EducationalDegreesRead)]
        public async Task<ActionResult<EducationalDegreeDto>> GetEducationalDegree(int id)
        {
            var degree = await _service.GetByIdAsync(id);
            if (degree == null)
                return NotFound();

            return Ok(degree);
        }

        [HttpPost]
        [RequirePermission(RbacPermissions.EducationalDegreesCreate)]
        public async Task<ActionResult<EducationalDegreeDto>> CreateEducationalDegree(CreateEducationalDegreeDto dto)
        {
            var resultDto = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetEducationalDegree), new { id = resultDto.IdEducationalDegree }, resultDto);
        }

        [HttpPut("{id}")]
        [RequirePermission(RbacPermissions.EducationalDegreesUpdate)]
        public async Task<IActionResult> UpdateEducationalDegree(int id, UpdateEducationalDegreeDto dto)
        {
            var (success, statusCode, errorMessage) = await _service.UpdateAsync(id, dto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [RequirePermission(RbacPermissions.EducationalDegreesDelete)]
        public async Task<IActionResult> DeleteEducationalDegree(int id)
        {
            var (success, statusCode, errorMessage) = await _service.DeleteAsync(id);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }
    }
}