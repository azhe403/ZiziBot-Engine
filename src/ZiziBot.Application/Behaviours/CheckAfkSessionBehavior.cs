using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Behaviours;

public class CheckAfkSessionBehavior<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : RequestBase, IRequest<TResponse>
    where TResponse : ResponseBase
{
    private readonly ILogger<CheckAfkSessionBehavior<TRequest, TResponse>> _logger;
    private readonly IMediator _mediator;
    private readonly TelegramService _telegramService;
    private readonly MediatorService _mediatorService;
    private readonly ChatDbContext _chatDbContext;

    public CheckAfkSessionBehavior(
        ILogger<CheckAfkSessionBehavior<TRequest, TResponse>> logger,
        IMediator mediator,
        TelegramService telegramService,
        MediatorService mediatorService,
        ChatDbContext chatDbContext
    )
    {
        _logger = logger;
        _mediator = mediator;
        _telegramService = telegramService;
        _mediatorService = mediatorService;
        _chatDbContext = chatDbContext;
    }

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        if (request.GetType() == typeof(DeleteMessageRequestModel))
            return;

        request.ReplyMessage = true;
        request.DeleteAfter = TimeSpan.FromMinutes(1);

        _telegramService.SetupResponse(request);

        _logger.LogInformation("CheckAfkSessionBehavior Started");

        var afkEntity = await _chatDbContext.Afk
            .FirstOrDefaultAsync(entity =>
                    entity.UserId == request.UserId &&
                    entity.Status == (int) EventStatus.Complete,
                cancellationToken: cancellationToken);

        if (afkEntity != null)
        {
            await _telegramService.SendMessageText("Sudah tidak AFK");
            afkEntity.Status = (int) EventStatus.Deleted;
        }

        await _chatDbContext.SaveChangesAsync(cancellationToken);
    }
}