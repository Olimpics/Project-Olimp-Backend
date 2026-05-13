using AutoMapper;
using OlimpBack.Application.DTO.Conversations;
using OlimpBack.Models;

namespace OlimpBack.MappingProfiles;

public class ConversationProfile : Profile
{
    public ConversationProfile()
    {
        CreateMap<Conversation, ConversationResponse>();
        CreateMap<ConversationParticipant, ConversationParticipantDto>();
    }
}
