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
        await _mediatorService.Send(new RegisterRssJobRequest()
        {
            ResetStatus = true
        });

        _logger.LogDebug("Registering RSS Jobs Completed");
    }
}