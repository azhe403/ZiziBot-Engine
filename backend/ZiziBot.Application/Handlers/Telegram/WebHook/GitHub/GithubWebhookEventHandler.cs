using Flurl;
using Humanizer;
using Microsoft.Extensions.Logging;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using Octokit.Webhooks.Events.Deployment;
using Octokit.Webhooks.Events.DeploymentStatus;
using Octokit.Webhooks.Events.PullRequest;
using Octokit.Webhooks.Events.Star;
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
        var branchName = pushEvent.Ref.Split('/').Last();
        var treeUrl = repository.HtmlUrl.AppendPathSegment($"tree/{branchName}");
        var commitsStr = "commit".ToQuantity(commitCount);

        var htmlMessage = HtmlMessage.Empty
            .Url(pushEvent.Compare, $"🏗 {commitsStr}").Bold($" to ").Url(treeUrl, $"{repository.FullName}:{branchName}")
            .Br().Br();

        commits.ForEach(commit => {
            htmlMessage.Url(commit.Url.ToString(), commit.Id[..7])
                .Text(": ")
                .TextBr($"{commit.Message} by {commit.Author.Name}");
        });

        await SendMessage(htmlMessage.ToString());
    }

    protected override Task ProcessPullRequestWebhookAsync(WebhookHeaders headers, PullRequestEvent pullRequestEvent, PullRequestAction action)
    {
        var htmlMessage = HtmlMessage.Empty;
        var repository = pullRequestEvent.Repository;
        var pullRequest = pullRequestEvent.PullRequest;
        var headUrl = pullRequest.Head.Repo.HtmlUrl.AppendPathSegment($"tree/{pullRequest.Head.Ref}");
        var baseUrl = pullRequest.Base.Repo.HtmlUrl.AppendPathSegment($"tree/{pullRequest.Base.Ref}");

        htmlMessage.Bold(action == PullRequestAction.Opened ? "🔌 Opened " : "🔌 Updated ")
            .Url(pullRequest.HtmlUrl, $"PR #{pullRequest.Number}").Text(": ")
            .Text(pullRequest.Title).Br()
            .Bold("From: ").Url(headUrl, pullRequest.Head.Repo.FullName + ":" + pullRequest.Head.Ref).Br()
            .Bold("To: ").Url(baseUrl, pullRequest.Base.Repo.FullName + ":" + pullRequest.Base.Ref).Br();

        return SendMessage(htmlMessage.ToString());
    }

    protected override Task ProcessStarWebhookAsync(WebhookHeaders headers, StarEvent starEvent, StarAction action)
    {
        var htmlMessage = HtmlMessage.Empty;
        var repository = starEvent.Repository;
        var watcherCount = repository.WatchersCount;

        htmlMessage.Bold(action == StarAction.Created ? "⭐️ Starred " : "🌟 Unstarred ")
            .Url(repository.HtmlUrl, repository.FullName).Br()
            .Bold("Total: ").Code(watcherCount.ToString()).Br();

        return SendMessage(htmlMessage.ToString());
    }

    protected override Task ProcessStatusWebhookAsync(WebhookHeaders headers, StatusEvent statusEvent)
    {
        var htmlMessage = HtmlMessage.Empty;
        var repository = statusEvent.Repository;

        htmlMessage
            .Bold("ℹ️ Status").Br()
            .Bold("Creator: ").TextBr(statusEvent.Sender.Login)
            .Bold("Repo: ").Url(repository.HtmlUrl, repository.FullName).Br()
            .Bold("Status: ").Url(statusEvent.TargetUrl, statusEvent.State.StringValue);

        return SendMessage(htmlMessage.ToString());
    }

    protected override Task ProcessDeploymentStatusWebhookAsync(WebhookHeaders headers, DeploymentStatusEvent deploymentStatusEvent, DeploymentStatusAction action)
    {
        var htmlMessage = HtmlMessage.Empty;
        var repository = deploymentStatusEvent.Repository;

        htmlMessage
            .Bold("🚀 Deployment Status").Br()
            .Bold("Creator: ").TextBr(deploymentStatusEvent.Deployment.Creator.Login)
            .Bold("Repo: ").Url(repository.HtmlUrl, repository.FullName).Br()
            .Bold("Environment: ").TextBr(deploymentStatusEvent.DeploymentStatus.Environment).Br()
            .Bold("Status: ").TextBr(deploymentStatusEvent.DeploymentStatus.State.StringValue);

        return SendMessage(htmlMessage.ToString());
    }

    protected override Task ProcessDeploymentWebhookAsync(WebhookHeaders headers, DeploymentEvent deploymentEvent, DeploymentAction action)
    {
        var htmlMessage = HtmlMessage.Empty;
        var repository = deploymentEvent.Repository;

        htmlMessage
            .Bold("🚀 New Deployment").Br()
            .Bold("Creator: ").TextBr(deploymentEvent.Deployment.Creator.Login)
            .Bold("Repo: ").Url(repository.HtmlUrl, repository.FullName).Br()
            .Bold("Environment: ").TextBr(deploymentEvent.Deployment.Environment).Br()
            .Bold("Status: ").TextBr(deploymentEvent.Deployment.Task);

        return SendMessage(htmlMessage.ToString());
    }

    private async Task SendMessage(string message)
    {
        var botClient = new TelegramBotClient(Token);
        await botClient.SendTextMessageAsync(
            chatId: ChatId,
            text: message.MdToHtml(),
            messageThreadId: ThreadId,
            parseMode: ParseMode.Html,
            disableWebPagePreview: true
        );
    }
}