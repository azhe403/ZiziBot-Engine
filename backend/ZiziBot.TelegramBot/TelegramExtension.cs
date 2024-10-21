using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ZiziBot.DataSource.MongoEf;
using ZiziBot.DataSource.MongoEf.Entities;
using ZiziBot.TelegramBot.Framework.Extensions;
using ZiziBot.TelegramBot.Framework.Models.Configs;

namespace ZiziBot.TelegramBot;

public static class TelegramExtension
{
    public static IServiceCollection ConfigureTelegramBot(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var environment = provider.GetRequiredService<IWebHostEnvironment>();
        var config = provider.GetRequiredService<IOptions<EngineConfig>>().Value;
        var appSettingDbContext = provider.GetRequiredService<MongoEfContext>();

        var listBotData = appSettingDbContext.BotSettings
            .Where(settings => settings.Status == EventStatus.Complete)
            .ToList();

        services.AddZiziBotTelegramBot(new BotEngineConfig() {
            EngineMode = config.TelegramEngineMode,
            WebhookUrl = EnvUtil.GetEnv(Env.WEB_CONSOLE_URL),
            Bot = listBotData.Select(x => new BotTokenConfig() {
                Name = x.Name,
                Token = x.Token
            }).ToList()
        });

        return services;
    }

    public async static Task<IApplicationBuilder> RunTelegramBot(this IApplicationBuilder app)
    {
        var provider = app.ApplicationServices;
        var appSettingsDbContext = provider.GetRequiredService<MongoEfContext>();

        var listBotOptions = provider.GetRequiredService<List<BotTokenConfig>>();

        if (listBotOptions.Count == 0)
        {
            appSettingsDbContext.BotSettings.Add(new BotSettingsEntity() {
                Name = "BOT_NAME",
                Token = "BOT_TOKEN",
                Status = EventStatus.InProgress
            });

            await appSettingsDbContext.SaveChangesAsync();

            throw new ApplicationException("No bot data found. Please ensure config for 'BotSettings'");
        }

        var validBot = listBotOptions
            .Where(x => x.Name != "BOT_NAME")
            .ToList();

        if (validBot.Count == 0)
        {
            throw new ApplicationException("Please add your Bot to 'BotSettings'");
        }

        await app.UseZiziBotTelegramBot();

        return app;
    }
}