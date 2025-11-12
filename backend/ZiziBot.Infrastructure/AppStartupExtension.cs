using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Infrastructure;

public static class AppStartupExtension
{
    public static async Task<IApplicationBuilder> PrintAbout(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(AppStartupExtension));

        logger.LogInformation("ZiziBot 5");
        logger.LogInformation("Version: {Version}", VersionUtil.GetVersionNumber());

        // await app.PrefetchRepository();

        return app;
    }

    private static async Task<IApplicationBuilder> PrefetchRepository(this IApplicationBuilder app)
    {
        var scope = app.ApplicationServices.CreateScope();
        var provider = scope.ServiceProvider;

        #region Feature Flags

        var featureFlagRepository = provider.GetRequiredService<FeatureFlagRepository>();
        await featureFlagRepository.GetFlags();

        #endregion

        #region Env

        var appSettingRepository = provider.GetRequiredService<AppSettingRepository>();
        var botRepository = provider.GetRequiredService<BotRepository>();
        var sentryConfig = appSettingRepository.GetConfigSection<SentryConfig>();

        if (sentryConfig?.IsEnabled == true)
        {
            Env.SentryDsn = sentryConfig.Dsn;
        }

        await botRepository.GetApiKeyAsync(ApiKeyCategory.Internal, ApiKeyVendor.GitHub);
        await botRepository.GetBotMain();

        #endregion

        return app;
    }
}