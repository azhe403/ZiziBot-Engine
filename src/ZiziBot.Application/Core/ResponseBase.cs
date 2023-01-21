using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ZiziBot.Application.Handlers.Telegram.Core;
using File=System.IO.File;

namespace ZiziBot.Application.Core;

public class ResponseBase
{
    private readonly RequestBase _request = new();

    private readonly Stopwatch _stopwatch = new();
    public ILogger Logger { get; set; }
    public ITelegramBotClient Bot { get; }
    public TimeSpan ExecutionTime { get; private set; }

    public IMediator XMediator => _request.Mediator;

    public ChatId ChatId { get; set; }

    public TimeSpan DeleteAfter => _request.DeleteAfter;

    public bool DirectAction { get; set; }

    public Message SentMessage { get; set; }

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
        ChatId = _request.ChatId;
        Bot = new TelegramBotClient(request.Options.Token);
        _stopwatch.Start();
    }

    public async Task<ResponseBase> SendMessageText(string text)
    {
        // Logger?.LogDebug("Sending message to chat {ChatId}", ChatId);
        SentMessage = await Bot.SendTextMessageAsync(
            chatId: ChatId,
            text: text,
            replyToMessageId: _request.ReplyToMessageId,
            parseMode: ParseMode.Html,
            allowSendingWithoutReply: true
        );

        // Logger?.LogInformation("Message sent to chat {ChatId}", ChatId);

        if (DeleteAfter.Ticks <= 0)
            return Complete();

        // Logger?.LogDebug("Deleting message {MessageId} in {DeleteAfter} seconds", SentMessage.MessageId, DeleteAfter.TotalSeconds);
        XMediator.Schedule(
            new DeleteMessageRequestModel()
            {
                Options = _request.Options,
                Message = _request.Message,
                MessageId = SentMessage.MessageId,
                DeleteAfter = _request.DeleteAfter
            }
        );

        if (_request.CleanupStrategy == CleanupStrategy.FromBotAndSender)
        {
            XMediator.Schedule(
                new DeleteMessageRequestModel()
                {
                    Options = _request.Options,
                    Message = _request.Message,
                    MessageId = _request.Message.MessageId,
                    DeleteAfter = _request.DeleteAfter,
                }
            );
        }

        // Logger.LogInformation("Message {MessageId} scheduled for deletion in {DeleteAfter} seconds", SentMessage.MessageId, DeleteAfter.TotalSeconds);

        return Complete();
    }

    public async Task<ResponseBase> EditMessageText(string text)
    {
        await Bot.EditMessageTextAsync(ChatId, SentMessage.MessageId, text, parseMode: ParseMode.Html);

        return Complete();
    }

    public async Task<string> DownloadFileAsync(string prefixName)
    {
        var photo = (_request.ReplyToMessage ?? _request.Message).Photo?.LastOrDefault();
        var fileId = photo?.FileId;

        var filePath = PathConst.TEMP_PATH + prefixName + photo?.FileUniqueId + ".jpg";

        await using Stream fileStream = File.OpenWrite(filePath.EnsureDirectory());
        await Bot.GetInfoAndDownloadFileAsync(fileId, fileStream);

        return filePath;
    }

    public ResponseBase Complete()
    {
        _stopwatch.Stop();

        ExecutionTime = _stopwatch.Elapsed;

        return this;
    }
}