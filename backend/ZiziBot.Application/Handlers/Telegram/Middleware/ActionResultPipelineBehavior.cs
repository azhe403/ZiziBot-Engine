using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class ActionResultPipelineBehavior<TRequest, TResponse>(
    ILogger<ActionResultPipelineBehavior<TRequest, TResponse>> logger,
    ServiceFacade serviceFacade,
    DataFacade dataFacade
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BotRequestBase
    where TResponse : BotResponseBase, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.Source != ResponseSource.Bot ||
            request.PipelineResult.Actions.IsEmpty())
            return await next();

        if (ValueConst.SAFE_IDS.Contains(request.UserId))
        {
            //todo. auto detect based on user activity
            return await next();
        }

        var actions = request.PipelineResult.Actions;
        var htmlMessage = HtmlMessage.Empty
            .User(request.UserId, request.User.GetFullName());

        serviceFacade.TelegramService.SetupResponse(request);

        if (actions.Contains(PipelineResultAction.Warn))
        {
            htmlMessage.Text(" telah diperingatkan karena: ").Br();
            if (!request.PipelineResult.IsMessagePassed)
                htmlMessage.Text(" - mengirim pesan yang mengandung teks yang dilarang");

            if (!request.PipelineResult.HasUsername)
                htmlMessage.Text(" - belum menetapkan Username").Br();
        }

        try
        {
            //todo. incremental
            var muteDuration = MemberMuteDuration.Select(0);

            if (actions.Count != 0)
                htmlMessage.Br().Br().BoldBr("Aksi: ");

            if (actions.Contains(PipelineResultAction.Delete))
            {
                try
                {
                    await serviceFacade.TelegramService.DeleteMessageAsync();
                    htmlMessage.TextBr(" - pesan dihapus");
                }
                catch (Exception e)
                {
                    logger.LogWarning("Unable to delete message: {MessageId} in ChatId: {ChatId}", request.MessageId, request.ChatId);
                }
            }

            if (actions.Contains(PipelineResultAction.Mute))
            {
                try
                {
                    await serviceFacade.TelegramService.MuteMemberAsync(request.UserId, muteDuration);
                    htmlMessage.TextBr($" - pengguna disenyapkan selama {muteDuration.ForHuman()}");
                }
                catch (Exception e)
                {
                    logger.LogWarning("Unable to mute user: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);
                }
            }

            if (actions.Contains(PipelineResultAction.Kick))
            {
                try
                {
                    await serviceFacade.TelegramService.KickMember();
                    htmlMessage.TextBr(" - pengguna dikeluarkan dari grub");
                }
                catch (Exception e)
                {
                    logger.LogWarning("Unable to kick user: {UserId} in ChatId: {ChatId}", request.UserId, request.ChatId);
                }
            }

            if(actions.Contains(PipelineResultAction.Warn))
                await serviceFacade.TelegramService.SendMessageText(htmlMessage.ToString());

            return await next();
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Error when running action for userId: {UserId}. Message: {Message}", request.UserId, exception.Message);
        }

        return new TResponse();
    }
}