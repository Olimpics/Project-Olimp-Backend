using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlimpBack.Models;
using OlimpBack.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OlimpBack.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace OlimpBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BindLoansMainController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BindLoansMainController> _logger;

        public BindLoansMainController(AppDbContext context, IMapper mapper, ILogger<BindLoansMainController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetBindLoansMain(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] string? addDisciplinesIds = null,
            [FromQuery] string? educationalProgramIds = null,
            [FromQuery] int sortOrder = 0)
        {
            var query = _context.BindLoansMains
                .Include(b => b.AddDisciplines)
                .Include(b => b.EducationalProgram)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowerSearch = search.Trim().ToLower();
                query = query.Where(b =>
                    EF.Functions.Like(b.AddDisciplines.NameAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(b.AddDisciplines.CodeAddDisciplines.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(b.EducationalProgram.NameEducationalProgram.ToLower(), $"%{lowerSearch}%") ||
                    EF.Functions.Like(b.EducationalProgram.SpecialityCode.ToLower(), $"%{lowerSearch}%"));
            }

            // Apply addDisciplines filter
            if (!string.IsNullOrWhiteSpace(addDisciplinesIds))
            {
                var disciplineIdList = addDisciplinesIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var val) ? val : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (disciplineIdList.Any())
                {
                    query = query.Where(b => disciplineIdList.Contains(b.AddDisciplinesId));
                }
            }

            // Apply educationalProgram filter
            if (!string.IsNullOrWhiteSpace(educationalProgramIds))
            {
                var programIdList = educationalProgramIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => int.TryParse(id.Trim(), out var val) ? val : (int?)null)
                    .Where(id => id.HasValue)
                    .Select(id => id.Value)
                    .ToList();

                if (programIdList.Any())
                {
                    query = query.Where(b => programIdList.Contains(b.EducationalProgramId));
                }
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var bindings = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            bindings = sortOrder switch
            {
                1 => bindings.OrderByDescending(d => d.AddDisciplines.CodeAddDisciplines).ToList(),
                2 => bindings.OrderBy(d => d.EducationalProgram.SpecialityCode).ToList(),
                3 => bindings.OrderByDescending(d => d.EducationalProgram.SpecialityCode).ToList(),
                _ => bindings.OrderBy(d => d.AddDisciplines.CodeAddDisciplines).ToList()
            };
            var result = _mapper.Map<IEnumerable<BindLoansMainDto>>(bindings);

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
        public async Task<ActionResult<BindLoansMainDto>> GetBindLoansMain(int id)
        {
            var binding = await _context.BindLoansMains
                .Include(b => b.AddDisciplines)
                .Include(b => b.EducationalProgram)
                .FirstOrDefaultAsync(b => b.IdBindLoan == id);

            if (binding == null)
            {
                return NotFound("Binding not found");
            }

            return Ok(_mapper.Map<BindLoansMainDto>(binding));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BindLoansMainDto>> CreateBindLoansMain(CreateBindLoansMainDto dto)
        {
            var binding = _mapper.Map<BindLoansMain>(dto);
            _context.BindLoansMains.Add(binding);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<BindLoansMainDto>(binding);
            return CreatedAtAction(nameof(GetBindLoansMain), new { id = binding.IdBindLoan }, result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBindLoansMain(int id, UpdateBindLoansMainDto dto)
        {
            if (id != dto.IdBindLoan)
            {
                return BadRequest();
            }

            var binding = await _context.BindLoansMains.FindAsync(id);
            if (binding == null)
            {
                return NotFound("Binding not found");
            }

            _mapper.Map(dto, binding);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BindLoansMainExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBindLoansMain(int id)
        {
            var binding = await _context.BindLoansMains.FindAsync(id);
            if (binding == null)
            {
                return NotFound("Binding not found");
            }

            _context.BindLoansMains.Remove(binding);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BindLoansMainExists(int id)
        {
            return _context.BindLoansMains.Any(e => e.IdBindLoan == id);
        }
    }
} 