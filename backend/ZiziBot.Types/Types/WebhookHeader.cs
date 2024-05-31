using Microsoft.Extensions.Primitives;

namespace ZiziBot.Types.Types;

public class WebhookHeader
{
    public required string UserAgent { get; init; }
    public string? Delivery { get; init; }
    public string? Event { get; init; }
    public string? HookId { get; init; }
    public string? HookInstallationTargetId { get; init; }
    public string? HookInstallationTargetType { get; init; }

    public static WebhookHeader Parse(IDictionary<string, StringValues> headers)
    {
        ArgumentNullException.ThrowIfNull(headers);

        headers.TryGetValue("User-Agent", out var userAgent);
        headers.TryGetValue("X-GitHub-Hook-Installation-Target-ID", out var hookInstallationTargetId);
        headers.TryGetValue("X-GitHub-Hook-Installation-Target-Type", out var hookInstallationTargetType);

        var webhookHeader = new WebhookHeader {
            UserAgent = userAgent.ToString(),
            Delivery = headers.FirstOrDefault(kv => kv.Key.Contains("Delivery")).Value,
            Event = headers.FirstOrDefault(kv => kv.Key.Contains("Event")).Value,
            HookId = headers.FirstOrDefault(kv => kv.Key.Contains("Hook-ID")).Value,
            HookInstallationTargetId = hookInstallationTargetId.ToString(),
            HookInstallationTargetType = hookInstallationTargetType.ToString(),
        };

        return webhookHeader;
    }
}