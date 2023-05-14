using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class UpsertBotUserHandler<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : RequestBase, IRequest<TResponse>
    where TResponse : ResponseBase
{
    private readonly ILogger<UpsertBotUserHandler<TRequest, TResponse>> _logger;
    private readonly UserDbContext _userDbContext;

    public UpsertBotUserHandler(ILogger<UpsertBotUserHandler<TRequest, TResponse>> logger, UserDbContext userDbContext)
    {
        _logger = logger;
        _userDbContext = userDbContext;
    }

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating User information for UserId: {UserId}", request.UserId);

        var botUser = await _userDbContext.BotUser
            .Where(entity => entity.UserId == request.UserId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (botUser == null)
        {
            _logger.LogDebug("Adding User with UserId: {UserId}", request.UserId);

            _userDbContext.BotUser.Add(new BotUserEntity()
            {
                UserId = request.UserId,
                FirstName = request.User?.FirstName,
                LastName = request.User?.LastName,
                LanguageCode = request.UserLanguageCode,
                Status = (int)EventStatus.Complete
            });
        }
        else
        {
            _logger.LogDebug("Updating User with UserId: {UserId}", request.UserId);

            botUser.UserId = request.UserId;
            botUser.FirstName = request.User?.FirstName;
            botUser.LastName = request.User?.LastName;
            botUser.LanguageCode = request.UserLanguageCode;
            botUser.Status = (int)EventStatus.Complete;
        }

        await _userDbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User information for UserId: {UserId} has been updated", request.UserId);
    }
}