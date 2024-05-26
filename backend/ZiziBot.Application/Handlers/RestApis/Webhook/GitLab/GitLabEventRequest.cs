using Flurl;
using Humanizer;
using ZiziBot.Types.Vendor.GitLab;

namespace ZiziBot.Application.Handlers.RestApis.Webhook.GitLab;

public class GitLabEventRequest : GitLabEvent, IWebhookRequestBase<bool>
{
}

public class GitLabEventHandler : IRequestHandler<GitLabEventRequest, WebhookResponseBase<bool>>
{
    public async Task<WebhookResponseBase<bool>> Handle(GitLabEventRequest request, CancellationToken cancellationToken)
    {
        var response = new WebhookResponseBase<bool>();

        var eventName = request.EventName;
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

        await Task.Delay(1, cancellationToken);

        response.FormattedHtml = htmlMessage.ToString();

        return response;
    }
}