using Octokit.Webhooks;

namespace ZiziBot.Application.Handlers.Telegram.WebHook.GitHub;

public abstract class GithubWebhookEventProcessor : WebhookEventProcessor
{
    public string RouteId { get; set; }
    public long ChatId { get; set; }
    public int ThreadId { get; set; }
    public string Token { get; set; }
}