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
    [MaximumConcurrentExecutions(3)]
    [AutomaticRetry(Attempts = 2, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task Handle()
    {
        logger.LogDebug("Updating GitHub token usage statistic");

        var githubApiKey = await dataFacade.MongoEf.ApiKey
            .Where(x => x.Name == ApiKeyVendor.GitHub)
            .Where(x => x.Status != EventStatus.Inactive)
            .ToListAsync();

        var client = new GitHubClient(new ProductHeaderValue("ZiziBot"));

        foreach (var apiKey in githubApiKey)
        {
            var token = apiKey.ApiKey;

            if (!string.IsNullOrWhiteSpace(token))
                client.Credentials = new Credentials(token);

            var limit = await client.RateLimit.GetRateLimits();

            apiKey.LastUsedDate = DateTime.UtcNow;
            apiKey.Remaining = limit.Rate.Remaining;
            apiKey.Limit = limit.Rate.Limit;
            apiKey.LimitUnit = "Hourly";
            apiKey.ResetUsageDate = limit.Rate.Reset.UtcDateTime;
        }

        await dataFacade.SaveChangesAsync();
    }
}