using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacultyController : ControllerBase
    {
        private readonly IFacultyService _facultyService;

        public FacultyController(IFacultyService facultyService)
        {
            _facultyService = facultyService;
        }

        [HttpGet]
        [RequirePermission(RbacPermissions.FacultiesRead)]
        public async Task<ActionResult<IEnumerable<FacultyDto>>> GetFaculties()
        {
            var faculties = await _facultyService.GetFacultiesAsync();
            return Ok(faculties);
        }

        [HttpGet("{id}")]
        [RequirePermission(RbacPermissions.FacultiesRead)]
        public async Task<ActionResult<FacultyDto>> GetFaculty(int id)
        {
            var faculty = await _facultyService.GetFacultyAsync(id);
            if (faculty == null)
                return NotFound();

            return Ok(faculty);
        }

        [HttpPost]
        [RequirePermission(RbacPermissions.FacultiesCreate)]
        public async Task<ActionResult<FacultyDto>> CreateFaculty(FacultyCreateDto facultyDto)
        {
            var resultDto = await _facultyService.CreateFacultyAsync(facultyDto);
            return CreatedAtAction(nameof(GetFaculty), new { id = resultDto.IdFaculty }, resultDto);
        }


        [HttpPut("{id}")]
        [RequirePermission(RbacPermissions.FacultiesUpdate)]
        public async Task<IActionResult> UpdateFaculty(int id, FacultyDto facultyDto)
        {
            if (id != facultyDto.IdFaculty)
                return BadRequest();

            var (success, statusCode, errorMessage) = await _facultyService.UpdateFacultyAsync(id, facultyDto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

    }
}