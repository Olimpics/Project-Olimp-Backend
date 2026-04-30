using Microsoft.AspNetCore.Mvc;
using OlimpBack.Application.DTO;
using OlimpBack.Application.Permissions;
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
        [RequirePermission(RbacPermissions.GroupsRead)]
        public async Task<ActionResult<IEnumerable<GroupFilterDto>>> GetGroups(
           [FromQuery] GroupListQueryDto query)
        {
            var groups = await _groupService.GetGroupsAsync(query);
            return Ok(groups);
        }
       
        [HttpGet("{id}")]
        [RequirePermission(RbacPermissions.GroupsRead)]
        public async Task<ActionResult<GroupDto>> GetGroup(int id)
        {
            var group = await _groupService.GetGroupAsync(id);

            if (group == null)
                return NotFound();

            return Ok(group);
        }

        [HttpGet("{id}/details")]
        [RequirePermission(RbacPermissions.GroupsRead)]
        public async Task<ActionResult<GroupDetailsDto>> GetGroupDetails(int id)
        {
            var group = await _groupService.GetGroupDetailsAsync(id);
            if (group == null)
                return NotFound();

            return Ok(group);
        }

        [HttpGet("{groupId}/students")]
        [RequirePermission(RbacPermissions.GroupsRead)]
        public async Task<ActionResult<IReadOnlyList<GroupStudentDto>>> GetStudentsByGroupId(int groupId)
        {
            var students = await _groupService.GetStudentsByGroupIdAsync(groupId);
            return Ok(students);
        }

        [HttpPost]
        [RequirePermission(RbacPermissions.GroupsCreate)]
        public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupDto dto)
        {
            var resultDto = await _groupService.CreateGroupAsync(dto);
            return CreatedAtAction(nameof(GetGroup), new { id = resultDto.IdGroup }, resultDto);
        }


        [HttpGet("{id}/curriculum")]
        [RequirePermission(RbacPermissions.GroupsRead)]
        public async Task<ActionResult<GroupMainDisciplineDto>> GetGroupCurriculum(int id)
        {
            var curriculum = await _groupService.GetGroupCurriculumAsync(id);
            if (curriculum == null)
                return NotFound();
            return Ok(curriculum);
        }



        [HttpPut("{id}")]
        [RequirePermission(RbacPermissions.GroupsUpdate)]
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
        [RequirePermission(RbacPermissions.GroupsDelete)]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var (success, statusCode, errorMessage) = await _groupService.DeleteGroupAsync(id);

            if (!success)
                return StatusCode(statusCode, errorMessage);

            return NoContent();
        }

    }
} 