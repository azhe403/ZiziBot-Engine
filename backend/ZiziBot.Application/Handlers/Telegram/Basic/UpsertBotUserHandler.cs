using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class UpsertBotUserHandler<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    private readonly ILogger<UpsertBotUserHandler<TRequest, TResponse>> _logger;
    private readonly TelegramService _telegramService;
    private readonly MongoDbContextBase _mongoDbContext;

    public UpsertBotUserHandler(ILogger<UpsertBotUserHandler<TRequest, TResponse>> logger, TelegramService telegramService, MongoDbContextBase mongoDbContext)
    {
        _logger = logger;
        _telegramService = telegramService;
        _mongoDbContext = mongoDbContext;
    }

    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating User information for UserId: {UserId}", request.UserId);

        if (request.Source != ResponseSource.Bot ||
            request.IsChannel)
            return;

        request.DeleteAfter = TimeSpan.FromDays(1);

        _telegramService.SetupResponse(request);

        var botUser = await _mongoDbContext.BotUser
            .Where(entity => entity.UserId == request.UserId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (botUser == null)
        {
            _logger.LogDebug("Adding User with UserId: {UserId}", request.UserId);

            _mongoDbContext.BotUser.Add(new BotUserEntity()
            {
                UserId = request.UserId,
                Username = request.User?.Username,
                FirstName = request.User?.FirstName,
                LastName = request.User?.LastName,
                LanguageCode = request.UserLanguageCode,
                Status = (int)EventStatus.Complete
            });
        }
        else
        {
            _logger.LogDebug("Updating User with UserId: {UserId}", request.UserId);

            var trackingMessage = HtmlMessage.Empty;

            if (botUser.FirstName != request.User?.FirstName)
                trackingMessage.TextBr("Mengubah nama depannya");

            if (botUser.LastName != request.User?.LastName)
                trackingMessage.TextBr("Mengubah nama belakangnya");

            if (botUser.Username != request.User?.Username)
                trackingMessage.TextBr("Mengubah username-nya");

            await _telegramService.SendMessageText(trackingMessage.ToString());


            botUser.UserId = request.UserId;
            botUser.Username = request.User?.Username;
            botUser.FirstName = request.User?.FirstName;
            botUser.LastName = request.User?.LastName;
            botUser.LanguageCode = request.UserLanguageCode;
            botUser.Status = (int)EventStatus.Complete;
        }

        await _mongoDbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User information for UserId: {UserId} has been updated", request.UserId);
    }
}