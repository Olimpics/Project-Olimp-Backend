using System;

namespace OlimpBack.Application.DTO.Sg;

/// <summary>Lightweight reference item for SG event dropdowns (catalog years, regulations, roles).</summary>
public class SgRefItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}
