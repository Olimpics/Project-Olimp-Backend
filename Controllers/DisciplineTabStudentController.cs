using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplineTabStudentController : ControllerBase
    {
        private readonly IDisciplineTabService _service;
        private readonly IStudentChoiceCacheService _cacheService;

        public DisciplineTabStudentController(IDisciplineTabService service, IStudentChoiceCacheService cacheService)
        {
            _service = service;
            _cacheService = cacheService;
        }

        [HttpPost("RecalculateCache/{idStudent}")]
        [RequirePermission(RbacPermissions.DisciplineUpdate)]
        public async Task<ActionResult> RecalculateCache(Guid idStudent)
        {
            try
            {
                await _cacheService.InitializeCacheAsync(idStudent, null);
                return Ok(new { message = "Cache successfully recalculated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while recalculating cache",
                    details = ex.Message
                });
            }
        }

        [HttpGet("GetAllDisciplinesWithAvailability")]
        [RequirePermission(RbacPermissions.DisciplineRead)]
        public async Task<ActionResult<object>> GetAllDisciplinesWithAvailability(
            [FromQuery] GetAllDisciplinesWithAvailabilityQueryDto query)
        {
            var result = await _service.GetAllDisciplinesWithAvailabilityAsync(query);
            if (result == null)
                return NotFound("Student not found");
            return Ok(result);
        }

        [HttpPost("SelectiveDisciplineBind")]
        [RequirePermission(RbacPermissions.DisciplineCreate)]
        public async Task<ActionResult> SelectiveDisciplineBind(SelectiveDisciplineBindDto dto)
        {
            try
            {
                var (bindId, error) = await _service.SelectiveDisciplineBindAsync(dto);
                if (error != null)
                {
                    if (error.StartsWith("Student not found"))
                        return NotFound(new { error });
                    return BadRequest(new { error });
                }
                return Ok(new
                {
                    message = "Discipline successfully bound to student",
                    bindId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while processing your request",
                    details = ex.Message
                });
            }
        }

        [HttpGet("GetDisciplineWithDetails/{id}")]
        [RequirePermission(RbacPermissions.DisciplineRead)]
        public async Task<ActionResult<FullDisciplineWithDetailsDto>> GetDisciplineWithDetails(Guid id)
        {
            var result = await _service.GetDisciplineWithDetailsAsync(id);
            if (result == null)
                return NotFound("Discipline not found");
            return Ok(result);
        }

        [HttpGet("GetSimilarDisciplines/{disciplineId}")]
        [RequirePermission(RbacPermissions.DisciplineRead)]
        public async Task<ActionResult<List<SimilarDisciplineGroupDto>>> GetSimilarDisciplines(Guid disciplineId)
        {
            var result = await _service.GetSimilarDisciplinesAsync(disciplineId);
            return Ok(result);
        }

        [HttpDelete("RemoveFromSimilarityGroup/{disciplineId}/{groupId}")]
        [RequirePermission(RbacPermissions.DisciplineDelete)]
        public async Task<IActionResult> RemoveFromSimilarityGroup(Guid disciplineId, Guid groupId)
        {
            var success = await _service.RemoveDisciplineFromSimilarityGroupAsync(disciplineId, groupId);
            if (!success) return NotFound("Similarity binding not found.");
            return NoContent();
        }

        [HttpGet("GetDisciplinesBySemester")]
        [RequirePermission(RbacPermissions.DisciplineRead)]
        public async Task<ActionResult<DisciplineTabResponseDto>> GetDisciplinesBySemester(
           [FromQuery] GetDisciplinesBySemesterQueryDto query)
        {
            var result = await _service.GetDisciplinesBySemesterAsync(query);

            if (result == null)
                return BadRequest(new { message = "Student not found or choice period is not active" });

            return Ok(result);
        }
    }
}
