namespace ZiziBot.Application.Core;

public class BotMiddlewareResponseBase<TResult>
{
    public bool CanContinue { get; set; }
    public string Message { get; set; } = string.Empty;
    public TResult? Result { get; set; }
}