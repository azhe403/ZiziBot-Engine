using AsyncAwaitBestPractices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.HostedServices;

public class TaskRunnerHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TaskRunnerHostedService> _logger;

    public TaskRunnerHostedService(IServiceProvider serviceProvider, ILogger<TaskRunnerHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (EnvUtil.GetEnv("ASPNETCORE_ENVIRONMENT") == "UnitTest")
        {
            _logger.LogWarning("Skip executing startup tasks in unit test environment");
            return;
        }

        var services = _serviceProvider.GetServices<IStartupTask>().ToList();

        foreach (var service in services)
        {
            _logger.LogInformation("Executing task: {Task}", service.GetType());

            if (service.SkipAwait)
            {
                service.ExecuteAsync().SafeFireAndForget(exception =>
                    _logger.LogError(exception, "Error while executing startup task: {Service}", service));
            }
            else
            {
                await service.ExecuteAsync();
            }

            _logger.LogInformation("Task executed: {Task}", service.GetType());
        }

        _logger.LogInformation("All tasks executed. Count: {Count}", services.Count);
    }
}