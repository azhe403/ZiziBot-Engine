using MediatR.Pipeline;

namespace ZiziBot.Application.Behaviours;

public class TgBotPostProcessorBehavior<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : RequestBase, IRequest<TResponse>
    where TResponse : ResponseBase
{
    private readonly IMediator _mediator;
    private readonly TelegramService _telegramService;
    private readonly MediatorService _mediatorService;

    public TgBotPostProcessorBehavior(IMediator mediator, TelegramService telegramService, MediatorService mediatorService)
    {
        _mediator = mediator;
        _telegramService = telegramService;
        _mediatorService = mediatorService;
    }

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        if (response.ResponseSource != ResponseSource.Bot)
            return;

        if (request.ChatIdentifier == 0 ||
            request.UserId == 0) return;

        if (!await _telegramService.IsBotName("Main")) return;

        await _mediatorService.EnqueueAsync(
            new EnsureChatAdminRequestModel()
            {
                BotToken = request.BotToken,
                ChatId = request.ChatIdentifier,
                UserId = request.UserId,
                ExecutionStrategy = ExecutionStrategy.Instant
            }
        );
    }
}