namespace ZiziBot.Application.Facades;

public class ServiceFacade(
    MediatorService mediatorService,
    TelegramService telegramService,
    AntiSpamService antiSpamService,
    SudoService sudoService
)
{
    public MediatorService MediatorService { get; } = mediatorService;
    public TelegramService TelegramService { get; } = telegramService;
    public AntiSpamService AntiSpamService { get; } = antiSpamService;
    public SudoService SudoService { get; } = sudoService;
}