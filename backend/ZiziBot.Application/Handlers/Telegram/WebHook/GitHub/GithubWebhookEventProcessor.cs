using Octokit.Webhooks;

namespace ZiziBot.Application.Handlers.Telegram.WebHook.GitHub;

public abstract class GithubWebhookEventProcessor : WebhookEventProcessor
{
    public string RouteId { get; set; }
    public string TransactionId { get; set; }
    public string Payload { get; set; }
}