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
      

    }
}
