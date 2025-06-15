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
    public class BindLoansMainController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public BindLoansMainController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetBindLoansMain([FromQuery] BindLoansMainFilterDto filter)
        {
            var query = _context.BindLoansMains
                .Include(b => b.AddDisciplines)
                    .ThenInclude(d => d.FacultyNavigation)
                .Include(b => b.EducationalProgram)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchLower = filter.Search.Trim().ToLower();
                query = query.Where(b =>
                    EF.Functions.Like(b.AddDisciplines.NameAddDisciplines.ToLower(), $"%{searchLower}%") ||
                    EF.Functions.Like(b.EducationalProgram.NameEducationalProgram.ToLower(), $"%{searchLower}%"));
            }

            // Apply faculty filter
            if (!string.IsNullOrWhiteSpace(filter.Faculties))
            {
                var facultyValues = filter.Faculties.Split(',').Select(f => f.Trim()).ToList();
                var numericValues = facultyValues.Where(f => int.TryParse(f, out _)).Select(int.Parse).ToList();
                var textValues = facultyValues.Where(f => !int.TryParse(f, out _)).Select(f => f.ToLower()).ToList();

                if (numericValues.Any())
                {
                    query = query.Where(b => numericValues.Contains(b.AddDisciplines.Faculty));
                }

                if (textValues.Any())
                {
                    foreach (var val in textValues)
                    {
                        var temp = val; // Avoid closure issue
                        query = query.Where(b =>
                            EF.Functions.Like(b.AddDisciplines.FacultyNavigation.NameFaculty.ToLower(), $"%{temp}%") ||
                            EF.Functions.Like(b.AddDisciplines.FacultyNavigation.Abbreviation.ToLower(), $"%{temp}%"));
                    }
                }
            }

            // Apply speciality filter
            if (!string.IsNullOrWhiteSpace(filter.Specialities))
            {
                var specialityValues = filter.Specialities.Split(',').Select(s => s.Trim().ToLower()).ToList();
                query = query.Where(b => 
                    specialityValues.Any(s => 
                        EF.Functions.Like(b.EducationalProgram.Speciality.ToLower(), $"%{s}%")));
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new
            {
                Items = _mapper.Map<IEnumerable<BindLoansMainDto>>(items),
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize),
                Filters = new
                {
                    Faculties = filter.Faculties?.Split(',').Select(f => f.Trim()).ToList(),
                    Specialities = filter.Specialities?.Split(',').Select(s => s.Trim()).ToList()
                }
            };

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BindLoansMainDto>> GetBindLoansMain(int id)
        {
            var binding = await _context.BindLoansMains
                .Include(b => b.AddDisciplines)
                .Include(b => b.EducationalProgram)
                .FirstOrDefaultAsync(b => b.IdBindLoan == id);

            if (binding == null)
                return NotFound();

            return Ok(_mapper.Map<BindLoansMainDto>(binding));
        }

        [HttpPost]
        public async Task<ActionResult<BindLoansMainDto>> CreateBindLoansMain(CreateBindLoansMainDto dto)
        {
            var binding = _mapper.Map<BindLoansMain>(dto);
            _context.BindLoansMains.Add(binding);
            await _context.SaveChangesAsync();

            var resultDto = await GetBindLoansMainWithIncludes(binding.IdBindLoan);
            return CreatedAtAction(nameof(GetBindLoansMain), new { id = binding.IdBindLoan }, resultDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBindLoansMain(int id, UpdateBindLoansMainDto dto)
        {
            if (id != dto.IdBindLoan)
                return BadRequest();

            var binding = await _context.BindLoansMains.FindAsync(id);
            if (binding == null)
                return NotFound();

            _mapper.Map(dto, binding);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!BindLoansMainExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBindLoansMain(int id)
        {
            var binding = await _context.BindLoansMains.FindAsync(id);
            if (binding == null)
                return NotFound();

            _context.BindLoansMains.Remove(binding);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<BindLoansMainDto> GetBindLoansMainWithIncludes(int id)
        {
            var binding = await _context.BindLoansMains
                .Include(b => b.AddDisciplines)
                .Include(b => b.EducationalProgram)
                .FirstOrDefaultAsync(b => b.IdBindLoan == id);

            return _mapper.Map<BindLoansMainDto>(binding);
        }

        private bool BindLoansMainExists(int id)
        {
            return _context.BindLoansMains.Any(b => b.IdBindLoan == id);
        }
    }
} 