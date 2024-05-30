namespace ZiziBot.Application.Core;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IBotRequestHandler<in TRequest> : IRequestHandler<TRequest, BotResponseBase> where TRequest : BotRequestBase
{
}