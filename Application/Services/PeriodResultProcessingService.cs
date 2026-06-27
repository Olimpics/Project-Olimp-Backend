using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OlimpBack.Data;
using OlimpBack.Models;

namespace OlimpBack.Application.Services;

/// <summary>
/// Capacity-based distribution of discipline choices once a period's check window ends.
/// Replaces the previous hard-coded "mark accepted" behaviour.
/// </summary>
public class PeriodResultProcessingService : IPeriodResultProcessingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PeriodResultProcessingService> _logger;
    private readonly IPeriodCacheService _periodCacheService;
    private readonly IDisciplineCacheService _disciplineCacheService;

    public PeriodResultProcessingService(
        AppDbContext context,
        ILogger<PeriodResultProcessingService> logger,
        IPeriodCacheService periodCacheService,
        IDisciplineCacheService disciplineCacheService)
    {
        _context = context;
        _logger = logger;
        _periodCacheService = periodCacheService;
        _disciplineCacheService = disciplineCacheService;
    }

    public async Task<int> ProcessDuePeriodsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var duePeriods = await _context.DisciplineChoicePeriods
            .Where(p => p.ResultsProcessedAt == null
                        && (p.EndOfCheckPeriod < today))
            .Select(p => p.IdDisciplineChoicePeriod)
            .ToListAsync();

        int processed = 0;
        foreach (var periodId in duePeriods)
        {
            var result = await ProcessPeriodAsync(periodId);
            if (result.Success) processed++;
        }
        return processed;
    }

    public async Task<PeriodProcessingResult> ProcessPeriodAsync(Guid periodId)
    {
        var period = await _context.DisciplineChoicePeriods
            .FirstOrDefaultAsync(p => p.IdDisciplineChoicePeriod == periodId);

        if (period == null)
            return PeriodProcessingResult.Fail("Period not found");

        // Idempotency: never reprocess a period.
        if (period.ResultsProcessedAt != null)
            return PeriodProcessingResult.Fail("Period results already processed");

        var binds = await _context.BindSelectiveDisciplines
            .Where(b => b.PeriodId == periodId && b.InProcess)
            .ToListAsync();

        var disciplineIds = binds.Select(b => b.SelectiveDisciplineId).Distinct().ToList();
        var disciplines = await _context.SelectiveDisciplines
            .Where(d => disciplineIds.Contains(d.IdSelectiveDisciplines))
            .ToDictionaryAsync(d => d.IdSelectiveDisciplines);

        int confirmed = 0, rejected = 0;

        foreach (var group in binds.GroupBy(b => b.SelectiveDisciplineId))
        {
            if (!disciplines.TryGetValue(group.Key, out var discipline))
                continue;

            // Priority: earliest choice wins on ties / overflow.
            var ordered = group.OrderBy(b => b.CreatedAt).ToList();
            int count = ordered.Count;

            // Under-subscribed: not enough students -> reject the whole group.
            if (discipline.MinCountPeople.HasValue && count < discipline.MinCountPeople.Value)
            {
                foreach (var b in ordered) { b.InProcess = false; b.IsRejected = true; rejected++; }
                continue;
            }

            // Over-subscribed: confirm up to the cap, reject the overflow.
            int cap = discipline.MaxCountPeople ?? count;
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].InProcess = false;
                if (i < cap) { ordered[i].IsRejected = false; confirmed++; }
                else { ordered[i].IsRejected = true; rejected++; }
            }
        }

        period.ResultsProcessedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Selection is over for this period: drop now-stale caches.
        await _periodCacheService.InvalidateForPeriodAsync(period);
        foreach (var id in disciplineIds)
            await _disciplineCacheService.ClearCacheAsync(id);

        _logger.LogInformation(
            "Processed period {PeriodId}: {Confirmed} confirmed, {Rejected} rejected",
            periodId, confirmed, rejected);

        return new PeriodProcessingResult { Success = true, Confirmed = confirmed, Rejected = rejected };
    }
}
