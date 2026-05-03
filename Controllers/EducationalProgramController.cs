using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OlimpBack.Application.Services;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;

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
        [RequirePermission(RbacPermissions.EducationalProgramsRead)]
        public async Task<ActionResult<object>> GetEducationalPrograms(
            [FromQuery] EducationalProgramListQueryDto query)
        {
            var result = await _educationalProgramService.GetEducationalProgramsAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [RequirePermission(RbacPermissions.EducationalProgramsRead)]
        public async Task<ActionResult<EducationalProgramDto>> GetEducationalProgram(int id)
        {
            var program = await _educationalProgramService.GetEducationalProgramAsync(id);

            if (program == null)
                return NotFound("Educational program not found");

            return Ok(program);
        }

        [HttpGet("{id}/students")]
        [RequirePermission(RbacPermissions.EducationalProgramsRead)]
        public async Task<ActionResult<PaginatedResponseDto<ProgramStudentDto>>> GetProgramStudents(
            int id,
            [FromQuery] ProgramStudentQueryDto query)
        {
            var result = await _educationalProgramService.GetStudentsPagedAsync(id, query);
            return Ok(result);
        }

        [HttpGet("{id}/disciplines")]
        [RequirePermission(RbacPermissions.EducationalProgramsRead)]
        public async Task<ActionResult<List<ProgramDisciplinesBySemesterDto>>> GetProgramDisciplines(int id)
        {
            var result = await _educationalProgramService.GetMainDisciplinesGroupedBySemesterAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [RequirePermission(RbacPermissions.EducationalProgramsCreate)]
        public async Task<ActionResult<EducationalProgramDto>> CreateEducationalProgram(CreateEducationalProgramDto dto)
        {
            var result = await _educationalProgramService.CreateEducationalProgramAsync(dto);

            return CreatedAtAction(nameof(GetEducationalProgram), new { id = result.IdEducationalProgram }, result);
        }

        [Authorize]
        [HttpPut("{id}")]
        [RequirePermission(RbacPermissions.EducationalProgramsUpdate)]
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