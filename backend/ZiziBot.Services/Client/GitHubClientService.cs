using GitHub;
using GitHub.Models;
using GitHub.Octokit.Client;
using GitHub.Octokit.Client.Authentication;
using Microsoft.Extensions.Logging;
using ZiziBot.Common.Enums;

namespace ZiziBot.Services.Client;

public class GitHubClientService(
    ILogger<GitHubClientService> logger,
    BotRepository botRepository
)
{
    public async Task<IReadOnlyList<Release>> GetReleaseAssets(string url)
    {
        logger.LogDebug("Fetching GitHub release assets from URL: {Url}", url);

        var uri = new Uri(url);
        var owner = uri.Segments[1].TrimEnd('/');
        var repo = uri.Segments[2].TrimEnd('/');

        var client = await GetClient();
        var releases = await client.Repos[owner][repo].Releases.GetAsync();

        logger.LogInformation("Fetched {Count} release assets from {Owner}/{Repo}", releases.Count, owner, repo);

        return releases;
    }

    private async Task<GitHubClient> GetClient()
    {
        var githubToken = Env.GithubToken ?? await botRepository.GetApiKeyAsync(ApiKeyCategory.Internal, ApiKeyVendor.GitHub);

        var tokenProvider = new TokenProvider(githubToken);
        var adapter = RequestAdapter.Create(new TokenAuthProvider(tokenProvider));
        var client = new GitHubClient(adapter);
        return client;
    }
}