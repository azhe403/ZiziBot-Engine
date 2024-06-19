using Microsoft.Extensions.Primitives;

namespace ZiziBot.Types.Types;

public class WebhookHeader
{
    public string? Host { get; init; }
    public string? UserAgent { get; init; }
    public string? ContentType { get; init; }
    public string? Delivery { get; init; }
    public string? Event { get; init; }
    public string? HookId { get; init; }
    public string? HookInstallationTargetId { get; init; }
    public string? HookInstallationTargetType { get; init; }

    public static WebhookHeader Parse(IDictionary<string, StringValues> headers)
    {
        ArgumentNullException.ThrowIfNull(headers);

        var webhookHeader = new WebhookHeader {
            Host = headers.FirstOrDefault(kv => kv.Key.Like("host")).Value,
            UserAgent = headers.FirstOrDefault(kv => kv.Key.Like("user-agent")).Value,
            ContentType = headers.FirstOrDefault(kv => kv.Key.Like("content-type")).Value,
            Delivery = headers.FirstOrDefault(kv => kv.Key.Like("delivery")).Value,
            Event = headers.FirstOrDefault(kv => kv.Key.Like("event")).Value,
            HookId = headers.FirstOrDefault(kv => kv.Key.Like("hook-id")).Value,
            HookInstallationTargetId = headers.FirstOrDefault(kv => kv.Key.Like("hook-installation-target-id")).Value,
            HookInstallationTargetType = headers.FirstOrDefault(kv => kv.Key.Like("hook-installation-target-type")).Value
        };

        return webhookHeader;
    }
}