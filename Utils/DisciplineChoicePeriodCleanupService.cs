using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OlimpBack.Data;
using OlimpBack.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OlimpBack.Utils;

public class DisciplineChoicePeriodCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DisciplineChoicePeriodCleanupService> _logger;

    public DisciplineChoicePeriodCleanupService(IServiceProvider serviceProvider, ILogger<DisciplineChoicePeriodCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPeriodsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing discipline choice periods.");
            }

            // Run once an hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task ProcessPeriodsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateOnly.FromDateTime(DateTime.UtcNow);

        // 1. Auto-close periods after EndDate
        var periodsToClose = await context.DisciplineChoicePeriods
            .Where(p => !p.IsClose && p.EndDate < now)
            .ToListAsync();

        if (periodsToClose.Any())
        {
            foreach (var p in periodsToClose)
            {
                p.IsClose = true;
                _logger.LogInformation($"Auto-closed period {p.IdDisciplineChoicePeriod} (EndDate was {p.EndDate})");
            }
            await context.SaveChangesAsync();
        }

        // 2. Mark disciplines as accepted after EndOfCheckPeriod
        var periodsToCheckFinished = await context.DisciplineChoicePeriods
            .Where(p => p.EndOfCheckPeriod < now)
            .ToListAsync();

        if (periodsToCheckFinished.Any())
        {
            var acceptedTypeId = Guid.Parse("00000000-0000-0000-0000-000000000003"); // Assuming 3 is 'Selected/Accepted'

            foreach (var period in periodsToCheckFinished)
            {
                bool isEven = period.PeriodType.Length > 0 && period.PeriodType[0];

                var disciplines = await context.SelectiveDisciplines
                    .Where(d => d.DepartmentId == period.DepartmentId 
                             && d.DegreeLevelId == period.DegreeLevelId
                             && d.IsEven == isEven
                             && !d.IsForseChange) // Only those not already forced
                    .ToListAsync();
                
                // Filter by courses in memory since it's a List<int>
                var periodDisciplines = disciplines.Where(d => d.Courses != null && d.Courses.Contains(period.PeriodCourse)).ToList();

                if (periodDisciplines.Any())
                {
                    foreach (var d in periodDisciplines)
                    {
                        d.IsForseChange = true;
                        d.TypeId = acceptedTypeId;
                        _logger.LogInformation($"Marked discipline {d.IdSelectiveDisciplines} as accepted for period {period.IdDisciplineChoicePeriod}");
                    }
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
