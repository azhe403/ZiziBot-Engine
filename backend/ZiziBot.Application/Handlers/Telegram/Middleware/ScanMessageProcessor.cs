using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Application.Handlers.Telegram.Middleware;

public class ScanMessageProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : BotRequestBase
{
    private readonly ILogger<ScanMessageProcessor<TRequest>> _logger;
    private readonly TelegramService _telegramService;
    private readonly AppSettingRepository _appSettingRepository;
    private readonly MongoDbContextBase _mongoDbContext;
    private readonly WordFilterRepository _wordFilterRepository;
    private static readonly char[] Separator = new[] { ' ', '\n', ':', ';', ',' };

    public ScanMessageProcessor(
        ILogger<ScanMessageProcessor<TRequest>> logger,
        TelegramService telegramService,
        AppSettingRepository appSettingRepository,
        MongoDbContextBase mongoDbContext,
        WordFilterRepository wordFilterRepository
    )
    {
        _logger = logger;
        _telegramService = telegramService;
        _appSettingRepository = appSettingRepository;
        _mongoDbContext = mongoDbContext;
        _wordFilterRepository = wordFilterRepository;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        if (request.IsChannel ||
            request.Source == ResponseSource.Hangfire ||
            request.IsPrivateChat)
            return;

        if (request.MessageTexts.IsEmpty())
            return;

        request.ReplyMessage = true;

        _telegramService.SetupResponse(request);

        var hasBadword = false;
        var matchPattern = string.Empty;

        var words = await _wordFilterRepository.GetAll();

        var messageTexts = request.Message?.Text?.Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

        foreach (var source in messageTexts)
        {
            foreach (var dto in words)
            {
                if (source == dto.Word)
                    hasBadword = true;

                if (dto.Word.StartsWith('*'))
                    if (source.StartsWith(dto.Word))
                        hasBadword = true;

                if (dto.Word.EndsWith('*'))
                    if (source.EndsWith(dto.Word))
                        hasBadword = true;

                if (dto.Word.StartsWith('*') && dto.Word.EndsWith('*'))
                    if (source.Contains(dto.Word))
                        hasBadword = true;

                if (!hasBadword)
                    continue;

                _logger.LogDebug("Scan message: Match word: {Word} with a source: {Source}", dto.Word, source);

                matchPattern = dto.Word;
                break;
            }

            if (hasBadword)
                break;
        }

        if (!hasBadword)
            return;

        var htmlMessage = HtmlMessage.Empty
            .User(request.UserId, request.User.GetFullName()).Text(" telah diperingatkan.").Br()
            .Text("Karena: mengirim pesan yang mengandung pola: ").Bold(matchPattern).Br();

        try
        {
            var muteDuration = MemberMuteDuration.Select(0);

            await _telegramService.MuteMemberAsync(request.UserId, muteDuration);

            htmlMessage.Text($"Aksi: Senyap selama {muteDuration.ForHuman()}");
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Error when trying to Mute UserId: {UserId}. Message: {Message}", request.UserId, exception.Message);
        }

        await _telegramService.SendMessageText(htmlMessage.ToString());
    }
}