using System;
using System.Threading.Tasks;

namespace OlimpBack.Application.Services;

public interface ISystemEventService
{
    Task DispatchEventAsync(string eventName);
}
