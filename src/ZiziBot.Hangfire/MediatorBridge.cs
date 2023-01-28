using System.ComponentModel;

namespace ZiziBot.Hangfire;

public class MediatorBridge
{
    private readonly IMediator _mediator;

    public MediatorBridge(IMediator mediator)
    {
        _mediator = mediator;
    }

    [DisplayName("{0}")]
    [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete, Attempts = 3)]
    public async Task<object?> Send(IBaseRequest command)
    {
        return await _mediator.Send(command);
    }

    [DisplayName("{0}")]
    [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete, Attempts = 3)]
    public async Task<object?> Send(string jobName, IBaseRequest command)
    {
        return await _mediator.Send(command);
    }
}