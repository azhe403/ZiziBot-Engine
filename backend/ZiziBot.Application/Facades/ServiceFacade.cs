using ZiziBot.Interfaces;

namespace ZiziBot.Application.Facades;

public class ServiceFacade(
    IMediator mediator,
    ICacheService cacheService,
    MediatorService mediatorService,
    JobService jobService,
    TelegramService telegramService,
    AntiSpamService antiSpamService,
    TonjooService tonjooService,
    BinderByteService binderByteService,
    FathimahApiService fathimahApiService,
    MirrorPaymentService mirrorPaymentService,
    OcrSpaceService ocrSpaceService,
    SubdlService subdlService,
    UupDumpService uupDumpService,
    WebhookService webhookService
)
{
    public IMediator Mediator { get; } = mediator;
    public ICacheService CacheService { get; } = cacheService;
    public MediatorService MediatorService { get; } = mediatorService;
    public JobService JobService { get; } = jobService;
    public TelegramService TelegramService { get; } = telegramService;
    public AntiSpamService AntiSpamService { get; } = antiSpamService;
    public TonjooService TonjooService { get; } = tonjooService;
    public BinderByteService BinderByteService { get; } = binderByteService;
    public FathimahApiService FathimahApiService { get; } = fathimahApiService;
    public MirrorPaymentService MirrorPaymentService { get; } = mirrorPaymentService;
    public OcrSpaceService OcrSpaceService { get; } = ocrSpaceService;
    public SubdlService SubdlService { get; } = subdlService;
    public UupDumpService UupDumpService { get; } = uupDumpService;
    public WebhookService WebhookService { get; } = webhookService;
}