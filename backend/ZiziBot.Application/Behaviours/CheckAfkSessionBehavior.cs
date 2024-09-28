using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Behaviours;

public class CheckAfkSessionBehavior<TRequest, TResponse>(
    ILogger<CheckAfkSessionBehavior<TRequest, TResponse>> logger,
    TelegramService telegramService,
    MongoDbContextBase mongoDbContext)
    : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
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


        telegramService.SetupResponse(request);

        if (telegramService.IsCommand("/afk"))
            return;

        logger.LogInformation("CheckAfkSessionBehavior Started");

        var userId = request.UserId;
        var userName = request.User.GetFullMention();

        if (request.ReplyToMessage != null)
        {
            userId = request.ReplyToMessage.From.Id;
            userName = request.ReplyToMessage.From.GetFullMention();
        }

        var afkEntity = await mongoDbContext.Afk
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == userId &&
                    entity.Status == (int)EventStatus.Complete,
                cancellationToken);

        if (afkEntity == null)
            return;

        if (userId == request.UserId)
        {
            await telegramService.SendMessageText($"{userName} sudah tidak AFK");
            afkEntity.Status = (int)EventStatus.Deleted;
        }
        else
        {
            await telegramService.SendMessageText($"{userName} sedang AFK");
        }


        await mongoDbContext.SaveChangesAsync(cancellationToken);
    }
}