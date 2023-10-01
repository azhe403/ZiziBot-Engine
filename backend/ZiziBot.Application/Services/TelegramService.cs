using System.Diagnostics;
using AsyncAwaitBestPractices;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using File = System.IO.File;

namespace ZiziBot.Application.Services;

public class TelegramService
{
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly CacheService _cacheService;
    private readonly ILogger<TelegramService> _logger;
    private readonly MediatorService _mediatorService;

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private BotRequestBase _request = new();
    private string _timeInit = string.Empty;

    public ITelegramBotClient Bot { get; set; }
    public Message? SentMessage { get; set; }

    public TelegramService(ILogger<TelegramService> logger, CacheService cacheService, MongoDbContextBase mongoDbContext, MediatorService mediatorService)
    {
        _logger = logger;
        _cacheService = cacheService;
        _mongoDbContext = mongoDbContext;
        _mediatorService = mediatorService;
    }

    public void SetupResponse(BotRequestBase request)
    {
        ArgumentNullException.ThrowIfNull(request);

        _request = request;

        _timeInit = request.MessageDate.GetDelay();
        Bot = new TelegramBotClient(request.BotToken);

        if (_request.ReplyMessage)
            _request.ReplyToMessageId = _request.Message?.MessageId ?? default;
    }

    public async Task<bool> IsBotName(string name)
    {
        var botSettings = await _mongoDbContext.BotSettings
            .FirstOrDefaultAsync(x => x.Token == _request.BotToken);

        return botSettings.Name == name;
    }

    private string GetExecStamp()
    {
        var timeProc = _request.MessageDate.GetDelay();
        var stamp = $"⏳ <code>{_timeInit} s</code> | ⏱ <code>{timeProc} s</code>";
        return stamp;
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

    public BotResponseBase Complete()
    {
        _stopwatch.Stop();

        return new BotResponseBase
        {
            ChatId = _request.ChatId,
            SentMessage = SentMessage,
            ResponseSource = ResponseSource.Bot,
            ExecutionTime = _stopwatch.Elapsed
        };
    }

    #region Response
    public async Task<BotResponseBase> SendMessageText(HtmlMessage text, IReplyMarkup? replyMarkup = null, long chatId = -1)
    {
        return await SendMessageText(text.ToString(), replyMarkup, chatId);
    }

    public async Task<BotResponseBase> SendMessageText(string? text, IReplyMarkup? replyMarkup = null, long chatId = -1, int threadId = -1)
    {
        if (text.IsNullOrEmpty())
            return Complete();

        text += "\n\n" + GetExecStamp();

        var targetChatId = chatId != -1 ? chatId : _request.ChatId;

        if (threadId == -1)
            threadId = _request.MessageThreadId;

        _logger.LogInformation("Sending message to chat {ChatId}", _request.ChatId);
        try
        {
            SentMessage = await Bot.SendTextMessageAsync(
                chatId: targetChatId,
                messageThreadId: threadId,
                text: text,
                replyToMessageId: _request.ReplyMessage ? _request.ReplyToMessageId : -1,
                parseMode: ParseMode.Html,
                allowSendingWithoutReply: true,
                replyMarkup: replyMarkup,
                disableWebPagePreview: true
            );
        }
        catch (Exception exception)
        {
            if (exception.Message.Contains("thread not found"))
            {
                _logger.LogWarning("Trying send message without thread to ChatId: {ChatId}", _request.ChatId);
                SentMessage = await Bot.SendTextMessageAsync(
                    chatId: targetChatId,
                    text: text,
                    replyToMessageId: _request.ReplyMessage ? _request.ReplyToMessageId : -1,
                    parseMode: ParseMode.Html,
                    allowSendingWithoutReply: true,
                    replyMarkup: replyMarkup,
                    disableWebPagePreview: true
                );
            }
        }

        if (SentMessage == null)
            return Complete();

        _logger.LogInformation("Message sent to chat {ChatId}", _request.ChatId);

        if (_request.CleanupTargets.Contains(CleanupTarget.None))
            return Complete();

        _logger.LogDebug("Scheduling delete message {MessageId} on ChatId: {ChatId} in {DeleteAfter} seconds",
            SentMessage.MessageId, _request.ChatId, _request.DeleteAfter.TotalSeconds);

        _mediatorService.Schedule(new DeleteMessageBotRequestModel
        {
            BotToken = _request.BotToken,
            Message = _request.Message,
            MessageId = SentMessage.MessageId,
            DeleteAfter = _request.DeleteAfter,
            Source = ResponseSource.Hangfire
        });

        if (_request.CleanupTargets.Contains(CleanupTarget.FromSender))
        {
            _mediatorService.Schedule(new DeleteMessageBotRequestModel
            {
                BotToken = _request.BotToken,
                Message = _request.Message,
                MessageId = _request.MessageId,
                DeleteAfter = _request.DeleteAfter,
                Source = ResponseSource.Hangfire
            });
        }

        _logger.LogInformation("Message {MessageId} scheduled for deletion in {DeleteAfter} seconds", SentMessage.MessageId, _request.DeleteAfter.TotalSeconds);

        return Complete();
    }

    public async Task<BotResponseBase> EditMessageText(string text, InlineKeyboardMarkup? replyMarkup = null)
    {
        text += "\n\n" + GetExecStamp();

        await Bot.EditMessageTextAsync(_request.ChatId, SentMessage.MessageId, text, replyMarkup: replyMarkup, parseMode: ParseMode.Html);

        return Complete();
    }

    public async Task<BotResponseBase> SendMediaAsync(
        string fileId,
        CommonMediaType? mediaType,
        string? caption = null,
        IReplyMarkup? replyMarkup = null,
        long customChatId = -1,
        string? customFileName = null,
        int? threadId = null
    )
    {
        var targetChatId = customChatId == -1 ? _request.ChatId : customChatId;
        var targetThreadId = threadId ?? _request.MessageThreadId;

        _logger.LogInformation("Sending media: {MediaType}, fileId: {FileId} to {ChatId}", mediaType, fileId, targetChatId);
        InputFile inputFile = InputFile.FromFileId(fileId);

        switch (mediaType)
        {
            case CommonMediaType.Document:

                if (fileId.IsValidUrl())
                {
                    _logger.LogInformation("Converting URL: '{Url}' to stream", fileId);
                    var stream = await fileId.GetStreamAsync();

                    inputFile = InputFile.FromStream(stream, customFileName);
                }

                SentMessage = await Bot.SendDocumentAsync(
                    chatId: targetChatId,
                    document: inputFile,
                    caption: caption,
                    parseMode: ParseMode.Html,
                    replyMarkup: replyMarkup,
                    replyToMessageId: _request.ReplyToMessageId,
                    allowSendingWithoutReply: true,
                    messageThreadId: targetThreadId
                );
                break;

            case CommonMediaType.LocalDocument:
                var fileName = Path.GetFileName(path: fileId);

                await using (var fileStream = File.OpenRead(path: fileId))
                {
                    var inputOnlineFile = InputFile.FromStream(stream: fileStream, fileName: fileName);

                    SentMessage = await Bot.SendDocumentAsync(
                        chatId: targetChatId,
                        document: inputOnlineFile,
                        caption: caption,
                        parseMode: ParseMode.Html,
                        replyMarkup: replyMarkup,
                        replyToMessageId: _request.ReplyToMessageId,
                        allowSendingWithoutReply: true,
                        messageThreadId: targetThreadId
                    );
                }

                break;

            case CommonMediaType.Photo:
                SentMessage = await Bot.SendPhotoAsync(
                    chatId: targetChatId,
                    photo: inputFile,
                    caption: caption,
                    parseMode: ParseMode.Html,
                    replyMarkup: replyMarkup,
                    replyToMessageId: _request.ReplyToMessageId,
                    allowSendingWithoutReply: true,
                    messageThreadId: targetThreadId
                );
                break;

            case CommonMediaType.Audio:
                SentMessage = await Bot.SendAudioAsync(
                    chatId: targetChatId,
                    audio: inputFile,
                    caption: caption,
                    parseMode: ParseMode.Html,
                    replyMarkup: replyMarkup,
                    replyToMessageId: _request.ReplyToMessageId,
                    allowSendingWithoutReply: true,
                    messageThreadId: targetThreadId
                );
                break;

            case CommonMediaType.Video:
                SentMessage = await Bot.SendVideoAsync(
                    chatId: targetChatId,
                    video: inputFile,
                    caption: caption,
                    parseMode: ParseMode.Html,
                    replyMarkup: replyMarkup,
                    replyToMessageId: _request.ReplyToMessageId,
                    allowSendingWithoutReply: true,
                    messageThreadId: targetThreadId
                );
                break;

            case CommonMediaType.Sticker:
                SentMessage = await Bot.SendStickerAsync(
                    chatId: targetChatId,
                    sticker: inputFile,
                    replyMarkup: replyMarkup,
                    replyToMessageId: _request.ReplyToMessageId,
                    allowSendingWithoutReply: true,
                    messageThreadId: targetThreadId
                );

                break;

            case CommonMediaType.Unknown:
            case CommonMediaType.Text:
                await SendMessageText(caption, replyMarkup);
                break;
            default:
                _logger.LogWarning("Media unknown: {MediaType}", mediaType);
                return default;
        }

        return Complete();
    }

    public async Task<BotResponseBase> EditMediaAsync(
        string fileId,
        CommonMediaType mediaType,
        string? caption = null,
        IReplyMarkup? replyMarkup = null,
        long customChatId = -1,
        string? customFileName = null,
        int? threadId = null,
        int? messageId = null
    )
    {
        var targetChatId = customChatId == -1 ? _request.ChatId : customChatId;
        var targetThreadId = threadId ?? _request.MessageThreadId;
        var targetMessageId = messageId ?? SentMessage.MessageId;

        await Task.Delay(1);

        _logger.LogInformation("Sending media: {MediaType}, fileId: {FileId} to {ChatId}", mediaType, fileId, targetChatId);
        _logger.LogDebug("Updating media caption in {ChatId}:{ThreadId}:{MessageId}", targetChatId, targetThreadId, targetMessageId);

        Bot.EditMessageCaptionAsync(
            chatId: targetChatId,
            messageId: targetMessageId,
            caption: caption,
            parseMode: ParseMode.Html,
            replyMarkup: replyMarkup as InlineKeyboardMarkup
        ).SafeFireAndForget(e => _logger.LogWarning(e, "Error updating media caption"));

        InputMedia media = mediaType switch
        {
            CommonMediaType.Photo => new InputMediaPhoto(new InputFileId(fileId)),
            CommonMediaType.Audio => new InputMediaAudio(new InputFileId(fileId)),
            CommonMediaType.Video => new InputMediaVideo(new InputFileId(fileId)),
            CommonMediaType.Document => new InputMediaDocument(new InputFileId(fileId)),
            _ => throw new ArgumentOutOfRangeException(nameof(mediaType), mediaType, null)
        };

        _logger.LogDebug("Updating media file in {ChatId}:{ThreadId}:{MessageId}", targetChatId, targetThreadId, targetMessageId);
        Bot.EditMessageMediaAsync(
            chatId: targetChatId,
            messageId: targetMessageId,
            media: media,
            replyMarkup: replyMarkup as InlineKeyboardMarkup
        ).SafeFireAndForget(e => _logger.LogWarning(e, "Error updating media file"));

        return Complete();
    }

    public async Task<BotResponseBase> SendMessageAsync(
        string? text,
        InlineKeyboardMarkup? replyMarkup = null,
        string? fileId = null,
        CommonMediaType? mediaType = CommonMediaType.Unknown,
        long customChatId = -1,
        int threadId = -1,
        int customMessageId = -1,
        string? customFileName = null
    )
    {
        if (SentMessage != null)
        {
            await EditMessageText(text, replyMarkup);
        }
        else
        {
            if (fileId.IsNullOrEmpty() ||
                mediaType == CommonMediaType.Text ||
                mediaType == CommonMediaType.Unknown)
            {
                await SendMessageText(
                    text: text,
                    replyMarkup: replyMarkup,
                    chatId: customChatId,
                    threadId: threadId
                );
            }
            else
            {
                await SendMediaAsync(
                    fileId: fileId,
                    mediaType: mediaType,
                    caption: text,
                    replyMarkup: replyMarkup,
                    customChatId: customChatId,
                    customFileName: customFileName
                );

            }
        }

        return Complete();
    }

    public async Task DeleteMessageAsync()
    {
        try
        {
            await Bot.DeleteMessageAsync(_request.ChatId, _request.MessageId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error deleting message {MessageId}", _request.MessageId);
            if (e.Message.IsIgnorable())
                return;

            throw;
        }
    }

    public async Task<BotResponseBase> AnswerCallbackAsync(string message, bool showAlert = false)
    {
        await Bot.AnswerCallbackQueryAsync(_request.CallbackQueryId, message, showAlert);
        return Complete();
    }

    public async Task<BotResponseBase> AnswerInlineQueryAsync(IEnumerable<InlineQueryResult> results)
    {
        if (_request.InlineQuery == null)
        {
            return Complete();
        }

        var reducedResults = results.Take(50);
        await Bot.AnswerInlineQueryAsync(_request.InlineQuery.Id, reducedResults, cacheTime: 60);
        return Complete();
    }

    public async Task LeaveChatAsync()
    {
        await Bot.LeaveChatAsync(_request.ChatId);
    }
    #endregion


    #region Command
    public string? GetCommand(bool withoutSlash = false, bool withoutUsername = true)
    {
        var cmd = string.Empty;

        if (!_request.MessageText?.StartsWith("/") ?? true) return cmd;

        cmd = _request.MessageTexts?.ElementAtOrDefault(0);

        if (withoutSlash)
            cmd = cmd?.TrimStart('/');
        if (withoutUsername)
            cmd = cmd?.Split("@").FirstOrDefault();

        return cmd;
    }

    public bool IsCommand(string command)
    {
        return GetCommand() == command;
    }
    #endregion

    #region Member
    public async Task<int> GetMemberCount()
    {
        var memberCount = await Bot.GetChatMemberCountAsync(_request.ChatId);
        return memberCount;
    }

    public async Task MuteMemberAsync(long userId, TimeSpan duration)
    {
        var untilDate = DateTime.UtcNow + duration;
        await Bot.RestrictChatMemberAsync(chatId: _request.ChatId, userId: userId, permissions: new ChatPermissions()
        {
            CanSendAudios = false,
            CanSendPhotos = false,
            CanSendVideos = false,
            CanSendVideoNotes = false,
            CanSendVoiceNotes = false,
            CanSendDocuments = false
        }, untilDate: untilDate);
    }

    public async Task AnswerJoinRequestAsync(ChatJoinRequest joinRequest)
    {
        // var result = await Bot.ApproveChatJoinRequest(_request.ChatId, joinRequest.From.Id);

    }
    #endregion

    #region Role
    public async Task PromoteMember(long userId)
    {
        _logger.LogInformation("Promoting user {UserId} in chat {ChatId}", userId, _request.ChatId);

        await Bot.PromoteChatMemberAsync(
            chatId: _request.ChatId,
            userId: _request.UserId,
            canPostMessages: true,
            canEditMessages: true,
            canDeleteMessages: true,
            canInviteUsers: true,
            canRestrictMembers: true,
            canPromoteMembers: true,
            canManageVideoChats: true,
            canPinMessages: true
        );
    }

    public async Task DemoteMember(long userId)
    {
        _logger.LogInformation("Demoting user {UserId} in chat {ChatId}", userId, _request.ChatId);

        await Bot.PromoteChatMemberAsync(
            chatId: _request.ChatId,
            userId: _request.UserId,
            canPostMessages: false,
            canEditMessages: false,
            canDeleteMessages: false,
            canInviteUsers: false,
            canRestrictMembers: false,
            canPromoteMembers: false,
            canManageVideoChats: false,
            canPinMessages: false
        );
    }

    public async Task<ChatMember[]> GetChatAdministrator()
    {
        var cacheValue = await _cacheService.GetOrSetAsync(
            cacheKey: CacheKey.LIST_CHAT_ADMIN + _request.ChatId,
            action: async () => {
                var chatAdmins = await Bot.GetChatAdministratorsAsync(_request.ChatId);
                return chatAdmins;
            }
        );
        return cacheValue;
    }

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

}