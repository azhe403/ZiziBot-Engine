using MediatR.Pipeline;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class ChatRestrictionProcessorBotRequest<TRequest> : IRequestPreProcessor<TRequest>
    where TRequest : BotRequestBase
{
    private readonly TelegramService _telegramService;
    private readonly AppSettingRepository _appSettingRepository;
    private readonly MongoDbContextBase _mongoDbContext;

    public ChatRestrictionProcessorBotRequest(TelegramService telegramService, AppSettingRepository appSettingRepository, MongoDbContextBase mongoDbContext)
    {
        _telegramService = telegramService;
        _appSettingRepository = appSettingRepository;
        _mongoDbContext = mongoDbContext;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (request.IsChannel ||
            request.InlineQuery != null ||
            request.IsPrivateChat)
            return;

        var config = await _appSettingRepository.GetConfigSectionAsync<EngineConfig>();

        if (!(config?.EnableChatRestriction ?? false))
            return;

        _telegramService.SetupResponse(request);

        var check = await _mongoDbContext.ChatRestriction
            .Where(entity => entity.Status == (int)EventStatus.Complete)
            .Where(entity => entity.ChatId == request.ChatIdentifier)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (check == null)
        {
            await _telegramService.SendMessageText("Untuk mendapatkan pengalaman terbaik, silakan gunakan @MissZiziBot di Grub Anda.");
            await _telegramService.LeaveChatAsync();
        }
    }
}