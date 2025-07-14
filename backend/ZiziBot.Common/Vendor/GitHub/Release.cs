﻿using System.Text.Json.Serialization;

namespace ZiziBot.Common.Vendor.GitHub;

public class Release
{
    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("assets_url")]
    public Uri AssetsUrl { get; set; }

    [JsonPropertyName("upload_url")]
    public string UploadUrl { get; set; }

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("author")]
    public Author? Author { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }

    [JsonPropertyName("target_commitish")]
    public string TargetCommitish { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("draft")]
    public bool Draft { get; set; }

    [JsonPropertyName("immutable")]
    public bool Immutable { get; set; }

    [JsonPropertyName("prerelease")]
    public bool Prerelease { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("published_at")]
    public DateTime? PublishedAt { get; set; }

    [JsonPropertyName("assets")]
    public List<Asset>? Assets { get; set; }

    [JsonPropertyName("tarball_url")]
    public Uri TarballUrl { get; set; }

    [JsonPropertyName("zipball_url")]
    public Uri ZipballUrl { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }

    [JsonPropertyName("reactions")]
    public Reactions Reactions { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mentions_count")]
    public long? MentionsCount { get; set; }
}

public class Asset
{
    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("label")]
    public object Label { get; set; }

    [JsonPropertyName("uploader")]
    public Author Uploader { get; set; }

    [JsonPropertyName("content_type")]
    public string ContentType { get; set; }

    [JsonPropertyName("state")]
    public string State { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("digest")]
    public string Digest { get; set; }

    [JsonPropertyName("download_count")]
    public long DownloadCount { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("browser_download_url")]
    public string BrowserDownloadUrl { get; set; }
}

public partial class Author
{
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

    [JsonPropertyName("user_view_type")]
    public string UserViewType { get; set; }

    [JsonPropertyName("site_admin")]
    public bool SiteAdmin { get; set; }
}

public class Reactions
{
    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("total_count")]
    public long TotalCount { get; set; }

    [JsonPropertyName("+1")]
    public long The1 { get; set; }

    [JsonPropertyName("-1")]
    public long Reactions1 { get; set; }

    [JsonPropertyName("laugh")]
    public long Laugh { get; set; }

    [JsonPropertyName("hooray")]
    public long Hooray { get; set; }

    [JsonPropertyName("confused")]
    public long Confused { get; set; }

    [JsonPropertyName("heart")]
    public long Heart { get; set; }

    [JsonPropertyName("rocket")]
    public long Rocket { get; set; }

    [JsonPropertyName("eyes")]
    public long Eyes { get; set; }
}