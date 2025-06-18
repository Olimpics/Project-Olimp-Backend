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
            var data = await _context.EducationalPrograms
                .Select(ep => new
                {
                    ep.IdEducationalProgram,
                    ep.NameEducationalProgram,
                    ep.Speciality
                })
                .ToListAsync();

            var grouped = data
                .GroupBy(ep => ep.NameEducationalProgram)
                .Select(g => new
                {
                    Id = g.First().IdEducationalProgram,
                    NameEP = g.Key,
                    NameSpeciality = g.First().Speciality 
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                grouped = grouped.Where(s =>
                    s.NameEP.ToLower().Contains(searchLower) ||
                    s.NameSpeciality.ToLower().Contains(searchLower));
            }

            var result = grouped
                .Select(s => new SpecialityFilterDto
                {
                    Id = s.Id,
                    NameEP = s.NameEP
                })
                .OrderBy(s => s.NameEP)
                .ToList();

            return Ok(result);
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

        [HttpGet("add-disciplines")]
        public async Task<ActionResult<IEnumerable<SpecialityFilterDto>>> GetAddDisciplines([FromQuery] string? search = null)
        {
            var query = _context.AddDisciplines
                .Select(ad => new SpecialityFilterDto
                {
                    Id = ad.IdAddDisciplines,
                    NameEP = ad.NameAddDisciplines
                });

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.Trim().ToLower();
                query = query.Where(ad => 
                    EF.Functions.Like(ad.NameEP.ToLower(), $"%{searchLower}%"));
            }

            var disciplines = await query
                .OrderBy(ad => ad.NameEP)
                .ToListAsync();

            return Ok(disciplines);
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
