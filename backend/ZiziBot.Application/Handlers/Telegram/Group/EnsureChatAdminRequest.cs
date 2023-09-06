using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class EnsureChatAdminRequestHandler<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    private readonly ILogger<EnsureChatAdminRequestHandler<TRequest, TResponse>> _logger;
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;

    public EnsureChatAdminRequestHandler(ILogger<EnsureChatAdminRequestHandler<TRequest, TResponse>> logger, TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _logger = logger;
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;

    }

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        if (request.ChatType == ChatType.Private ||
            request.IsChannel ||
            request.InlineQuery != null)
            return;

        _mongoDbContext.ChatAdmin
            .RemoveRange(
                entity =>
                    entity.ChatId == request.ChatIdentifier
            );

        await _mongoDbContext.SaveChangesAsync(cancellationToken);

        var chatAdministrators = await _telegramService.GetChatAdministrator();
        _logger.LogDebug("List of Administrator in ChatId: {ChatId} found {ChatAdministrators} item(s)", request.ChatId, chatAdministrators.Length);

        var chatAdminEntities = chatAdministrators
            .Select(
                x => new ChatAdminEntity()
                {
                    ChatId = request.ChatIdentifier,
                    UserId = x.User.Id,
                    Role = x.Status,
                    Status = (int)EventStatus.Complete
                }
            )
            .ToList();

        _mongoDbContext.ChatAdmin.AddRange(chatAdminEntities);

        await _mongoDbContext.SaveChangesAsync(cancellationToken);
    }
}