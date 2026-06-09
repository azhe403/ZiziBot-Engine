using Microsoft.Extensions.Logging;
using ZiziBot.Common.Types;

namespace ZiziBot.Application.Pipelines.PrePipeline;

public class ActionResultPipeline<TRequest, TResponse>(
    ILogger<ActionResultPipeline<TRequest, TResponse>> logger,
    ServiceFacade serviceFacade
) : IPreProcessPipeline<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<PreProcessResult<TResponse>> ProcessAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (request is not BotRequestBase botRequest)
            return PreProcessResult<TResponse>.Continue;

        if (botRequest.Source != ResponseSource.Bot ||
            botRequest.PipelineResult.Actions.IsEmpty())
            return PreProcessResult<TResponse>.Continue;

        //todo. auto detect based on user activity
        if (ValueConst.SAFE_IDS.Contains(botRequest.UserId))
        {
            return PreProcessResult<TResponse>.Continue;
        }

        var actions = botRequest.PipelineResult.Actions;
        var htmlMessage = HtmlMessage.Empty
            .Bold("ID: ").CodeBr(botRequest.UserId.ToString())
            .Bold("Name: ").User(botRequest.UserId, botRequest.User.GetFullName());

        serviceFacade.TelegramService.SetupResponse(botRequest);

        if (actions.Contains(PipelineResultAction.Warn))
        {
            htmlMessage.Br().Text("Mendapat diperingatkan karena: ").Br();
            if (!botRequest.PipelineResult.IsMessagePassed)
                htmlMessage.Text(" - mengirim pesan yang mengandung teks yang dilarang").Br();

            if (!botRequest.PipelineResult.HasUsername)
                htmlMessage.Text(" - belum menetapkan Username").Br();
        }

        try
        {
            //todo. incremental
            var muteDuration = MemberMuteDuration.Select(0);

            if (actions.Count != 0)
                htmlMessage.Br().Br();

            var actionResults = HtmlMessage.Empty;

            if (actions.Contains(PipelineResultAction.Delete))
            {
                try
                {
                    await serviceFacade.TelegramService.DeleteMessageAsync();
                    actionResults.TextBr(" - pesan dihapus");
                }
                catch (Exception e)
                {
                    logger.LogWarning("Unable to delete message: {MessageId} in ChatId: {ChatId}. Message: {Message}",
                        botRequest.MessageId, botRequest.ChatId, e.Message);
                }
            }

            if (actions.Contains(PipelineResultAction.Mute))
            {
                try
                {
                    await serviceFacade.TelegramService.MuteMemberAsync(botRequest.UserId, muteDuration);
                    actionResults.TextBr($" - pengguna disenyapkan selama {muteDuration.ForHuman()}");
                }
                catch (Exception e)
                {
                    logger.LogWarning("Unable to mute user: {UserId} in ChatId: {ChatId}. Message: {Message}", botRequest.UserId, botRequest.ChatId, e.Message);
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
                    logger.LogWarning("Unable to kick user: {UserId} in ChatId: {ChatId}. Message: {Message}", botRequest.UserId, botRequest.ChatId, e.Message);
                }
            }

            if (!actionResults.IsEmpty)
            {
                htmlMessage.BoldBr("Aksi: ").Append(actionResults);
            }

            if (actions.Contains(PipelineResultAction.Warn))
                await serviceFacade.TelegramService.SendMessageText(htmlMessage.ToString());

            return PreProcessResult<TResponse>.Stop();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error on Action Result pipeline");
            return PreProcessResult<TResponse>.Stop();
        }
    }
}
