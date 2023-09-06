using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Behaviours;

public class CheckAfkSessionBehavior<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly ILogger<CheckAfkSessionBehavior<TRequest, TResponse>> _logger;
    private readonly TelegramService _telegramService;

    public CheckAfkSessionBehavior(
        ILogger<CheckAfkSessionBehavior<TRequest, TResponse>> logger,
        TelegramService telegramService,
        MongoDbContextBase mongoDbContext
    )
    {
        _logger = logger;
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
    }

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (request.Source != ResponseSource.Bot ||
            request.IsChannel)
            return;

        request.ReplyMessage = true;
        request.DeleteAfter = TimeSpan.FromMinutes(1);
        request.CleanupTargets = new[]
        {
            CleanupTarget.FromBot
        };


        _telegramService.SetupResponse(request);

        if (_telegramService.IsCommand("/afk"))
            return;

        _logger.LogInformation("CheckAfkSessionBehavior Started");

        var userId = request.UserId;
        var userName = request.User.GetFullMention();

        if (request.ReplyToMessage != null)
        {
            userId = request.ReplyToMessage.From.Id;
            userName = request.ReplyToMessage.From.GetFullMention();
        }

        var afkEntity = await _mongoDbContext.Afk
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == userId &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken);

        if (afkEntity == null)
            return;

        if (userId == request.UserId)
        {
            await _telegramService.SendMessageText($"{userName} sudah tidak AFK");
            afkEntity.Status = (int)EventStatus.Deleted;
        }
        else
        {
            await _telegramService.SendMessageText($"{userName} sedang AFK");
        }


        await _mongoDbContext.SaveChangesAsync(cancellationToken);
    }
}