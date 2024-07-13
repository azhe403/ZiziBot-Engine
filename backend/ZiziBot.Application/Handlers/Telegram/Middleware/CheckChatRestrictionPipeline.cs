namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class CheckChatRestrictionPipeline<TRequest, TResponse>(
    TelegramService telegramService,
    AppSettingRepository appSettingRepository,
    ChatSettingRepository chatSettingRepository
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BotRequestBase
    where TResponse : BotResponseBase, new()
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.IsChannel ||
            request.InlineQuery != null ||
            request.CallbackQuery != null ||
            request.IsPrivateChat ||
            request.Source != ResponseSource.Bot)
            return await next();

        var config = await appSettingRepository.GetConfigSectionAsync<EngineConfig>();

        if (!(config?.EnableChatRestriction ?? false))
            return await next();

        telegramService.SetupResponse(request);

        var check = await chatSettingRepository.GetChatRestriction(request.ChatIdentifier);

        if (check == null)
        {
            await telegramService.SendMessageText("Untuk mendapatkan pengalaman terbaik, silakan gunakan @MissZiziBot di Grub Anda.");
            await telegramService.LeaveChatAsync();
            return new TResponse();
        }

        return await next();
    }
}