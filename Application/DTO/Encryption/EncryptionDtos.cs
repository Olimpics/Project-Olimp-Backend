using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO.Encryption;

public class DeviceKeyUploadRequest
{
    public string DeviceName { get; set; } = null!;
    public string IdentityKey { get; set; } = null!;
    public SignedPreKeyDto SignedPreKey { get; set; } = null!;
    public List<PreKeyDto> OneTimePreKeys { get; set; } = new();
}

public class SignedPreKeyDto
{
    public int KeyId { get; set; }
    public string PublicKey { get; set; } = null!;
    public string Signature { get; set; } = null!;
}

public class PreKeyDto
{
    public Guid Id { get; set; }
    public string PublicKey { get; set; } = null!;
}

public class DeviceKeyResponse
{
    public Guid DeviceId { get; set; }
    public string IdentityKey { get; set; } = null!;
}

public class KeyBundleResponse
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
    public string IdentityKey { get; set; } = null!;
    public SignedPreKeyDto SignedPreKey { get; set; } = null!;
    public PreKeyDto? OneTimePreKey { get; set; }
}

public class UploadPreKeysRequest
{
    public List<PreKeyDto> OneTimePreKeys { get; set; } = new();
}

public class KeyRotationRequest
{
    public SignedPreKeyDto NewSignedPreKey { get; set; } = null!;
}
