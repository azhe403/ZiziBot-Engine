using System.Diagnostics;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace ZiziBot.Application.Core;

public class ResponseBase
{
    private readonly RequestBase _request = new();

    private readonly Stopwatch _stopwatch = new();
    public ITelegramBotClient Bot { get; }
    public TimeSpan ExecutionTime { get; private set; }

    public IMediator XMediator => _request.Mediator;

    public ChatId ChatId { get; set; }

    public string CallbackQueryId => _request.CallbackQuery?.Id;

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
        Bot = new TelegramBotClient(request.BotToken);

        if (_request.ReplyMessage)
            _request.ReplyToMessageId = _request.Message.MessageId;

        _stopwatch.Start();
    }

    public async Task<ResponseBase> SendMessageText(string text, IReplyMarkup? replyMarkup = null)
    {
        Log.Information("Sending message to chat {ChatId}", ChatId);
        SentMessage = await Bot.SendTextMessageAsync(
            chatId: ChatId,
            text: text,
            replyToMessageId: _request.ReplyMessage ? _request.ReplyToMessageId : -1,
            parseMode: ParseMode.Html,
            allowSendingWithoutReply: true,
            replyMarkup: replyMarkup
        );

        Log.Information("Message sent to chat {ChatId}", ChatId);

        if (DeleteAfter.Ticks <= 0 &&
            _request.CleanupTargets.Contains(CleanupTarget.FromBot))
            return Complete();

        Log.Debug("Deleting message {MessageId} in {DeleteAfter} seconds", SentMessage.MessageId, DeleteAfter.TotalSeconds);
        XMediator.Schedule(
            new DeleteMessageRequestModel()
            {
                BotToken = _request.BotToken,
                Message = _request.Message,
                MessageId = SentMessage.MessageId,
                DeleteAfter = _request.DeleteAfter
            }
        );

        if (_request.CleanupTargets.Contains(CleanupTarget.FromSender))
        {
            XMediator.Schedule(
                new DeleteMessageRequestModel()
                {
                    BotToken = _request.BotToken,
                    Message = _request.Message,
                    MessageId = _request.MessageId,
                    DeleteAfter = _request.DeleteAfter,
                }
            );
        }

        Log.Information("Message {MessageId} scheduled for deletion in {DeleteAfter} seconds", SentMessage.MessageId, DeleteAfter.TotalSeconds);

        return Complete();
    }

    public async Task<ResponseBase> EditMessageText(string text)
    {
        await Bot.EditMessageTextAsync(ChatId, SentMessage.MessageId, text, parseMode: ParseMode.Html);

        return Complete();
    }

    public async Task DeleteMessageAsync()
    {
        await Bot.DeleteMessageAsync(ChatId, _request.MessageId);
    }

    public async Task<ResponseBase> AnswerCallbackAsync(string message, bool showAlert = false)
    {
        await Bot.AnswerCallbackQueryAsync(CallbackQueryId, message, showAlert);
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

        Log.Information("Processing complete in: {Elapses}", ExecutionTime);

        return this;
    }
}