using AutoMapper;
using OlimpBack.Models;

namespace OlimpBack.DTO
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ... existing code ...

            // Login mappings
            CreateMap<User, LoginResponseDto>()
                .ForMember(dest => dest.IdStudents, opt => opt.MapFrom(src => src.Student != null ? src.Student.IdStudents : null))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId));
        }
    }
} 