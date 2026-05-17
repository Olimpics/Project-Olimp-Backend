using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public interface IEmailService
{
    Task SendPasswordEmailAsync(string email, string password);
}
