using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Services;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupFilterDto>>> GetGroups(
           [FromQuery] GroupListQueryDto query)
        {
            var groups = await _groupService.GetGroupsAsync(query);
            return Ok(groups);
        }
       
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroup(int id)
        {
            var group = await _groupService.GetGroupAsync(id);

            if (group == null)
                return NotFound();

            return Ok(group);
        }

        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupDto dto)
        {
            var resultDto = await _groupService.CreateGroupAsync(dto);
            return CreatedAtAction(nameof(GetGroup), new { id = resultDto.IdGroup }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, UpdateGroupDto dto)
        {
            if (id != dto.IdGroup)
                return BadRequest();

            var (success, statusCode, errorMessage) = await _groupService.UpdateGroupAsync(id, dto);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var (success, statusCode, errorMessage) = await _groupService.DeleteGroupAsync(id);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

    }
} 