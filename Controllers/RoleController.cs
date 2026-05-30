using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database;
using OlimpBack.Data;
using OlimpBack.Application.Permissions;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHierarchyAuthorizationService _hierarchyAuthorization;

        public RoleController(
            AppDbContext context,
            IMapper mapper,
            IHierarchyAuthorizationService hierarchyAuthorization)
        {
            _context = context;
            _mapper = mapper;
            _hierarchyAuthorization = hierarchyAuthorization;
        }

        // GET: api/Role
        [HttpGet]
        [RequirePermission(RbacPermissions.RolesRead)]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await GetRolesQuery()
                .OrderBy(role => role.IdRole)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<RoleDto>>(roles));
        }

        // GET: api/Role/5
        [HttpGet("{id}")]
        [RequirePermission(RbacPermissions.RolesRead)]
        public async Task<ActionResult<RoleDto>> GetRole(Guid id)
        {
            var role = await GetRoleEntityAsync(id);
            if (role == null)
                return NotFound();

            return Ok(_mapper.Map<RoleDto>(role));
        }

        // POST: api/Role
        [HttpPost]
        [RequirePermission(RbacPermissions.RolesCreate)]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleDto roleDto)
        {
            var granterUserId = User.GetUserId();
            if (!granterUserId.HasValue)
                return Unauthorized();

            if (!await CanCreateOrManageRoleAsync(granterUserId.Value, roleDto))
                return Forbid();

            if (!await IsValidParentRoleAsync(roleDto.ParentRoleId))
                return BadRequest("Parent role not found");

            var role = _mapper.Map<Role>(roleDto);
            role.IdRole = role.IdRole == Guid.Empty ? Guid.NewGuid() : role.IdRole;
            role.PermissionsMask = 0;
            role.CreatedByUserId = granterUserId.Value;

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<RoleDto>(role);
            return CreatedAtAction(nameof(GetRole), new { id = role.IdRole }, resultDto);
        }

        // PUT: api/Role/5
        [HttpPut("{id}")]
        [RequirePermission(RbacPermissions.RolesUpdate)]
        public async Task<IActionResult> UpdateRole(Guid id, RoleDto roleDto)
        {
            if (id != roleDto.IdRole)
                return BadRequest();

            var role = await GetRoleEntityAsync(id);
            if (role == null)
                return NotFound();

            var granterUserId = User.GetUserId();
            if (!granterUserId.HasValue)
                return Unauthorized();

            if (!await CanCreateOrManageRoleAsync(granterUserId.Value, roleDto))
                return Forbid();

            if (roleDto.ParentRoleId == id)
                return BadRequest("Role cannot be parent of itself");

            if (!await IsValidParentRoleAsync(roleDto.ParentRoleId))
                return BadRequest("Parent role not found");

            role.Name = roleDto.NameRole;
            role.ParentRoleId = roleDto.ParentRoleId;
            role.IsSystem = roleDto.IsSystem;
            role.IsStudent = roleDto.IsStudent;
            role.CreatedInFacultyId = roleDto.CreatedInFacultyId;
            role.CreatedInDepartmentId = roleDto.CreatedInDepartmentId;
            role.CreatedInGroupId = roleDto.CreatedInGroupId;

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Role/5
        [HttpDelete("{id}")]
        [RequirePermission(RbacPermissions.RolesDelete)]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await GetRoleEntityAsync(id);
            if (role == null)
                return NotFound();

            var granterUserId = User.GetUserId();
            if (!granterUserId.HasValue)
                return Unauthorized();

            if (!await CanManageExistingRoleAsync(granterUserId.Value, role))
                return Forbid();

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private IQueryable<Role> GetRolesQuery()
        {
            return _context.Roles
                .AsNoTracking();
        }

        private async Task<Role?> GetRoleEntityAsync(Guid id)
        {
            return await _context.Roles
                .AsTracking()
                .Where(role => role.IdRole == id)
                .FirstOrDefaultAsync();
        }

        private async Task<bool> CanCreateOrManageRoleAsync(Guid granterUserId, RoleDto roleDto)
        {
            var granterWeight = await _hierarchyAuthorization.GetUserMaxManagementWeightAsync(granterUserId);
            if (roleDto.IsSystem && !roleDto.IsStudent && granterWeight < 100)
                return false;

            if (granterWeight < 90)
                return false;

            return await _hierarchyAuthorization.CanManageScopeAsync(
                granterUserId,
                roleDto.CreatedInFacultyId,
                roleDto.CreatedInDepartmentId,
                roleDto.CreatedInGroupId);
        }

        private async Task<bool> IsValidParentRoleAsync(Guid? parentRoleId)
        {
            return !parentRoleId.HasValue ||
                   await _context.Roles.AsNoTracking().AnyAsync(role => role.IdRole == parentRoleId.Value);
        }

        private async Task<bool> CanManageExistingRoleAsync(Guid granterUserId, Role role)
        {
            var granterWeight = await _hierarchyAuthorization.GetUserMaxManagementWeightAsync(granterUserId);
            if (role.IsSystem && !role.IsStudent && granterWeight < 100)
                return false;

            if (granterWeight < 90)
                return false;

            return await _hierarchyAuthorization.CanManageScopeAsync(
                granterUserId,
                role.CreatedInFacultyId,
                role.CreatedInDepartmentId,
                role.CreatedInGroupId);
        }
    }

}