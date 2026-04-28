//using Microsoft.AspNetCore.Mvc;
//using OlimpBack.Application.DTO;
//using OlimpBack.Application.Services;

//namespace OlimpBack.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class BindRolePermissionController : ControllerBase
//    {
//        private readonly IBindRolePermissionService _service;

//        public BindRolePermissionController(IBindRolePermissionService service)
//        {
//            _service = service;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<BindRolePermissionDto>>> GetBindRolePermissions()
//        {
//            var result = await _service.GetAllAsync();
//            return Ok(result);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<BindRolePermissionDto>> GetBindRolePermission(int id)
//        {
//            var result = await _service.GetByIdAsync(id);

//            if (result == null)
//                return NotFound();

//            return Ok(result);
//        }

//        [HttpPost]
//        public async Task<ActionResult<BindRolePermissionDto>> CreateBindRolePermission(CreateBindRolePermissionDto dto)
//        {
//            var (resultDto, statusCode, errorMessage) = await _service.CreateAsync(dto);

//            if (resultDto == null)
//                return StatusCode(statusCode!.Value, errorMessage);

//            return CreatedAtAction(nameof(GetBindRolePermission), new { id = resultDto.IdBindRolePermission }, resultDto);
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateBindRolePermission(int id, UpdateBindRolePermissionDto dto)
//        {
//            var (success, statusCode, errorMessage) = await _service.UpdateAsync(id, dto);

//            if (!success)
//                return StatusCode(statusCode, errorMessage);

//            return NoContent();
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteBindRolePermission(int id)
//        {
//            var (success, statusCode, errorMessage) = await _service.DeleteAsync(id);

//            if (!success)
//                return StatusCode(statusCode, errorMessage);

//            return NoContent();
//        }
//    }
//}