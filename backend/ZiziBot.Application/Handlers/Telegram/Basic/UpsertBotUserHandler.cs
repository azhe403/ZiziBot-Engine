using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace ZiziBot.Application.Handlers.Telegram.Basic;

public class UpsertBotUserHandler<TRequest, TResponse>(
    ILogger<UpsertBotUserHandler<TRequest, TResponse>> logger,
    ServiceFacade serviceFacade,
    DataFacade dataFacade
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

        serviceFacade.TelegramService.SetupResponse(request);

        var userProfilePhotos = await serviceFacade.TelegramService.Bot.GetUserProfilePhotos(request.UserId, cancellationToken: cancellationToken);
        var profilePhotoId = userProfilePhotos.Photos.FirstOrDefault()?.OrderByDescending(size => size.FileSize).FirstOrDefault()?.FileId ?? string.Empty;

        var userActivity = await dataFacade.ChatSetting.SaveUserActivity(new BotUserDto() {
            User = request.User,
            FirstName = request.FirstName,
            LastName = request.LastName,
            LanguageCode = request.UserLanguageCode,
            ProfilePhotoId = profilePhotoId,
            Username = request.Username,
            UserId = request.UserId,
            TransactionId = request.TransactionId
        });

        if (!string.IsNullOrEmpty(userActivity))
        {
            await serviceFacade.TelegramService.SendMessageText(userActivity);
        }

        logger.LogInformation("User information for UserId: {UserId} has been updated", request.UserId);
    }
}