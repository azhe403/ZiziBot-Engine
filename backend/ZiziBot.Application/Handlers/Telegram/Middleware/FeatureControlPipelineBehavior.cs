using System.Reflection;
using Microsoft.EntityFrameworkCore;
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
        var featureName = requestType.GetCustomAttribute<FeatureFlagAttribute>()?.FeatureName;

        var featureFlag = await dataFacade.MongoEf.FeatureFlag
            .Where(x => x.Status == EventStatus.Complete)
            .Where(x => x.Name == featureName)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (featureFlag == null)
            return await next();

        if (featureFlag.IsEnabled)
        {
            return await next();
        }

        logger.LogWarning("Feature {FeatureName} is disabled by global flag", featureName);
        return new TResponse();
    }
}