using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using SharpX.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class ForwardChannelPostRequest : BotRequestBase
{
}

public class ForwardChannelPostHandler : IRequestHandler<ForwardChannelPostRequest, BotResponseBase>
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
        _logger.LogInformation("Forwarding channel post to ..");
        var channel = request.ChannelPostAny;
        var channelId = channel?.Chat.Id;

        var channelMaps = await _mongoDbContext.ChannelMap
            .Where(entity => entity.ChannelId == channelId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .ToListAsync(cancellationToken: cancellationToken);

        await channelMaps.ForEachAsync(async entity => {
            var channelPost = await _mongoDbContext.ChannelPost
                .Where(x => x.SourceMessageId == channel!.MessageId)
                .Where(x => x.DestinationThreadId == entity.ThreadId)
                .Where(x => x.DestinationChatId == entity.ChatId)
                .Where(x => x.Status == (int)EventStatus.Complete)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (request.ChannelPost != null)
            {
                _logger.LogDebug("Sending channel post from {ChannelId} to {ChatId}:{ThreadId} ..", entity.ChannelId, entity.ChatId, entity.ThreadId);
                var sent = await _telegramService.Bot.SendTextMessageAsync(
                    chatId: entity.ChatId,
                    text: channel.Text,
                    messageThreadId: entity.ThreadId.Convert<int>(),
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);

                _mongoDbContext.ChannelPost.Add(new ChannelPostEntity()
                {
                    SourceChannelId = entity.ChannelId,
                    SourceMessageId = channel.MessageId,
                    DestinationChatId = entity.ChatId,
                    DestinationThreadId = entity.ThreadId,
                    DestinationMessageId = sent.MessageId,
                    Status = (int)EventStatus.Complete
                });
            }

            if (request.ChannelPostEdited != null)
            {
                _logger.LogDebug("Updating channel post to {ChannelId} to {ChatId}:{ThreadId} ..", entity.ChannelId, entity.ChatId, entity.ThreadId);
                var edit = await _telegramService.Bot.EditMessageTextAsync(
                    chatId: entity.ChatId,
                    messageId: channelPost.DestinationMessageId.Convert<int>(),
                    text: channel.Text,
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
            }
        });

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        return _telegramService.Complete();
    }
}