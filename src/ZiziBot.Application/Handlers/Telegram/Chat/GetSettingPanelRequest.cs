using Microsoft.Extensions.Logging;
using MongoFramework.Linq;

namespace ZiziBot.Application.Handlers.Telegram.Chat;

public class GetSettingPanelRequestModel : RequestBase
{

}

public class GetSettingPanelRequestHandler : IRequestHandler<GetSettingPanelRequestModel, ResponseBase>
{
    private readonly ILogger<GetSettingPanelRequestHandler> _logger;
    private readonly TelegramService _telegramService;
    private readonly ChatDbContext _chatDbContext;

    public GetSettingPanelRequestHandler(ILogger<GetSettingPanelRequestHandler> logger, TelegramService telegramService, ChatDbContext chatDbContext)
    {
        _logger = logger;
        _telegramService = telegramService;
        _chatDbContext = chatDbContext;
    }

    public async Task<ResponseBase> Handle(GetSettingPanelRequestModel request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);

        var chat = await _chatDbContext.ChatSetting.FirstOrDefaultAsync(x => x.ChatId == request.ChatIdentifier, cancellationToken);
        if (chat == null)
        {
            _chatDbContext.ChatSetting.Add(new ChatSettingEntity()
            {
                ChatId = request.ChatIdentifier,
            });

            await _chatDbContext.SaveChangesAsync(cancellationToken);
        }

        await _telegramService.SendMessageText("Sedang memuat tombol..");

        return _telegramService.Complete();
    }
}