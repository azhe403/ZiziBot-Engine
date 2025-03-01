using Telegram.Bot.Types.ReplyMarkups;

namespace ZiziBot.Application.Core;

public class BotMiddlewareResponseBase<TResult>
{
    public bool CanContinue { get; set; }
    public string Message { get; set; } = string.Empty;
    public ReplyMarkup ReplyMarkup { get; set; } = InlineKeyboardMarkup.Empty();

    public bool DeleteMessage { get; set; }
    public TimeSpan MuteDuration { get; set; }
    public TResult? Result { get; set; }
}