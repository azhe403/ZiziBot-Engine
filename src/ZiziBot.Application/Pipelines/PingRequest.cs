using MediatR;

namespace ZiziBot.Application.Pipelines;

public class PingRequestModel : RequestBase
{
}

public class PingRequestHandler : IRequestHandler<PingRequestModel, ResponseBase>
{
    private readonly IMediator _mediator;

    public PingRequestHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ResponseBase> Handle(PingRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new(request);

        await responseBase.SendMessageText("Pong!");

        return responseBase.Complete();
    }
}