using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.DTO;
using System.Linq;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationalProgramController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EducationalProgramController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EducationalProgramDto>>> GetEducationalPrograms()
        {
            var programs = await _context.EducationalPrograms
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<EducationalProgramDto>>(programs));
        }

        [HttpGet("FilterEducationalProgram")]
        public async Task<ActionResult<object>> FilterEducationalProgram(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] int? degreeId = null,
            [FromQuery] string? search = null)
        {
            var query = _context.EducationalPrograms.AsQueryable();

            // Apply degreeId filter
            if (degreeId.HasValue)
            {
                query = query.Where(ep => ep.DegreeId == degreeId.Value);
            }

            // Apply search filter for both idEducationalProgram and nameEducationalProgram
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(ep =>
                    EF.Functions.Like(ep.NameEducationalProgram.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(ep.IdEducationalProgram.ToString().ToLower(), $"%{lowerSearch}%"));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var programs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = programs.Select(ep => new EducationalProgramDto
            {
                IdEducationalProgram = ep.IdEducationalProgram,
                NameEducationalProgram = ep.NameEducationalProgram,
                DegreeId = ep.DegreeId,
                Speciality = ep.Speciality,
                Accreditation = ep.Accreditation,
                AccreditationType = ep.AccreditationType,
                StudentsAmount = ep.StudentsAmount,
                StudentsCount = 0, // Will be calculated separately if needed
                DisciplinesCount = 0 // Will be calculated separately if needed
            }).ToList();

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
                .Include(d => d.Degree)
                .FirstOrDefaultAsync(p => p.IdEducationalProgram == id);

            if (program == null)
                return NotFound();

            return Ok(_mapper.Map<EducationalProgramDto>(program));
        }

        [HttpPost]
        public async Task<ActionResult<EducationalProgramDto>> CreateEducationalProgram(CreateEducationalProgramDto dto)
        {
            var entity = _mapper.Map<EducationalProgram>(dto);
            _context.EducationalPrograms.Add(entity);
            await _context.SaveChangesAsync();

            var resultDto = _mapper.Map<EducationalProgramDto>(entity);
            return CreatedAtAction(nameof(GetEducationalProgram), new { id = entity.IdEducationalProgram }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEducationalProgram(int id, UpdateEducationalProgramDto dto)
        {
            if (id != dto.IdEducationalProgram)
                return BadRequest();

            var entity = _mapper.Map<EducationalProgram>(dto);
            _context.Entry(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EducationalProgramExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEducationalProgram(int id)
        {
            var program = await _context.EducationalPrograms.FindAsync(id);
            if (program == null)
                return NotFound();

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