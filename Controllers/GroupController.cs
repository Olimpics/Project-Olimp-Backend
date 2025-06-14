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
    public class GroupController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GroupController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupDto>>> GetGroups()
        {
            var groups = await _context.Groups
                .Include(g => g.Students)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<GroupDto>>(groups));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupDto>> GetGroup(int id)
        {
            var group = await _context.Groups
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.IdGroup == id);

            if (group == null)
                return NotFound();

            return Ok(_mapper.Map<GroupDto>(group));
        }

        [HttpPost]
        public async Task<ActionResult<GroupDto>> CreateGroup(CreateGroupDto dto)
        {
            var group = _mapper.Map<Group>(dto);
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<GroupDto>(group);
            return CreatedAtAction(nameof(GetGroup), new { id = group.IdGroup }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, UpdateGroupDto dto)
        {
            if (id != dto.IdGroup)
                return BadRequest();

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound();

            _mapper.Map(dto, group);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!GroupExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
                return NotFound();

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(g => g.IdGroup == id);
        }
    }
} 