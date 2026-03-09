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
        [HttpPut("UpdateAfterStart")]
        public async Task<ActionResult> UpdateAfterStart(int id, [FromBody] UpdateDisciplineChoicePeriodAfterStartDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            var period = await _context.DisciplineChoicePeriods.FindAsync(id);
            if (period == null) return NotFound();

            _mapper.Map(dto, period);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("OpenOrClose")]
        public async Task<ActionResult> OpenOrClose(int id, [FromBody] UpdateDisciplineChoicePeriodOpenOrCloseDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            var period = await _context.DisciplineChoicePeriods.FindAsync(id);
            if (period == null) return NotFound();

            _mapper.Map(dto, period);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisciplineChoicePeriodDto>>> GetAll(
     [FromQuery] GetDisciplineChoicePeriodsQueryDto queryDto)
        {
            var periods = await _context.DisciplineChoicePeriods
                .Where(p =>
                    (!queryDto.FacultyId.HasValue || p.FacultyId == queryDto.FacultyId) &&
                    (!queryDto.DepartmentId.HasValue || p.DepartmentId == queryDto.DepartmentId) &&
                    (!queryDto.DegreeLevelId.HasValue || p.DegreeLevelId == queryDto.DegreeLevelId) &&
                    (!queryDto.PeriodType.HasValue || p.PeriodType == queryDto.PeriodType) &&
                    (!queryDto.IsClose.HasValue || p.IsClose == queryDto.IsClose) &&
                    (!queryDto.PeriodCourse.HasValue || p.PeriodCourse == queryDto.PeriodCourse)
                )
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
            return Ok(_mapper.Map<List<DisciplineChoicePeriodDto>>(periods));
        }
    }

}
