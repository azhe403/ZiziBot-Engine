namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class ResetRssStatusRequest : IRequest<bool>
{

}

public class ResetRssStatusHandler : IRequestHandler<ResetRssStatusRequest, bool>
{
    private readonly MediatorService _mediatorService;

    public ResetRssStatusHandler(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    public async Task<bool> Handle(ResetRssStatusRequest request, CancellationToken cancellationToken)
    {
        await _mediatorService.Send(new RegisterRssJobRequest()
        {
            ResetStatus = true
        });

        return true;
    }
}