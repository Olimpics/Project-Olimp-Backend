using Microsoft.AspNetCore.Mvc;
using OlimpBack.DTO;
using OlimpBack.Services;

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
        public async Task<ActionResult<IEnumerable<FacultyDto>>> GetFaculties()
        {
            var faculties = await _facultyService.GetFacultiesAsync();
            return Ok(faculties);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacultyDto>> GetFaculty(int id)
        {
            var faculty = await _facultyService.GetFacultyAsync(id);
            if (faculty == null)
                return NotFound();

            return Ok(faculty);
        }

        [HttpPost]
        public async Task<ActionResult<FacultyDto>> CreateFaculty(FacultyCreateDto facultyDto)
        {
            var resultDto = await _facultyService.CreateFacultyAsync(facultyDto);
            return CreatedAtAction(nameof(GetFaculty), new { id = resultDto.IdFaculty }, resultDto);
        }


        [HttpPut("{id}")]
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