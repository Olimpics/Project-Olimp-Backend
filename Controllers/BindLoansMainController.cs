using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OlimpBack.Application.Services;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindLoansMainController : ControllerBase
    {
        private readonly IBindLoansMainService _bindLoansMainService;
        private readonly ILogger<BindLoansMainController> _logger;

        public BindLoansMainController(IBindLoansMainService bindLoansMainService, ILogger<BindLoansMainController> logger)
        {
            _bindLoansMainService = bindLoansMainService;
            _logger = logger;
        }

        [HttpGet]
        [RequirePermission(RbacPermissions.MainDisciplinesRead)]
        public async Task<ActionResult<object>> GetBindLoansMain(
            [FromQuery] BindLoansMainQueryDto query)
        {
            var result = await _bindLoansMainService.GetBindLoansMainAsync(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [RequirePermission(RbacPermissions.MainDisciplinesRead)]
        public async Task<ActionResult<BindLoansMainDto>> GetBindLoansMain(int id)
        {
            var binding = await _bindLoansMainService.GetBindLoansMainAsync(id);

            if (binding == null)
            {
                return NotFound("Binding not found");
            }

            return Ok(binding);
        }

        [Authorize]
        [HttpPost]
        [RequirePermission(RbacPermissions.MainDisciplinesCreate)]
        public async Task<ActionResult<BindLoansMainDto>> CreateBindLoansMain(CreateBindLoansMainDto dto)
        {
            var result = await _bindLoansMainService.CreateBindLoansMainAsync(dto);
            return CreatedAtAction(nameof(GetBindLoansMain), new { id = result.IdBindLoan }, result);
        }

        [Authorize]
        [HttpPut("{id}")]
        [RequirePermission(RbacPermissions.MainDisciplinesUpdate)]
        public async Task<IActionResult> UpdateBindLoansMain(int id, UpdateBindLoansMainDto dto)
        {
            var (success, statusCode, errorMessage) =
                await _bindLoansMainService.UpdateBindLoansMainAsync(id, dto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        [RequirePermission(RbacPermissions.MainDisciplinesDelete)]
        public async Task<IActionResult> DeleteBindLoansMain(int id)
        {
            var (success, statusCode, errorMessage) =
                await _bindLoansMainService.DeleteBindLoansMainAsync(id);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }
    }
} 