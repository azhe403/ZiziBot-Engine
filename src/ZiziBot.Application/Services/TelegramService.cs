using System.Diagnostics;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using File=System.IO.File;

namespace ZiziBot.Application.Services;

public class TelegramService
{
    private readonly ILogger<TelegramService> _logger;
    private readonly CacheService _cacheService;
    private readonly AppSettingsDbContext _appSettingsDbContext;
    private readonly MediatorService _mediatorService;
    private RequestBase _request = new();

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    public ITelegramBotClient Bot { get; set; }

    public ChatId ChatId { get; set; }

    public DateTime MessageDate => _request.Message?.Date ?? _request.Message?.EditDate ?? DateTime.UtcNow;

    public string CallbackQueryId => _request.CallbackQuery?.Id;
    private string _timeInit;

    public TimeSpan DeleteAfter => _request.DeleteAfter;

    public bool DirectAction { get; set; }

    public Message SentMessage { get; set; }

    public ResponseSource ResponseSource { get; set; } = ResponseSource.Unknown;

    public TelegramService(ILogger<TelegramService> logger, CacheService cacheService, AppSettingsDbContext appSettingsDbContext, MediatorService mediatorService)
    {
        _logger = logger;
        _cacheService = cacheService;
        _appSettingsDbContext = appSettingsDbContext;
        _mediatorService = mediatorService;
    }

    public void SetupResponse(RequestBase request)
    {
        _request = request ?? throw new ArgumentNullException(nameof(request));

        _timeInit = MessageDate.GetDelay();
        ChatId = _request.ChatId;
        Bot = new TelegramBotClient(request.BotToken);

        if (_request.ReplyMessage)
            _request.ReplyToMessageId = _request.Message?.MessageId ?? default;
    }

    public async Task<bool> IsBotName(string name)
    {
        var botSettings = await _appSettingsDbContext.BotSettings
            .FirstOrDefaultAsync(x => x.Token == _request.BotToken);

        return botSettings.Name == name;
    }

    private string GetExecStamp()
    {
        var timeProc = MessageDate.GetDelay();
        var stamp = $"⏳ <code>{_timeInit} s</code> | ⏱ <code>{timeProc} s</code>";
        return stamp;
    }

    public async Task<ResponseBase> SendMessageText(string text, IReplyMarkup? replyMarkup = null)
    {
        text += "\n\n" + GetExecStamp();

        _logger.LogInformation("Sending message to chat {ChatId}", ChatId);
        SentMessage = await Bot.SendTextMessageAsync(
            chatId: ChatId,
            text: text,
            replyToMessageId: _request.ReplyMessage ? _request.ReplyToMessageId : -1,
            parseMode: ParseMode.Html,
            allowSendingWithoutReply: true,
            replyMarkup: replyMarkup,
            disableWebPagePreview: true
        );

        _logger.LogInformation("Message sent to chat {ChatId}", ChatId);

        if (_request.CleanupTargets.Contains(CleanupTarget.Nothing))
            return Complete();

        _logger.LogDebug("Deleting message {MessageId} in {DeleteAfter} seconds", SentMessage.MessageId, DeleteAfter.TotalSeconds);
        _mediatorService.Schedule(
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
            _mediatorService.Schedule(
                new DeleteMessageRequestModel()
                {
                    BotToken = _request.BotToken,
                    Message = _request.Message,
                    MessageId = _request.MessageId,
                    DeleteAfter = _request.DeleteAfter
                }
            );
        }

        _logger.LogInformation("Message {MessageId} scheduled for deletion in {DeleteAfter} seconds", SentMessage.MessageId, DeleteAfter.TotalSeconds);

        return Complete();
    }

    public async Task<ResponseBase> EditMessageText(string text)
    {
        text += "\n\n" + GetExecStamp();

        await Bot.EditMessageTextAsync(ChatId, SentMessage.MessageId, text, parseMode: ParseMode.Html);

        return Complete();
    }

    public async Task<ResponseBase> SendMediaAsync(
        string? fileId,
        CommonMediaType mediaType,
        string? caption = "",
        IReplyMarkup? replyMarkup = null,
        long customChatId = -1,
        string customFileName = ""
    )
    {
        var targetChatId = customChatId == -1 ? ChatId : customChatId;

        _logger.LogInformation("Sending media: {MediaType}, fileId: {FileId} to {ChatId}", mediaType, fileId, targetChatId);

        switch (mediaType)
        {
            case CommonMediaType.Document:
                var inputFile = new InputOnlineFile(fileId);

                if (fileId.IsValidUrl())
                {
                    _logger.LogInformation("Converting URL: '{Url}' to stream", fileId);
                    var stream = await fileId.GetStreamAsync();

                    inputFile = new InputOnlineFile(stream, customFileName);
                }

                SentMessage = await Bot.SendDocumentAsync(
                    chatId: targetChatId,
                    document: inputFile,
                    caption: caption,
                    parseMode: ParseMode.Html,
                    replyMarkup: replyMarkup,
                    replyToMessageId: _request.ReplyToMessageId
                );
                break;

            case CommonMediaType.LocalDocument:
                var fileName = Path.GetFileName(path: fileId);

                await using (var fileStream = File.OpenRead(path: fileId))
                {
                    var inputOnlineFile = new InputOnlineFile(content: fileStream, fileName: fileName);

                    SentMessage = await Bot.SendDocumentAsync(
                        chatId: targetChatId,
                        document: inputOnlineFile,
                        caption: caption,
                        parseMode: ParseMode.Html,
                        replyMarkup: replyMarkup,
                        replyToMessageId: _request.ReplyToMessageId
                    );
                }

                break;

            case CommonMediaType.Photo:
                SentMessage = await Bot.SendPhotoAsync(
                    chatId: targetChatId,
                    photo: fileId,
                    caption: caption,
                    parseMode: ParseMode.Html,
                    replyMarkup: replyMarkup,
                    replyToMessageId: _request.ReplyToMessageId
                );
                break;

            case CommonMediaType.Video:
                SentMessage = await Bot.SendVideoAsync(
                    chatId: targetChatId,
                    video: fileId,
                    caption: caption,
                    parseMode: ParseMode.Html,
                    replyMarkup: replyMarkup,
                    replyToMessageId: _request.ReplyToMessageId
                );
                break;

            case CommonMediaType.Sticker:
                SentMessage = await Bot.SendStickerAsync(
                    chatId: targetChatId,
                    sticker: fileId,
                    replyMarkup: replyMarkup,
                    replyToMessageId: _request.ReplyToMessageId
                );

                break;

            default:
                _logger.LogInformation("Media unknown: {MediaType}", mediaType);
                return default;
        }

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

    #region Role Management

    public async Task<bool> CheckAdministration()
    {
        var chatAdmins = await GetChatAdministrator();
        var isAdmin = chatAdmins.Any(x => x.User.Id == _request.UserId);
        return isAdmin;
    }

    public async Task<bool> CheckChatAdminOrPrivate()
    {
        if (_request.ChatType == ChatType.Private)
            return true;

        return await CheckAdministration();
    }

    public async Task<bool> CheckChatCreator()
    {
        var chatAdmins = await GetChatAdministrator();
        var isAdmin = chatAdmins.Any(x =>
            x.User.Id == _request.UserId &&
            x.Status == ChatMemberStatus.Creator
        );
        return isAdmin;
    }

    #endregion

    public async Task<ChatMember[]> GetChatAdministrator()
    {
        var cacheValue = await _cacheService.GetOrSetAsync(
            cacheKey: CacheKey.LIST_CHAT_ADMIN + ChatId,
            action: async () => {
                var chatAdmins = await Bot.GetChatAdministratorsAsync(ChatId);
                return chatAdmins;
            }
        );
        return cacheValue;
    }

    public ResponseBase Complete()
    {
        _stopwatch.Stop();

        return new ResponseBase()
        {
            ChatId = _request.ChatId,
            SentMessage = SentMessage,
            ResponseSource = ResponseSource.Bot,
            ExecutionTime = _stopwatch.Elapsed
        };
    }
}