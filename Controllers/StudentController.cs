using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET: api/Student
        [HttpGet]
        [RequirePermission(RbacPermissions.StudentsRead)]
        public async Task<ActionResult<object>> GetStudents(
            [FromQuery] StudentQueryDto query)
        {
            var result = await _studentService.GetStudentsAsync(query);
            return Ok(result);
        }

        // GET: api/Student/5
        [HttpGet("{id}")]
        [RequirePermission(RbacPermissions.StudentsRead)]
        public async Task<ActionResult<StudentDto>> GetStudent(int id)
        {
            var student = await _studentService.GetStudentAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // POST: api/Student
        [HttpPost]
        [RequirePermission(RbacPermissions.StudentsCreate)]
        public async Task<ActionResult<StudentDto>> CreateStudent([FromBody] List<CreateStudentDto> dtos)
        {
            var results = await _studentService.CreateStudentsAsync(dtos);
            return Ok(results);
        }

        // PUT: api/Student/5
        [HttpPut("{id}")]
        [RequirePermission(RbacPermissions.StudentsUpdate)]
        public async Task<IActionResult> UpdateStudent(int id, UpdateStudentDto updateDto)
        {
            var (success, statusCode, errorMessage) =
                await _studentService.UpdateStudentAsync(id, updateDto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }
    }
}
