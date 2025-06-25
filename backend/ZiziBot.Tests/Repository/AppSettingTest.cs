using Xunit;
using ZiziBot.Common.Configs;

namespace ZiziBot.Tests.Repository;

public class AppSettingTest(AppSettingRepository appSettingRepository)
{
    [Fact]
    public async Task GetAppSetting()
    {
        var config = await appSettingRepository.GetConfigSectionAsync<EngineConfig>();

        config.ShouldNotBeNull();
    }
}