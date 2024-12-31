using AsyncAwaitBestPractices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.HostedServices;

public class TaskRunnerHostedService(IServiceProvider serviceProvider, ILogger<TaskRunnerHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (EnvUtil.GetEnv("ASPNETCORE_ENVIRONMENT") == "UnitTest")
        {
            logger.LogWarning("Skip executing startup tasks in unit test environment");
            return;
        }

        var scope = serviceProvider.CreateAsyncScope();
        var services = scope.ServiceProvider.GetServices<IStartupTask>().ToList();

        foreach (var service in services)
        {
            try
            {
                logger.LogInformation("Executing task: {Task}", service.GetType());

                if (service.SkipAwait)
                {
                    service.ExecuteAsync().SafeFireAndForget(exception => logger.LogError(exception, "Error while executing startup task: {Service}", service));
                }
                else
                {
                    await service.ExecuteAsync();
                }

                logger.LogInformation("Task executed: {ServiceType}", service.GetType());
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error while executing startup task: {ServiceType}", service.GetType());
            }
        }

        await scope.DisposeAsync();

        logger.LogInformation("All tasks executed. Count: {Count}", services.Count);
    }
}