using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class CheckMessagePipelineBehavior<TRequest, TResponse>(
    ILogger<CheckMessagePipelineBehavior<TRequest, TResponse>> logger,
    TelegramService telegramService,
    AppSettingRepository appSettingRepository,
    MongoDbContextBase mongoDbContext,
    WordFilterRepository wordFilterRepository
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : BotRequestBase
    where TResponse : BotResponseBase, new()
{
    private readonly AppSettingRepository _appSettingRepository = appSettingRepository;
    private readonly MongoDbContextBase _mongoDbContext = mongoDbContext;
    private static readonly char[] Separator = new[] { ' ', '\n', ':', ';', ',' };

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request.IsChannel ||
            request.Source == ResponseSource.Hangfire ||
            request.IsPrivateChat)
            return await next();

        if (request.MessageTexts.IsEmpty())
            return await next();

        if (request.RolesLevels.Any(x => x == RoleLevel.Sudo))
        {
            if (request.Command is "/dwf" or "/awf")
            {
                return await next();
            }
        }

        request.ReplyMessage = true;

        telegramService.SetupResponse(request);

        var hasBadword = false;
        var matchPattern = string.Empty;
        WordFilterAction[] action = [];

        var words = await wordFilterRepository.GetAllAsync();

        var messageTexts = request.Message?.Text?.Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

        foreach (var messageText in messageTexts)
        {
            foreach (var dto in words)
            {
                var pattern = dto.Word;

                if (messageText == pattern)
                    hasBadword = true;

                if (pattern.StartsWith('*'))
                    if (messageText.StartsWith(pattern))
                        hasBadword = true;

                if (pattern.EndsWith('*'))
                    if (messageText.EndsWith(pattern))
                        hasBadword = true;

                if (pattern.StartsWith('*') && pattern.EndsWith('*'))
                    if (messageText.Contains(pattern))
                        hasBadword = true;

                if (!hasBadword)
                    continue;

                logger.LogWarning("Scan message: Match word: {Pattern} with a source: {MessageText}", pattern, messageText);

                matchPattern = dto.Word;
                action = dto.Action;

                break;
            }

            if (hasBadword)
                break;
        }

        if (!hasBadword)
            return await next();

        var htmlMessage = HtmlMessage.Empty
            .User(request.UserId, request.User.GetFullName()).Text(" telah diperingatkan.").Br()
            .Text("Karena: mengirim pesan yang mengandung pola: ").Bold(matchPattern).Br();

        try
        {
            //todo. incremental
            var muteDuration = MemberMuteDuration.Select(0);

            if (action.NotEmpty())
            {
                foreach (var actionItem in action)
                    switch (actionItem)
                    {
                        case WordFilterAction.Delete:
                            await telegramService.DeleteMessageAsync();

                            break;
                        case WordFilterAction.Warn:
                            break;
                        case WordFilterAction.Mute:
                            await telegramService.MuteMemberAsync(request.UserId, muteDuration);
                            htmlMessage.Text($"Aksi: Senyap selama {muteDuration.ForHuman()}");

                            break;
                        case WordFilterAction.Kick:
                            break;
                        default:
                            break;
                    }
            }
            else
            {
                await telegramService.MuteMemberAsync(request.UserId, muteDuration);

                htmlMessage.Text($"Aksi: Senyap selama {muteDuration.ForHuman()}");
            }
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Error when running action for userId: {UserId}. Message: {Message}", request.UserId, exception.Message);
        }

        if (action.Any(x => x == WordFilterAction.Warn))
            await telegramService.SendMessageText(htmlMessage.ToString());

        return new TResponse();
    }
}