﻿using Xunit;

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