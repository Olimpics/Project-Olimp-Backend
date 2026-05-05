using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GradeController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public GradeController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    [HttpGet("main-students")]
    [RequirePermission(RbacPermissions.MainDisciplinesRead)]
    public async Task<ActionResult<PaginatedResponseDto<GradeStudentDto>>> GetMainDisciplineStudents(
        [FromQuery] GradeQueryDto query)
    {
        var result = await _gradeService.GetMainDisciplineStudentsAsync(query);
        return Ok(result);
    }

    [HttpGet("selective-students")]
    [RequirePermission(RbacPermissions.DisciplineRead)]
    public async Task<ActionResult<PaginatedResponseDto<GradeStudentDto>>> GetSelectiveDisciplineStudents(
        [FromQuery] GradeQueryDto query)
    {
        var result = await _gradeService.GetSelectiveDisciplineStudentsAsync(query);
        return Ok(result);
    }

    [HttpPost("main-grade")]
    [RequirePermission(RbacPermissions.MainDisciplinesUpdate)]
    public async Task<IActionResult> SetMainDisciplineGrade([FromBody] SetGradeDto dto)
    {
        var success = await _gradeService.SetMainDisciplineGradeAsync(dto);
        if (!success) return NotFound("Main discipline binding not found");
        return Ok();
    }

    [HttpPost("selective-grade")]
    [RequirePermission(RbacPermissions.DisciplineUpdate)]
    public async Task<IActionResult> SetSelectiveDisciplineGrade([FromBody] SetGradeDto dto)
    {
        var success = await _gradeService.SetSelectiveDisciplineGradeAsync(dto);
        if (!success) return NotFound("Selective discipline binding not found");
        return Ok();
    }

    [HttpGet("instructor/main-disciplines")]
    [RequirePermission(RbacPermissions.MainDisciplinesRead)]
    public async Task<ActionResult<List<InstructorDisciplineDto>>> GetMainDisciplinesByInstructor(
        [FromQuery] int adminId, [FromQuery] int catalogYearId, [FromQuery] bool isEvenSemester)
    {
        var result = await _gradeService.GetMainDisciplinesByInstructorAsync(adminId, catalogYearId, isEvenSemester);
        return Ok(result);
    }

    [HttpGet("instructor/selective-disciplines")]
    [RequirePermission(RbacPermissions.DisciplineRead)]
    public async Task<ActionResult<List<InstructorDisciplineDto>>> GetSelectiveDisciplinesByInstructor(
        [FromQuery] int adminId, [FromQuery] int catalogYearId, [FromQuery] bool isEvenSemester)
    {
        var result = await _gradeService.GetSelectiveDisciplinesByInstructorAsync(adminId, catalogYearId, isEvenSemester);
        return Ok(result);
    }

    [HttpGet("academic-period")]
    public async Task<ActionResult<AcademicPeriodDto>> GetAcademicPeriod([FromQuery] DateTime date)
    {
        var result = await _gradeService.GetAcademicPeriodAsync(date);
        return Ok(result);
    }
}
