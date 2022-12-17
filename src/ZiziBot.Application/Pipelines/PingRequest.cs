using Allowed.Telegram.Bot.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace ZiziBot.Application.Pipelines;

public class PingRequestModel : RequestBase
{
}

public class PingRequestHandler : IRequestHandler<PingRequestModel, ResponseBase>
{
    private readonly IMediator _mediator;

    public PingRequestHandler(IOptions<BotData[]> option, IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ResponseBase> Handle(PingRequestModel request, CancellationToken cancellationToken)
    {
        ResponseBase responseBase = new();

        responseBase.Complete();

        _mediator.Enqueue(new SendMessageTextRequestModel()
        {
            Message = request.Message,
            BotData = request.BotData,
            ReplyToMessageId = request.Message.MessageId,
            DeleteAfter = TimeSpan.FromMinutes(1),
            Text = "Pong!"
        });

        return responseBase;
    }
}