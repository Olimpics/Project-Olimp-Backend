using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Data;
using OlimpBack.DTO;
using OlimpBack.Models;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddDisciplineController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AddDisciplineController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FullDisciplineWithDetailsDto>>> GetAddDisciplines()
        {
            var disciplines = await _context.AddDisciplines
                .Include(d => d.DegreeLevel)
                .ToListAsync();

            var result = new List<FullDisciplineWithDetailsDto>();

            foreach (var discipline in disciplines)
            {
                var details = await _context.AddDetails
                    .Include(d => d.Department)
                    .FirstOrDefaultAsync(d => d.IdAddDetails == discipline.IdAddDisciplines);

                if (details != null)
                {
                    result.Add(_mapper.Map<FullDisciplineWithDetailsDto>((discipline, details)));
                }
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FullDisciplineWithDetailsDto>> GetAddDiscipline(int id)
        {
            var discipline = await _context.AddDisciplines
                .Include(d => d.DegreeLevel)
                .FirstOrDefaultAsync(d => d.IdAddDisciplines == id);

            if (discipline == null)
            {
                return NotFound("Discipline not found");
            }

            var details = await _context.AddDetails
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.IdAddDetails == id);

            if (details == null)
            {
                return NotFound("Discipline details not found");
            }

            var result = _mapper.Map<FullDisciplineWithDetailsDto>((discipline, details));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<FullDisciplineWithDetailsDto>> CreateAddDiscipline(CreateAddDisciplineWithDetailsDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var discipline = _mapper.Map<AddDiscipline>(dto);
                _context.AddDisciplines.Add(discipline);
                await _context.SaveChangesAsync();

                var details = _mapper.Map<AddDetail>(dto.Details);
                details.IdAddDetails = discipline.IdAddDisciplines;
                _context.AddDetails.Add(details);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var createdDiscipline = await _context.AddDisciplines
                    .Include(d => d.DegreeLevel)
                    .FirstOrDefaultAsync(d => d.IdAddDisciplines == discipline.IdAddDisciplines);

                var createdDetails = await _context.AddDetails
                    .Include(d => d.Department)
                    .FirstOrDefaultAsync(d => d.IdAddDetails == discipline.IdAddDisciplines);

                var result = _mapper.Map<FullDisciplineWithDetailsDto>((createdDiscipline, createdDetails));
                return CreatedAtAction(nameof(GetAddDiscipline), new { id = discipline.IdAddDisciplines }, result);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddDiscipline(int id, UpdateAddDisciplineWithDetailsDto dto)
        {
            if (id != dto.IdAddDisciplines)
                return BadRequest();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var discipline = await _context.AddDisciplines.FindAsync(id);
                if (discipline == null)
                    return NotFound("Discipline not found");

                var details = await _context.AddDetails.FindAsync(id);
                if (details == null)
                    return NotFound("Discipline details not found");

                _mapper.Map(dto, discipline);
                _mapper.Map(dto.Details, details);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddDiscipline(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var discipline = await _context.AddDisciplines.FindAsync(id);
                if (discipline == null)
                    return NotFound("Discipline not found");

                var details = await _context.AddDetails.FindAsync(id);
                if (details != null)
                {
                    _context.AddDetails.Remove(details);
                }

                _context.AddDisciplines.Remove(discipline);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private bool AddDisciplineExists(int id)
        {
            return _context.AddDisciplines.Any(e => e.IdAddDisciplines == id);
        }
    }
}