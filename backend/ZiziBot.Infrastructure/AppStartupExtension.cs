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

        await app.PrefetchRepository();

        return app;
    }

    private static async Task<IApplicationBuilder> PrefetchRepository(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var provider = scope.ServiceProvider;

        #region Feature Flags
        var featureFlagRepository = provider.GetRequiredService<FeatureFlagRepository>();
        await featureFlagRepository.GetFlags();
        #endregion

        #region Env
        var appSettingRepository = provider.GetRequiredService<AppSettingRepository>();
        var sentryConfig = appSettingRepository.GetConfigSection<SentryConfig>();

        if (sentryConfig?.IsEnabled == true)
        {
            Env.SentryDsn = sentryConfig.Dsn;
        }

        await appSettingRepository.GetApiKeyAsync(ApiKeyCategory.Internal, ApiKeyVendor.GitHub);
        await appSettingRepository.GetBotMain();
        #endregion

        return app;
    }
}