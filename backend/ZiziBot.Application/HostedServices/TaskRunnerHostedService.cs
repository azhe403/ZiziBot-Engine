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
            logger.LogWarning("Skip triggering startup tasks in unit test environment");
            return;
        }

        var scope = serviceProvider.CreateAsyncScope();
        var services = scope.ServiceProvider.GetServices<IStartupTask>().ToList();

        foreach (var service in services)
        {
            var task = service.GetType();

            _ = Task.Run(async () => {
                try
                {
                    logger.LogInformation("Executing task: {Task}", task);
                    await service.ExecuteAsync();

                    logger.LogInformation("Task executed: {Task}", task);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "An error occured when executing task {Task}", task);
                }
            }, stoppingToken);

            logger.LogInformation("Task triggered: {Task}", task);
        }

        await Task.Delay(0, stoppingToken);

        logger.LogInformation("All tasks are triggered. Count: {Count}", services.Count);
    }
}