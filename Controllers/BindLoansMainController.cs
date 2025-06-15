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

        [HttpGet("with-details/{id}")]
        public async Task<ActionResult<BindLoansMainWithDetailsDto>> GetBindLoansMainWithDetails(int id)
        {
            var binding = await _context.BindLoansMains
                .Include(b => b.AddDisciplines)
                    .ThenInclude(d => d.FacultyNavigation)
                .Include(b => b.EducationalProgram)
                .FirstOrDefaultAsync(b => b.IdBindLoan == id);

            if (binding == null)
            {
                return NotFound();
            }

            var details = await _context.AddDetails
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.IdAddDetails == binding.AddDisciplinesId);

            if (details == null)
            {
                return NotFound("Details not found for this binding");
            }

            var dto = new BindLoansMainWithDetailsDto
            {
                IdBindLoan = binding.IdBindLoan,
                AddDisciplinesId = binding.AddDisciplinesId,
                EducationalProgramId = binding.EducationalProgramId,
                AddDisciplineName = binding.AddDisciplines.NameAddDisciplines,
                EducationalProgramName = binding.EducationalProgram.NameEducationalProgram,
                Faculty = binding.AddDisciplines.FacultyNavigation.NameFaculty,
                Speciality = binding.EducationalProgram.Speciality,
                DepartmentName = details.Department.NameDepartment,
                Teacher = details.Teacher,
                Recomend = details.Recomend,
                Prerequisites = details.Prerequisites,
                Language = details.Language,
                Determination = details.Determination,
                WhyInterestingDetermination = details.WhyInterestingDetermination,
                ResultEducation = details.ResultEducation,
                UsingIrl = details.UsingIrl,
                AdditionaLiterature = details.AdditionaLiterature,
                TypesOfTraining = details.TypesOfTraining,
                TypeOfControll = details.TypeOfControll
            };

            return dto;
        }

        [HttpPost("with-details")]
        public async Task<ActionResult<BindLoansMainWithDetailsDto>> CreateBindLoansMainWithDetails(CreateBindLoansMainWithDetailsDto dto)
        {
            // Check if discipline exists
            var discipline = await _context.AddDisciplines
                .Include(d => d.FacultyNavigation)
                .FirstOrDefaultAsync(d => d.IdAddDisciplines == dto.AddDisciplinesId);
            if (discipline == null)
            {
                return NotFound("Discipline not found");
            }

            // Check if educational program exists
            var program = await _context.EducationalPrograms
                .FirstOrDefaultAsync(p => p.IdEducationalProgram == dto.EducationalProgramId);
            if (program == null)
            {
                return NotFound("Educational program not found");
            }

            // Create new binding
            var binding = new BindLoansMain
            {
                AddDisciplinesId = dto.AddDisciplinesId,
                EducationalProgramId = dto.EducationalProgramId
            };

            _context.BindLoansMains.Add(binding);
            await _context.SaveChangesAsync();

            // Create details
            var details = new AddDetail
            {
                IdAddDetails = dto.AddDisciplinesId,
                DepartmentId = dto.Details.DepartmentId,
                Teacher = dto.Details.Teacher,
                Recomend = dto.Details.Recomend,
                Prerequisites = dto.Details.Prerequisites,
                Language = dto.Details.Language,
                Determination = dto.Details.Determination,
                WhyInterestingDetermination = dto.Details.WhyInterestingDetermination,
                ResultEducation = dto.Details.ResultEducation,
                UsingIrl = dto.Details.UsingIrl,
                AdditionaLiterature = dto.Details.AdditionaLiterature,
                TypesOfTraining = dto.Details.TypesOfTraining,
                TypeOfControll = dto.Details.TypeOfControll
            };

            _context.AddDetails.Add(details);
            await _context.SaveChangesAsync();

            // Return created binding with details
            return await GetBindLoansMainWithDetails(binding.IdBindLoan);
        }

        [HttpPut("with-details/{id}")]
        public async Task<IActionResult> UpdateBindLoansMainWithDetails(int id, UpdateBindLoansMainWithDetailsDto dto)
        {
            if (id != dto.IdBindLoan)
            {
                return BadRequest();
            }

            // Check if binding exists
            var binding = await _context.BindLoansMains
                .Include(b => b.AddDisciplines)
                    .ThenInclude(d => d.FacultyNavigation)
                .Include(b => b.EducationalProgram)
                .FirstOrDefaultAsync(b => b.IdBindLoan == id);

            if (binding == null)
            {
                return NotFound();
            }

            // Update binding
            binding.AddDisciplinesId = dto.AddDisciplinesId;
            binding.EducationalProgramId = dto.EducationalProgramId;

            // Update or create details
            var details = await _context.AddDetails
                .FirstOrDefaultAsync(d => d.IdAddDetails == dto.AddDisciplinesId);

            if (details == null)
            {
                details = new AddDetail
                {
                    IdAddDetails = dto.AddDisciplinesId
                };
                _context.AddDetails.Add(details);
            }

            details.DepartmentId = dto.Details.DepartmentId;
            details.Teacher = dto.Details.Teacher;
            details.Recomend = dto.Details.Recomend;
            details.Prerequisites = dto.Details.Prerequisites;
            details.Language = dto.Details.Language;
            details.Determination = dto.Details.Determination;
            details.WhyInterestingDetermination = dto.Details.WhyInterestingDetermination;
            details.ResultEducation = dto.Details.ResultEducation;
            details.UsingIrl = dto.Details.UsingIrl;
            details.AdditionaLiterature = dto.Details.AdditionaLiterature;
            details.TypesOfTraining = dto.Details.TypesOfTraining;
            details.TypeOfControll = dto.Details.TypeOfControll;

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

        [HttpDelete("with-details/{id}")]
        public async Task<IActionResult> DeleteBindLoansMainWithDetails(int id)
        {
            var binding = await _context.BindLoansMains
                .FirstOrDefaultAsync(b => b.IdBindLoan == id);

            if (binding == null)
            {
                return NotFound();
            }

            // Delete details if they exist
            var details = await _context.AddDetails
                .FirstOrDefaultAsync(d => d.IdAddDetails == binding.AddDisciplinesId);

            if (details != null)
            {
                _context.AddDetails.Remove(details);
            }

            // Delete binding
            _context.BindLoansMains.Remove(binding);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
} 