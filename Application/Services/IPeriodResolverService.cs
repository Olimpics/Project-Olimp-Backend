using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

/// <summary>
/// Result of resolving the single active discipline-choice period for a student.
/// </summary>
public class PeriodResolutionResult
{
    public DisciplineChoicePeriod? Period { get; set; }

    /// <summary>True when more than one equally-suitable period matched — a configuration error.</summary>
    public bool HasConflict { get; set; }

    public List<Guid> ConflictingPeriodIds { get; set; } = new();

    public static PeriodResolutionResult None() => new();
    public static PeriodResolutionResult Found(DisciplineChoicePeriod period) => new() { Period = period };
    public static PeriodResolutionResult Conflict(List<Guid> ids) => new() { HasConflict = true, ConflictingPeriodIds = ids };
}

public interface IPeriodResolverService
{
    /// <summary>
    /// Resolves the one open, confirmed period a student may currently select from.
    /// Returns a conflict result when the period configuration is ambiguous.
    /// </summary>
    Task<PeriodResolutionResult> ResolveActivePeriodAsync(Guid studentId);

    /// <summary>Returns all students that fall under the given period's filters.</summary>
    Task<List<Student>> GetStudentsForPeriodAsync(DisciplineChoicePeriod period);
}
