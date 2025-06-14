using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public FilterController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet("departments")]
        public async Task<ActionResult<IEnumerable<FiltersDepartmentDTO>>> GetDepartmentsForFilter()
        {
            var departments = await _context.Departments
                .Include(d => d.Faculty)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<FiltersDepartmentDTO>>(departments));
        }

        [HttpGet("specialities")]
        public async Task<ActionResult<IEnumerable<SpecialityFilterDto>>> GetSpecialities([FromQuery] string? search = null)
        {
            var query = _context.EducationalPrograms
                .GroupBy(ep => ep.Speciality)
                .Select(g => new SpecialityFilterDto
                {
                    Id = g.First().IdEducationalProgram,
                    Name = g.Key
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(s => 
                    EF.Functions.Like(s.Name.ToLower(), $"%{searchLower}%"));
            }

            var specialities = await query
                .OrderBy(s => s.Name)
                .ToListAsync();

            return Ok(specialities);
        }

        [HttpGet("groups")]
        public async Task<ActionResult<IEnumerable<GroupFilterDto>>> GetGroups([FromQuery] string? search = null)
        {
            var query = _context.Groups
                .Select(g => new GroupFilterDto
                {
                    Id = g.IdGroup,
                    Code = g.GroupCode,
                    StudentsCount = g.NumberOfStudents
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(g => 
                    EF.Functions.Like(g.Code.ToLower(), $"%{searchLower}%"));
            }

            var groups = await query
                .OrderBy(g => g.Code)
                .ToListAsync();

            return Ok(groups);
        }

        [HttpGet("notification-templates")]
        public async Task<ActionResult<IEnumerable<NotificationTemplateFilterDto>>> GetNotificationTemplates([FromQuery] string? search = null)
        {
            var query = _context.NotificationTemplates
                .Select(t => new NotificationTemplateFilterDto
                {
                    IdNotificationTemplates = t.IdNotificationTemplates,
                    NotificationType = t.NotificationType
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(t => 
                    EF.Functions.Like(t.NotificationType.ToLower(), $"%{searchLower}%"));
            }

            var templates = await query
                .OrderBy(t => t.NotificationType)
                .ToListAsync();

            return Ok(templates);
        }
    }
}
