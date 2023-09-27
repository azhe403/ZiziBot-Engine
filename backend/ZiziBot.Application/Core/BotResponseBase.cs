using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ZiziBot.Application.Core;

public class BotResponseBase
{
    private readonly BotRequestBase _request = new();

    private Stopwatch Stopwatch => Stopwatch.StartNew();
    public ITelegramBotClient Bot { get; }
    public TimeSpan ExecutionTime { get; internal set; }

    public ChatId ChatId { get; set; }

    public string CallbackQueryId => _request.CallbackQuery?.Id;

    public TimeSpan DeleteAfter => _request.DeleteAfter;

    public bool DirectAction { get; set; }

    public Message? SentMessage { get; set; }
    public ResponseSource ResponseSource { get; set; } = ResponseSource.Unknown;

    public BotResponseBase()
    {
    }

    public BotResponseBase Complete()
    {
        Stopwatch.Stop();

        ExecutionTime = Stopwatch.Elapsed;
        ResponseSource = ResponseSource.Bot;

        return this;
    }
}