using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Application.Facades;

public class ServiceFacade(IServiceProvider serviceProvider)
{
    public IMediator Mediator => serviceProvider.GetRequiredService<IMediator>();
    public ICacheService CacheService => serviceProvider.GetRequiredService<ICacheService>();
    public MediatorService MediatorService => serviceProvider.GetRequiredService<MediatorService>();
    public JobService JobService => serviceProvider.GetRequiredService<JobService>();
    public TelegramService TelegramService => serviceProvider.GetRequiredService<TelegramService>();
    public AntiSpamService AntiSpamService => serviceProvider.GetRequiredService<AntiSpamService>();
    public TonjooService TonjooService => serviceProvider.GetRequiredService<TonjooService>();
    public BinderByteService BinderByteService => serviceProvider.GetRequiredService<BinderByteService>();
    public FathimahApiService FathimahApiService => serviceProvider.GetRequiredService<FathimahApiService>();
    public MirrorPaymentService MirrorPaymentService => serviceProvider.GetRequiredService<MirrorPaymentService>();
    public OcrSpaceService OcrSpaceService => serviceProvider.GetRequiredService<OcrSpaceService>();
    public SubdlService SubdlService => serviceProvider.GetRequiredService<SubdlService>();
    public UupDumpService UupDumpService => serviceProvider.GetRequiredService<UupDumpService>();
    public WebhookService WebhookService => serviceProvider.GetRequiredService<WebhookService>();
}