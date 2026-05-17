using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public class LoggingEmailService : IEmailService
{
    private readonly ILogger<LoggingEmailService> _logger;

    public LoggingEmailService(ILogger<LoggingEmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendPasswordEmailAsync(string email, string password)
    {
        var message = $"To: {email}\nSubject: Your login details\nBody: Your login is your email: {email}. Your temporary password is: {password}\n---\n";
        
        // Log to console
        _logger.LogInformation(message);

        // Also log to a file for easy retrieval
        await File.AppendAllTextAsync("sent_emails.log", message);
    }
}
