using System;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public class PeriodProcessingResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public int Confirmed { get; set; }
    public int Rejected { get; set; }

    public static PeriodProcessingResult Fail(string error) => new() { Error = error };
}

public interface IPeriodResultProcessingService
{
    /// <summary>
    /// Runs capacity-based distribution for a single period and confirms/rejects choices.
    /// Idempotent: a period whose results are already processed is skipped.
    /// </summary>
    Task<PeriodProcessingResult> ProcessPeriodAsync(Guid periodId);

    /// <summary>
    /// Processes every period whose check window has finished (or was stopped early)
    /// and whose results have not yet been processed. Returns the number of periods processed.
    /// </summary>
    Task<int> ProcessDuePeriodsAsync();
}
