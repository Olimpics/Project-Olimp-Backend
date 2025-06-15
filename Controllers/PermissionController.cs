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
    public class PermissionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PermissionController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Permission
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetPermissions()
        {
            var permissions = await _context.Permissions.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<PermissionDto>>(permissions));
        }

        // GET: api/Permission/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionDto>> GetPermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                return NotFound();

            return Ok(_mapper.Map<PermissionDto>(permission));
        }

        // POST: api/Permission
        [HttpPost]
        public async Task<ActionResult<PermissionDto>> CreatePermission(CreatePermissionDto permissionDto)
        {
            var permission = _mapper.Map<Permission>(permissionDto);
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<PermissionDto>(permission);
            return CreatedAtAction(nameof(GetPermission), new { id = permission.IdPermissions }, resultDto);
        }

        // PUT: api/Permission/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, UpdatePermissionDto permissionDto)
        {
            if (id != permissionDto.IdPermissions)
                return BadRequest();

            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                return NotFound();

            _mapper.Map(permissionDto, permission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Permission/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                return NotFound();

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PermissionExists(int id)
        {
            return _context.Permissions.Any(e => e.IdPermissions == id);
        }
    }
} 