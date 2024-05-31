using Flurl;
using Humanizer;
using ZiziBot.Types.Vendor.GitHub;

namespace ZiziBot.Application.Handlers.RestApis.Webhook.Partial;

public class GitHubEventRequest : GitHubEvent, IWebhookRequestBase<bool>
{
}

public class GitHubEventHandler : IRequestHandler<GitHubEventRequest, WebhookResponseBase<bool>>
{
    public async Task<WebhookResponseBase<bool>> Handle(GitHubEventRequest request, CancellationToken cancellationToken)
    {
        var response = new WebhookResponseBase<bool>();

        var commits = request.Commits.ToList();
        var commitCount = commits.Count;
        var repository = request.Repository;
        var branchName = request.Ref.Split('/').Last();
        var treeUrl = repository.HtmlUrl.AppendPathSegment($"tree/{branchName}");
        var commitsStr = "commit".ToQuantity(commitCount);

        var htmlMessage = HtmlMessage.Empty
            .Url(request.Compare, $"🏗 {commitsStr}").Bold($" to ").Url(repository.HtmlUrl, $"{repository.FullName}")
            .Text(":").Url(treeUrl, $"{branchName}")
            .Br().Br();

        commits.ForEach(commit => {
            htmlMessage.Url(commit.Url.ToString(), commit.Id[..7])
                .Text(": ")
                .TextBr($"{commit.Message} by {commit.Author.Name}");
        });

        await Task.Delay(0, cancellationToken);

        response.FormattedHtml = htmlMessage.ToString();

        return response;
    }
}