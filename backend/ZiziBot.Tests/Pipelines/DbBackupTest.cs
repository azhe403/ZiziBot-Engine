using FluentAssertions;
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
        var result = await _mediatorService.Send(new MongoDbBackupRequest());

        result.Should().BeTrue();
    }
}