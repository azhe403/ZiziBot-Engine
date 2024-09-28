using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using SharpX.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class ForwardChannelPostRequest : BotRequestBase
{
}

public class ForwardChannelPostHandler(
    ILogger<ForwardChannelPostHandler> logger,
    TelegramService telegramService,
    MongoDbContextBase mongoDbContext)
    : IBotRequestHandler<ForwardChannelPostRequest>
{
    public async Task<BotResponseBase> Handle(ForwardChannelPostRequest request, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);
        logger.LogInformation("Prepare forwarding channel post..");
        var channel = request.ChannelPostAny;

        var channelId = channel?.Chat.Id;
        var textCaption = channel?.Text ?? channel?.Caption ?? string.Empty;
        var fileId = channel.GetFileId();
        var messageLink = channel.GetMessageLink();
        var fileUniqueId = channel.GetFileUniqueId();

        var channelMaps = await mongoDbContext.ChannelMap
            .Where(entity => entity.ChannelId == channelId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .ToListAsync(cancellationToken: cancellationToken);

        var replyMarkup = new InlineKeyboardMarkup(new[] {
            new[] {
                InlineKeyboardButton.WithUrl("↗️ Source", messageLink)
            }
        });

        await channelMaps.ForEachAsync(async channelMap => {
            var channelPost = await mongoDbContext.ChannelPost.AsNoTracking()
                .Where(x => x.DestinationChatId == channelMap.ChatId)
                .Where(x => x.DestinationThreadId == channelMap.ThreadId)
                .Where(x => x.SourceMessageId == channel!.MessageId)
                .Where(x => x.Status == (int)EventStatus.Complete)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            try
            {
                if (request.ChannelPost != null)
                {
                    logger.LogDebug(
                        "Sending channel post from ChannelId: {ChannelId} to ChatId: {ChatId}, ThreadId: {ThreadId} ..",
                        channelMap.ChannelId, channelMap.ChatId, channelMap.ThreadId);

                    var send = channel.Type switch {
                        MessageType.Text => await telegramService.SendMessageText(
                            text: textCaption,
                            threadId: channelMap.ThreadId.Convert<int>(),
                            replyMarkup: replyMarkup,
                            chatId: channelMap.ChatId),
                        MessageType.Document => await telegramService.SendMediaAsync(
                            fileId: fileId,
                            caption: textCaption,
                            mediaType: CommonMediaType.Document,
                            threadId: channelMap.ThreadId.Convert<int>(),
                            replyMarkup: replyMarkup,
                            customChatId: channelMap.ChatId),
                        MessageType.Photo => await telegramService.SendMediaAsync(
                            fileId: fileId,
                            caption: textCaption,
                            mediaType: CommonMediaType.Photo,
                            threadId: channelMap.ThreadId.Convert<int>(),
                            replyMarkup: replyMarkup,
                            customChatId: channelMap.ChatId),
                        MessageType.Video => await telegramService.SendMediaAsync(
                            fileId: fileId,
                            caption: textCaption,
                            mediaType: CommonMediaType.Video,
                            threadId: channelMap.ThreadId.Convert<int>(),
                            replyMarkup: replyMarkup,
                            customChatId: channelMap.ChatId),
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    mongoDbContext.ChannelPost.Add(new ChannelPostEntity() {
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
                        logger.LogDebug(
                            "No channel post found from ChannelId: {ChannelId} for ChatId: {ChatId}, ThreadId: {ThreadId} ..",
                            channelMap.ChannelId, channelMap.ChatId, channelMap.ThreadId);

                        return;
                    }

                    logger.LogDebug(
                        "Updating channel post to ChannelId: {ChannelId} to ChatId: {ChatId}, ThreadId: {ThreadId} ..",
                        channelMap.ChannelId, channelMap.ChatId, channelMap.ThreadId);

                    if (channel.Type == MessageType.Text)
                    {
                        var edit = await telegramService.Bot.EditMessageTextAsync(
                            chatId: channelMap.ChatId,
                            messageId: channelPost.DestinationMessageId.Convert<int>(),
                            text: textCaption,
                            entities: channel.Entities,
                            parseMode: ParseMode.Html,
                            replyMarkup: replyMarkup,
                            cancellationToken: cancellationToken
                        );

                        channelPost.Text = textCaption;
                    }
                    else
                    {
                        if (channelPost.Text != textCaption)
                        {
                            await telegramService.Bot.EditMessageCaptionAsync(
                                chatId: channelMap.ChatId,
                                messageId: channelPost.DestinationMessageId.Convert<int>(),
                                caption: textCaption,
                                replyMarkup: replyMarkup,
                                cancellationToken: cancellationToken
                            );

                            channelPost.Text = textCaption;
                        }

                        if (channelPost.FileUniqueId != fileUniqueId)
                        {
                            InputMedia media = (int)channel.Type switch {
                                (int)CommonMediaType.Photo => new InputMediaPhoto(new InputFileId(fileId)),
                                (int)CommonMediaType.Audio => new InputMediaAudio(new InputFileId(fileId)),
                                (int)CommonMediaType.Video => new InputMediaVideo(new InputFileId(fileId)),
                                (int)CommonMediaType.Document => new InputMediaDocument(new InputFileId(fileId)),
                                _ => throw new ArgumentOutOfRangeException(null)
                            };

                            await telegramService.Bot.EditMessageMediaAsync(
                                chatId: channelMap.ChatId,
                                messageId: channelPost.DestinationMessageId.Convert<int>(),
                                media: media,
                                replyMarkup: replyMarkup,
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
                    logger.LogWarning(exception, "Error when forwarding channel post");
                }
            }
        });

        await mongoDbContext.SaveChangesAsync(cancellationToken);

        return telegramService.Complete();
    }
}