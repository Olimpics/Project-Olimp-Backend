using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;

namespace OlimpBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisciplineChoicePeriodController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DisciplineChoicePeriodController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<DisciplineChoicePeriodDto>> Create([FromBody] CreateDisciplineChoicePeriodDto dto)
        {
            var period = _mapper.Map<DisciplineChoicePeriod>(dto);
            _context.DisciplineChoicePeriods.Add(period);
            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<DisciplineChoicePeriodDto>(period));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateDisciplineChoicePeriodDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            var period = await _context.DisciplineChoicePeriods.FindAsync(id);
            if (period == null) return NotFound();

            _mapper.Map(dto, period);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var period = await _context.DisciplineChoicePeriods.FindAsync(id);
            if (period == null) return NotFound();

            _context.DisciplineChoicePeriods.Remove(period);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisciplineChoicePeriodDto>>> GetAll(
            [FromQuery] int? facultyId,
            [FromQuery] int? departmentId,
            [FromQuery] int? periodType,
            [FromQuery] int? levelType)
        {
            var query = _context.DisciplineChoicePeriods.AsQueryable();

            if (facultyId.HasValue)
                query = query.Where(p => p.FacultyId == facultyId);

            if (departmentId.HasValue)
                query = query.Where(p => p.DepartmentId == departmentId);

            if (periodType.HasValue)
                query = query.Where(p => p.PeriodType == periodType);

            if (levelType.HasValue)
                query = query.Where(p => p.LevelType == levelType);

            var periods = await query
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();

            return Ok(_mapper.Map<List<DisciplineChoicePeriodDto>>(periods));
        }
    }

}
