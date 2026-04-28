//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using OlimpBack.Models;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using AutoMapper;
//using OlimpBack.Application.DTO;
//using OlimpBack.Application.Permissions;
//using OlimpBack.Infrastructure.Database;
//using OlimpBack.Utils;
//using OlimpBack.Data;

//namespace OlimpBack.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PermissionController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly IRoleMaskService _roleMaskService;
//        private readonly IMapper _mapper;

//        public PermissionController(AppDbContext context, IRoleMaskService roleMaskService, IMapper mapper)
//        {
//            _context = context;
//            _roleMaskService = roleMaskService;
//            _mapper = mapper;
//        }

//        // GET: api/Permission
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
//        {
//            var permissions = await _context.Permissions.ToListAsync();
//            return Ok(_mapper.Map<IEnumerable<PermissionDto>>(permissions));
//        }

//        // GET: api/Permission/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<PermissionDto>> GetPermission(int id)
//        {
//            var permission = await _context.Permissions.FindAsync(id);
//            if (permission == null)
//                return NotFound();

//            return Ok(_mapper.Map<PermissionDto>(permission));
//        }

//        // POST: api/Permission
//        [HttpPost]
//        public async Task<ActionResult<PermissionDto>> CreatePermission(CreatePermissionDto permissionDto)
//        {
//            if (permissionDto.BitIndex < PermissionMaskHelper.MinBitIndex || permissionDto.BitIndex > PermissionMaskHelper.MaxBitIndex)
//                return BadRequest($"BitIndex must be in range [{PermissionMaskHelper.MinBitIndex}, {PermissionMaskHelper.MaxBitIndex}].");

//            var code = $"{permissionDto.TypePermission}:{permissionDto.TableName}";
//            var existsByCode = await _context.Permissions.AnyAsync(x => x.Code == code);
//            if (existsByCode)
//                return BadRequest("Permission with this TypePermission + TableName already exists.");

//            var bitIndexAlreadyUsed = await _context.Permissions.AnyAsync(x => x.BitIndex == permissionDto.BitIndex);
//            if (bitIndexAlreadyUsed)
//                return BadRequest("BitIndex is already used by another permission.");

//            var permission = _mapper.Map<Permission>(permissionDto);
//            _context.Permissions.Add(permission);
//            await _context.SaveChangesAsync();

//            var resultDto = _mapper.Map<PermissionDto>(permission);
//            return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, resultDto);
//        }

//        // PUT: api/Permission/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdatePermission(int id, UpdatePermissionDto permissionDto)
//        {
//            if (id != permissionDto.IdPermissions)
//                return BadRequest();

//            var permission = await _context.Permissions.FindAsync(id);
//            if (permission == null)
//                return NotFound();

//            if (permissionDto.BitIndex < PermissionMaskHelper.MinBitIndex || permissionDto.BitIndex > PermissionMaskHelper.MaxBitIndex)
//                return BadRequest($"BitIndex must be in range [{PermissionMaskHelper.MinBitIndex}, {PermissionMaskHelper.MaxBitIndex}].");

//            var newCode = $"{permissionDto.TypePermission}:{permissionDto.TableName}";
//            var existsByCode = await _context.Permissions.AnyAsync(x =>
//                x.Id != id && x.Code == newCode);
//            if (existsByCode)
//                return BadRequest("Permission with this TypePermission + TableName already exists.");

//            var bitIndexAlreadyUsed = await _context.Permissions.AnyAsync(x =>
//                x.Id != id && x.BitIndex == permissionDto.BitIndex);
//            if (bitIndexAlreadyUsed)
//                return BadRequest("BitIndex is already used by another permission.");

//            var affectedRoleIds = await _context.BindRolePermissions
//                .Where(x => x.PermissionId == id)
//                .Select(x => x.RoleId)
//                .Distinct()
//                .ToListAsync();

//            _mapper.Map(permissionDto, permission);
//            await _context.SaveChangesAsync();

//            foreach (var roleId in affectedRoleIds)
//            {
//                if (roleId.HasValue)
//                    await _roleMaskService.RecalculateRoleMaskAsync(roleId.Value);
//            }

//            return NoContent();
//        }

//        // DELETE: api/Permission/5
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeletePermission(int id)
//        {
//            var permission = await _context.Permissions.FindAsync(id);
//            if (permission == null)
//                return NotFound();

//            var affectedRoleIds = await _context.BindRolePermissions
//                .Where(x => x.PermissionId == id)
//                .Select(x => x.RoleId)
//                .Distinct()
//                .ToListAsync();

//            _context.Permissions.Remove(permission);
//            await _context.SaveChangesAsync();

//            foreach (var roleId in affectedRoleIds)
//            {
//                if (roleId.HasValue)
//                    await _roleMaskService.RecalculateRoleMaskAsync(roleId.Value);
//            }

//            return NoContent();
//        }
//    }
//} 