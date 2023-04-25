using Xunit;

namespace ZiziBot.Tests.Pipelines;

public class DbBackupTest
{
    private readonly MediatorService _mediatorService;
    private readonly AppSettingRepository _appSettingRepository;

    public DbBackupTest(MediatorService mediatorService, AppSettingRepository appSettingRepository)
    {
        _mediatorService = mediatorService;
        _appSettingRepository = appSettingRepository;
    }

    [Fact]
    public async Task ExportMongoDbTest()
    {
        await _mediatorService.Send(new MainDbBackupRequest());
    }
}