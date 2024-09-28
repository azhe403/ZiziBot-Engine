using MediatR.Pipeline;

namespace ZiziBot.Application.Behaviours;

public class TgBotPostProcessorBehavior<TRequest, TResponse>(IMediator mediator, TelegramService telegramService, MediatorService mediatorService)
    : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    private readonly IMediator _mediator = mediator;
    private readonly MediatorService _mediatorService = mediatorService;

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        telegramService.SetupResponse(request);

        await Task.Delay(1, cancellationToken);

        // if (response.ResponseSource != ResponseSource.Bot)
        //     return;
        //
        // if (request.ChatIdentifier == 0 ||
        //     request.UserId == 0) return;
        //
        // if (!await _telegramService.IsBotName("Main")) return;
    }
}