using FluentValidation;
using OlimpBack.Application.DTO.Messages;

namespace OlimpBack.Application.Validators.Messages;

public class SendEncryptedMessageRequestValidator : AbstractValidator<SendEncryptedMessageRequest>
{
    public SendEncryptedMessageRequestValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.SenderDevicePublicKey).NotEmpty();
        RuleFor(x => x.EncryptedPayload).NotEmpty();
        RuleFor(x => x.Nonce).NotEmpty();
    }
}

public class MessageCursorPaginationRequestValidator : AbstractValidator<MessageCursorPaginationRequest>
{
    public MessageCursorPaginationRequestValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.Limit).InclusiveBetween(1, 100);
    }
}

public class SyncMessagesRequestValidator : AbstractValidator<SyncMessagesRequest>
{
    public SyncMessagesRequestValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.Since).NotEmpty();
    }
}
