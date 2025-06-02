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
        public async Task<ActionResult<IEnumerable<BindLoansMainDto>>> GetBindLoansMain()
        {
            var bindings = await _context.BindLoansMains
                .Include(b => b.AddDisciplines)
                .Include(b => b.EducationalProgram)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<BindLoansMainDto>>(bindings));
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