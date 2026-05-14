using FluentValidation;
using OlimpBack.Application.DTO;

namespace OlimpBack.Application.Validators;

public class CreateTransferSessionRequestValidator : AbstractValidator<CreateTransferSessionRequest>
{
    public CreateTransferSessionRequestValidator()
    {
        RuleFor(x => x.OldDeviceId).NotEmpty();
        RuleFor(x => x.OldDevicePublicKey).NotEmpty();
    }
}

public class JoinTransferSessionRequestValidator : AbstractValidator<JoinTransferSessionRequest>
{
    public JoinTransferSessionRequestValidator()
    {
        RuleFor(x => x.TransferCode).NotEmpty().Length(8);
        RuleFor(x => x.NewDeviceId).NotEmpty();
        RuleFor(x => x.NewDevicePublicKey).NotEmpty();
    }
}

public class UploadTransferPayloadRequestValidator : AbstractValidator<UploadTransferPayloadRequest>
{
    public UploadTransferPayloadRequestValidator()
    {
        RuleFor(x => x.TransferSessionToken).NotEmpty();
        RuleFor(x => x.EncryptedTransferPayload).NotEmpty();
    }
}
