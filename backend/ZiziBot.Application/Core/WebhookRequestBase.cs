namespace ZiziBot.Application.Core;

public interface IWebhookRequestBase<TResult> : IRequest<WebhookResponseBase<TResult>>
{
}

public class WebhookResponseBase<TResult>
{
    public string FormattedHtml { get; set; }
}