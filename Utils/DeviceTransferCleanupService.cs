using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OlimpBack.Infrastructure.Database.Repositories;

namespace OlimpBack.Utils;

public class DeviceTransferCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DeviceTransferCleanupService> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(10);

    public DeviceTransferCleanupService(IServiceProvider serviceProvider, ILogger<DeviceTransferCleanupService> _logger)
    {
        _serviceProvider = serviceProvider;
        this._logger = _logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Device Transfer Cleanup Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IDeviceTransferRepository>();
                    _logger.LogDebug("Running device transfer cleanup...");
                    await repository.CleanupExpiredAsync(DateTime.UtcNow);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up expired device transfers.");
            }

            await Task.Delay(_cleanupInterval, stoppingToken);
        }

        _logger.LogInformation("Device Transfer Cleanup Service is stopping.");
    }
}
