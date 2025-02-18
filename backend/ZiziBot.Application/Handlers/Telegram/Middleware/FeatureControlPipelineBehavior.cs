using System.Reflection;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class FeatureControlPipelineBehavior<TRequest, TResponse>(
    ILogger<FeatureControlPipelineBehavior<TRequest, TResponse>> logger,
    DataFacade dataFacade
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();
        var featureName = requestType.GetCustomAttribute<FeatureFlagAttribute>()?.FeatureName ?? string.Empty;

        var featureFlag = await dataFacade.AppSetting.GetFlag(featureName);

        if (featureFlag == null ||
            featureFlag.IsEnabled)
            return await next();

        logger.LogWarning("Feature {FeatureName} is disabled by global flag", featureName);
        return new TResponse();
    }
}