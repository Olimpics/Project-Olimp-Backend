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
    public class UniversityController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UniversityController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartmentsByFaculty([FromQuery] string? facultyIds)
        {
            var query = _context.Departments
                .Include(d => d.Faculty)
                .AsQueryable();

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
                    query = query.Where(d => facultyIdList.Contains(d.FacultyId));
                }
            }

            var departments = await query.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<DepartmentDto>>(departments));
        }
        [HttpGet("groups")]
        public async Task<ActionResult<IEnumerable<GroupFilterDto>>> GetGroups(
            [FromQuery] string? facultyIds,
            [FromQuery] string? departmentIds,
            [FromQuery] string? courses,
            [FromQuery] string? degreeLevelIds)
        {
            var query = _context.Groups
                .Include(g => g.Students)
                .Include(g => g.Faculty)
                .Include(g => g.Department)
                .Include(g => g.Degree)
                .AsQueryable();

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

    }
}
