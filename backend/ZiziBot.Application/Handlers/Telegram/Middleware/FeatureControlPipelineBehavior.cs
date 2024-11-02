using Microsoft.EntityFrameworkCore;
using ZiziBot.DataSource.MongoEf.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class FeatureControlPipelineBehavior<TRequest, TResponse>(DataFacade dataFacade) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = request.Command ?? nameof(request);

        var featureFlag = await dataFacade.MongoEf.FeatureFlag.Where(x => x.Name == requestName).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (featureFlag == null)
        {
            dataFacade.MongoEf.FeatureFlag.Add(new FeatureFlagEntity {
                Status = EventStatus.Complete,
                TransactionId = request.TransactionId,
                Name = requestName,
                IsEnabled = false
            });

            await dataFacade.MongoEf.SaveChangesAsync(cancellationToken);
        }

        return await next();
    }
}