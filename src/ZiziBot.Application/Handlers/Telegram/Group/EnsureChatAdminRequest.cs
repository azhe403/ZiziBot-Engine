using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class EnsureChatAdminRequestHandler : IRequestPostProcessor<RequestBase, ResponseBase>
{
    private readonly ILogger<EnsureChatAdminRequestHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly ChatDbContext _chatDbContext;

    public EnsureChatAdminRequestHandler(ILogger<EnsureChatAdminRequestHandler> logger, TelegramService telegramService, ChatDbContext chatDbContext)
    {
        _logger = logger;
        _telegramService = telegramService;
        _chatDbContext = chatDbContext;

    }

    public async Task Process(RequestBase request, ResponseBase response, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        _chatDbContext.ChatAdmin
            .RemoveRange(
                entity =>
                    entity.ChatId == request.ChatId
            );

        await _chatDbContext.SaveChangesAsync(cancellationToken);

        var chatAdministrators = await _telegramService.GetChatAdministrator(request.ChatIdentifier);
        _logger.LogDebug("List of Administrator in ChatId: {ChatId} found {ChatAdministrators} item(s)", request.ChatId, chatAdministrators.Length);

        var chatAdminEntities = chatAdministrators
            .Select(
                x => new ChatAdminEntity()
                {
                    ChatId = request.ChatIdentifier,
                    UserId = x.User.Id,
                    Role = x.Status,
                    Status = (int) EventStatus.Complete
                }
            )
            .ToList();

        _chatDbContext.ChatAdmin.AddRange(chatAdminEntities);

        await _chatDbContext.SaveChangesAsync(cancellationToken);
    }
}