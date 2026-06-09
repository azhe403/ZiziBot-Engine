using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Pipelines.PrePipeline;

public class CheckMessagePipeline<TRequest, TResponse>(
    ILogger<CheckMessagePipeline<TRequest, TResponse>> logger,
    DataFacade dataFacade
) : IPreProcessPipeline<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<PreProcessResult<TResponse>> ProcessAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (request is not BotRequestBase botRequest)
            return PreProcessResult<TResponse>.Continue;

        if (botRequest.IsChannel ||
            botRequest.Source == ResponseSource.Hangfire ||
            botRequest.IsPrivateChat)
            return PreProcessResult<TResponse>.Continue;

        if (botRequest.MessageTexts.IsEmpty())
            return PreProcessResult<TResponse>.Continue;

        if (botRequest.RolesLevels.Any(x => x == RoleLevel.Sudo))
        {
            if (botRequest.Command is "/dwf" or "/awf")
            {
                return PreProcessResult<TResponse>.Continue;
            }
        }

        botRequest.ReplyMessage = true;

        var hasBadword = false;
        var matchPattern = string.Empty;
        PipelineResultAction[]? action = [];

        var words = await dataFacade.WordFilter.GetAllAsync();

        var messageTexts = botRequest.Message?.Text?.Explode();

        foreach (var messageText in messageTexts)
        {
            foreach (var dto in words)
            {
                var pattern = dto.Word;

                hasBadword = messageText.Match(pattern);

                if (!hasBadword)
                    continue;

                logger.LogWarning("Check Message, pattern: {Pattern}, source: {MessageText}, action: {Action}, chatId: {ChatId}", pattern, messageText, dto.Action, botRequest.ChatId);

                matchPattern = dto.Word;
                action = dto.Action;

                break;
            }

            if (hasBadword)
                break;
        }

        botRequest.PipelineResult.IsMessagePassed = !hasBadword;
        botRequest.PipelineResult.Actions.AddRange(action ?? []);

        return PreProcessResult<TResponse>.Continue;
    }
}
