using System;
using System.Collections.Generic;

namespace OlimpBack.Application.DTO.Sg;

/// <summary>One additional-points regulation/standard row (Нормативи table).</summary>
public class RegulationStandardDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string? SubType { get; set; }
    public int AmountMin { get; set; }
    public int AmountMax { get; set; }
    public string? Notes { get; set; }
}

/// <summary>One earned additional-points entry for a student (from event participation).</summary>
public class StudentPointItemDto
{
    public Guid EventId { get; set; }
    public string EventName { get; set; } = null!;
    public DateOnly Date { get; set; }
    public int Point { get; set; }
    public string RoleName { get; set; } = null!;
    public string? ActivityType { get; set; }
}

public class StudentPointsDto
{
    public Guid StudentId { get; set; }
    public int Total { get; set; }
    public List<StudentPointItemDto> Items { get; set; } = new();
}
