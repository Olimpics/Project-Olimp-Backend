using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteDisciplineService _service;

        public FavoriteController(IFavoriteDisciplineService service)
        {
            _service = service;
        }

        [HttpGet("ByStudent/{studentId}")]
        [RequirePermission(RbacPermissions.DisciplineRead)]
        public async Task<ActionResult<List<FavoriteDisciplineDto>>> GetByStudent(Guid studentId)
        {
            var result = await _service.GetByStudentAsync(studentId);
            return Ok(result);
        }

        [HttpPost]
        [RequirePermission(RbacPermissions.DisciplineCreate)]
        public async Task<ActionResult> Add([FromBody] AddFavoriteDisciplineDto dto)
        {
            var (success, error) = await _service.AddAsync(dto.StudentId, dto.DisciplineId);
            if (!success) return BadRequest(new { error });
            return Ok(new { message = "Added to favorites" });
        }

        [HttpDelete("{studentId}/{disciplineId}")]
        [RequirePermission(RbacPermissions.DisciplineDelete)]
        public async Task<ActionResult> Remove(Guid studentId, Guid disciplineId)
        {
            var (success, error) = await _service.RemoveAsync(studentId, disciplineId);
            if (!success) return NotFound(new { error });
            return NoContent();
        }
    }
}
