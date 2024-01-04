using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using SharpX.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class ForwardChannelPostRequest : BotRequestBase
{
}

public class ForwardChannelPostHandler : IBotRequestHandler<ForwardChannelPostRequest>
{
    private readonly ILogger<ForwardChannelPostHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;

    public ForwardChannelPostHandler(ILogger<ForwardChannelPostHandler> logger, TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _logger = logger;
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<BotResponseBase> Handle(ForwardChannelPostRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);
        _logger.LogInformation("Prepare forwarding channel post..");
        var channel = request.ChannelPostAny;

        var channelId = channel?.Chat.Id;
        var textCaption = channel.Text ?? channel.Caption ?? string.Empty;
        var fileId = channel.GetFileId();
        var fileUniqueId = channel.GetFileUniqueId();

        var channelMaps = await _mongoDbContext.ChannelMap
            .Where(entity => entity.ChannelId == channelId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .ToListAsync(cancellationToken: cancellationToken);

        await channelMaps.ForEachAsync(async channelMap => {
            var channelPost = await _mongoDbContext.ChannelPost.AsNoTracking()
                .Where(x => x.DestinationChatId == channelMap.ChatId)
                .Where(x => x.DestinationThreadId == channelMap.ThreadId)
                .Where(x => x.SourceMessageId == channel!.MessageId)
                .Where(x => x.Status == (int)EventStatus.Complete)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var channelPostX = await _mongoDbContext.ChannelPost.AsNoTracking()
                .Where(x => x.DestinationChatId == channelMap.ChatId)
                .Where(x => x.DestinationThreadId == channelMap.ThreadId)
                .Where(x => x.SourceMessageId == channel!.MessageId)
                .Where(x => x.Status == (int)EventStatus.Complete)
                .ToListAsync(cancellationToken: cancellationToken);

            try
            {
                if (request.ChannelPost != null)
                {
                    _logger.LogDebug("Sending channel post from ChannelId: {ChannelId} to ChatId: {ChatId}, ThreadId: {ThreadId} ..",
                        channelMap.ChannelId, channelMap.ChatId, channelMap.ThreadId);

                    var send = channel.Type switch
                    {
                        MessageType.Text => await _telegramService.SendMessageText(
                            text: textCaption,
                            threadId: channelMap.ThreadId.Convert<int>(),
                            chatId: channelMap.ChatId),
                        MessageType.Document => await _telegramService.SendMediaAsync(
                            fileId: fileId,
                            caption: textCaption,
                            mediaType: CommonMediaType.Document,
                            threadId: channelMap.ThreadId.Convert<int>(),
                            customChatId: channelMap.ChatId),
                        MessageType.Photo => await _telegramService.SendMediaAsync(
                            fileId: fileId,
                            caption: textCaption,
                            mediaType: CommonMediaType.Photo,
                            threadId: channelMap.ThreadId.Convert<int>(),
                            customChatId: channelMap.ChatId),
                        MessageType.Video => await _telegramService.SendMediaAsync(
                            fileId: fileId,
                            caption: textCaption,
                            mediaType: CommonMediaType.Video,
                            threadId: channelMap.ThreadId.Convert<int>(),
                            customChatId: channelMap.ChatId),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    _mongoDbContext.ChannelPost.Add(new ChannelPostEntity()
                    {
                        SourceChannelId = channelMap.ChannelId,
                        SourceMessageId = channel.MessageId,
                        DestinationChatId = channelMap.ChatId,
                        DestinationThreadId = channelMap.ThreadId,
                        DestinationMessageId = send.SentMessage!.MessageId,
                        Text = channel.Text ?? channel.Caption ?? string.Empty,
                        FileId = fileId,
                        FileUniqueId = fileUniqueId,
                        MediaType = (int)channel.Type,
                        Status = (int)EventStatus.Complete
                    });
                }

                if (request.ChannelPostEdited != null)
                {
                    if (channelPost == null)
                    {
                        _logger.LogDebug("No channel post found from ChannelId: {ChannelId} for ChatId: {ChatId}, ThreadId: {ThreadId} ..",
                            channelMap.ChannelId, channelMap.ChatId, channelMap.ThreadId);

                        return;
                    }

                    _logger.LogDebug("Updating channel post to ChannelId: {ChannelId} to ChatId: {ChatId}, ThreadId: {ThreadId} ..",
                        channelMap.ChannelId, channelMap.ChatId, channelMap.ThreadId);

                    if (channel.Type == MessageType.Text)
                    {
                        var edit = await _telegramService.Bot.EditMessageTextAsync(
                            chatId: channelMap.ChatId,
                            messageId: channelPost.DestinationMessageId.Convert<int>(),
                            text: textCaption,
                            entities: channel.Entities,
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken
                        );

                        channelPost.Text = textCaption;
                    }
                    else
                    {
                        if (channelPost.Text != textCaption)
                        {
                            await _telegramService.Bot.EditMessageCaptionAsync(
                                chatId: channelMap.ChatId,
                                messageId: channelPost.DestinationMessageId.Convert<int>(),
                                caption: textCaption,
                                cancellationToken: cancellationToken
                            );

                            channelPost.Text = textCaption;
                        }

                        if (channelPost.FileUniqueId != fileUniqueId)
                        {
                            InputMedia media = (int)channel.Type switch
                            {
                                (int)CommonMediaType.Photo => new InputMediaPhoto(new InputFileId(fileId)),
                                (int)CommonMediaType.Audio => new InputMediaAudio(new InputFileId(fileId)),
                                (int)CommonMediaType.Video => new InputMediaVideo(new InputFileId(fileId)),
                                (int)CommonMediaType.Document => new InputMediaDocument(new InputFileId(fileId)),
                                _ => throw new ArgumentOutOfRangeException(null)
                            };

                            await _telegramService.Bot.EditMessageMediaAsync(
                                chatId: channelMap.ChatId,
                                messageId: channelPost.DestinationMessageId.Convert<int>(),
                                media: media,
                                cancellationToken: cancellationToken
                            );

                            channelPost.FileId = fileId;
                            channelPost.FileUniqueId = fileUniqueId;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (exception.Message.IsIgnorable())
                {
                    _logger.LogWarning(exception, "Error when forwarding channel post");
                }
            }
        });

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return _telegramService.Complete();
    }
}