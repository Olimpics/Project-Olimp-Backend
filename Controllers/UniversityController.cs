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
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartmentsByFaculty([FromQuery] int[]? facultyIds)
        {
            var query = _context.Departments
                .Include(d => d.Faculty)
                .AsQueryable();

            if (facultyIds != null && facultyIds.Length > 0)
            {
                query = query.Where(d => facultyIds.Contains(d.FacultyId));
            }

            var departments = await query.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<DepartmentDto>>(departments));
        }

        [HttpGet("groups")]
        public async Task<ActionResult<IEnumerable<GroupFilterDto>>> GetGroups(
            [FromQuery] int[]? facultyIds,
            [FromQuery] int[]? educationalProgramIds,
            [FromQuery] int[]? courses)
        {
            var query = _context.Groups
                .Include(g => g.Students)
                .AsQueryable();

            if (facultyIds != null && facultyIds.Length > 0)
            {
                query = query.Where(g => g.Students.Any(s => facultyIds.Contains(s.FacultyId)));
            }

            if (educationalProgramIds != null && educationalProgramIds.Length > 0)
            {
                query = query.Where(g => g.Students.Any(s => educationalProgramIds.Contains(s.EducationalProgramId)));
            }

            if (courses != null && courses.Length > 0)
            {
                query = query.Where(g => g.Students.Any(s => courses.Contains(s.Course)));
            }

            var groups = await query.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<GroupFilterDto>>(groups));
        }
    }
}
