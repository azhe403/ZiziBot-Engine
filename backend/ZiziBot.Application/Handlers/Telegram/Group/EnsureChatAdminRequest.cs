using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class EnsureChatAdminRequestHandler<TRequest, TResponse>(
    ILogger<EnsureChatAdminRequestHandler<TRequest, TResponse>> logger,
    TelegramService telegramService,
    MongoDbContextBase mongoDbContext
) : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        if (request.ChatType == ChatType.Private ||
            request.IsChannel ||
            request.InlineQuery != null)
            return;

        mongoDbContext.ChatAdmin
            .RemoveRange(
                entity =>
                    entity.ChatId == request.ChatIdentifier
            );

        await mongoDbContext.SaveChangesAsync(cancellationToken);

        var chatAdministrators = await telegramService.GetChatAdministrator();
        logger.LogDebug("List of Administrator in ChatId: {ChatId} found {ChatAdministrators} item(s)", request.ChatId,
            chatAdministrators.Count);

        var chatAdminEntities = chatAdministrators
            .Select(
                x => new ChatAdminEntity() {
                    ChatId = request.ChatIdentifier,
                    UserId = x.User.Id,
                    Role = x.Status,
                    Status = (int)EventStatus.Complete
                }
            )
            .ToList();

        mongoDbContext.ChatAdmin.AddRange(chatAdminEntities);

        await mongoDbContext.SaveChangesAsync(cancellationToken);
    }
}