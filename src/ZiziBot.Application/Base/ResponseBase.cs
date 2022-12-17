using System.Diagnostics;
using Telegram.Bot;

namespace ZiziBot.Application.Base;

public class ResponseBase
{
    private readonly Stopwatch _stopwatch = new();
    public ITelegramBotClient Bot { get; }
    public TimeSpan ExecutionTime { get; private set; }

    public ResponseBase()
    {
        _stopwatch.Start();
    }

    public ResponseBase(string botToken)
    {
        Bot = new TelegramBotClient(botToken);
        _stopwatch.Start();
    }

    public ResponseBase Complete()
    {
        _stopwatch.Stop();

        ExecutionTime = _stopwatch.Elapsed;

        return this;
    }
}