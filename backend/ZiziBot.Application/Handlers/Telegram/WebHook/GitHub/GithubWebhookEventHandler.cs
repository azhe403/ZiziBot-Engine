using Microsoft.Extensions.Logging;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.WebHook.GitHub;

public class GithubWebhookEventHandler : GithubWebhookEventProcessor
{
    private readonly ILogger<GithubWebhookEventHandler> _logger;

    public GithubWebhookEventHandler(ILogger<GithubWebhookEventHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task ProcessPushWebhookAsync(WebhookHeaders headers, PushEvent pushEvent)
    {
        _logger.LogInformation("Push event received");

        var commits = pushEvent.Commits.ToList();
        var commitCount = commits.Count;
        var repository = pushEvent.Repository;

        var htmlMessage = HtmlMessage.Empty
            .Url(pushEvent.HeadCommit.Url, $"🏗 {commitCount} commit").Bold($" to ").Url(repository.HtmlUrl, repository.FullName).Br().Br();

        commits.ForEach(commit => {
            htmlMessage.Url(commit.Url.ToString(), commit.Id[..7])
                .Text(": ")
                .TextBr($"{commit.Message} by {commit.Author.Name}");
        });

        await SendMessage(htmlMessage.ToString());
    }

    private async Task SendMessage(string message)
    {
        var botClient = new TelegramBotClient(Token);
        await botClient.SendTextMessageAsync(ChatId, message, parseMode: ParseMode.Html, disableWebPagePreview: true);
    }
}