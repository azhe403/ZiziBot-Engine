using Flurl;
using Humanizer;
using Octokit.Webhooks;
using Octokit.Webhooks.Events;
using ZiziBot.Types.Vendor.GitHub;
using ZiziBot.Types.Vendor.GitLab;

namespace ZiziBot.Application.Services;

public class WebhookService
{
    public async Task<WebhookResponseBase<bool>> ParseGitHub(WebhookHeader header, string payload)
    {
        var htmlMessage = HtmlMessage.Empty;
        var response = new WebhookResponseBase<bool>();

        var githubEvent = payload.Deserialize<GitHubEventBase>();
        var repository = githubEvent!.Repository;
        var branchName = githubEvent.Ref.Split('/').Last();
        var treeUrl = repository.HtmlUrl.AppendPathSegment($"tree/{branchName}");

        htmlMessage.Url($"{repository?.HtmlUrl}", $"🗼 {repository?.FullName}").Br();

        switch (header.Event)
        {
            case WebhookEventType.Push:
                var pushEvent = payload.Deserialize<PushEvent>();
                var commits = pushEvent!.Commits.ToList();
                var commitCount = commits.Count;
                var commitsStr = "commit".ToQuantity(commitCount);

                htmlMessage.Text("🚀 Push ").Url(pushEvent.Compare, $"{commitsStr}").Bold($" to ").Url(treeUrl, $"{branchName}")
                    .Br().Br();

                commits.ForEach(commit => {
                    htmlMessage.Url(commit.Url.ToString(), commit.Id[..7])
                        .Text(": ")
                        .TextBr($"{commit.Message} by {commit.Author.Name}");
                });

                break;
            case WebhookEventType.PullRequest:
                // var request = payload.Deserialize<PullRequestEvent>();
                break;
            default:
                break;
        }


        await Task.Delay(0);

        response.FormattedHtml = htmlMessage.ToString();

        return response;
    }

    public async Task<WebhookResponseBase<bool>> ParseGitLab(WebhookHeader header, string payload)
    {
        var request = payload.Deserialize<GitLabEvent>();
        var response = new WebhookResponseBase<bool>();

        var eventName = request!.EventName;
        var repository = request.Repository;
        var project = request.Project;
        var branchName = request.Ref.Split('/').Last();
        var treeUrl = project.WebUrl.AppendPathSegment($"tree/{branchName}");

        var htmlMessage = HtmlMessage.Empty;

        switch (eventName)
        {
            case "push":
                var commits = request.Commits;
                var commitsStr = "commit".ToQuantity(commits.Count);

                htmlMessage
                    // .Url(pushEvent.Compare, $"🏗 {commitsStr}")
                    .Bold($"🏗 {commitsStr}")
                    .Bold($" to ").Url(project.WebUrl, $"{project.Name}")
                    .Text(":").Url(treeUrl, $"{branchName}")
                    .Br().Br();

                commits.ForEach(commit => {
                    htmlMessage.Url(commit.Url.ToString(), commit.Id[..7])
                        .Text(": ")
                        .TextBr($"{commit.Message.Trim()} by {commit.Author.Name}");
                });
                break;
        }

        await Task.Delay(1);

        response.FormattedHtml = htmlMessage.ToString();

        return response;
    }
}