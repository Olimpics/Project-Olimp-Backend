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
    public class RoleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public RoleController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Role
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<RoleDto>>(roles));
        }

        // GET: api/Role/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound();

            return Ok(_mapper.Map<RoleDto>(role));
        }

        // POST: api/Role
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleDto roleDto)
        {
            var role = _mapper.Map<Role>(roleDto);
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<RoleDto>(role);
            return CreatedAtAction(nameof(GetRole), new { id = role.IdRole }, resultDto);
        }

        // PUT: api/Role/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleDto roleDto)
        {
            if (id != roleDto.IdRole)
                return BadRequest();

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound();

            _mapper.Map(roleDto, role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Role/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound();

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.IdRole == id);
        }
    }

}