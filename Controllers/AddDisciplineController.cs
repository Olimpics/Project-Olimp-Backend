using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;
using OlimpBack.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OlimpBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddDisciplineController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AddDisciplineController> _logger;

        public AddDisciplineController(
            AppDbContext context,
            IMapper mapper,
            ILogger<AddDisciplineController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("GetAllDisciplines")]
        public async Task<ActionResult<object>> GetAllDisciplines(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] string? faculties = null,
            [FromQuery] string? courses = null,
            [FromQuery] bool? isEvenSemester = null,
            [FromQuery] string? degreeLevelIds = null,
            [FromQuery] int sortOrder = 0)
        {
            var query = _context.AddDisciplines
                .Include(d => d.DegreeLevel)
                .Include(d => d.Faculty)
                .Include(d => d.BindAddDisciplines)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(d =>
                    EF.Functions.Like(d.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(d.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%"));
            }

            // Apply faculty filter
            if (!string.IsNullOrWhiteSpace(faculties))
            {
                var facultyIds = faculties
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.Parse(id.Trim()))
                    .ToList();

                query = query.Where(d => facultyIds.Contains(d.FacultyId));
            }

            // Apply course filter
            if (!string.IsNullOrWhiteSpace(courses))
            {
                var courseList = courses.Split(',').Select(int.Parse).ToList();
                query = query.Where(d =>
                    (!d.MinCourse.HasValue || courseList.Contains(d.MinCourse.Value)) &&
                    (!d.MaxCourse.HasValue || courseList.Contains(d.MaxCourse.Value)));
            }

            // Apply semester filter
            if (isEvenSemester.HasValue)
            {
                query = query.Where(d => d.AddSemestr.HasValue && 
                    ((isEvenSemester.Value && d.AddSemestr.Value % 2 == 0) || 
                     (!isEvenSemester.Value && d.AddSemestr.Value % 2 == 1)));
            }

            // Apply degree level filter
            if (!string.IsNullOrWhiteSpace(degreeLevelIds))
            {
                var degreeLevelIdList = degreeLevelIds.Split(',').Select(int.Parse).ToList();
                query = query.Where(d => d.DegreeLevelId.HasValue && degreeLevelIdList.Contains(d.DegreeLevelId.Value));
            }

            // Apply sorting
            query = sortOrder switch
            {
                1 => query.OrderBy(d => d.NameAddDisciplines),
                2 => query.OrderByDescending(d => d.NameAddDisciplines),
                3 => query.OrderBy(d => d.CodeAddDisciplines),
                4 => query.OrderByDescending(d => d.CodeAddDisciplines),
                _ => query.OrderBy(d => d.IdAddDisciplines)
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var disciplines = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = disciplines.Select(d => _mapper.Map<FullDisciplineDto>(d)).ToList();

            // Add count of people for each discipline
            foreach (var discipline in result)
            {
                discipline.CountOfPeople = disciplines
                    .First(d => d.IdAddDisciplines == discipline.IdAddDisciplines)
                    .BindAddDisciplines.Count;
            }

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
        public async Task<ActionResult<FullDisciplineDto>> GetAddDiscipline(int id)
        {
            var discipline = await _context.AddDisciplines
                .Include(d => d.DegreeLevel)
                .Include(d => d.Faculty)
                .Include(d => d.BindAddDisciplines)
                .FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

            if (discipline == null)
            {
                return NotFound("Discipline not found");
            }

            var result = _mapper.Map<FullDisciplineDto>(discipline);
            result.CountOfPeople = discipline.BindAddDisciplines.Count;

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<FullDisciplineDto>> CreateAddDiscipline(CreateAddDisciplineDto dto)
        {
            var discipline = _mapper.Map<AddDiscipline>(dto);
            discipline.FacultyId = dto.FacultyId;
            discipline.DegreeLevelId = dto.DegreeLevelId;
            discipline.AddSemestr = sbyte.Parse(dto.AddSemestr ?? "0");

            _context.AddDisciplines.Add(discipline);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<FullDisciplineDto>(discipline);
            result.CountOfPeople = 0; // New discipline has no students yet

            return CreatedAtAction(nameof(GetAddDiscipline), new { id = discipline.IdAddDisciplines }, result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddDiscipline(int id, CreateAddDisciplineDto dto)
        {
            var discipline = await _context.AddDisciplines.FindAsync(id);
            if (discipline == null)
            {
                return NotFound("Discipline not found");
            }

            _mapper.Map(dto, discipline);
            discipline.FacultyId = dto.FacultyId;
            discipline.DegreeLevelId = dto.DegreeLevelId;
            discipline.AddSemestr = sbyte.Parse(dto.AddSemestr ?? "0");

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddDisciplineExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddDiscipline(int id)
        {
            var discipline = await _context.AddDisciplines.FindAsync(id);
            if (discipline == null)
            {
                return NotFound("Discipline not found");
            }

            _context.AddDisciplines.Remove(discipline);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddDisciplineExists(int id)
        {
            return _context.AddDisciplines.Any(e => e.IdAddDisciplines == id);
        }
    }
}