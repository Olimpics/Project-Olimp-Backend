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
        public async Task<ActionResult<IEnumerable<GroupFilterDto>>> GetGroups(
           [FromQuery] string? facultyIds,
           [FromQuery] string? departmentIds,
           [FromQuery] string? courses,
           [FromQuery] string? degreeLevelIds,
           [FromQuery] string? search = null)
        {
            var query = _context.Groups
                .Include(g => g.Students)
                .Include(g => g.Faculty)
                .Include(g => g.Department)
                .Include(g => g.Degree)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(g => EF.Functions.Like(g.GroupCode.ToLower(), $"%{lowerSearch}%"));
            }


            if (!string.IsNullOrWhiteSpace(facultyIds))
            {
                var facultyIdList = facultyIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var val) ? val : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (facultyIdList.Any())
                {
                    query = query.Where(g => g.FacultyId.HasValue && facultyIdList.Contains(g.FacultyId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(departmentIds))
            {
                var departmentIdList = departmentIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var val) ? val : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (departmentIdList.Any())
                {
                    query = query.Where(g => g.DepartmentId.HasValue && departmentIdList.Contains(g.DepartmentId.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(courses))
            {
                var courseList = courses
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var val) ? val : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (courseList.Any())
                {
                    query = query.Where(g => g.Course.HasValue && courseList.Contains(g.Course.Value));
                }
            }

            if (!string.IsNullOrWhiteSpace(degreeLevelIds))
            {
                var degreeLevelIdList = degreeLevelIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var val) ? val : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (degreeLevelIdList.Any())
                {
                    query = query.Where(g => g.DegreeId.HasValue && degreeLevelIdList.Contains(g.DegreeId.Value));
                }
            }

            var groups = await query.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<GroupFilterDto>>(groups));
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