namespace OlimpBack.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using OlimpBack.Data;

    public class DatabaseAvailabilityService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseAvailabilityService> _logger;

        private bool _dbAvailableLastState = false;
        private bool _dbAvailableFirstCheck = true;

        public DatabaseAvailabilityService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<DatabaseAvailabilityService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                bool dbAvailable = await CheckDatabaseAsync();

                if (dbAvailable && !_dbAvailableLastState)
                {
                    _logger.LogWarning("Database connection restored → StubLogin disabled");
                    _configuration["UseStubLogin"] = "false";
                }

                if (!dbAvailable && _dbAvailableLastState)
                {
                    _logger.LogWarning("Database connection lost → StubLogin enabled");
                    _configuration["UseStubLogin"] = "true";
                }
                else if (!dbAvailable && _dbAvailableFirstCheck)
                {
                    _logger.LogWarning("Database connection lost → StubLogin enabled");
                    _configuration["UseStubLogin"] = "true";
                    _dbAvailableFirstCheck = false;
                }

                _dbAvailableLastState = dbAvailable;

                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
            }
        }

        private async Task<bool> CheckDatabaseAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                return await db.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }
    }

}
