using FluentValidation;
using OlimpBack.Application.DTO.Encryption;

namespace OlimpBack.Application.Validators.Encryption;

public class DeviceKeyUploadRequestValidator : AbstractValidator<DeviceKeyUploadRequest>
{
    public DeviceKeyUploadRequestValidator()
    {
        RuleFor(x => x.DeviceName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.IdentityKey).NotEmpty();
        RuleFor(x => x.SignedPreKey).NotNull();
        RuleFor(x => x.SignedPreKey.PublicKey).NotEmpty();
        RuleFor(x => x.SignedPreKey.Signature).NotEmpty();
    }
}

public class UploadPreKeysRequestValidator : AbstractValidator<UploadPreKeysRequest>
{
    public UploadPreKeysRequestValidator()
    {
        RuleFor(x => x.OneTimePreKeys).NotEmpty().Must(x => x.Count > 0);
    }
}

public class KeyRotationRequestValidator : AbstractValidator<KeyRotationRequest>
{
    public KeyRotationRequestValidator()
    {
        RuleFor(x => x.NewSignedPreKey).NotNull();
        RuleFor(x => x.NewSignedPreKey.PublicKey).NotEmpty();
        RuleFor(x => x.NewSignedPreKey.Signature).NotEmpty();
    }
}
