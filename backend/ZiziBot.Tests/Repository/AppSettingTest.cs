using FluentAssertions;
using Xunit;

namespace ZiziBot.Tests.Repository;

public class AppSettingTest
{
    private readonly AppSettingRepository _appSettingRepository;

    public AppSettingTest(AppSettingRepository appSettingRepository)
    {
        _appSettingRepository = appSettingRepository;
    }

    [Fact]
    public async Task GetAppSetting()
    {
        var config = await _appSettingRepository.GetConfigSection<EngineConfig>();

        true.Should().Be(true);
    }
}