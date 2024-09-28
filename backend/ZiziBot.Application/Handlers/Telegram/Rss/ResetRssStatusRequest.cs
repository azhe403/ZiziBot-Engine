namespace ZiziBot.Application.Handlers.Telegram.Rss;

public class ResetRssStatusRequest : IRequest<bool>
{

}

public class ResetRssStatusHandler(MediatorService mediatorService) : IRequestHandler<ResetRssStatusRequest, bool>
{
    public async Task<bool> Handle(ResetRssStatusRequest request, CancellationToken cancellationToken)
    {
        await mediatorService.Send(new RegisterRssJobAllRequest()
        {
            ResetStatus = true
        });

        return true;
    }
}