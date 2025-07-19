using MediatR.Pipeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using ZiziBot.Database.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class EnsureChatAdminRequestHandler<TRequest, TResponse>(
    ILogger<EnsureChatAdminRequestHandler<TRequest, TResponse>> logger,
    DataFacade dataFacade,
    ServiceFacade serviceFacade
)
    : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (request.ChatType == ChatType.Private ||
            request.IsChannel ||
            request.Source != ResponseSource.Bot ||
            request.ChatId != 0 ||
            request.InlineQuery != null)
            return;

        serviceFacade.TelegramService.SetupResponse(request);

        var listChatAdmin = await dataFacade.MongoDb.ChatAdmin.Where(entity => entity.ChatId == request.ChatIdentifier)
            .Where(x => x.Status == EventStatus.Complete)
            .ToListAsync(cancellationToken);

        dataFacade.MongoDb.ChatAdmin.RemoveRange(listChatAdmin);

        await dataFacade.MongoDb.SaveChangesAsync(cancellationToken);

        var chatAdministrators = await serviceFacade.TelegramService.GetChatAdministrator();
        logger.LogDebug("Admin count in ChatId: {ChatId} found {ChatAdministrators} item(s)", request.ChatId, chatAdministrators.Count);

        var chatAdminEntities = chatAdministrators.Select(x => new ChatAdminEntity() {
            ChatId = request.ChatIdentifier,
            UserId = x.User.Id,
            Role = x.Status,
            Status = EventStatus.Complete
        }).ToList();

        dataFacade.MongoDb.ChatAdmin.AddRange(chatAdminEntities);

        await dataFacade.MongoDb.SaveChangesAsync(cancellationToken);
    }
}