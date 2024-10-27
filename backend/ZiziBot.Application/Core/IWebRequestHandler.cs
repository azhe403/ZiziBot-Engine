namespace ZiziBot.Application.Core;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IWebRequestHandler<in TRequest, TResponse> : IRequestHandler<TRequest, WebResponseBase<TResponse>>
    where TRequest : IRequest<WebResponseBase<TResponse>>
{ }