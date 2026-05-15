using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplineTabAdminController : ControllerBase
    {
        private readonly IDisciplineTabAdminService _service;

        public DisciplineTabAdminController(IDisciplineTabAdminService service)
        {
            _service = service;
        }

        [HttpGet("GetAllDisciplines")]
        [RequirePermission(RbacPermissions.DisciplineRead)]
        public async Task<ActionResult<PaginatedResponseDto<FullDisciplineDto>>> GetAllDisciplines(
            [FromQuery] GetAllDisciplinesAdminQueryDto query)
        {
            var result = await _service.GetAllDisciplinesAsync(query);
            return Ok(result);
        }

        [HttpGet("GetStudentsWithDisciplineChoices")]
        [RequirePermission(RbacPermissions.DisciplineTeachersPermission)]
        public async Task<ActionResult<object>> GetStudentsWithDisciplineChoices(
            [FromQuery] GetStudentsWithDisciplineChoicesQueryDto query)
        {
            var result = await _service.GetStudentsWithDisciplineChoicesAsync(query);
            return Ok(result);
        }

        [HttpPut("UpdateChoice")]
        [RequirePermission(RbacPermissions.DisciplineTeachersPermission)]
        public async Task<ActionResult<object>> UpdateChoice(ConfirmOrRejectChoiceDto[] items)
        {
            if (items == null || items.Length == 0)
                return BadRequest(new { error = "At least one item is required" });

            var result = await _service.UpdateChoiceAsync(items);
            return Ok(result);
        }

        [HttpGet("GetDisciplinesWithStatus")]
        [RequirePermission(RbacPermissions.DisciplineRead)]
        public async Task<ActionResult<object>> GetDisciplinesWithStatus(
            [FromQuery] GetDisciplinesWithStatusQueryDto query)
        {
            var result = await _service.GetDisciplinesWithStatusAsync(query);
            return Ok(result);
        }

        [HttpPut("UpdateDisciplineStatus")]
        [RequirePermission(RbacPermissions.DisciplineTeachersPermission)]
        public async Task<ActionResult<object>> UpdateDisciplineStatus(UpdateDisciplineStatusDto dto)
        {
            if (dto.Status < 1 || dto.Status > 4)
                return BadRequest(new { error = "Status must be 1 (Not Selected), 2 (Intellectually Selected), 3 (Selected) or 4 (Collected)" });

            var result = await _service.UpdateDisciplineStatusAsync(dto);
            if (result == null)
                return NotFound(new { error = "Discipline not found" });
            return Ok(result);
        }

        [HttpGet("{id}")]
        [RequirePermission(RbacPermissions.DisciplineRead)]
        public async Task<ActionResult<BindSelectiveDisciplineDto>> GetBind(Guid id)
        {
            var result = await _service.GetBindAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("GetStudentWithChoices/{studentId}")]
        [RequirePermission(RbacPermissions.DisciplineTeachersPermission)]
        public async Task<ActionResult<StudentWithDisciplineChoicesDto>> GetStudentWithChoices(Guid studentId)
        {
            var result = await _service.GetStudentWithChoicesAsync(studentId);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("GetStudentsBySelectiveDiscipline")]
        [RequirePermission(RbacPermissions.DisciplineTeachersPermission)]
        public async Task<ActionResult<PaginatedResponseDto<AdminStudentBySelectiveDisciplineDto>>> GetStudentsBySelectiveDiscipline(
            [FromQuery] GetStudentsBySelectiveDisciplineQueryDto query)
        {
            var result = await _service.GetStudentsBySelectiveDisciplineAsync(query);
            return Ok(result);
        }

        [HttpGet("GetStudentsByMainDiscipline")]
        [RequirePermission(RbacPermissions.DisciplineTeachersPermission)]
        public async Task<ActionResult<PaginatedResponseDto<AdminStudentByMainDisciplineDto>>> GetStudentsByMainDiscipline(
            [FromQuery] GetStudentsByMainDisciplineQueryDto query)
        {
            var result = await _service.GetStudentsByMainDisciplineAsync(query);
            return Ok(result);
        }

        [HttpGet("GetStudentsIncompleteAfterChoicePeriod")]
        [RequirePermission(RbacPermissions.DisciplineTeachersPermission)]
        public async Task<ActionResult<List<StudentIdNameDto>>> GetStudentsIncompleteAfterChoicePeriod([FromQuery] Guid facultyId)
        {
            if (facultyId == Guid.Empty)
                return BadRequest(new { error = "facultyId is required" });

            var result = await _service.GetStudentsIncompleteAfterChoicePeriodAsync(facultyId);
            return Ok(result);
        }


        //
        [HttpDelete("RepealChoice/{studentId}/{DisciplineId}")]
        [RequirePermission(RbacPermissions.DisciplineTeachersPermission)]
        public async Task<IActionResult> RepealChoice(Guid DisciplineId, Guid studentId)
        {
            var (success, errorMessage) = await _service.RepealChoiceAsync(DisciplineId, studentId);

            if (!success)
            {
                return BadRequest(new { message = errorMessage });
            }

            return Ok(new { message = "Choice rejected and student notified successfully." });
        }
    }
}
