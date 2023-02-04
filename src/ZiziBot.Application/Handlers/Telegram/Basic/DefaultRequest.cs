namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class DefaultRequestModel : RequestBase
{
}

public class DefaultRequestHandler : IRequestHandler<DefaultRequestModel, ResponseBase>
{
    public async Task<ResponseBase> Handle(DefaultRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        await Task.Delay(1, cancellationToken);

        return responseBase.Complete();
    }
}