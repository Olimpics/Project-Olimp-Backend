using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        [RequirePermission(RbacPermissions.DepartmentsRead)]
        public async Task<ActionResult<object>> GetDepartments(
          [FromQuery] DepartmentQueryDto query)
        {
            var result = await _departmentService.GetDepartmentsAsync(query);
            return Ok(result);
        }


        [HttpGet("{id}")]
        [RequirePermission(RbacPermissions.DepartmentsRead)]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(int id)
        {
            var department = await _departmentService.GetDepartmentAsync(id);

            if (department == null)
                return NotFound();

            return Ok(department);
        }

        [HttpPost]
        [RequirePermission(RbacPermissions.DepartmentsCreate)]
        public async Task<ActionResult<DepartmentDto>> CreateDepartment(CreateDepartmentDto dto)
        {
            var resultDto = await _departmentService.CreateDepartmentAsync(dto);
            return CreatedAtAction(nameof(GetDepartment), new { id = resultDto.IdDepartment }, resultDto);
        }

        [HttpPut("{id}")]
        [RequirePermission(RbacPermissions.DepartmentsUpdate)]
        public async Task<IActionResult> UpdateDepartment(int id, UpdateDepartmentDto dto)
        {
            var (success, statusCode, errorMessage) =
                await _departmentService.UpdateDepartmentAsync(id, dto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

        [HttpDelete("{id}")]
        [RequirePermission(RbacPermissions.DepartmentsDelete)]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var (success, statusCode, errorMessage) =
                await _departmentService.DeleteDepartmentAsync(id);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }
    }
} 