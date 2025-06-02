namespace OlimpBack.Utils
{
    public class FileCleanupService : BackgroundService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FileCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _fileLifetime = TimeSpan.FromHours(12);

        public FileCleanupService(IWebHostEnvironment env, ILogger<FileCleanupService> logger)
        {
            _env = env;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var uploadsPath = Path.Combine(_env.ContentRootPath, "Uploads");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (Directory.Exists(uploadsPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(uploadsPath);

                        foreach (var file in files)
                        {
                            var creationTime = File.GetCreationTimeUtc(file);
                            if (DateTime.UtcNow - creationTime > _fileLifetime)
                            {
                                File.Delete(file);
                                _logger.LogInformation($"Old file deleted: {file}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while deleting files");
                    }
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }

}
