using Hangfire;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Tasks;

public class RegisterRssJobTasks : IStartupTask
{
    private readonly ILogger<RegisterRssJobTasks> _logger;
    private readonly MediatorService _mediatorService;

    public bool SkipAwait { get; set; } = true;

    public RegisterRssJobTasks(ILogger<RegisterRssJobTasks> logger, MediatorService mediatorService)
    {
        _logger = logger;
        _mediatorService = mediatorService;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Registering RSS Jobs");
        await _mediatorService.Send(new RegisterRssJobAllRequest()
        {
            ResetStatus = false
        });

        RecurringJob.AddOrUpdate<MediatorService>(
            recurringJobId: "rss-reset",
            methodCall: mediatorService => mediatorService.Send(new RegisterRssJobAllRequest()
            {
                ResetStatus = true
            }),
            queue: "rss",
            cronExpression: TimeUtil.DayInterval(1));

        _logger.LogDebug("Registering RSS Jobs Completed");
    }
}