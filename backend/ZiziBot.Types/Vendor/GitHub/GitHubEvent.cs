using System.Text.Json.Serialization;

namespace ZiziBot.Types.Vendor.GitHub;

public partial class GitHubEvent
{
    [JsonPropertyName("ref")]
    public string Ref { get; set; }

    [JsonPropertyName("before")]
    public string Before { get; set; }

    [JsonPropertyName("after")]
    public string After { get; set; }

    [JsonPropertyName("repository")]
    public Repository Repository { get; set; }

    [JsonPropertyName("pusher")]
    public Pusher Pusher { get; set; }

    [JsonPropertyName("organization")]
    public Organization Organization { get; set; }

    [JsonPropertyName("sender")]
    public Sender Sender { get; set; }

    [JsonPropertyName("created")]
    public bool Created { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }

    [JsonPropertyName("forced")]
    public bool Forced { get; set; }

    [JsonPropertyName("base_ref")]
    public object BaseRef { get; set; }

    [JsonPropertyName("compare")]
    public string Compare { get; set; }

    [JsonPropertyName("commits")]
    public List<Commit> Commits { get; set; }

    [JsonPropertyName("head_commit")]
    public Commit HeadCommit { get; set; }
}

public partial class Commit
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("tree_id")]
    public string TreeId { get; set; }

    [JsonPropertyName("distinct")]
    public bool Distinct { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("author")]
    public Author Author { get; set; }

    [JsonPropertyName("committer")]
    public Author Committer { get; set; }

    [JsonPropertyName("added")]
    public List<object> Added { get; set; }

    [JsonPropertyName("removed")]
    public List<object> Removed { get; set; }

    [JsonPropertyName("modified")]
    public List<string> Modified { get; set; }
}

public partial class Author
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }
}

public partial class Organization
{
    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("repos_url")]
    public Uri ReposUrl { get; set; }

    [JsonPropertyName("events_url")]
    public Uri EventsUrl { get; set; }

    [JsonPropertyName("hooks_url")]
    public Uri HooksUrl { get; set; }

    [JsonPropertyName("issues_url")]
    public Uri IssuesUrl { get; set; }

    [JsonPropertyName("members_url")]
    public string MembersUrl { get; set; }

    [JsonPropertyName("public_members_url")]
    public string PublicMembersUrl { get; set; }

    [JsonPropertyName("avatar_url")]
    public Uri AvatarUrl { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }
}

public partial class Pusher
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}

public partial class Repository
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("full_name")]
    public string FullName { get; set; }

    [JsonPropertyName("private")]
    public bool Private { get; set; }

    [JsonPropertyName("owner")]
    public Sender Owner { get; set; }

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("fork")]
    public bool Fork { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("forks_url")]
    public Uri ForksUrl { get; set; }

    [JsonPropertyName("keys_url")]
    public string KeysUrl { get; set; }

    [JsonPropertyName("collaborators_url")]
    public string CollaboratorsUrl { get; set; }

    [JsonPropertyName("teams_url")]
    public Uri TeamsUrl { get; set; }

    [JsonPropertyName("hooks_url")]
    public Uri HooksUrl { get; set; }

    [JsonPropertyName("issue_events_url")]
    public string IssueEventsUrl { get; set; }

    [JsonPropertyName("events_url")]
    public Uri EventsUrl { get; set; }

    [JsonPropertyName("assignees_url")]
    public string AssigneesUrl { get; set; }

    [JsonPropertyName("branches_url")]
    public string BranchesUrl { get; set; }

    [JsonPropertyName("tags_url")]
    public Uri TagsUrl { get; set; }

    [JsonPropertyName("blobs_url")]
    public string BlobsUrl { get; set; }

    [JsonPropertyName("git_tags_url")]
    public string GitTagsUrl { get; set; }

    [JsonPropertyName("git_refs_url")]
    public string GitRefsUrl { get; set; }

    [JsonPropertyName("trees_url")]
    public string TreesUrl { get; set; }

    [JsonPropertyName("statuses_url")]
    public string StatusesUrl { get; set; }

    [JsonPropertyName("languages_url")]
    public Uri LanguagesUrl { get; set; }

    [JsonPropertyName("stargazers_url")]
    public Uri StargazersUrl { get; set; }

    [JsonPropertyName("contributors_url")]
    public Uri ContributorsUrl { get; set; }

    [JsonPropertyName("subscribers_url")]
    public Uri SubscribersUrl { get; set; }

    [JsonPropertyName("subscription_url")]
    public Uri SubscriptionUrl { get; set; }

    [JsonPropertyName("commits_url")]
    public string CommitsUrl { get; set; }

    [JsonPropertyName("git_commits_url")]
    public string GitCommitsUrl { get; set; }

    [JsonPropertyName("comments_url")]
    public string CommentsUrl { get; set; }

    [JsonPropertyName("issue_comment_url")]
    public string IssueCommentUrl { get; set; }

    [JsonPropertyName("contents_url")]
    public string ContentsUrl { get; set; }

    [JsonPropertyName("compare_url")]
    public string CompareUrl { get; set; }

    [JsonPropertyName("merges_url")]
    public Uri MergesUrl { get; set; }

    [JsonPropertyName("archive_url")]
    public string ArchiveUrl { get; set; }

    [JsonPropertyName("downloads_url")]
    public Uri DownloadsUrl { get; set; }

    [JsonPropertyName("issues_url")]
    public string IssuesUrl { get; set; }

    [JsonPropertyName("pulls_url")]
    public string PullsUrl { get; set; }

    [JsonPropertyName("milestones_url")]
    public string MilestonesUrl { get; set; }

    [JsonPropertyName("notifications_url")]
    public string NotificationsUrl { get; set; }

    [JsonPropertyName("labels_url")]
    public string LabelsUrl { get; set; }

    [JsonPropertyName("releases_url")]
    public string ReleasesUrl { get; set; }

    [JsonPropertyName("deployments_url")]
    public Uri DeploymentsUrl { get; set; }

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("pushed_at")]
    public long PushedAt { get; set; }

    [JsonPropertyName("git_url")]
    public string GitUrl { get; set; }

    [JsonPropertyName("ssh_url")]
    public string SshUrl { get; set; }

    [JsonPropertyName("clone_url")]
    public Uri CloneUrl { get; set; }

    [JsonPropertyName("svn_url")]
    public Uri SvnUrl { get; set; }

    [JsonPropertyName("homepage")]
    public Uri Homepage { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("stargazers_count")]
    public long StargazersCount { get; set; }

    [JsonPropertyName("watchers_count")]
    public long WatchersCount { get; set; }

    [JsonPropertyName("language")]
    public string Language { get; set; }

    [JsonPropertyName("has_issues")]
    public bool HasIssues { get; set; }

    [JsonPropertyName("has_projects")]
    public bool HasProjects { get; set; }

    [JsonPropertyName("has_downloads")]
    public bool HasDownloads { get; set; }

    [JsonPropertyName("has_wiki")]
    public bool HasWiki { get; set; }

    [JsonPropertyName("has_pages")]
    public bool HasPages { get; set; }

    [JsonPropertyName("has_discussions")]
    public bool HasDiscussions { get; set; }

    [JsonPropertyName("forks_count")]
    public long ForksCount { get; set; }

    [JsonPropertyName("mirror_url")]
    public object MirrorUrl { get; set; }

    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; }

    [JsonPropertyName("open_issues_count")]
    public long OpenIssuesCount { get; set; }

    [JsonPropertyName("license")]
    public License License { get; set; }

    [JsonPropertyName("allow_forking")]
    public bool AllowForking { get; set; }

    [JsonPropertyName("is_template")]
    public bool IsTemplate { get; set; }

    [JsonPropertyName("web_commit_signoff_required")]
    public bool WebCommitSignoffRequired { get; set; }

    [JsonPropertyName("topics")]
    public List<string> Topics { get; set; }

    [JsonPropertyName("visibility")]
    public string Visibility { get; set; }

    [JsonPropertyName("forks")]
    public long Forks { get; set; }

    [JsonPropertyName("open_issues")]
    public long OpenIssues { get; set; }

    [JsonPropertyName("watchers")]
    public long Watchers { get; set; }

    [JsonPropertyName("default_branch")]
    public string DefaultBranch { get; set; }

    [JsonPropertyName("stargazers")]
    public long Stargazers { get; set; }

    [JsonPropertyName("master_branch")]
    public string MasterBranch { get; set; }

    [JsonPropertyName("organization")]
    public string Organization { get; set; }
}

public partial class License
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("spdx_id")]
    public string SpdxId { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }
}

public partial class Sender
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public object Email { get; set; }

    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    [JsonPropertyName("avatar_url")]
    public Uri AvatarUrl { get; set; }

    [JsonPropertyName("gravatar_id")]
    public string GravatarId { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("html_url")]
    public Uri HtmlUrl { get; set; }

    [JsonPropertyName("followers_url")]
    public Uri FollowersUrl { get; set; }

    [JsonPropertyName("following_url")]
    public string FollowingUrl { get; set; }

    [JsonPropertyName("gists_url")]
    public string GistsUrl { get; set; }

    [JsonPropertyName("starred_url")]
    public string StarredUrl { get; set; }

    [JsonPropertyName("subscriptions_url")]
    public Uri SubscriptionsUrl { get; set; }

    [JsonPropertyName("organizations_url")]
    public Uri OrganizationsUrl { get; set; }

    [JsonPropertyName("repos_url")]
    public Uri ReposUrl { get; set; }

    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; }

    [JsonPropertyName("received_events_url")]
    public Uri ReceivedEventsUrl { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("site_admin")]
    public bool SiteAdmin { get; set; }
}