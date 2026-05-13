using FluentValidation;
using OlimpBack.Application.DTO.Conversations;

namespace OlimpBack.Application.Validators.Conversations;

public class CreateConversationRequestValidator : AbstractValidator<CreateConversationRequest>
{
    public CreateConversationRequestValidator()
    {
        RuleFor(x => x.ParticipantIds)
            .NotEmpty().WithMessage("Participants list cannot be empty.")
            .Must(x => x.Count > 0).WithMessage("At least one participant must be specified.");
    }
}

public class RevealIdentityRequestValidator : AbstractValidator<RevealIdentityRequest>
{
    public RevealIdentityRequestValidator()
    {
        RuleFor(x => x.ConversationId)
            .NotEmpty().WithMessage("Conversation ID is required.");
    }
}
