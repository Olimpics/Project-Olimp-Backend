using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpGet("educational-program/{studentId}")]
    public async Task<ActionResult<StudentEducationalProgramDto>> GetStudentEducationalProgram(int studentId)
    {
        var result = await _service.GetStudentEducationalProgramAsync(studentId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("add-disciplines/{studentId}")]
    public async Task<ActionResult<StudentAddDisciplinesDto>> GetStudentAddDisciplines(int studentId)
    {
        var result = await _service.GetStudentAddDisciplinesAsync(studentId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}