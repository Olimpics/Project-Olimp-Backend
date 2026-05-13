using AutoMapper;
using OlimpBack.Application.DTO.Messages;
using OlimpBack.Models;

namespace OlimpBack.MappingProfiles;

public class MessageProfile : Profile
{
    public MessageProfile()
    {
        CreateMap<Message, EncryptedMessageResponse>();
        CreateMap<Message, RealtimeEncryptedMessageDto>();
    }
}
