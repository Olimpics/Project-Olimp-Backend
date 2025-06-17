using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.DTO;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<object>> GetGroups(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null)
        {
            var query = _context.Groups
                .Include(g => g.Students)
                .AsQueryable();

            // Apply search filter for both idGroup and groupCode
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                
                // Check if search is a number (for ID search)
                if (int.TryParse(search.Trim(), out int searchId))
                {
                    // If search is a number, search by exact ID match
                    query = query.Where(g => g.IdGroup == searchId);
                }
                else
                {
                    // If search is not a number, search by group code
                    query = query.Where(g => EF.Functions.Like(g.GroupCode.ToLower(), $"%{lowerSearch}%"));
                }
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var groups = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = groups.Select(g => _mapper.Map<GroupDto>(g)).ToList();

            return Ok(new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = page,
                PageSize = pageSize,
                Items = result
            });
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