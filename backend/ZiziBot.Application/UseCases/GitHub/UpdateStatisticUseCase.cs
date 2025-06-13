using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Octokit;

namespace ZiziBot.Application.UseCases.GitHub;

public class UpdateStatisticUseCase(
    ILogger<UpdateStatisticUseCase> logger,
    DataFacade dataFacade
)
{
    [DisableConcurrentExecution(3)]
    public async Task Handle(string? token)
    {
        logger.LogDebug("Updating GitHub token usage statistic");

        var query = await dataFacade.MongoEf.ApiKey
            .Where(x => x.ApiKey == token)
            .Where(x => x.Status != EventStatus.Inactive)
            .FirstOrDefaultAsync();

        if (query == null)
            return;

        var client = new GitHubClient(new ProductHeaderValue("ZiziBot"));

        if (!string.IsNullOrWhiteSpace(token))
            client.Credentials = new Credentials(token);

        var limit = await client.RateLimit.GetRateLimits();

        query.LastUsedDate = DateTime.UtcNow;
        query.Remaining = limit.Rate.Remaining;
        query.Limit = limit.Rate.Limit;
        query.LimitUnit = "Hourly";
        query.ResetUsageDate = limit.Rate.Reset;

        await dataFacade.SaveChangesAsync();
    }
}