using System.Diagnostics;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ZiziBot.Application.MediatR;

public class ResponseBase
{
    private readonly RequestBase _request= new();

    private readonly Stopwatch _stopwatch = new();
    public ITelegramBotClient Bot { get; }
    public TimeSpan ExecutionTime { get; private set; }

    public IMediator XMediator => _request.Mediator;

    public ChatId ChatId => _request.ChatId;
    public TimeSpan DeleteAfter => _request.DeleteAfter;

    public bool DirectAction { get; set; }

    public ResponseBase()
    {
        _stopwatch.Start();
    }

    public ResponseBase(string botToken)
    {
        Bot = new TelegramBotClient(botToken);
        _stopwatch.Start();
    }

    public ResponseBase(RequestBase request)
    {
        _request = request;
        Bot = new TelegramBotClient(request.Options.Token);
        _stopwatch.Start();
    }

    public async Task<ResponseBase> SendMessageText(string text)
    {
        // _logger.LogDebug("Sending message to chat {ChatId}", ChatId);
        var sentMessage = await Bot.SendTextMessageAsync(chatId: ChatId, text: text, replyToMessageId: _request.ReplyToMessageId, allowSendingWithoutReply: true);

        // _logger.LogInformation("Message sent to chat {ChatId}", chatId);

        if (DeleteAfter.Ticks <= 0)
            return Complete();

        // _logger.LogDebug("Deleting message {MessageId} in {DeleteAfter} seconds", sentMessage.MessageId, request.DeleteAfter.TotalSeconds);
        XMediator.Schedule(new DeleteMessageRequestModel()
        {
            Options = _request.Options,
            Message = _request.Message,
            MessageId = sentMessage.MessageId
        }, DeleteAfter);

        // _logger.LogInformation("Message {MessageId} scheduled for deletion in {DeleteAfter} seconds", sentMessage.MessageId, request.DeleteAfter.TotalSeconds);

        return Complete();
    }

    public ResponseBase Complete()
    {
        _stopwatch.Stop();

        ExecutionTime = _stopwatch.Elapsed;

        return this;
    }
}