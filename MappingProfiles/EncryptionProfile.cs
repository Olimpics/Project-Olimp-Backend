using AutoMapper;
using OlimpBack.Application.DTO.Encryption;
using OlimpBack.Models;

namespace OlimpBack.MappingProfiles;

public class EncryptionProfile : Profile
{
    public EncryptionProfile()
    {
        CreateMap<UserDevice, DeviceKeyResponse>();
        CreateMap<UserDevice, KeyBundleResponse>()
            .ForMember(dest => dest.DeviceId, opt => opt.MapFrom(src => src.IdUserDevices))
            .ForMember(dest => dest.SignedPreKey, opt => opt.MapFrom(src => new SignedPreKeyDto
            {
                KeyId = src.SignedPreKeyId,
                PublicKey = src.SignedPreKey,
                Signature = src.SignedPreKeySignature
            }));
        
        CreateMap<PreKey, PreKeyDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdPreKeys))
            .ForMember(dest => dest.PublicKey, opt => opt.MapFrom(src => src.PublicPreKey));
    }
}
