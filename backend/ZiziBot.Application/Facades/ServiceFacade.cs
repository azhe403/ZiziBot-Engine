namespace ZiziBot.Application.Facades;

public class ServiceFacade(
    TelegramService telegramService,
    AntiSpamService antiSpamService,
    SudoService sudoService
)
{
    public TelegramService TelegramService { get; } = telegramService;
    public AntiSpamService AntiSpamService { get; } = antiSpamService;
    public SudoService SudoService { get; } = sudoService;
}