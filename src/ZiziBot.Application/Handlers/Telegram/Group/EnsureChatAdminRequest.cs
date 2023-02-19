using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class EnsureChatAdminRequestModel : RequestBase
{
    public long ChatId { get; set; }
    public long UserId { get; set; }
}

public class EnsureChatAdminRequestModelValidator : AbstractValidator<EnsureChatAdminRequestModel>
{
    public EnsureChatAdminRequestModelValidator()
    {
        RuleFor(x => x.ChatId).LessThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}

public class EnsureChatAdminRequestHandler : IRequestHandler<EnsureChatAdminRequestModel, ResponseBase>
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

    public async Task<ResponseBase> Handle(EnsureChatAdminRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);
        var validate = await request.ValidateAsync<EnsureChatAdminRequestModelValidator, EnsureChatAdminRequestModel>();

        if (!(validate?.IsValid ?? false))
        {
            _logger.LogDebug("Ensure Chat Admin. ChatId: {ChatId}, UserId: {UserId}. Payload invalid", request.ChatId, request.UserId);
            return _telegramService.Complete();
        }

        _chatDbContext.ChatAdmin
            .RemoveRange(
                entity =>
                    entity.ChatId == request.ChatId
            );

        await _chatDbContext.SaveChangesAsync(cancellationToken);

        var chatAdministrators = await _telegramService.GetChatAdministrator(request.ChatId);
        _logger.LogDebug("List of Administrator in ChatId: {ChatId} found {ChatAdministrators} item(s)", request.ChatId, chatAdministrators.Length);

        var chatAdminEntities = chatAdministrators
            .Select(
                x => new ChatAdminEntity()
                {
                    ChatId = request.ChatId,
                    UserId = x.User.Id,
                    Role = x.Status,
                    Status = (int) EventStatus.Complete
                }
            )
            .ToList();

        _chatDbContext.ChatAdmin.AddRange(chatAdminEntities);

        await _chatDbContext.SaveChangesAsync(cancellationToken);

        return _telegramService.Complete();
    }
}