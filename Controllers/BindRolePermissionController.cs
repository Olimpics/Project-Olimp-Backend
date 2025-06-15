using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.DTO;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindRolePermissionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BindRolePermissionController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/BindRolePermission
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BindRolePermissionDto>>> GetBindRolePermissions()
        {
            var bindings = await _context.BindRolePermissions
                .Include(b => b.Role)
                .Include(b => b.Permission)
                .ToListAsync();
            return Ok(_mapper.Map<IEnumerable<BindRolePermissionDto>>(bindings));
        }

        // GET: api/BindRolePermission/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BindRolePermissionDto>> GetBindRolePermission(int id)
        {
            var binding = await _context.BindRolePermissions
                .Include(b => b.Role)
                .Include(b => b.Permission)
                .FirstOrDefaultAsync(b => b.IdBindRolePermission == id);

            if (binding == null)
                return NotFound();

            return Ok(_mapper.Map<BindRolePermissionDto>(binding));
        }

        // POST: api/BindRolePermission
        [HttpPost]
        public async Task<ActionResult<BindRolePermissionDto>> CreateBindRolePermission(CreateBindRolePermissionDto dto)
        {
            // Check if role exists
            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null)
                return BadRequest("Role not found");

            // Check if permission exists
            var permission = await _context.Permissions.FindAsync(dto.PermissionId);
            if (permission == null)
                return BadRequest("Permission not found");

            // Check if binding already exists
            var existingBinding = await _context.BindRolePermissions
                .FirstOrDefaultAsync(b => b.RoleId == dto.RoleId && b.PermissionId == dto.PermissionId);
            if (existingBinding != null)
                return BadRequest("This role-permission binding already exists");

            var binding = _mapper.Map<BindRolePermission>(dto);
            _context.BindRolePermissions.Add(binding);
            await _context.SaveChangesAsync();

            var resultDto = await GetBindRolePermissionWithIncludes(binding.IdBindRolePermission);
            return CreatedAtAction(nameof(GetBindRolePermission), new { id = binding.IdBindRolePermission }, resultDto);
        }

        // PUT: api/BindRolePermission/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBindRolePermission(int id, UpdateBindRolePermissionDto dto)
        {
            if (id != dto.IdBindRolePermission)
                return BadRequest();

            var binding = await _context.BindRolePermissions.FindAsync(id);
            if (binding == null)
                return NotFound();

            // Check if role exists
            var role = await _context.Roles.FindAsync(dto.RoleId);
            if (role == null)
                return BadRequest("Role not found");

            // Check if permission exists
            var permission = await _context.Permissions.FindAsync(dto.PermissionId);
            if (permission == null)
                return BadRequest("Permission not found");

            // Check if new binding already exists
            var existingBinding = await _context.BindRolePermissions
                .FirstOrDefaultAsync(b => b.RoleId == dto.RoleId && b.PermissionId == dto.PermissionId && b.IdBindRolePermission != id);
            if (existingBinding != null)
                return BadRequest("This role-permission binding already exists");

            _mapper.Map(dto, binding);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/BindRolePermission/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBindRolePermission(int id)
        {
            var binding = await _context.BindRolePermissions.FindAsync(id);
            if (binding == null)
                return NotFound();

            _context.BindRolePermissions.Remove(binding);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<BindRolePermissionDto> GetBindRolePermissionWithIncludes(int id)
        {
            var binding = await _context.BindRolePermissions
                .Include(b => b.Role)
                .Include(b => b.Permission)
                .FirstOrDefaultAsync(b => b.IdBindRolePermission == id);

            return _mapper.Map<BindRolePermissionDto>(binding);
        }
    }
} 