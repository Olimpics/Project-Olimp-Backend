using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OlimpBack.Application.DTO;
using OlimpBack.Infrastructure.Database.Repositories;
using OlimpBack.Models;

namespace OlimpBack.Application.Services
{

    public interface ITypeOfDisciplineService
    {
        Task<IEnumerable<TypeOfDisciplineDto>> GetAllAsync();
        Task<TypeOfDisciplineDto?> GetByIdAsync(int id);
        Task<TypeOfDisciplineDto> CreateAsync(CreateTypeOfDisciplineDto dto);
        Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, TypeOfDisciplineDto dto);
        Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id);
    }

    public class TypeOfDisciplineService : ITypeOfDisciplineService
    {
        private readonly ITypeOfDisciplineRepository _repository;
        private readonly IMapper _mapper;
        public TypeOfDisciplineService(ITypeOfDisciplineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<TypeOfDisciplineDto>> GetAllAsync()
        {
            return await _repository.GetAllDtoAsync();
        }
        public async Task<TypeOfDisciplineDto?> GetByIdAsync(int id)
        {
            return await _repository.GetDtoByIdAsync(id);
        }
        public async Task<TypeOfDisciplineDto> CreateAsync(CreateTypeOfDisciplineDto dto)
        {
            var entity = _mapper.Map<TypeOfDiscipline>(dto);

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            var resultDto = await _repository.GetDtoByIdAsync(entity.IdTypeOfDiscipline);
            return resultDto ?? _mapper.Map<TypeOfDisciplineDto>(entity);
        }
        public async Task<(bool success, int statusCode, string? errorMessage)> UpdateAsync(int id, TypeOfDisciplineDto dto)
        {
            if (id != dto.IdTypeOfDiscipline)
                return (false, StatusCodes.Status400BadRequest, "Route ID does not match DTO ID.");

            var entity = await _repository.GetEntityByIdAsync(id);
            if (entity == null)
                return (false, StatusCodes.Status404NotFound, "Type of discipline not found.");
            _mapper.Map(dto, entity);

            try
            {
                await _repository.SaveChangesAsync();
                return (true, StatusCodes.Status204NoContent, null);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repository.ExistsAsync(id))
                    return (false, StatusCodes.Status404NotFound, "Type of discipline not found during update.");
                
                throw;
            }

            return (true, StatusCodes.Status204NoContent, null);
        }

        public async Task<(bool success, int statusCode, string? errorMessage)> DeleteAsync(int id)
        {
            var deletedRows = await _repository.DeleteAsync(id);

            if (deletedRows == 0)
                return (false, StatusCodes.Status404NotFound, "Normative not found.");

            return (true, StatusCodes.Status204NoContent, null);
        }
    }
}