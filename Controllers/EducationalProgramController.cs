using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.DTO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using OlimpBack.Utils;
using System;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationalProgramController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EducationalProgramController> _logger;

        public EducationalProgramController(
            AppDbContext context,
            IMapper mapper,
            ILogger<EducationalProgramController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetEducationalPrograms(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] string? degreeLevelIds = null,
            [FromQuery] int sortOrder = 0)
        {
            var query = _context.EducationalPrograms
                .Include(ep => ep.Degree)
                .Include(ep => ep.Students)
                .Include(ep => ep.BindMainDisciplines)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(ep =>
                    EF.Functions.Like(ep.NameEducationalProgram.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(ep.SpecialityCode.ToLower(), $"%{lowerSearch}%"));
            }

            // Apply degree level filter
            if (!string.IsNullOrWhiteSpace(degreeLevelIds))
            {
                var degreeLevelIdList = degreeLevelIds.Split(',').Select(int.Parse).ToList();
                query = query.Where(ep => degreeLevelIdList.Contains(ep.DegreeId));
            }

            // Apply sorting
            query = sortOrder switch
            {
                1 => query.OrderBy(ep => ep.NameEducationalProgram),
                2 => query.OrderByDescending(ep => ep.NameEducationalProgram),
                3 => query.OrderByDescending(ep => ep.SpecialityCode),
                4 => query.OrderBy(ep => ep.StudentsAmount),
                5 => query.OrderByDescending(ep => ep.StudentsAmount),
                _ => query.OrderBy(ep => ep.SpecialityCode)
            };

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var programs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = programs.Select(ep => _mapper.Map<EducationalProgramDto>(ep)).ToList();

            // Add count of students for each program
            foreach (var program in result)
            {
                program.StudentsCount = programs
                    .First(p => p.IdEducationalProgram == program.IdEducationalProgram)
                    .Students.Count;
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
        public async Task<ActionResult<EducationalProgramDto>> GetEducationalProgram(int id)
        {
            var program = await _context.EducationalPrograms
                .Include(ep => ep.Degree)
                .Include(ep => ep.Students)
                .Include(ep => ep.BindMainDisciplines)
                .FirstOrDefaultAsync(ep => ep.IdEducationalProgram == id);

            if (program == null)
            {
                return NotFound("Educational program not found");
            }

            var result = _mapper.Map<EducationalProgramDto>(program);
            result.StudentsCount = program.Students.Count;

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<EducationalProgramDto>> CreateEducationalProgram(CreateEducationalProgramDto dto)
        {
            var program = _mapper.Map<EducationalProgram>(dto);
            program.DegreeId = int.Parse(dto.Degree);

            _context.EducationalPrograms.Add(program);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<EducationalProgramDto>(program);
            result.StudentsCount = 0; // New program has no students yet

            return CreatedAtAction(nameof(GetEducationalProgram), new { id = program.IdEducationalProgram }, result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducationalProgram(int id, CreateEducationalProgramDto dto)
        {
            var program = await _context.EducationalPrograms.FindAsync(id);
            if (program == null)
            {
                return NotFound("Educational program not found");
            }

            _mapper.Map(dto, program);
            program.DegreeId = int.Parse(dto.Degree);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EducationalProgramExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducationalProgram(int id)
        {
            var program = await _context.EducationalPrograms.FindAsync(id);
            if (program == null)
            {
                return NotFound("Educational program not found");
            }

            _context.EducationalPrograms.Remove(program);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EducationalProgramExists(int id)
        {
            return _context.EducationalPrograms.Any(e => e.IdEducationalProgram == id);
        }
    }
}