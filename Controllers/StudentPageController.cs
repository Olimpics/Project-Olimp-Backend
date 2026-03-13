using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;
using System.Threading.Tasks;

namespace OlimpBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentPageController : ControllerBase
{
    private readonly IStudentPageService _service;

    public StudentPageController(IStudentPageService service)
    {
        _service = service;
    }

    [HttpGet("disciplines/{studentId}")]
    public async Task<ActionResult<StudentDisciplinesDto>> GetStudentDisciplines(int studentId)
    {
        var result = await _service.GetStudentDisciplinesAsync(studentId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}