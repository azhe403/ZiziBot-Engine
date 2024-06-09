using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using ZiziBot.DataSource.MongoDb.Entities;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class UpsertBotUserHandler<TRequest, TResponse>(
    ILogger<UpsertBotUserHandler<TRequest, TResponse>> logger,
    TelegramService telegramService,
    MongoDbContextBase mongoDbContext
) : IRequestPostProcessor<TRequest, TResponse>
    where TRequest : BotRequestBase, IRequest<TResponse>
    where TResponse : BotResponseBase
{
    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        logger.LogDebug("Updating User information for UserId: {UserId}", request.UserId);

        if (request.Source != ResponseSource.Bot ||
            request.IsChannel)
            return;

        request.DeleteAfter = TimeSpan.FromDays(1);

        telegramService.SetupResponse(request);

        var botUser = await mongoDbContext.BotUser
            .Where(entity => entity.UserId == request.UserId)
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (botUser == null)
        {
            logger.LogDebug("Adding User with UserId: {UserId}", request.UserId);

            mongoDbContext.BotUser.Add(new BotUserEntity() {
                UserId = request.UserId,
                Username = request.User?.Username,
                FirstName = request.User?.FirstName,
                LastName = request.User?.LastName,
                LanguageCode = request.UserLanguageCode,
                Status = (int)EventStatus.Complete,
                TransactionId = request.TransactionId
            });
        }
        else
        {
            logger.LogDebug("Updating User with UserId: {UserId}", request.UserId);

            var trackingMessage = HtmlMessage.Empty;

            if (botUser.FirstName != request.User?.FirstName)
                trackingMessage.TextBr("Mengubah nama depannya");

            if (botUser.LastName != request.User?.LastName)
                trackingMessage.TextBr("Mengubah nama belakangnya");

            if (botUser.Username != request.User?.Username)
                trackingMessage.TextBr("Mengubah username-nya");

            if (!trackingMessage.IsEmpty)
            {
                var message = HtmlMessage.Empty
                    .Bold("Pengguna: ").User(request.User).Br()
                    .Append(trackingMessage);

                await telegramService.SendMessageText(message.ToString());
            }

            botUser.UserId = request.UserId;
            botUser.Username = request.User?.Username;
            botUser.FirstName = request.User?.FirstName;
            botUser.LastName = request.User?.LastName;
            botUser.LanguageCode = request.UserLanguageCode;
            botUser.Status = (int)EventStatus.Complete;
            botUser.TransactionId = request.TransactionId;
        }

        await mongoDbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User information for UserId: {UserId} has been updated", request.UserId);
    }
}